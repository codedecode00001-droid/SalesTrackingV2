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
    public class PinCodeRep
    {
        private readonly DataAccessHelper _db;

        public PinCodeRep(DataAccessHelper db)
        {
            _db = db;
        }


        // GET Sale by date from and to
        public Task<Pin?> ValidatePin(int pin)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pPinCode", pin)
            };

            var result = _db.GetData<Pin>(
                "[dbo].[usp_ValidatePin]",
                parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(result.FirstOrDefault());
        }

        // Logout User
        public Task<Pin?> Logout(int pin)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pPinCode", pin)
            };

            var result = _db.GetData<Pin>(
                "[dbo].[usp_LogoutPin]",
                parameters,
                CommandType.StoredProcedure);

            return Task.FromResult(result.FirstOrDefault());
        }

        // GET ONE
        public Pin GetUserById(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pId", id)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_GetUserById]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Pin
            {
                id = Convert.ToInt32(row["id"]),
                pin_code = Convert.ToInt32(row["pin_code"]),
                first_name = row["first_name"].ToString(),
                middle_name = row["middle_name"].ToString(),
                last_name = row["last_name"].ToString(),
                position = row["position"].ToString(),
                status = row["status"].ToString()
            };
        }

        // GET ALL
        public List<Pin> GetListUsers()
        {
            List<Pin> userList = new List<Pin>();

            userList = _db.GetData<Pin>("[dbo].[usp_GetUserList]", CommandType.StoredProcedure);

            return userList;
        }

        // GET ONE
        //public Products GetProductById(int id)
        //{
        //    SqlParameter[] parameters = {
        //        new SqlParameter("@pProdId", id)
        //    };

        //    DataSet ds = _db.GetData(
        //        "[dbo].[usp_GetProductById]",
        //        parameters,
        //        CommandType.StoredProcedure
        //    );

        //    if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
        //        return null;

        //    DataRow row = ds.Tables[0].Rows[0];

        //    return new Products
        //    {
        //        prod_id = Convert.ToInt32(row["prod_id"]),
        //        cat_id = Convert.ToInt32(row["cat_id"]),
        //        product_name = row["product_name"].ToString(),
        //        price = Convert.ToInt32(row["price"]),
        //        stock = Convert.ToInt32(row["stock"]),
        //        date_expired = row["date_expired"].ToString(),
        //        status = row["status"].ToString(),
        //        description = row["description"].ToString()
        //    };
        //}

        // INSERT, and UPDATE
        public MessageResult SaveUpdateDeleteUser(Pin user, string? Actions)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pId", user.id),
                    new SqlParameter("@pPin_Code", user.pin_code),
                    new SqlParameter("@pFname", user.first_name),
                    new SqlParameter("@pMname", user.middle_name),
                    new SqlParameter("@pLname", user.last_name),
                    new SqlParameter("@pPosition", user.position),
                    new SqlParameter("@pStatus", user.status),
                    new SqlParameter("@pActions", Actions),
                };

            var result = _db.GetData<MessageResult>("[dbo].[usp_SaveUpdateDeleteUser]", parameters, CommandType.StoredProcedure)[0];

            return result;
        }

        // DELETE
        public MessageResult DeleteUser(int id, string? Actions)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pId", id),
                new SqlParameter("@pActions", Actions)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_SaveUpdateDeleteUser]",
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
