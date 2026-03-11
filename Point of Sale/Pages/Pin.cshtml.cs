using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;
using static Raven.Client.Linq.LinqPathProvider;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class PinModel : PageModel
    {
        private readonly PinCodeRep _repo;
        public string? Actions;

        public PinModel(PinCodeRep repo)
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
            return new JsonResult(_repo.GetListUsers());
        }

        // GET ONE FOR EDIT
        public JsonResult OnGetGet(int Id)
        {
            return new JsonResult(_repo.GetUserById(Id));
        }

        // INSERT         
        public async Task<JsonResult> OnPostInsertAsync()
        {
            try
            {
                Actions = "Insert";

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var users = JsonSerializer.Deserialize<Pin>(body);

                if (users == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid user data!" });
                }

                var result = _repo.SaveUpdateDeleteUser(users, Actions);

                return new JsonResult(new { success = result.Success, message = result.Message });
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
                var users = JsonSerializer.Deserialize<Pin>(body);

                if (users == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

                var result = _repo.SaveUpdateDeleteUser(users, Actions);

                return new JsonResult(new { success = result.Success, message = result.Message });
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
                    return new JsonResult(new { success = false, message = "Invalid user data!" });
                }

                int id = json["id"];

                var result = _repo.DeleteUser(id, Actions);

                return new JsonResult(new { success = true, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
