using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;

namespace Point_of_Sale.Pages
{
    [IgnoreAntiforgeryToken]
    public class OrdersModel : PageModel
    {
        private readonly SaveOrderRep _repo;
        public string? Actions;

        int pin_code = 0;

        public OrdersModel(SaveOrderRep repo)
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

        // Get the product based in category
        public async Task<JsonResult> OnGetGetProdAsync(int catId)
        {
            try
            {
                var prod = (await _repo.GetProducts(catId))
                      .Select(d => new { id = d.prod_id, name = d.product_name });

                return new JsonResult(prod);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // Get the units
        public async Task<JsonResult> OnGetGetUnitsAsync()
        {
            try
            {
                var unit = (await _repo.GetUnits())
                      .Select(u => new { id = u.unit_id, name = u.unit_name });

                return new JsonResult(unit);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        // GET ONE Price and Stock
        public JsonResult OnGetGetPriceStock(int prodId)
        {
            return new JsonResult(_repo.GetPriceStock(prodId));
        }

        //LIST
        public async Task<JsonResult> OnGetList()
        {
            pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

            var orderlist = await _repo.GetOrderList(pin_code);

            return new JsonResult(orderlist);
        }

        // GET ONE FOR EDIT
        public JsonResult OnGetGet(int Id)
        {
            return new JsonResult(_repo.GetOrderById(Id));
        }

        // GET ONE FOR EDIT
        public JsonResult OnGetOrderProd(int Id)
        {
            return new JsonResult(_repo.GetProdByScanQR(Id));
        }

        // INSERT         
        public async Task<JsonResult> OnPostAddCartAsync()
        {
            try
            {
                Actions = "Insert";
                pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

                //var pinCodeString = HttpContext.Session.GetString("pin_code");
                //int pinCode = 0;

                //int.TryParse(pinCodeString, out pinCode);

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var order = JsonSerializer.Deserialize<Order>(body);

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

                var result = _repo.SaveUpdateOrder(order, Actions, pin_code);

                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        //UPDATE
        public async Task<JsonResult> OnPostUpdateCartAsync()
        {
            try
            {
                Actions = "Update";

                pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

                var body = await new StreamReader(Request.Body).ReadToEndAsync();
                var order = JsonSerializer.Deserialize<Order>(body);

                if (order == null)
                {
                    return new JsonResult(new { success = false, message = "Invalid product data!" });
                }

                var result = _repo.SaveUpdateOrder(order, Actions, pin_code);

                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        //DELETE
        public async Task<JsonResult> OnPostDeleteItemCartAsync()
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

                int id = json["id"];
                int prodId = json["prodId"];
                int qty = json["qty"];

                var result = _repo.DeleteOrder(id, prodId, qty, Actions);

                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });

            }
        }

        // Save Order         
        public JsonResult OnPostSaveOrder()
        {
            pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

            try
            {
                var result = _repo.SaveOrder(pin_code);
                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}