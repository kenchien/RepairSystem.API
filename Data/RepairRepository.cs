using Microsoft.EntityFrameworkCore;
using RepairSystem.API.Models;

namespace RepairSystem.API.Data
{
    /// <summary>
    /// 維修系統數據訪問實現類，實現了IRepairRepository接口
    /// </summary>
    public class RepairRepository : IRepairRepository
    {
        private readonly RepairDbContext _context;
        private readonly ILogger<RepairRepository> _logger;

        /// <summary>
        /// 構造函數，通過依賴注入獲取數據庫上下文和日誌記錄器
        /// </summary>
        /// <param name="context">數據庫上下文</param>
        /// <param name="logger">日誌記錄器</param>
        public RepairRepository(RepairDbContext context, ILogger<RepairRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// 獲取所有報修單，包括關聯的用戶、處理人員和設備信息
        /// </summary>
        /// <returns>報修單集合，按創建時間降序排序</returns>
        public async Task<IEnumerable<RepairTicket>> GetAllTicketsAsync()
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// 根據ID獲取報修單，包括關聯的用戶、處理人員和設備信息
        /// </summary>
        /// <param name="id">報修單ID</param>
        /// <returns>報修單對象，如果不存在則返回null</returns>
        public async Task<RepairTicket?> GetTicketByIdAsync(int id)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .FirstOrDefaultAsync(t => t.TicketId == id);
        }

        /// <summary>
        /// 創建新的報修單，設置創建時間和更新時間
        /// </summary>
        /// <param name="ticket">報修單對象</param>
        /// <returns>創建後的報修單</returns>
        public async Task<RepairTicket> CreateTicketAsync(RepairTicket ticket)
        {
            ticket.CreatedAt = DateTime.Now;
            ticket.CreateTime = DateTime.Now;
            ticket.UpdatedAt = DateTime.Now;
            ticket.UpdateTime = DateTime.Now;
            
            _context.RepairTickets.Add(ticket);
            await SaveChangesAsync();
            
            return ticket;
        }

        public async Task<RepairTicket> UpdateTicketAsync(RepairTicket ticket)
        {
            ticket.UpdatedAt = DateTime.Now;
            ticket.UpdateTime = DateTime.Now;
            _context.RepairTickets.Update(ticket);
            await SaveChangesAsync();
            
            return ticket;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.RepairTickets.FindAsync(id);
            if (ticket == null)
                return false;
                
            _context.RepairTickets.Remove(ticket);
            return await SaveChangesAsync();
        }

        public async Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .Where(t => t.HandledBy == technicianId || t.HandledBy == null)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(EquipmentQueryParams queryParams)
        {
            var query = _context.Equipment.AsQueryable();
            
            // 搜索條件
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                query = query.Where(e => 
                    e.Name.Contains(queryParams.SearchTerm) ||
                    e.SerialNumber.Contains(queryParams.SearchTerm) ||
                    e.DeviceType.Contains(queryParams.SearchTerm));
            }
            
            // 篩選
            if (!string.IsNullOrEmpty(queryParams.DeviceType))
                query = query.Where(e => e.DeviceType == queryParams.DeviceType);
                
            if (!string.IsNullOrEmpty(queryParams.Status))
                query = query.Where(e => e.Status == queryParams.Status);
                
            if (!string.IsNullOrEmpty(queryParams.Department))
                query = query.Where(e => e.Department == queryParams.Department);
            
            // 排序
            if (queryParams.SortDescending)
            {
                query = queryParams.SortBy switch
                {
                    "Name" => query.OrderByDescending(e => e.Name),
                    "DeviceType" => query.OrderByDescending(e => e.DeviceType),
                    "Department" => query.OrderByDescending(e => e.Department),
                    "PurchaseDate" => query.OrderByDescending(e => e.PurchaseDate),
                    _ => query.OrderByDescending(e => e.EquipmentId)
                };
            }
            else
            {
                query = queryParams.SortBy switch
                {
                    "Name" => query.OrderBy(e => e.Name),
                    "DeviceType" => query.OrderBy(e => e.DeviceType),
                    "Department" => query.OrderBy(e => e.Department),
                    "PurchaseDate" => query.OrderBy(e => e.PurchaseDate),
                    _ => query.OrderBy(e => e.EquipmentId)
                };
            }
            
            // 分頁
            return await query
                .Skip((queryParams.PageNumber - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipment.FindAsync(id);
        }

        public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
        {
            equipment.CreateTime = DateTime.Now;
            equipment.UpdateTime = DateTime.Now;
            
            _context.Equipment.Add(equipment);
            await SaveChangesAsync();
            
            return equipment;
        }

        public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
        {
            equipment.UpdateTime = DateTime.Now;
            _context.Equipment.Update(equipment);
            await SaveChangesAsync();
            
            return equipment;
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
                return false;
                
            _context.Equipment.Remove(equipment);
            return await SaveChangesAsync();
        }

        public async Task<int> GetEquipmentTotalCountAsync(EquipmentQueryParams queryParams)
        {
            var query = _context.Equipment.AsQueryable();
            
            // 搜索條件
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                query = query.Where(e => 
                    e.Name.Contains(queryParams.SearchTerm) ||
                    e.SerialNumber.Contains(queryParams.SearchTerm) ||
                    e.DeviceType.Contains(queryParams.SearchTerm));
            }
            
            // 篩選
            if (!string.IsNullOrEmpty(queryParams.DeviceType))
                query = query.Where(e => e.DeviceType == queryParams.DeviceType);
                
            if (!string.IsNullOrEmpty(queryParams.Status))
                query = query.Where(e => e.Status == queryParams.Status);
                
            if (!string.IsNullOrEmpty(queryParams.Department))
                query = query.Where(e => e.Department == queryParams.Department);
                
            return await query.CountAsync();
        }
        
        public async Task<IEnumerable<string>> GetDeviceTypesAsync()
        {
            return await _context.Equipment
                .Select(e => e.DeviceType)
                .Distinct()
                .ToListAsync();
        }
        
        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            return await _context.Equipment
                .Select(e => e.Department)
                .Distinct()
                .ToListAsync();
        }

        public async Task<User?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }
        
        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email);
        }
        
        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<User>> GetTechniciansAsync()
        {
            return await _context.Users
                .Where(u => u.Role == "Technician" || u.Role == "Admin")
                .OrderBy(u => u.Name)
                .ToListAsync();
        }
        
        public async Task<User> CreateUserAsync(User user)
        {
            user.CreatedAt = DateTime.Now;
            _context.Users.Add(user);
            await SaveChangesAsync();
            
            return user;
        }
        
        public async Task<User> UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await SaveChangesAsync();
            
            return user;
        }

        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return await _context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "保存數據時發生錯誤");
                throw;
            }
        }
    }
} 