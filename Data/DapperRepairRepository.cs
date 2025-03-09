using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using RepairSystem.API.Models;

namespace RepairSystem.API.Data
{
    /// <summary>
    /// 使用 Dapper 實現的維修系統存儲庫
    /// </summary>
    public class DapperRepairRepository : IRepairRepository
    {
        private readonly DapperContext _context;
        private readonly ILogger<DapperRepairRepository> _logger;

        /// <summary>
        /// 構造函數
        /// </summary>
        /// <param name="context">Dapper 連線上下文</param>
        /// <param name="logger">日誌記錄器</param>
        public DapperRepairRepository(DapperContext context, ILogger<DapperRepairRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取所有報修單
        /// </summary>
        /// <returns>報修單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetAllTicketsAsync()
        {
            try
            {
                var query = @"
                    SELECT r.*, u.*, h.*, e.*
                    FROM RepairTickets r
                    LEFT JOIN Users u ON r.UserId = u.Id
                    LEFT JOIN Users h ON r.HandledBy = h.Id
                    LEFT JOIN Equipment e ON r.EquipmentId = e.EquipmentId
                    ORDER BY r.CreatedAt DESC";

                using var connection = _context.CreateConnection();
                
                var ticketDict = new Dictionary<int, RepairTicket>();
                
                var tickets = await connection.QueryAsync<RepairTicket, User, User, Equipment, RepairTicket>(
                    query,
                    (ticket, user, handler, equipment) => 
                    {
                        if (!ticketDict.TryGetValue(ticket.Id, out var existingTicket))
                        {
                            existingTicket = ticket;
                            existingTicket.User = user;
                            existingTicket.Handler = handler;
                            existingTicket.Equipment = equipment;
                            ticketDict.Add(existingTicket.Id, existingTicket);
                        }
                        return existingTicket;
                    },
                    splitOn: "Id,Id,EquipmentId"
                );

                return ticketDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有報修單時發生錯誤");
                return new List<RepairTicket>();
            }
        }

        /// <summary>
        /// 根據 ID 獲取報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <returns>報修單</returns>
        public async Task<RepairTicket?> GetTicketByIdAsync(int id)
        {
            try
            {
                var query = @"
                    SELECT r.*, u.*, h.*, e.*
                    FROM RepairTickets r
                    LEFT JOIN Users u ON r.UserId = u.Id
                    LEFT JOIN Users h ON r.HandledBy = h.Id
                    LEFT JOIN Equipment e ON r.EquipmentId = e.EquipmentId
                    WHERE r.Id = @Id";

                using var connection = _context.CreateConnection();
                
                var ticketDict = new Dictionary<int, RepairTicket>();
                
                var tickets = await connection.QueryAsync<RepairTicket, User, User, Equipment, RepairTicket>(
                    query,
                    (ticket, user, handler, equipment) => 
                    {
                        if (!ticketDict.TryGetValue(ticket.Id, out var existingTicket))
                        {
                            existingTicket = ticket;
                            existingTicket.User = user;
                            existingTicket.Handler = handler;
                            existingTicket.Equipment = equipment;
                            ticketDict.Add(existingTicket.Id, existingTicket);
                        }
                        return existingTicket;
                    },
                    new { Id = id },
                    splitOn: "Id,Id,EquipmentId"
                );

                var ticket = ticketDict.Values.FirstOrDefault();
                
                // 加載附件
                if (ticket != null)
                {
                    var attachmentsQuery = "SELECT * FROM AttachmentFiles WHERE TicketId = @TicketId";
                    var attachments = await connection.QueryAsync<AttachmentFile>(attachmentsQuery, new { TicketId = id });
                    ticket.Attachments = attachments.ToList();
                }
                
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取 ID:{id} 的報修單時發生錯誤");
                return null;
            }
        }
        
        /// <summary>
        /// 創建新報修單
        /// </summary>
        /// <param name="ticket">報修單</param>
        /// <returns>創建的報修單</returns>
        public async Task<RepairTicket> CreateTicketAsync(RepairTicket ticket)
        {
            try
            {
                var query = @"
                    INSERT INTO RepairTickets (
                        Title, Description, DeviceType, DeviceNumber, 
                        Problem, Solution, CreatedAt, UpdatedAt, 
                        CreateTime, UpdateTime, Status, Priority, 
                        Location, EquipmentId, UserId, HandledBy
                    ) VALUES (
                        @Title, @Description, @DeviceType, @DeviceNumber,
                        @Problem, @Solution, @CreatedAt, @UpdatedAt,
                        @CreateTime, @UpdateTime, @Status, @Priority,
                        @Location, @EquipmentId, @UserId, @HandledBy
                    );
                    SELECT LAST_INSERT_ID();";

                using var connection = _context.CreateConnection();
                
                // 設置時間
                var now = DateTime.Now;
                ticket.CreatedAt = now;
                ticket.CreateTime = now;
                ticket.UpdatedAt = now;
                ticket.UpdateTime = now;
                
                // 執行插入並獲取新ID
                var id = await connection.QuerySingleAsync<int>(query, ticket);
                ticket.Id = id;
                
                return ticket;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建報修單時發生錯誤");
                throw;
            }
        }
        
        /// <summary>
        /// 更新報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <param name="ticket">報修單</param>
        /// <returns>更新後的報修單</returns>
        public async Task<RepairTicket?> UpdateTicketAsync(int id, RepairTicket ticket)
        {
            try
            {
                var query = @"
                    UPDATE RepairTickets
                    SET Title = @Title,
                        Description = @Description,
                        DeviceType = @DeviceType,
                        DeviceNumber = @DeviceNumber,
                        Problem = @Problem,
                        Solution = @Solution,
                        UpdatedAt = @UpdatedAt,
                        UpdateTime = @UpdateTime,
                        Status = @Status,
                        Priority = @Priority,
                        Location = @Location,
                        EquipmentId = @EquipmentId,
                        HandledBy = @HandledBy
                    WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                
                // 更新時間
                var now = DateTime.Now;
                ticket.UpdatedAt = now;
                ticket.UpdateTime = now;
                
                // 執行更新
                var result = await connection.ExecuteAsync(query, new {
                    Id = id,
                    ticket.Title,
                    ticket.Description,
                    ticket.DeviceType,
                    ticket.DeviceNumber,
                    ticket.Problem,
                    ticket.Solution,
                    ticket.UpdatedAt,
                    ticket.UpdateTime,
                    ticket.Status,
                    ticket.Priority,
                    ticket.Location,
                    ticket.EquipmentId,
                    ticket.HandledBy
                });
                
                return result > 0 ? await GetTicketByIdAsync(id) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新 ID:{id} 的報修單時發生錯誤");
                return null;
            }
        }
        
        /// <summary>
        /// 更新報修單
        /// </summary>
        /// <param name="ticket">報修單</param>
        /// <returns>更新後的報修單</returns>
        public async Task<RepairTicket> UpdateTicketAsync(RepairTicket ticket)
        {
            var result = await UpdateTicketAsync(ticket.Id, ticket);
            if (result == null)
            {
                throw new Exception($"更新報修單 ID:{ticket.Id} 失敗");
            }
            return result;
        }
        
        /// <summary>
        /// 刪除報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteTicketAsync(int id)
        {
            try
            {
                var query = "DELETE FROM RepairTickets WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除 ID:{id} 的報修單時發生錯誤");
                return false;
            }
        }
        
