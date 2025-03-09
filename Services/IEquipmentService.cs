using System.Collections.Generic;
using System.Threading.Tasks;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public interface IEquipmentService
    {
        Task<IEnumerable<Equipment>> GetAllEquipmentAsync();
        Task<Equipment?> GetEquipmentByIdAsync(int id);
        Task<Equipment> CreateEquipmentAsync(Equipment equipment);
        Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment);
        Task<bool> DeleteEquipmentAsync(int id);
        Task<PaginatedResult<Equipment>> GetPaginatedEquipmentAsync(EquipmentQueryParams queryParams);
        
        // 添加缺失的方法
        Task<IEnumerable<Equipment>> GetAllEquipmentAsync(string? searchTerm);
        Task<int> GetTotalCountAsync();
        Task<Equipment> AddEquipmentAsync(Equipment equipment);
    }
} 