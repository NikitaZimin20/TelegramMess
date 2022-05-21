using System.Data;
using System.Data.SQLite;
using DataWorker;
using System.IO;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;

namespace DataWorker
{
   public class DataTableWorker
    {
        SqlConnection connection;
        
        internal string GetConnection()
        {
            string output = "";
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("Configuration.json");
            var config = builder.Build();
            output = config["Default"];
            return output;
        }
        public DataTable Open(string quary)
        {
            if (Connect(GetConnection()))
            {
                return FillData(quary);
            }
            return null;
        }
        private bool Connect(string fileName)
        {
            try
            {
                connection = new SqlConnection(fileName);
                connection.Open();
                return true;
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"Ошибка доступа к базе данных. Исключение: {ex.Message}");
                return false;
            }
        }
        private DataTable FillData(string query)
        {

            SqlDataAdapter adapter = new SqlDataAdapter(query, connection);
            var table = new DataTable();
            adapter.Fill(table);
            return table;

        }
    }
}
