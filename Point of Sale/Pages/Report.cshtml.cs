using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Infrastructure.Repositories;


namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class ReportModel : PageModel
    {
        private readonly GetReportRep _repo;

        public ReportModel(GetReportRep repo)
        {
            _repo = repo;
        }

        public IActionResult OnGet()
        {
            var user = HttpContext.Session.GetString("fname");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToPage("/EnterPin");
            }

            return Page();
        }

        // GET Sales List By date from and to
        // Now the response JSON is a real array
        // JS .forEach() will work
        public async Task<JsonResult> OnGetGetSalesList(string from, string to)
        {
            var data = await _repo.GetSalesByFilter(from, to);
            return new JsonResult(data);
        }

        //LIST
        public JsonResult OnGetList()
        {
            return new JsonResult(_repo.GetAllSalesList());
        }
    }
}
