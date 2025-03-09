using System.Collections.Generic;
using System.Threading.Tasks;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    /// <summary>
    /// 使用Dapper實現的設備服務
    /// </summary>
    public class DapperEquipmentService : IEquipmentService
    {
        private readonly IRepairRepository _repository;

        /// <summary>
        /// 構建DapperEquipmentService的新實例
        /// </summary>
        /// <param name="repository">數據訪問存儲庫</param>
        public DapperEquipmentService(IRepairRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// 獲取所有設備
        /// </summary>
        /// <returns>設備列表</returns>
        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync()
        {
            var queryParams = new EquipmentQueryParams();
            return await _repository.GetAllEquipmentAsync(queryParams);
        }

        /// <summary>
        /// 根據搜索條件獲取所有設備
        /// </summary>
        /// <param name="searchTerm">搜索條件</param>
        /// <returns>設備列表</returns>
        public async Task<IEnumerable<Equipment>> GetAllEquipmentAsync(string? searchTerm)
        {
            var queryParams = new EquipmentQueryParams { SearchTerm = searchTerm };
            return await _repository.GetAllEquipmentAsync(queryParams);
        }

        /// <summary>
        /// 獲取分頁設備信息
        /// </summary>
        /// <param name="queryParams">查詢參數</param>
        /// <returns>分頁設備信息</returns>
        public async Task<PaginatedResult<Equipment>> GetPaginatedEquipmentAsync(EquipmentQueryParams queryParams)
        {
            var items = await _repository.GetAllEquipmentAsync(queryParams);
            var totalCount = await _repository.GetEquipmentTotalCountAsync(queryParams);
            
            return new PaginatedResult<Equipment>
            {
                Items = items,
                TotalItems = totalCount,
                Page = queryParams.Page,
                PageSize = queryParams.PageSize
            };
        }

        /// <summary>
        /// 獲取設備總數
        /// </summary>
        /// <returns>設備總數</returns>
        public async Task<int> GetTotalCountAsync()
        {
            var queryParams = new EquipmentQueryParams();
            return await _repository.GetEquipmentTotalCountAsync(queryParams);
        }

        /// <summary>
        /// 根據ID獲取設備信息
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>設備信息</returns>
        public async Task<Equipment?> GetEquipmentByIdAsync(int id)
        {
            return await _repository.GetEquipmentByIdAsync(id);
        }

        /// <summary>
        /// 添加新設備
        /// </summary>
        /// <param name="equipment">設備信息</param>
        /// <returns>添加後的設備信息</returns>
        public async Task<Equipment> AddEquipmentAsync(Equipment equipment)
        {
            return await _repository.AddEquipmentAsync(equipment);
        }

        /// <summary>
        /// 創建新設備（別名方法，調用AddEquipmentAsync）
        /// </summary>
        /// <param name="equipment">設備信息</param>
        /// <returns>創建後的設備</returns>
        public async Task<Equipment> CreateEquipmentAsync(Equipment equipment)
        {
            return await AddEquipmentAsync(equipment);
        }

        /// <summary>
        /// 更新設備信息
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <param name="equipment">設備信息</param>
        /// <returns>更新後的設備信息</returns>
        public async Task<Equipment?> UpdateEquipmentAsync(int id, Equipment equipment)
        {
            var existingEquipment = await _repository.GetEquipmentByIdAsync(id);
            if (existingEquipment == null)
                return null;
                
            equipment.EquipmentId = id; // 確保ID正確
            return await _repository.UpdateEquipmentAsync(equipment);
        }

        /// <summary>
        /// 更新設備信息（不需要ID參數的版本）
        /// </summary>
        /// <param name="equipment">設備信息</param>
        /// <returns>更新後的設備信息</returns>
        public async Task<Equipment> UpdateEquipmentAsync(Equipment equipment)
        {
            return await _repository.UpdateEquipmentAsync(equipment);
        }

        /// <summary>
        /// 刪除設備
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteEquipmentAsync(int id)
        {
            return await _repository.DeleteEquipmentAsync(id);
        }

        /// <summary>
        /// 獲取所有設備類型
        /// </summary>
        /// <returns>設備類型集合</returns>
        public async Task<IEnumerable<string>> GetDeviceTypesAsync()
        {
            return await _repository.GetDeviceTypesAsync();
        }

        /// <summary>
        /// 獲取所有部門
        /// </summary>
        /// <returns>部門集合</returns>
        public async Task<IEnumerable<string>> GetDepartmentsAsync()
        {
            return await _repository.GetDepartmentsAsync();
        }
    }
} 