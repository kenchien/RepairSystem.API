using RepairSystem.API.Models;

namespace RepairSystem.API.Data
{
    /// <summary>
    /// 維修系統數據訪問接口，定義了對報修單、設備和用戶的數據操作
    /// </summary>
    public interface IRepairRepository
    {
        // 報修單相關
        /// <summary>
        /// 獲取所有報修單
        /// </summary>
        /// <returns>報修單集合</returns>
        Task<IEnumerable<RepairTicket>> GetAllTicketsAsync();
        
        /// <summary>
        /// 根據ID獲取報修單
        /// </summary>
        /// <param name="id">報修單ID</param>
        /// <returns>報修單對象，如果不存在則返回null</returns>
        Task<RepairTicket?> GetTicketByIdAsync(int id);
        
        /// <summary>
        /// 創建新的報修單
        /// </summary>
        /// <param name="ticket">報修單對象</param>
        /// <returns>創建後的報修單</returns>
        Task<RepairTicket> CreateTicketAsync(RepairTicket ticket);
        
        /// <summary>
        /// 更新報修單信息
        /// </summary>
        /// <param name="ticket">報修單對象</param>
        /// <returns>更新後的報修單</returns>
        Task<RepairTicket> UpdateTicketAsync(RepairTicket ticket);
        
        /// <summary>
        /// 刪除報修單
        /// </summary>
        /// <param name="id">報修單ID</param>
        /// <returns>是否刪除成功</returns>
        Task<bool> DeleteTicketAsync(int id);
        
        /// <summary>
        /// 獲取用戶的所有報修單
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <returns>報修單集合</returns>
        Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId);
        
        /// <summary>
        /// 獲取技術人員負責的所有報修單
        /// </summary>
        /// <param name="technicianId">技術人員ID</param>
        /// <returns>報修單集合</returns>
        Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId);
        
        // 設備相關
        /// <summary>
        /// 獲取所有設備，支持分頁和篩選
        /// </summary>
        /// <param name="queryParams">查詢參數</param>
        /// <returns>設備集合</returns>
        Task<IEnumerable<Equipment>> GetAllEquipmentAsync(EquipmentQueryParams queryParams);
        
        /// <summary>
        /// 根據ID獲取設備
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>設備對象，如果不存在則返回null</returns>
        Task<Equipment?> GetEquipmentByIdAsync(int id);
        
        /// <summary>
        /// 添加新設備
        /// </summary>
        /// <param name="equipment">設備對象</param>
        /// <returns>添加後的設備</returns>
        Task<Equipment> AddEquipmentAsync(Equipment equipment);
        
        /// <summary>
        /// 更新設備信息
        /// </summary>
        /// <param name="equipment">設備對象</param>
        /// <returns>更新後的設備</returns>
        Task<Equipment> UpdateEquipmentAsync(Equipment equipment);
        
        /// <summary>
        /// 刪除設備
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>是否刪除成功</returns>
        Task<bool> DeleteEquipmentAsync(int id);
        
        /// <summary>
        /// 獲取設備總數
        /// </summary>
        /// <param name="queryParams">查詢參數</param>
        /// <returns>設備總數</returns>
        Task<int> GetEquipmentTotalCountAsync(EquipmentQueryParams queryParams);
        
        /// <summary>
        /// 獲取所有設備類型
        /// </summary>
        /// <returns>設備類型集合</returns>
        Task<IEnumerable<string>> GetDeviceTypesAsync();
        
        /// <summary>
        /// 獲取所有部門
        /// </summary>
        /// <returns>部門集合</returns>
        Task<IEnumerable<string>> GetDepartmentsAsync();
        
        // 用戶相關
        /// <summary>
        /// 根據ID獲取用戶
        /// </summary>
        /// <param name="id">用戶ID</param>
        /// <returns>用戶對象，如果不存在則返回null</returns>
        Task<User?> GetUserByIdAsync(int id);
        
        /// <summary>
        /// 根據用戶名獲取用戶
        /// </summary>
        /// <param name="username">用戶名</param>
        /// <returns>用戶對象，如果不存在則返回null</returns>
        Task<User?> GetUserByUsernameAsync(string username);
        
        /// <summary>
        /// 根據電子郵件獲取用戶
        /// </summary>
        /// <param name="email">電子郵件</param>
        /// <returns>用戶對象，如果不存在則返回null</returns>
        Task<User?> GetUserByEmailAsync(string email);
        
        /// <summary>
        /// 獲取所有用戶
        /// </summary>
        /// <returns>用戶集合</returns>
        Task<IEnumerable<User>> GetAllUsersAsync();
        
        /// <summary>
        /// 獲取所有技術人員
        /// </summary>
        /// <returns>技術人員集合</returns>
        Task<IEnumerable<User>> GetTechniciansAsync();
        
        /// <summary>
        /// 創建新用戶
        /// </summary>
        /// <param name="user">用戶對象</param>
        /// <returns>創建後的用戶</returns>
        Task<User> CreateUserAsync(User user);
        
        /// <summary>
        /// 更新用戶信息
        /// </summary>
        /// <param name="user">用戶對象</param>
        /// <returns>更新後的用戶</returns>
        Task<User> UpdateUserAsync(User user);
        
        // 保存變更
        /// <summary>
        /// 保存所有變更到數據庫
        /// </summary>
        /// <returns>是否保存成功</returns>
        Task<bool> SaveChangesAsync();
    }
} 