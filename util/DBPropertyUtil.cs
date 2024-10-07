using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

using System.IO;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;

namespace CrimeAnalysisReportingSystem2.util
{
    public class DBPropertyUtil
    {
        public static string GetPropertyString(string fileName)
        {
            var jsonData = File.ReadAllText(fileName);
            var jsonObject = JObject.Parse(jsonData);
            string connectionString = jsonObject["ConnectionString"].ToString();
            return connectionString;
        }
    }
}

