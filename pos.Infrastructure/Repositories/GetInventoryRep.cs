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
    public class GetInventoryRep
    {
        private readonly DataAccessHelper _db;

        public GetInventoryRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET Category Id and Name
        public Task<List<Inventory>> GetCategories()
        {
            var categoryList = _db.GetData<Inventory>(
                "[dbo].[usp_GetCategory]",
                CommandType.StoredProcedure);

            return Task.FromResult(categoryList);
        }

        // GET Product Id and Name
        public Task<List<Inventory>> GetProducts(int catID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", catID)
            };

            var productList = _db.GetData<Inventory>(
                "[dbo].[usp_GetProduct]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(productList);
        }


        // GET ALL
        public List<Inventory> GetInventoryList()
        {
            List<Inventory> inventoryList = new List<Inventory>();

            inventoryList = _db.GetData<Inventory>("[dbo].[usp_GetInventoryList]", CommandType.StoredProcedure);

            return inventoryList;
        }

        // GET Category Id and Name
        public Task<List<Inventory>> GetInvByCat(int catID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", catID)
            };

            var invCatList = _db.GetData<Inventory>(
                "[dbo].[usp_GetInvByFilter]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(invCatList);
        }


        // GET Product Id and Name
        public Task<List<Inventory>> GetInvByCatandProd(int catID, int prodID)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", catID),
                new SqlParameter("@pProdId", prodID)
            };

            var invProdList = _db.GetData<Inventory>(
                "[dbo].[usp_GetInvByFilter]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(invProdList);
        }

    }
}
