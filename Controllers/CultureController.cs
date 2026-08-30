using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace TheBestBean.Controllers
{
    [Route("[controller]/[action]")]
    public class CultureController : Controller
    {
        [HttpPost]
        public IActionResult SetCulture(string culture, string returnUrl)
        {
            if (culture != null)
            {
                Response.Cookies.Append(
                    CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                    new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
                );
            }

            return LocalRedirect(returnUrl ?? "/");
        }
    }
}
