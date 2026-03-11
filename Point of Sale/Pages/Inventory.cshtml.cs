using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Infrastructure.Repositories;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class InventoryModel : PageModel
    {
        private readonly GetInventoryRep _repo;
        private readonly SaveOrderRep _repoCatProd;
        public string? Actions;

        public InventoryModel(GetInventoryRep repo, SaveOrderRep repoCatProd)
        {
            _repo = repo;
            _repoCatProd = repoCatProd;
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

        // Get the category
        public async Task<JsonResult> OnGetCategoryAsync()
        {
            try
            {
                var cat = (await _repoCatProd.GetCategories())
                      .Select(d => new { id = d.cat_id, name = d.category_name });

                return new JsonResult(cat);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // Get the product based in category
        public async Task<JsonResult> OnGetGetProdAsync(int catId)
        {
            try
            {
                var prod = (await _repoCatProd.GetProducts(catId))
                      .Select(d => new { id = d.prod_id, name = d.product_name });

                return new JsonResult(prod);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // GET Inventory List By Categeroy
        // Now the response JSON is a real array
        // JS .forEach() will work
        public async Task<JsonResult> OnGetGetInvListCat(int catId)
        {
            var data = await _repo.GetInvByCat(catId);
            return new JsonResult(data);
        }

        // GET Inventory List By Category and Product
        // Now the response JSON is a real array
        // JS .forEach() will work
        public async Task<JsonResult> OnGetGetInvListCatProd(int catId, int prodId)
        {
            var data = await _repo.GetInvByCatandProd(catId, prodId);
            return new JsonResult(data);
        }

        //LIST
        public JsonResult OnGetList()
        {
            return new JsonResult(_repo.GetInventoryList());
        }
    }
}
