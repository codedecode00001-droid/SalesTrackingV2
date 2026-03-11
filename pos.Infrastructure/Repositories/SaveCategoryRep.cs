using Microsoft.Data.SqlClient;
using pos.Core.Entities;
using pos.Core.Model;
using Rachis.Messages;
using System.Data;

namespace pos.Infrastructure.Repositories
{
    public class SaveCategory 
    {
        private readonly DataAccessHelper _db;
    
        public SaveCategory(DataAccessHelper db)
        {
            _db = db;
        }

        // GET ALL
        public List<Categories> GetAllCategories()
        {
            List<Categories> categoryList = new List<Categories>();

            categoryList = _db.GetData<Categories>("[dbo].[usp_GetCategoryList]", CommandType.StoredProcedure);

            return categoryList;        
        }

        // GET ONE
        public Categories GetCategoryById(int id)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", id)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_GetCategoryById]",
                parameters,
                CommandType.StoredProcedure
            );

            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
                return null;

            DataRow row = ds.Tables[0].Rows[0];

            return new Categories
            {
                cat_id = Convert.ToInt32(row["cat_id"]),
                category_name = row["category_name"].ToString(),
                status = row["status"].ToString(),
                description = row["description"].ToString()
            };
        }

        // INSERT, and UPDATE
        public MessageResult SaveUpdateDeleteCat(Categories category, string? Actions)
        {
            SqlParameter[] parameters = new SqlParameter[] {
                    new SqlParameter("@pCatId", category.cat_id),
                    new SqlParameter("@pCategoryName", category.category_name),
                    new SqlParameter("@pStatus", category.status),
                    new SqlParameter("@pDescription", category.description),
                    new SqlParameter("@pActions", Actions),
                };

            var result = _db.GetData<MessageResult>("[dbo].[usp_SaveUpdateDeleteCategory]", parameters, CommandType.StoredProcedure)[0];
         
            return result;
        }

        // DELETE
        public MessageResult DeleteCat(int id, string? Actions)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@pCatId", id),
                new SqlParameter("@pActions", Actions)
            };

            DataSet ds = _db.GetData(
                "[dbo].[usp_SaveUpdateDeleteCategory]",
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
