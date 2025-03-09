using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public class EquipmentService : IEquipmentService
    {
        private readonly RepairDbContext _context;

        public EquipmentService(RepairDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
        {
            return await _context.Equipment.ToListAsync();
        }

        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _context.Equipment.FindAsync(id);
        }

        public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
        {
            _context.Equipment.Add(equipment);
            await _context.SaveChangesAsync();
            return equipment;
        }

        public async Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment)
        {
            var existingEquipment = await _context.Equipment.FindAsync(id);
            if (existingEquipment == null)
                return null;

            _context.Entry(existingEquipment).CurrentValues.SetValues(equipment);
            await _context.SaveChangesAsync();
            
            return existingEquipment;
        }

        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            var equipment = await _context.Equipment.FindAsync(id);
            if (equipment == null)
                return false;

            _context.Equipment.Remove(equipment);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResult<Equipment>> GetPaginatedEquipmentAsync(EquipmentQueryParams queryParams)
        {
            var query = _context.Equipment.AsQueryable();

            // 應用過濾條件
            if (!string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(queryParams.SearchTerm) || 
                                       e.SerialNumber.Contains(queryParams.SearchTerm));
            }

            // 計算總數
            var totalItems = await query.CountAsync();

            // 應用分頁
            var items = await query
                .Skip((queryParams.Page - 1) * queryParams.PageSize)
                .Take(queryParams.PageSize)
                .ToListAsync();

            return new PaginatedResult<Equipment>
            {
                Items = items,
                TotalItems = totalItems,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(string? searchTerm)
        {
            var query = _context.Equipment.AsQueryable();
            
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.Name.Contains(searchTerm) || 
                                       e.SerialNumber.Contains(searchTerm));
            }
            
            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(EquipmentQueryParams queryParams)
        {
            var result = await GetPaginatedEquipmentAsync(queryParams);
            return result.Items;
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Equipment.CountAsync();
        }

        public async Task<int> GetTotalCountAsync(EquipmentQueryParams? queryParams = null)
        {
            var query = _context.Equipment.AsQueryable();
            
            if (queryParams != null && !string.IsNullOrEmpty(queryParams.SearchTerm))
            {
                query = query.Where(e => e.Name.Contains(queryParams.SearchTerm) || 
                                     e.SerialNumber.Contains(queryParams.SearchTerm));
            }
            
            return await query.CountAsync();
        }

        public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
        {
            return await CreateEquipmentAsync(equipment);
        }
    }
} 