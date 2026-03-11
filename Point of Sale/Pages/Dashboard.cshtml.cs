using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class DashboardModel : PageModel
    {
        private readonly GetAllListRep _repo;


        public DashboardModel(GetAllListRep repo)
        {
            _repo = repo;
        }

        //public void OnGet()
        //{
        //}

        public IActionResult OnGet()
        {
            var user = HttpContext.Session.GetString("fname");

            if (string.IsNullOrEmpty(user))
            {
                return RedirectToPage("/EnterPin");
            }

            return Page();
        }

        //LIST
        public JsonResult OnGetListSales()
        {
            return new JsonResult(_repo.GetSalesList());
        }

        //LIST
        public JsonResult OnGetListStock()
        {
            return new JsonResult(_repo.GetStockList());
        }

        //LIST
        public JsonResult OnGetListExpired()
        {
            return new JsonResult(_repo.GetExpiredList());
        }

        //Get Details
        public JsonResult OnGetDetails()
        {
            return new JsonResult(_repo.GetDetails());
        }
    }
}
