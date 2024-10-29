using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EAMI.DataEngine
{
    public class DapperContext
    {
        private readonly string _connectionString;
        public DapperContext(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("") ?? throw new ArgumentNullException(nameof(configuration), "Connection string cannot be null");
        }

        public IDbConnection CreateConnection()
        {
            IDbConnection connection = new SqlConnection(_connectionString);
            connection.Open();
            return connection;
        }
    }
}
