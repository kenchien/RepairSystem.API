using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairSystem.API.Models;
using RepairSystem.API.Services;
using System.Security.Claims;

namespace RepairSystem.API.Controllers
{
    /// <summary>
    /// 設備管理控制器，提供設備信息的 CRUD 操作
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EquipmentController : ControllerBase
    {
        private readonly IEquipmentService _equipmentService;
        private readonly ILogger<EquipmentController> _logger;

        /// <summary>
        /// EquipmentController 構造函數
        /// </summary>
        /// <param name="equipmentService">設備服務</param>
        /// <param name="logger">日誌服務</param>
        public EquipmentController(IEquipmentService equipmentService, ILogger<EquipmentController> logger)
        {
            _equipmentService = equipmentService;
            _logger = logger;
        }

        /// <summary>
        /// 獲取設備列表，支持分頁和篩選
        /// </summary>
        /// <param name="queryParams">查詢參數，包括頁碼、每頁大小和篩選條件</param>
        /// <returns>設備列表及分頁信息</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Equipment>>> GetEquipments([FromQuery] EquipmentQueryParams queryParams)
        {
            var equipments = await _equipmentService.GetPaginatedEquipmentAsync(queryParams);
            var totalCount = equipments.TotalItems;  // 直接從分頁結果中獲取總數
            
            Response.Headers["X-Total-Count"] = totalCount.ToString();
            Response.Headers["X-Page-Size"] = queryParams.PageSize.ToString();
            Response.Headers["X-Current-Page"] = queryParams.Page.ToString();
            Response.Headers["Access-Control-Expose-Headers"] = "X-Total-Count, X-Page-Size, X-Current-Page";
            
            return Ok(equipments.Items);
        }

        /// <summary>
        /// 根據ID獲取特定設備
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>設備詳細信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEquipment(int id)
        {
            var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
            
            if (equipment == null)
            {
                _logger.LogWarning($"未找到ID為{id}的設備");
                return NotFound($"未找到ID為{id}的設備");
            }
            
            return Ok(equipment);
        }

        /// <summary>
        /// 創建新設備
        /// </summary>
        /// <param name="equipment">設備信息</param>
        /// <returns>創建後的設備</returns>
        [HttpPost]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> CreateEquipment([FromBody] Equipment equipment)
        {
            try
            {
                var createdEquipment = await _equipmentService.AddEquipmentAsync(equipment);
                _logger.LogInformation($"設備創建成功：{createdEquipment.Name}");
                
                return CreatedAtAction(nameof(GetEquipment), new { id = createdEquipment.EquipmentId }, createdEquipment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "添加設備時發生錯誤");
                return StatusCode(500, "添加設備時發生錯誤");
            }
        }

        /// <summary>
        /// 更新設備信息
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <param name="equipment">更新的設備信息</param>
        /// <returns>更新後的設備</returns>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Technician")]
        public async Task<IActionResult> UpdateEquipment(int id, [FromBody] Equipment equipment)
        {
            if (id != equipment.EquipmentId)
            {
                _logger.LogWarning("設備ID不匹配");
                return BadRequest("設備ID不匹配");
            }
            
            try
            {
                var existingEquipment = await _equipmentService.GetEquipmentByIdAsync(id);
                if (existingEquipment == null)
                {
                    _logger.LogWarning($"未找到ID為{id}的設備");
                    return NotFound($"未找到ID為{id}的設備");
                }
                
                var updatedEquipment = await _equipmentService.UpdateEquipmentAsync(id, equipment);
                _logger.LogInformation($"設備更新成功：{updatedEquipment.Name}");
                
                return Ok(updatedEquipment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新ID為{id}的設備時發生錯誤");
                return StatusCode(500, $"更新設備時發生錯誤: {ex.Message}");
            }
        }

        /// <summary>
        /// 刪除設備
        /// </summary>
        /// <param name="id">設備ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteEquipment(int id)
        {
            try
            {
                var equipment = await _equipmentService.GetEquipmentByIdAsync(id);
                if (equipment == null)
                {
                    _logger.LogWarning($"未找到ID為{id}的設備");
                    return NotFound($"未找到ID為{id}的設備");
                }
                
                var result = await _equipmentService.DeleteEquipmentAsync(id);
                if (result)
                {
                    _logger.LogInformation($"設備刪除成功：ID={id}");
                    return NoContent();
                }
                else
                {
                    _logger.LogWarning($"無法刪除ID為{id}的設備");
                    return StatusCode(500, "刪除設備失敗");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除ID為{id}的設備時發生錯誤");
                return StatusCode(500, $"刪除設備時發生錯誤: {ex.Message}");
            }
        }
    }
} 