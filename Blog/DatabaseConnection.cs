using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog
{
    public class DatabaseConnection
    {
        public NpgsqlConnection GetConnection()
        {
            string connectionString = "Server=172.24.101.63;Port=5432;Database=PersonalBlog;User Id=postgres;Password=bzx2003430;";
            NpgsqlConnection conn = new NpgsqlConnection(connectionString);
            return conn;
        }
    }
}