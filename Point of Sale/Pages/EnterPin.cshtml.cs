using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Infrastructure.Repositories;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class EnterPinModel : PageModel
    {
        private readonly PinCodeRep _repo;

        public EnterPinModel(PinCodeRep repo)
        {
            _repo = repo;
        }

        public async Task<JsonResult> OnGetValidate(int Pin)
        {
            var data = await _repo.ValidatePin(Pin);

            if (data != null)
            {
                HttpContext.Session.SetString("fname", data.first_name ?? "");
                HttpContext.Session.SetString("lname", data.last_name ?? "");
                HttpContext.Session.SetString("position", data.position ?? "");
                HttpContext.Session.SetString("pin_code", data.pin_code.ToString());

                return new JsonResult(new { success = true });
            }

            return new JsonResult(new { success = false });
        }

        public void OnGet()
        {
        }
    }
}
