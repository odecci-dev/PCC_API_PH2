using API_PCC.ApplicationModels;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using NuGet.Protocol.Core.Types;
using System.Data;
using System.Data.SqlClient;
using System.Text.Json;
using SqlConnection = System.Data.SqlClient.SqlConnection;
using SqlParameter = System.Data.SqlClient.SqlParameter;
using SqlCommand = System.Data.SqlClient.SqlCommand;
using SqlDataAdapter = System.Data.SqlClient.SqlDataAdapter;
using SqlException = System.Data.SqlClient.SqlException;

namespace API_PCC.Manager
{
    public class DbManager
    {
        public SqlConnection Connection { get; set; }
        public SqlConnection conn = new SqlConnection();
        public SqlCommand cmd = new SqlCommand();
        public SqlDataAdapter da = new SqlDataAdapter();
        string cnnstr = "";
        DBConn db = new DBConn();


        public void ConnectioStr()
        {
            //cnnstr = "Data Source=LEARI-PC;Initial Catalog=PCC_DEV;User ID=pcc-server;Password=pccdev12345!";// local
            //cnnstr = "Data Source=EC2AMAZ-V52FJK1;Initial Catalog=PCC_DEV;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server
            //cnnstr = "Data Source=WIN-RLNNENEQU3R;Initial Catalog=PCC_SIT;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server
            //cnnstr = "Data Source=WIN-RLNNENEQU3R;Initial Catalog=PCC_DEV_ENV;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server Env
            //cnnstr = "Data Source=EC2AMAZ-V52FJK1;Initial Catalog=PCC_SIT;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server ...
            //cnnstr = "Data Source=DESKTOP-9P0BJ07;Initial Catalog=PCC_SIT;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server 
            //cnnstr = "Data Source=DESKTOP-9P0BJ07;Initial Catalog=PCC_DEV;User ID=pcc-server;Password=pccdev1234!"; //Odecci Server
            //cnnstr = "Data Source=DESKTOP-0SN0AHC;Initial Catalog=PCC_DEV;User ID=pcc-server;Password=pccdev1234!"; //Odecci pc 
            cnnstr = "Server=(localdb)\\ProjectModels;Database=PCC_SIT;Trusted_Connection=True;MultipleActiveResultSets=True;"; // brandon loacl db
            conn = new SqlConnection(cnnstr);
        }
        public DataSet SelectDb(string value)
        {
            DataSet ds = new DataSet();
            try
            {
                ConnectioStr();
                SQLConnOpen();
                cmd.CommandTimeout = 0;
                cmd.CommandText = value;
                da.SelectCommand = cmd;
                da.Fill(ds);

            }
            catch (Exception e)
            {
                /*DataTable dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(new object[] { e.Message });
                ds.Tables.Add(dt);*/
                conn.Close();
                conn = null;
                throw e;
            }

            conn.Close();
            conn = null;
            return ds;
        }

        public DataTable SelectDb_WithParamAndSorting(string strSql, SortByModel sortByModel, params SqlParameter[] sqlParams)
        {
            DataTable result = new DataTable();
            DataSet ds = new DataSet();
            try
            {
                ConnectioStr();
                SQLConnOpen();
                cmd.CommandTimeout = 0;
                cmd.CommandText = strSql;
                cmd.Parameters.Clear();

                foreach (SqlParameter sqlParameterInput in sqlParams)
                {
                    cmd.Parameters.Add(sqlParameterInput);
                }

                da.SelectCommand = cmd;
                da.Fill(ds);

                result = ds.Tables[0];

                // Sorting of results 
                // Moved sorting here for us to gain a understandable exception in case of sorting error

                result.DefaultView.Sort = "ID DESC";

                if (sortByModel != null && !sortByModel.Field.IsNullOrEmpty())
                {
                    result.DefaultView.Sort = sortByModel.Field + " " + sortByModel.Sort;
                }

                result = result.DefaultView.ToTable();

            }
            catch (Exception e)
            {
                /*DataTable dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(new object[] { e.Message });
                ds.Tables.Add(dt);*/
                conn.Close();
                conn = null;
                throw e;

            }

            conn.Close();
            conn = null;

            return result;
        }

        public void SQLConnOpen()
        {
            if (conn.State != ConnectionState.Closed) conn.Close();
            conn.Open();
            cmd.Connection = conn;
            cmd.CommandTimeout = 0;
            cmd.CommandType = CommandType.Text;
        }


        public DataSet SelectDb_SP(string strSql, params IDataParameter[] sqlParams)
        {
            DataSet ds = new DataSet();
            int ctr = 0;
        retry:
            try
            {
                ConnectioStr();
                SqlCommand cmd = new SqlCommand(strSql, conn);

                conn.Open();

                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                if (sqlParams != null)
                {

                    foreach (IDataParameter para in sqlParams)
                    {
                        SqlParameter nameParam = new SqlParameter(para.ParameterName, para.Value);
                        cmd.Parameters.Add(nameParam);
                    }
                }
                da.SelectCommand = cmd;
                da.Fill(ds);
                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                if (ctr <= 3)
                {
                    Thread.Sleep(1000);
                    ctr++;
                    goto retry;
                }

                /*DataTable dt = new DataTable();
                dt.Columns.Add("Error");
                dt.Rows.Add(new object[] { ex.Message });
                ds.Tables.Add(dt);*/
                conn.Close();
                conn = null;
                throw ex;
            }

            conn.Close();
            return ds;
        }
        public string DB_WithParam(string strSql, params IDataParameter[] sqlParams)
        {
            try
            {
                ConnectioStr();
                SqlCommand cmd = new SqlCommand(strSql, conn);

                conn.Open();

                cmd.Connection = conn;
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.Text;
                if (sqlParams != null)
                {
                    foreach (IDataParameter para in sqlParams)
                    {
                        cmd.Parameters.Add(para);
                    }
                }
                //   cmd.ExecuteNonQuery();
                int rowsaffected = cmd.ExecuteNonQuery();
                conn.Close();
                string filePath = @"C:\data\SQL_Error.json"; // Replace with your desired file path
                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(rowsaffected + " Successfully"));
                return rowsaffected + " Successfully";

            }
            catch (SqlException ex)
            {
                conn.Close();
                conn = null;
                string filePath = @"C:\data\SQL_Error.json"; // Replace with your desired file path

                System.IO.File.WriteAllText(filePath, JsonSerializer.Serialize(ex.Message + "!"));
                return ex.Message + "!";
            }
        }

		public async Task<string> DB_WithParamAsync(string query, params SqlParameter[] parameters)
		{
			ConnectioStr();

			using (SqlConnection conn = new SqlConnection(cnnstr))
			{
				await conn.OpenAsync();
				using (SqlCommand cmd = new SqlCommand(query, conn))
				{
					cmd.Parameters.AddRange(parameters); // Add parameters properly
					int rowsAffected = await cmd.ExecuteNonQueryAsync();
					return rowsAffected > 0 ? "Update successful" : "No rows updated";
				}
			}
		}
	}
}
