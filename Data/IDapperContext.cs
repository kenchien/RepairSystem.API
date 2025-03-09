using System.Data;

namespace RepairSystem.API.Data
{
    /// <summary>
    /// Dapper 資料庫連線上下文接口
    /// </summary>
    public interface IDapperContext
    {
        /// <summary>
        /// 創建資料庫連線
        /// </summary>
        /// <returns>數據庫連線</returns>
        IDbConnection CreateConnection();
    }
} 