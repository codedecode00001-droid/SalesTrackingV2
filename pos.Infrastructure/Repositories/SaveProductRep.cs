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
    public class SaveProductRep
    {
        private readonly DataAccessHelper _db;

        public SaveProductRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET Category Id and Name
        public Task<List<Products>> GetCategories()
        {
            var categoryList = _db.GetData<Products>(
                "[dbo].[usp_GetCategory]",
                CommandType.StoredProcedure);

            return Task.FromResult(categoryList);
        }

        // GET ALL
        public List<Products> GetAllListProduct()
        {
            List<Products> productList = new List<Products>();

            productList = _db.GetData<Products>("[dbo].[usp_GetProductList]", CommandType.StoredProcedure);

            return productList;
        }

        // GET ONE
        public Products GetProductById(int id)
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
                prod_id = Convert.ToInt32(row["prod_id"]),
                cat_id = Convert.ToInt32(row["cat_id"]),
                product_name = row["product_name"].ToString(),
                price = Convert.ToInt32(row["price"]),
                stock = Convert.ToInt32(row["stock"]),
                date_expired = row["date_expired"].ToString(),
                status = row["status"].ToString(),
                description = row["description"].ToString()
            };
        }

        // INSERT, and UPDATE
        public MessageResult SaveUpdateDeleteProd(Products prod, string? Actions)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pProdId", prod.prod_id),
                    new SqlParameter("@pCatId", prod.cat_id),
                    new SqlParameter("@pProductName", prod.product_name),
                    new SqlParameter("@pPrice", prod.price),
                    new SqlParameter("@pStock", prod.stock),
                    new SqlParameter("@pDateExpired", prod.date_expired),
                    new SqlParameter("@pStatus", prod.status),
                    new SqlParameter("@pDescription", prod.description),
                    new SqlParameter("@pActions", Actions),
                };

            var result = _db.GetData<MessageResult>("[dbo].[usp_SaveUpdateDeleteProduct]", parameters, CommandType.StoredProcedure)[0];

            return result;
        }

        // DELETE
        public MessageResult DeleteProd(int id, string? Actions)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pProdId", id),
                new SqlParameter("@pActions", Actions)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_SaveUpdateDeleteProduct]",
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
