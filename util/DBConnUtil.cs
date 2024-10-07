using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System;
using CrimeAnalysisReportingSystem2.exception;



namespace CrimeAnalysisReportingSystem2.util
{
    public class DBConnUtil
    {
        public static SqlConnection GetConnection()
        {
            try
            {
                string connectionString = DBPropertyUtil.GetPropertyString("appsettings.json");
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                return connection;
            }
            catch (SqlException ex)
            {
                throw new DatabaseException("Failed to connect to the database: " + ex.Message, ex);
            }
        }
    }
}
