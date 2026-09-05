using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using TheBestBean.Data;
using TheBestBean.Models;
using TheBestBean.Services;
using Microsoft.AspNetCore.StaticFiles;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
builder.Services.AddRazorPages(options => 
{
    options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
}).AddViewLocalization().AddDataAnnotationsLocalization();
builder.Services.AddControllers();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
});

// Configure JSON options for case-insensitive property matching
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

// Add session support for shopping cart
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Configure the database context for our coffee application
builder.Services.AddDbContext<TheBestBeanContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")));

// Add ASP.NET Core Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<TheBestBeanContext>();

// Register services
builder.Services.AddScoped<FarmProfileService>();
builder.Services.AddScoped<TheBestBean.Services.CartService>();

// Add Response Compression
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
});

var app = builder.Build();

// Apply database migrations automatically
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TheBestBeanContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        
        logger.LogInformation("Applying database migrations...");
        // Migrations handle schema creation
        await context.Database.MigrateAsync();
        logger.LogInformation("Database migrations applied successfully.");
    }
}
catch (Exception ex)
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error applying database migrations. Application will continue, but database may be incomplete.");
}

// Seed sample data for demonstration
try
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<TheBestBeanContext>();
        // Clean seed for fresh DB
        await SampleDataSeeder.SeedSampleDataAsync(context);
        await SampleDataSeeder.FixDataAsync(context);
        
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        await SampleDataSeeder.SeedAdminUserAsync(userManager, roleManager);
    }
}
catch (Exception ex)
{
    // Log the error but don't crash the application
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Error seeding sample data. Application will continue without seeded data.");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Configure forwarded headers for reverse proxy
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// Set path base from X-Forwarded-Prefix header
app.Use((context, next) =>
{
    var pathBase = context.Request.Headers["X-Forwarded-Prefix"].FirstOrDefault();

    if (!string.IsNullOrEmpty(pathBase))
    {
        context.Request.PathBase = pathBase;
        if (context.Request.Path.StartsWithSegments(pathBase, out var remaining))
        {
            context.Request.Path = remaining;
        }
    }
    return next();
});


var provider = new FileExtensionContentTypeProvider();
provider.Mappings[".geojson"] = "application/geo+json";

app.UseHttpsRedirection();
app.UseResponseCompression();
app.UseStaticFiles(new StaticFileOptions
{
    ContentTypeProvider = provider,
    OnPrepareResponse = ctx =>
    {
        ctx.Context.Response.Headers.Append("Cache-Control", "public,max-age=31536000");
    }
});

// Middleware to block registration route
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value;
    if (path != null && path.Equals("/Identity/Account/Register", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.Redirect("/Identity/Account/Login");
        return;
    }
    await next();
});

app.UseRouting();

var supportedCultures = new[] { new CultureInfo("en-US"), new CultureInfo("es-PE") };
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/Shop", () => Results.Redirect("/Coffee", permanent: true));
app.MapGet("/GreenBeans", () => Results.Redirect("/Coffee", permanent: true));

app.MapRazorPages();
app.MapControllers();

app.Run();
