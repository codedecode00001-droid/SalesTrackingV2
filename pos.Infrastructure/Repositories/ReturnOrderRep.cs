using Microsoft.Data.SqlClient;
using pos.Core.Entities;
using pos.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Infrastructure.Repositories
{
    public class ReturnOrderRep
    {
        private readonly DataAccessHelper _db;

        public ReturnOrderRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET ALL Base on User
        // New version for getting order 3/9/2026@pin_code
        public Task<List<ReturnOrder>> GetReturnOrderList(int pinCode)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pPinCode", pinCode)
            };

            var orderList = _db.GetData<ReturnOrder>(
                "[dbo].[usp_GetReturnOrderList]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(orderList);
        }

        // UPDATE
        public MessageResult UpdateOrder(ReturnOrder order, string? Actions, int pinCode)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pOrederNo", order.order_no),
                    new SqlParameter("@pProdId", order.prod_id),
                    new SqlParameter("@pPrice", order.price),
                    new SqlParameter("@pQty", order.qty),
                    new SqlParameter("@pPinCode", pinCode),
                    new SqlParameter("@pActions", Actions),
                };

            var result = _db.GetData<MessageResult>("[dbo].[usp_ReturnOrder]", parameters, CommandType.StoredProcedure)[0];

            return result;
        }

        // DELETE
        public MessageResult DeleteOrder(int orderNo, int prodId, int price, int qty, int pinCode, string? Actions)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pOrederNo", orderNo),
                new SqlParameter("@pProdId", prodId),
                new SqlParameter("@pPrice", price),
                new SqlParameter("@pQty", qty),
                new SqlParameter("@pPinCode", pinCode),
                new SqlParameter("@pActions", Actions)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_ReturnOrder]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new MessageResult
            {
                Success = Convert.ToBoolean(row["Success"].ToString()),
                Message = row["Message"].ToString(),
            };
        }
    }
}
