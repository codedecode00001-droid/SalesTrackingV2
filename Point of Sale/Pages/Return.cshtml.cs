using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using pos.Core.Entities;
using pos.Infrastructure.Repositories;
using System.Text.Json;

namespace Point_of_Sale.Pages
{
    public class ReturnModel : PageModel
    {
        private readonly ReturnOrderRep _repo;

        public string? Actions;
        int pin_code = 0;

        public ReturnModel(ReturnOrderRep repo)
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
        public async Task<JsonResult> OnGetList()
        {
            pin_code = Convert.ToInt32(HttpContext.Session.GetString("pin_code"));

            var orderlist = await _repo.GetReturnOrderList(pin_code);

            var grouped = orderlist
                .GroupBy(o => o.order_id)
                .Select(g => new {
                    order_id = g.Key,
                    products = g.Select(p => new {
                        order_no = p.order_no,
                        prod_id = p.prod_id,
                        product_name = p.product_name,
                        price = p.price,
                        qty = p.qty
                    }).ToList()
                }).ToList();

            return new JsonResult(grouped);
        }

        //UPDATE
        public JsonResult OnPostUpdateItemAsync([FromBody] ReturnOrder order)
        {
            try
            {
                if (order == null)
                    return new JsonResult(new { success = false, message = "Invalid product data!" });

                Actions = "Update";

                var pinStr = HttpContext.Session.GetString("pin_code");
                if (string.IsNullOrEmpty(pinStr))
                    return new JsonResult(new { success = false, message = "Session expired." });

                int pin_code = Convert.ToInt32(pinStr);

                var result = _repo.UpdateOrder(order, Actions, pin_code);

                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }

        //DELETE
        public JsonResult OnPostDeleteItemAsync([FromBody] ReturnOrder order)
        {
            try
            {
                if (order == null)
                    return new JsonResult(new { success = false, message = "Invalid product data!" });

                Actions = "Delete";

                var pinStr = HttpContext.Session.GetString("pin_code");
                if (string.IsNullOrEmpty(pinStr))
                    return new JsonResult(new { success = false, message = "Session expired." });

                int pin_code = Convert.ToInt32(pinStr);

                var result = _repo.DeleteOrder(
                    order.order_no,
                    order.prod_id,
                    Convert.ToInt32(order.price),
                    Convert.ToInt32(order.qty),
                    pin_code,
                    Actions
                );

                return new JsonResult(new { success = result.Success, message = result.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }
        }
    }
}
