using Microsoft.Data.SqlClient;
using pos.Core.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pos.Infrastructure.Repositories
{
    public class GetAllListRep
    {
        private readonly DataAccessHelper _db;

        public GetAllListRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET ALL Sales Today
        public List<Dashboard> GetSalesList()
        {
            List<Dashboard> salesList = new List<Dashboard>();

            salesList = _db.GetData<Dashboard>("[dbo].[usp_GetSalesList]", CommandType.StoredProcedure);

            return salesList;
        }

        // GET ALL Stocks
        public List<Dashboard> GetStockList()
        {
            List<Dashboard> stockList = new List<Dashboard>();

            stockList = _db.GetData<Dashboard>("[dbo].[usp_GetStockList]", CommandType.StoredProcedure);

            return stockList;
        }

        // GET ALL Stocks
        public List<Dashboard> GetExpiredList()
        {
            List<Dashboard> expiredList = new List<Dashboard>();

            expiredList = _db.GetData<Dashboard>("[dbo].[usp_GetExpiredList]", CommandType.StoredProcedure);

            return expiredList;
        }

        // GET Details
        public Dashboard GetDetails()
        {
            var detailsList = _db.GetData<Dashboard>("[dbo].[usp_GetDetailsList]", CommandType.StoredProcedure)[0];

            return detailsList;
        }
    }
}
