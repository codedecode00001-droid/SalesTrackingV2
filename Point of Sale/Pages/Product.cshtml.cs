using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class ProductModel : PageModel
    {
        private readonly SaveProductRep _repo;
        public string? Actions;

        public ProductModel(SaveProductRep repo)
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

        // Get the category
        public async Task<JsonResult> OnGetCategoryAsync()
        {
            try
            {
                var cat = (await _repo.GetCategories())
                      .Select(d => new { id = d.cat_id, name = d.category_name });

                return new JsonResult(cat);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }

        //LIST
        public JsonResult OnGetList()
        {
            return new JsonResult(_repo.GetAllListProduct());
        }

        // GET ONE FOR EDIT
        public JsonResult OnGetGet(int prodId)
        {
            return new JsonResult(_repo.GetProductById(prodId));
        }

        // INSERT         
        public async Task<JsonResult> OnPostInsertAsync()
        {
            try
            {
                Actions = "Insert";

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var product = JsonSerializer.Deserialize<Products>(body);

                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

               var result = _repo.SaveUpdateDeleteProd(product, Actions);

                return new JsonResult(new { success = result.Success, message = result.Message, prod_id = result.prodId });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        //UPDATE
        public async Task<JsonResult> OnPostUpdateAsync()
        {
            try
            {
                Actions = "Update";

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var product = JsonSerializer.Deserialize<Products>(body);

                if (product == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

                var result = _repo.SaveUpdateDeleteProd(product, Actions);

                return new JsonResult(new { success = result.Success, message = result.Message, prod_id = result.prodId });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        //DELETE
        public async Task<JsonResult> OnPostDeleteAsync()
        {
            try
            {
                Actions = "Delete";

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var json = JsonSerializer.Deserialize<Dictionary<string, int>>(body);

                if (json == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

                int id = json["prod_id"];

                _repo.DeleteProd(id, Actions);

                return new JsonResult(new { success = true, message = "Product deleted!" });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
