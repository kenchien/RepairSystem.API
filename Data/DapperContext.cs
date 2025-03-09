using System.Data;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace RepairSystem.API.Data
{
    /// <summary>
    /// Dapper 資料庫連線上下文
    /// </summary>
    public class DapperContext
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="configuration">配置對象</param>
        public DapperContext(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DapperConnection")!;
        }

        /// <summary>
        /// 創建資料庫連線
        /// </summary>
        /// <returns>數據庫連線</returns>
        public IDbConnection CreateConnection()
            => new MySqlConnection(_connectionString);
    }
} 