using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairSystem.API.Models;
using RepairSystem.API.Services;
using System.Security.Claims;

namespace RepairSystem.API.Controllers
{
    /// <summary>
    /// 報修單管理控制器，提供報修單的 CRUD 操作和狀態管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RepairController : ControllerBase
    {
        private readonly IRepairService _repairService;
        private readonly IFileService _fileService;
        private readonly ILogger<RepairController> _logger;
        private readonly IEquipmentService _equipmentService;
        private readonly IEmailService _emailService;
        private readonly IAuthService _authService;

        /// <summary>
        /// RepairController 構造函數
        /// </summary>
        /// <param name="repairService">報修單服務</param>
        /// <param name="fileService">文件服務</param>
        /// <param name="logger">日誌服務</param>
        /// <param name="equipmentService">設備服務</param>
        /// <param name="emailService">郵件服務</param>
        /// <param name="authService">身份驗證服務</param>
        public RepairController(IRepairService repairService, IFileService fileService, ILogger<RepairController> logger, IEquipmentService equipmentService, IEmailService emailService, IAuthService authService)
        {
            _repairService = repairService;
            _fileService = fileService;
            _logger = logger;
            _equipmentService = equipmentService;
            _emailService = emailService;
            _authService = authService;
        }

        /// <summary>
        /// 獲取所有報修單列表
        /// </summary>
        /// <returns>報修單列表</returns>
        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            try
            {
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                IEnumerable<RepairTicket> tickets;
                
                // 根據角色返回不同的報修單列表
                if (role == "Admin")
                {
                    tickets = await _repairService.GetAllTicketsAsync();
                }
                else if (role == "Technician")
                {
                    tickets = await _repairService.GetTechnicianTicketsAsync(userId);
                }
                else
                {
                    tickets = await _repairService.GetUserTicketsAsync(userId);
                }

                return Ok(tickets);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "獲取報修單列表時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 根據 ID 獲取特定報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <returns>報修單詳細信息</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicket(int id)
        {
            try
            {
                var ticket = await _repairService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });

                // 確認用戶有權限查看此報修單
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (role != "Admin" && role != "Technician" && ticket.UserId != userId)
                    return Forbid();

                return Ok(ticket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取報修單 ID:{id} 時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 創建新的報修單
        /// </summary>
        /// <param name="model">報修單創建模型</param>
        /// <returns>創建後的報修單</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RepairTicket>> CreateTicket([FromForm] RepairTicketCreateModel model)
        {
            try
            {
                // 獲取當前用戶ID
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int userIdInt))
                {
                    return Unauthorized(new { message = "無法確認用戶身份" });
                }

                // 檢查設備是否存在
                var equipment = await _equipmentService.GetEquipmentByIdAsync(model.EquipmentId);
                if (equipment == null)
                {
                    return BadRequest(new { message = "指定的設備不存在" });
                }

                // 創建工單
                var ticket = new RepairTicket
                {
                    Title = model.Title,
                    Description = model.Description,
                    EquipmentId = model.EquipmentId,
                    Location = model.Location,
                    Priority = model.Priority,
                    Status = "待處理",
                    CreatedAt = DateTime.Now,
                    UserId = userIdInt
                };

                var createdTicket = await _repairService.CreateTicketAsync(ticket);

                // 上傳附件
                if (model.Attachments != null && model.Attachments.Count > 0)
                {
                    foreach (var file in model.Attachments)
                    {
                        if (file.Length > 0)
                        {
                            await _fileService.UploadFileAsync(file, createdTicket.Id);
                        }
                    }
                }

                // 發送郵件通知
                await _emailService.SendRepairNotificationAsync(createdTicket);

                return CreatedAtAction(nameof(GetTicket), new { id = createdTicket.Id }, createdTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "創建維修工單時發生錯誤");
                return StatusCode(500, new { message = "創建維修工單時發生錯誤", details = ex.Message });
            }
        }

        /// <summary>
        /// 更新報修單狀態
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <param name="ticket">更新的報修單信息</param>
        /// <returns>更新後的報修單</returns>
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTicket(int id, [FromBody] RepairTicket ticket)
        {
            try
            {
                if (id != ticket.Id)
                    return BadRequest(new { message = "ID 不匹配" });

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                // 獲取原始報修單
                var originalTicket = await _repairService.GetTicketByIdAsync(id);
                if (originalTicket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });
                
                // 權限檢查：只有管理員、技術人員或報修人可以更新報修單
                if (role != "Admin" && role != "Technician" && originalTicket.UserId != userId)
                    return Forbid();
                
                // 記錄狀態更改
                var oldStatus = originalTicket.Status;
                
                // 更新報修單
                var updatedTicket = await _repairService.UpdateTicketAsync(id, ticket);
                
                // 如果狀態發生變化，發送郵件通知
                if (updatedTicket != null && oldStatus != updatedTicket.Status)
                {
                    await _emailService.SendStatusUpdateAsync(updatedTicket, oldStatus);
                }
                
                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"更新報修單 ID:{id} 時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 分配技術人員處理報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <param name="technicianId">技術人員 ID</param>
        /// <returns>操作結果</returns>
        [HttpPut("{id}/assign/{technicianId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AssignTicket(int id, int technicianId)
        {
            try
            {
                // 檢查報修單是否存在
                var ticket = await _repairService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });
                
                // 檢查技術人員是否存在
                var technician = await _authService.GetUserByIdAsync(technicianId);
                if (technician == null)
                    return NotFound(new { message = $"找不到ID為 {technicianId} 的技術人員" });
                
                // 分配技術人員（使用現有的 UpdateTicketAsync 方法）
                ticket.HandledBy = technicianId;
                ticket.Status = "處理中";
                var updatedTicket = await _repairService.UpdateTicketAsync(id, ticket);
                
                if (updatedTicket == null)
                    return StatusCode(500, new { message = "分配技術人員失敗" });
                
                // 發送郵件通知
                await _emailService.SendAssignmentNotificationAsync(updatedTicket, technician);
                
                return Ok(updatedTicket);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"分配報修單 ID:{id} 給技術人員 ID:{technicianId} 時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 刪除報修單
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteTicket(int id)
        {
            try
            {
                // 檢查報修單是否存在
                var ticket = await _repairService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });
                
                // 刪除報修單
                var result = await _repairService.DeleteTicketAsync(id);
                if (!result)
                    return StatusCode(500, new { message = "刪除報修單失敗" });
                
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"刪除報修單 ID:{id} 時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 上傳報修單附件
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <param name="file">上傳的文件</param>
        /// <returns>上傳結果</returns>
        [HttpPost("{id}/attachments")]
        [Authorize]
        public async Task<IActionResult> UploadAttachment(int id, IFormFile file)
        {
            try
            {
                // 檢查報修單是否存在
                var ticket = await _repairService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });
                
                // 權限檢查：只有管理員、技術人員或報修人可以上傳附件
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (role != "Admin" && role != "Technician" && ticket.UserId != userId)
                    return Forbid();
                
                // 檢查文件是否有效
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "未提供有效的文件" });
                
                // 上傳文件
                var fileInfo = await _fileService.UploadFileAsync(file, id);
                
                return Ok(fileInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"上傳報修單 ID:{id} 附件時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }

        /// <summary>
        /// 獲取報修單的所有附件
        /// </summary>
        /// <param name="id">報修單 ID</param>
        /// <returns>附件列表</returns>
        [HttpGet("{id}/attachments")]
        [Authorize]
        public async Task<IActionResult> GetAttachments(int id)
        {
            try
            {
                // 檢查報修單是否存在
                var ticket = await _repairService.GetTicketByIdAsync(id);
                if (ticket == null)
                    return NotFound(new { message = $"找不到ID為 {id} 的報修單" });
                
                // 權限檢查：只有管理員、技術人員或報修人可以查看附件
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var role = User.FindFirst(ClaimTypes.Role)?.Value;
                
                if (role != "Admin" && role != "Technician" && ticket.UserId != userId)
                    return Forbid();
                
                // 獲取附件列表（直接從報修單中獲取）
                if (ticket.Attachments == null || !ticket.Attachments.Any())
                    return Ok(new List<AttachmentFile>());
                    
                return Ok(ticket.Attachments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取報修單 ID:{id} 附件列表時發生錯誤");
                return StatusCode(500, new { message = "內部服務器錯誤" });
            }
        }
    }
} 