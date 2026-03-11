using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Infrastructure.Repositories;

namespace Point_of_Sale.Pages
{
    public class LogoutModel : PageModel
    {
        private readonly PinCodeRep _repo;

        public LogoutModel(PinCodeRep repo)
        {
            _repo = repo;
        }
        public IActionResult OnPost()
        {
            int pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

            _repo.Logout(pin_code);

            HttpContext.Session.Clear(); // remove session

            return RedirectToPage("/EnterPin"); // go back to login page
        }
    }
}
