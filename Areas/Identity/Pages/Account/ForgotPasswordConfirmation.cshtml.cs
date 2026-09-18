using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TheBestBean.Areas.Identity.Pages.Account
{
    public class ForgotPasswordConfirmationModel : PageModel
    {
        public IActionResult OnGet()
        {
            return RedirectToPage("./ForgotPassword");
        }
    }
}
