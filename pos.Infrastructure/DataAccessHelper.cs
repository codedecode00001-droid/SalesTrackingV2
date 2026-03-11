using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;


namespace pos.Infrastructure
{
    public class DataAccessHelper
    {
        private readonly string _connString; // other way if _connecString is not null "private readonly string? _connString;"
        private SqlConnection sqlConnection;
        private SqlCommand? sqlCommand;
        private SqlTransaction? sqlTransaction;

        public CommandType cmdType { get; set; }
        public string? SQL { get; set; }
        public string[,]? ParamAndValue { get; set; }

        public DataAccessHelper(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            _connString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("DefaultConnection not found in configuration.");
            sqlConnection = new SqlConnection(_connString);
        }

        public void Open()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }

            sqlConnection.Open();
        }

        public void Close()
        {
            if (sqlConnection.State == ConnectionState.Open)
            {
                sqlConnection.Close();
            }
        }

        public string ExecuteScalar(string spName, CommandType commandType)
        {
            return this.ExecuteScalar(spName, new string[] { }, new string[] { }, commandType);
        }

        public string ExecuteScalar(string spName, string[] paramName, string[] paramValues, CommandType commandType)
        {
            if (paramName.Length != paramValues.Length)
                throw new ArgumentException("Parameter names and values must match.");

            using (SqlConnection conn = new SqlConnection(_connString))
            using (SqlCommand cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = commandType;
                cmd.CommandTimeout = 30; // 30 seconds

                for (int i = 0; i < paramName.Length; i++)
                {
                    cmd.Parameters.AddWithValue(paramName[i], paramValues[i]);
                }

                conn.Open();
                var result = cmd.ExecuteScalar();
                return result?.ToString() ?? string.Empty;
            }
        }

        public List<T> GetData<T>(string spName, CommandType commandType) where T : new()
        {
            return this.GetData<T>(spName, new string[] { }, new string[] { }, commandType);
        }

        public List<T> GetData<T>(string spName, string[] paramName, string[] paramValues, CommandType commandType) where T : new()
        {
            List<T> retList = new List<T>();

            using (SqlConnection conn = new SqlConnection(_connString))
            using (SqlCommand cmd = new SqlCommand(spName, conn))
            {
                cmd.CommandType = commandType;
                cmd.CommandTimeout = 30;

                for (int i = 0; i < paramName.Length; i++)
                {
                    cmd.Parameters.Add(paramName[i], SqlDbType.VarChar).Value = paramValues[i];
                }

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        T item = new T();
                        foreach (var prop in typeof(T).GetProperties())
                        {
                            try
                            {
                                if (!reader.IsDBNull(reader.GetOrdinal(prop.Name)))
                                {
                                    Type targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                    prop.SetValue(item, Convert.ChangeType(reader[prop.Name], targetType));
                                }
                            }
                            catch
                            {
                                // optional: log mapping errors
                            }
                        }
                        retList.Add(item);
                    }
                }
            }

            return retList;
        }


        public List<T> GetData<T>(string spName, SqlParameter[] parameters, CommandType commandType)
        {
            List<T> retList = new List<T>();

            Open();
            sqlCommand = new SqlCommand(spName);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = commandType;
            sqlCommand.Parameters.AddRange(parameters);
            sqlCommand.CommandTimeout = 0;

            using (SqlDataReader reader = sqlCommand.ExecuteReader())
            {
                while (reader.Read())
                {
                    var item = Activator.CreateInstance<T>();

                    foreach (var property in typeof(T).GetProperties())
                    {
                        try
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                            {
                                Type convertTo = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
                                property.SetValue(item, Convert.ChangeType(reader[property.Name], convertTo), null);
                            }
                        }
                        catch
                        {

                        }

                    }

                    retList.Add(item);
                }
            }

            Close();

            return retList;
        }

        public DataSet GetData(string spName, SqlParameter[] parameters, CommandType commandType)
        {
            DataSet ds = new DataSet();

            Open();

            sqlCommand = new SqlCommand(spName, sqlConnection);
            sqlCommand.CommandType = commandType;
            sqlCommand.Parameters.AddRange(parameters);
            sqlCommand.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(sqlCommand);
            da.Fill(ds);

            Close();

            return ds;
        }

        public DataTable GetData()
        {
            try
            {
                DataTable dt = new DataTable();
                Open();
                using (SqlConnection con = new SqlConnection(_connString))
                {
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        cmd.CommandText = this.SQL;
                        cmd.CommandType = cmdType;
                        cmd.CommandTimeout = 0;

                        if (this.ParamAndValue != null)
                        {
                            for (int i = 0; i <= ParamAndValue.GetUpperBound(0); i++)
                            {
                                cmd.Parameters.AddWithValue(this.ParamAndValue[i, 0].ToString().Trim(), this.ParamAndValue[i, 1].ToString().Trim());
                            }
                        }

                        cmd.Connection.Open();
                        using (SqlDataAdapter sda = new SqlDataAdapter(cmd))
                        {
                            sda.Fill(dt);
                        }
                        cmd.Connection.Close();
                    }
                }
                // Handle Exception
                return dt;
            }
            catch (Exception ex)
            {
                string x = ex.Message;
                return new DataTable();
            }
        }

        public int ExecuteNonQuery(string spName, string[] paramName, string[] paramValues, CommandType commandType)
        {
            Open();

            sqlCommand = new SqlCommand(spName);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = commandType;

            for (int i = 0; i < paramName.Length; i++)
            {
                sqlCommand.Parameters.Add(new SqlParameter(paramName[i], paramValues[i]));
            }

            int ret = sqlCommand.ExecuteNonQuery();

            Close();
            return ret;
        }

        public int UploadImage(string spName, string[] paramName, string id, byte[] image, CommandType commandType)
        {
            Open();

            sqlCommand = new SqlCommand(spName);
            sqlCommand.Connection = sqlConnection;
            sqlCommand.CommandType = commandType;

            SqlParameter param0 = new SqlParameter(paramName[0], SqlDbType.VarChar);
            param0.Value = id;
            sqlCommand.Parameters.Add(param0);

            SqlParameter param1 = new SqlParameter(paramName[1], SqlDbType.Image);
            param1.Value = image;
            sqlCommand.Parameters.Add(param1);

            int ret = sqlCommand.ExecuteNonQuery();

            Close();
            return ret;
        }

        public void BeginTransaction()
        {
            if (sqlConnection == null)
                sqlConnection = new SqlConnection(_connString);

            if (sqlConnection.State != ConnectionState.Open)
                sqlConnection.Open();

            sqlTransaction = sqlConnection.BeginTransaction();
        }

        public void CommitTransaction()
        {
            sqlTransaction?.Commit();
            sqlConnection?.Close();
            sqlTransaction = null;
        }

        public void RollbackTransaction()
        {
            sqlTransaction?.Rollback();
            sqlConnection?.Close();
            sqlTransaction = null;
        }
    }
}