        /// <summary>
        /// 獲取用戶的報修單
        /// </summary>
        /// <param name="userId">用戶 ID</param>
        /// <returns>報修單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId)
        {
            try
            {
                var query = @"
                    SELECT r.*, u.*, h.*, e.*
                    FROM RepairTickets r
                    LEFT JOIN Users u ON r.UserId = u.Id
                    LEFT JOIN Users h ON r.HandledBy = h.Id
                    LEFT JOIN Equipment e ON r.EquipmentId = e.EquipmentId
                    WHERE r.UserId = @UserId
                    ORDER BY r.CreatedAt DESC";

                using var connection = _context.CreateConnection();
                
                var ticketDict = new Dictionary<int, RepairTicket>();
                
                var tickets = await connection.QueryAsync<RepairTicket, User, User, Equipment, RepairTicket>(
                    query,
                    (ticket, user, handler, equipment) => 
                    {
                        if (!ticketDict.TryGetValue(ticket.Id, out var existingTicket))
                        {
                            existingTicket = ticket;
                            existingTicket.User = user;
                            existingTicket.Handler = handler;
                            existingTicket.Equipment = equipment;
                            ticketDict.Add(existingTicket.Id, existingTicket);
                        }
                        return existingTicket;
                    },
                    new { UserId = userId },
                    splitOn: "Id,Id,EquipmentId"
                );

                return ticketDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取用戶 ID:{userId} 的報修單時發生錯誤");
                return new List<RepairTicket>();
            }
        }
        
