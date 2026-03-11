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
    public class GetReportRep
    {
        private readonly DataAccessHelper _db;

        public GetReportRep(DataAccessHelper db)
        {
            _db = db;
        }

        // GET ALL
        public List<Report> GetAllSalesList()
        {
            List<Report> reportList = new List<Report>();

            reportList = _db.GetData<Report>("[dbo].[usp_GetAllSalesList]", CommandType.StoredProcedure);

            return reportList;
        }

        // GET Sale by date from and to
        public Task<List<Report>> GetSalesByFilter(string dateFrom, string dateTo)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pDateFrom", dateFrom),
                new SqlParameter("@pDateTo", dateTo)
            };

            var saleDateList = _db.GetData<Report>(
                "[dbo].[usp_GetSalesByFilter]", parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(saleDateList);
        }
    }
}
