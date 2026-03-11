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
    public class SaveOrderRep
    {
        private readonly DataAccessHelper _db;

        public SaveOrderRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET Category Id and Name
        public Task<List<Order>> GetCategories()
        {
            var categoryList = _db.GetData<Order>(
                "[dbo].[usp_GetCategory]",
                CommandType.StoredProcedure);

            return Task.FromResult(categoryList);
        }

        // GET Product Id and Name
        public Task<List<Order>> GetProducts(int catID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", catID)
            };

            var productList = _db.GetData<Order>(
                "[dbo].[usp_GetProduct]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(productList);
        }

        // GET Unit Id and Name
        public Task<List<Order>> GetUnits()
        {
            var unitList = _db.GetData<Order>(
                "[dbo].[usp_GetUnit]",
                CommandType.StoredProcedure);

            return Task.FromResult(unitList);
        }

        // GET Price and Stock
        public Products GetPriceStock(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pProdId", id)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_GetProductById]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Products
            {
                price = Convert.ToInt32(row["price"]),
                stock = Convert.ToInt32(row["stock"])
            };
        }

        // GET ALL Base on User
        // New version for getting order 3/9/2026@pin_code
        public Task<List<Order>> GetOrderList(int pinCode)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pPinCode", pinCode)
            };

            var orderList = _db.GetData<Order>(
                "[dbo].[usp_GetOrderList]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(orderList);
        }

        // Old version for getting order
        //public List<Order> GetOrderList()
        //{
        //    List<Order> orderList = new List<Order>();

        //    orderList = _db.GetData<Order>("[dbo].[usp_GetOrderList]", CommandType.StoredProcedure);

        //    return orderList;
        //}

        // GET ONE Order
        public Order GetOrderById(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pId", id)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_GetOrderById]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Order
            {
                id = Convert.ToInt32(row["id"]),
                prod_id = Convert.ToInt32(row["prod_id"]),
                cat_id = Convert.ToInt32(row["cat_id"]),
                price = Convert.ToInt32(row["price"]),
                qty = Convert.ToInt32(row["qty"]),
                stock = Convert.ToInt32(row["stock"]),
                units = Convert.ToInt32(row["units"]),
                description = row["description"].ToString(),
                total_price = Convert.ToInt32(row["total_price"]) 
            };
        }

        // GET ONE Product By Scan QR
        public Order GetProdByScanQR(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pId", id)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_GetOrderProdByScanQR]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Order
            {
                prod_id = Convert.ToInt32(row["prod_id"]),
                cat_id = Convert.ToInt32(row["cat_id"]),
                price = Convert.ToInt32(row["price"]),
                stock = Convert.ToInt32(row["stock"]),
                description = row["description"].ToString(),
            };
        }


        // INSERT, and UPDATE
        public MessageResult SaveUpdateOrder(Order order, string? Actions, int pinCode)
        {
            // new SqlParameter("@pStock", order.stock),
            // new SqlParameter("@pTotal_Price", order.total_price),

            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pId", order.id),
                    new SqlParameter("@pProdId", order.prod_id),
                    new SqlParameter("@pCatId", order.cat_id),
                    new SqlParameter("@pPrice", order.price),
                    new SqlParameter("@pQty", order.qty),
                    new SqlParameter("@pUnits", order.units),
                    new SqlParameter("@pDescription", order.description),
                    new SqlParameter("@pPinCode", pinCode),
                    new SqlParameter("@pActions", Actions),
                };

            var result = _db.GetData<MessageResult>("[dbo].[usp_SaveUpdateTempOrder]", parameters, CommandType.StoredProcedure)[0];

            return result;
        }

        // DELETE
        public MessageResult DeleteOrder(int id, int prodId, int qty, string? Actions)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pId", id),
                new SqlParameter("@pProdId", prodId),
                new SqlParameter("@pQty", qty),
                new SqlParameter("@pActions", Actions)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_SaveUpdateTempOrder]",
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

        // Save Order
        public MessageResult SaveOrder(int pinCode)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pPinCode", pinCode),
                 };

            var result = _db.GetData<MessageResult>("[dbo].[usp_SaveOrder]", parameters, CommandType.StoredProcedure)[0];

            return result;
        }
    }
}