        /// <summary>
        /// 獲取技術人員的報修單
        /// </summary>
        /// <param name="technicianId">技術人員 ID</param>
        /// <returns>報修單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId)
        {
            try
            {
                var query = @"
                    SELECT r.*, u.*, h.*, e.*
                    FROM RepairTickets r
                    LEFT JOIN Users u ON r.UserId = u.Id
                    LEFT JOIN Users h ON r.HandledBy = h.Id
                    LEFT JOIN Equipment e ON r.EquipmentId = e.EquipmentId
                    WHERE r.HandledBy = @TechnicianId
                    ORDER BY r.CreatedAt DESC";

                using var connection = _context.CreateConnection();
                
                var ticketDict = new Dictionary<int, RepairTicket>();
                
                var tickets = await connection.QueryAsync<RepairTicket, User, User, Equipment, RepairTicket>(
                    query,
                    (ticket, user, handler, equipment) => 
                    {
                        if (!ticketDict.TryGetValue(ticket.Id, out var existingTicket))
                        {
                            existingTicket = ticket;
                            existingTicket.User = user;
                            existingTicket.Handler = handler;
                            existingTicket.Equipment = equipment;
                            ticketDict.Add(existingTicket.Id, existingTicket);
                        }
                        return existingTicket;
                    },
                    new { TechnicianId = technicianId },
                    splitOn: "Id,Id,EquipmentId"
                );

                return ticketDict.Values;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取技術人員 ID:{technicianId} 的報修單時發生錯誤");
                return new List<RepairTicket>();
            }
        }

        // 設備相關方法
        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(EquipmentQueryParams queryParams)
        {
            try
            {
                var whereClause = new List<string>();
                var parameters = new DynamicParameters();
                
                // 過濾條件
                if (!string.IsNullOrEmpty(queryParams.DeviceType))
                {
                    whereClause.Add("DeviceType = @DeviceType");
                    parameters.Add("DeviceType", queryParams.DeviceType);
                }
                
                if (!string.IsNullOrEmpty(queryParams.Department))
                {
                    whereClause.Add("Department = @Department");
                    parameters.Add("Department", queryParams.Department);
                }
                
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    whereClause.Add("Status = @Status");
                    parameters.Add("Status", queryParams.Status);
                }
                
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    whereClause.Add("(Name LIKE @SearchTerm OR SerialNumber LIKE @SearchTerm OR Notes LIKE @SearchTerm)");
                    parameters.Add("SearchTerm", $"%{queryParams.SearchTerm}%");
                }
                
                // 構建查詢
                var query = "SELECT * FROM Equipment";
                if (whereClause.Any())
                {
                    query += " WHERE " + string.Join(" AND ", whereClause);
                }
                
                // 分頁
                query += " ORDER BY Name";
                query += " LIMIT @PageSize OFFSET @Offset";
                
