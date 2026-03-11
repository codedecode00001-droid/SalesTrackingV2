using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Data.SqlClient;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class CategoryModel : PageModel
    {
        private readonly SaveCategory _repo;
        public string? Actions;

        public CategoryModel(SaveCategory repo)
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

        //LIST
        public JsonResult OnGetList()
        {
            return new JsonResult(_repo.GetAllCategories());
        }

        // GET ONE FOR EDIT
        public JsonResult OnGetGet(int catid)
        {
            return new JsonResult(_repo.GetCategoryById(catid));
        }

        // INSERT         
        public async Task<JsonResult> OnPostInsertAsync()
        {
            try
            {
                Actions = "Insert";

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var category = JsonSerializer.Deserialize<Categories>(body);

                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid category data!" });
                }

                _repo.SaveUpdateDeleteCat(category, Actions);

                return new JsonResult(new { success = true, message = "Category inserted!" });
            }
            catch (Exception ex) {
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
                var category = JsonSerializer.Deserialize<Categories>(body);

                if (category == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid category data!" });
                }

                _repo.SaveUpdateDeleteCat(category, Actions);

                return new JsonResult(new { success = true, message = "Category updated!" });
            }
            catch (Exception ex) {
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
                    return new JsonResult(new { success = false, message = "Invalid category data!" });
                }

                int id = json["cat_id"];

                _repo.DeleteCat(id, Actions);

                return new JsonResult(new { success = true, message = "Category deleted!" });
            }
            catch (Exception ex) {
                return new JsonResult(new { success = false, message = ex.Message });
            }      
        }
    }
}
