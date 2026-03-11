using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logic
{
    internal class InsertCategory
    {
        public static void InsertCategory()
        {
            var data = SaveCategoryRep.getSqlData();
            try
            {
                if (data.Tables.Count > 0)
                {
                    // Assuming you want to insert into the first table
                    DataTable sqlTable = data.Tables[0];
                   // Console.WriteLine($"Bulk insert of {sqlTable.Rows.Count} rows to world table");

                    foreach (DataRow row in sqlTable.Rows)
                    {
                        SaveCategoryRep.SaveinMySql(new Province
                        {
                            province_name = row["province_name"].ToString(), // Replace with actual column name
                            post_code = Convert.ToInt32(row["post_code"].ToString()) // Replace with actual column name
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }

            // await Task.Delay(1000);
        }
    }
}
