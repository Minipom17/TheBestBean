using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheBestBean.Pages.Talks;

public class PeruCoffeeModel : PageModel
{
    public void OnGet()
    {
        ViewData["Title"] = "Coffee in Peru — 10 minutes";
        ViewData["MetaDescription"] = "A 10-minute lab talk: Peru’s coffee regions, altitude, and why denser beans taste the way they do. Maps of Cusco, Cajamarca, and Junín.";
        ViewData["MetaImage"] = "https://purplebean.coffee/brand/og-share.jpg?v=2";
        ViewData["CanonicalUrl"] = "https://purplebean.coffee/Resources/CoffeeInPeru";
    }
}