                parameters.Add("PageSize", queryParams.PageSize);
                parameters.Add("Offset", (queryParams.Page - 1) * queryParams.PageSize);
                
                using var connection = _context.CreateConnection();
                var equipment = await connection.QueryAsync<Equipment>(query, parameters);
                
                return equipment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取設備列表時發生錯誤");
                return new List<Equipment>();
            }
        }
        
        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM Equipment WHERE EquipmentId = @Id";
                using var connection = _context.CreateConnection();
                
                var equipment = await connection.QuerySingleOrDefaultAsync<Equipment>(query, new { Id = id });
                return equipment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取 ID:{id} 的設備時發生錯誤");
                return null;
            }
        }
        
        public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
        {
            try
            {
                var query = @"
                    INSERT INTO Equipment (
                        Name, DeviceType, SerialNumber, Status, Department,
                        Location, PurchaseDate, LastMaintenanceDate, Notes, 
                        ImageUrl, CreateTime, UpdateTime
                    ) VALUES (
                        @Name, @DeviceType, @SerialNumber, @Status, @Department,
                        @Location, @PurchaseDate, @LastMaintenanceDate, @Notes,
                        @ImageUrl, @CreateTime, @UpdateTime
                    );
                    SELECT LAST_INSERT_ID();";

                using var connection = _context.CreateConnection();
                
                // 設置時間
                var now = DateTime.Now;
                equipment.CreateTime = now;
                equipment.UpdateTime = now;
                
                // 執行插入並獲取新ID
                var id = await connection.QuerySingleAsync<int>(query, equipment);
                equipment.EquipmentId = id;
                
                return equipment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加設備時發生錯誤");
                throw;
            }
        }
        
        public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
        {
            try
            {
                var query = @"
                    UPDATE Equipment
                    SET Name = @Name,
                        DeviceType = @DeviceType,
                        SerialNumber = @SerialNumber,
                        Status = @Status,
                        Department = @Department,
                        Location = @Location,
                        PurchaseDate = @PurchaseDate,
                        LastMaintenanceDate = @LastMaintenanceDate,
                        Notes = @Notes,
                        ImageUrl = @ImageUrl,
                        UpdateTime = @UpdateTime
                    WHERE EquipmentId = @EquipmentId";

                using var connection = _context.CreateConnection();
                
                // 更新時間
                equipment.UpdateTime = DateTime.Now;
                
                // 執行更新
                await connection.ExecuteAsync(query, equipment);
                
                return equipment;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新 ID:{equipment.EquipmentId} 的設備時發生錯誤");
                throw;
            }
        }
        
        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            try
            {
                var query = "DELETE FROM Equipment WHERE EquipmentId = @Id";
                using var connection = _context.CreateConnection();
                
                var result = await connection.ExecuteAsync(query, new { Id = id });
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除 ID:{id} 的設備時發生錯誤");
                return false;
            }
        }
        
        public async Task<int> GetEquipmentTotalCountAsync(EquipmentQueryParams queryParams)
        {
            try
            {
                var whereClause = new List<string>();
                var parameters = new DynamicParameters();
                
                // 過濾條件
                if (!string.IsNullOrEmpty(queryParams.DeviceType))
                {
                    whereClause.Add("DeviceType = @DeviceType");
                    parameters.Add("DeviceType", queryParams.DeviceType);
                }
                
                if (!string.IsNullOrEmpty(queryParams.Department))
                {
                    whereClause.Add("Department = @Department");
                    parameters.Add("Department", queryParams.Department);
                }
                
                if (!string.IsNullOrEmpty(queryParams.Status))
                {
                    whereClause.Add("Status = @Status");
                    parameters.Add("Status", queryParams.Status);
                }
                
                if (!string.IsNullOrEmpty(queryParams.SearchTerm))
                {
                    whereClause.Add("(Name LIKE @SearchTerm OR SerialNumber LIKE @SearchTerm OR Notes LIKE @SearchTerm)");
                    parameters.Add("SearchTerm", $"%{queryParams.SearchTerm}%");
                }
                
                // 構建查詢
                var query = "SELECT COUNT(*) FROM Equipment";
                if (whereClause.Any())
                {
                    query += " WHERE " + string.Join(" AND ", whereClause);
                }
                
                using var connection = _context.CreateConnection();
                return await connection.ExecuteScalarAsync<int>(query, parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取設備總數時發生錯誤");
                return 0;
            }
        }
        
        public async Task<IEnumerable<string>> GetDeviceTypesAsync()
        {
            try
            {
                var query = "SELECT DISTINCT DeviceType FROM Equipment ORDER BY DeviceType";
                using var connection = _context.CreateConnection();
                
                return await connection.QueryAsync<string>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取設備類型時發生錯誤");
                return new List<string>();
            }
        }
        
        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            try
            {
                var query = "SELECT DISTINCT Department FROM Equipment ORDER BY Department";
                using var connection = _context.CreateConnection();
                
                return await connection.QueryAsync<string>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取部門列表時發生錯誤");
                return new List<string>();
            }
        }
        
        // 用戶相關方法
        public async Task<User?> GetUserByIdAsync(int id)
        {
            try
            {
                var query = "SELECT * FROM Users WHERE Id = @Id";
                using var connection = _context.CreateConnection();
                
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Id = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取 ID:{id} 的用戶時發生錯誤");
                return null;
            }
        }
        
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            try
            {
                var query = "SELECT * FROM Users WHERE Username = @Username";
                using var connection = _context.CreateConnection();
                
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Username = username });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取用戶名:{username} 的用戶時發生錯誤");
                return null;
            }
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            try
            {
                var query = "SELECT * FROM Users WHERE Email = @Email";
                using var connection = _context.CreateConnection();
                
                return await connection.QuerySingleOrDefaultAsync<User>(query, new { Email = email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取郵箱:{email} 的用戶時發生錯誤");
                return null;
            }
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            try
            {
                var query = "SELECT * FROM Users ORDER BY Name";
                using var connection = _context.CreateConnection();
                
                return await connection.QueryAsync<User>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取所有用戶時發生錯誤");
                return new List<User>();
            }
        }
        
        public async Task<IEnumerable<User>> GetTechniciansAsync()
        {
            try
            {
                var query = "SELECT * FROM Users WHERE Role = 'Technician' ORDER BY Name";
                using var connection = _context.CreateConnection();
                
                return await connection.QueryAsync<User>(query);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取技術人員列表時發生錯誤");
                return new List<User>();
            }
        }
        
        public async Task<User> CreateUserAsync(User user)
        {
            try
            {
                var query = @"
                    INSERT INTO Users (
                        Username, Name, Email, Role, Phone, Department,
                        PasswordHash, PasswordSalt, CreatedAt
                    ) VALUES (
                        @Username, @Name, @Email, @Role, @Phone, @Department,
                        @PasswordHash, @PasswordSalt, @CreatedAt
                    );
                    SELECT LAST_INSERT_ID();";

                using var connection = _context.CreateConnection();
                
                // 設置時間
                user.CreatedAt = DateTime.Now;
                
                // 執行插入並獲取新ID
                var id = await connection.QuerySingleAsync<int>(query, user);
                user.Id = id;
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建用戶時發生錯誤");
                throw;
            }
        }
        
        public async Task<User> UpdateUserAsync(User user)
        {
            try
            {
                var query = @"
                    UPDATE Users
                    SET Name = @Name,
                        Email = @Email,
                        Role = @Role,
                        Phone = @Phone,
                        Department = @Department
                    WHERE Id = @Id";

                using var connection = _context.CreateConnection();
                
                // 執行更新
                await connection.ExecuteAsync(query, user);
                
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新 ID:{user.Id} 的用戶時發生錯誤");
                throw;
            }
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            // 對於 Dapper，沒有顯式的 SaveChanges 方法，每個查詢都直接執行
            return await Task.FromResult(true);
        }
    }
} 