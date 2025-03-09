using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RepairSystem.API.Models;
using RepairSystem.API.Services;
using System.Security.Claims;

namespace RepairSystem.API.Controllers
{
    /// <summary>
    /// 用戶認證控制器，處理用戶登錄、註冊和密碼管理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        /// <summary>
        /// AuthController 構造函數
        /// </summary>
        /// <param name="authService">認證服務</param>
        /// <param name="logger">日誌服務</param>
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        /// <summary>
        /// 用戶登錄
        /// </summary>
        /// <param name="model">登錄模型，包含用戶名和密碼</param>
        /// <returns>JWT令牌及用戶信息</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.AuthenticateAsync(model.Username, model.Password);
            
            if (!result.success)
            {
                _logger.LogWarning($"登錄失敗: {result.message}");
                return Unauthorized(new { message = result.message });
            }
            
            _logger.LogInformation($"用戶 {model.Username} 登錄成功");
            return Ok(new 
            { 
                token = result.token, 
                userId = result.user!.Id,
                username = result.user.Username,
                name = result.user.Name,
                email = result.user.Email,
                role = result.user.Role,
                message = "登錄成功"
            });
        }

        /// <summary>
        /// 用戶註冊
        /// </summary>
        /// <param name="model">註冊模型，包含用戶信息</param>
        /// <returns>註冊結果</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            try
            {
                var user = new User
                {
                    Username = model.Username,
                    Name = model.Name,
                    Email = model.Email,
                    Phone = model.Phone,
                    Department = model.Department,
                    Role = "User", // 默認角色
                    CreatedAt = DateTime.Now
                };
                
                var result = await _authService.RegisterUserAsync(user, model.Password);
                
                if (!result.success)
                {
                    _logger.LogWarning($"註冊失敗: {result.message}");
                    return BadRequest(new { message = result.message });
                }
                
                _logger.LogInformation($"用戶 {user.Username} 註冊成功");
                return Ok(new { 
                    message = "註冊成功",
                    userId = result.user!.Id,
                    username = result.user.Username,
                    name = result.user.Name,
                    email = result.user.Email,
                    role = result.user.Role
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "註冊過程中發生錯誤");
                return StatusCode(500, new { message = "註冊時發生內部錯誤" });
            }
        }

        /// <summary>
        /// 變更密碼
        /// </summary>
        /// <param name="model">密碼變更模型，包含舊密碼和新密碼</param>
        /// <returns>操作結果</returns>
        [HttpPost("change-password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
            if (userId == 0)
            {
                _logger.LogWarning("無法從令牌獲取用戶ID");
                return Unauthorized(new { message = "無效的認證信息" });
            }
            
            try
            {
                var result = await _authService.ChangePasswordAsync(userId, model.OldPassword, model.NewPassword);
                
                if (!result)
                {
                    _logger.LogWarning($"用戶 ID:{userId} 密碼變更失敗");
                    return BadRequest(new { message = "舊密碼不正確或發生其他錯誤" });
                }
                
                _logger.LogInformation($"用戶 ID:{userId} 密碼變更成功");
                return Ok(new { message = "密碼已成功更改" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"用戶 ID:{userId} 密碼變更過程中發生錯誤");
                return StatusCode(500, new { message = "密碼變更時發生內部錯誤" });
            }
        }
    }

    /// <summary>
    /// 用戶註冊模型
    /// </summary>
    public class RegisterModel
    {
        /// <summary>
        /// 用戶名
        /// </summary>
        public string Username { get; set; } = string.Empty;
        /// <summary>
        /// 密碼
        /// </summary>
        public string Password { get; set; } = string.Empty;
        /// <summary>
        /// 真實姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// 電子郵件
        /// </summary>
        public string Email { get; set; } = string.Empty;
        /// <summary>
        /// 電話號碼
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        /// <summary>
        /// 部門
        /// </summary>
        public string Department { get; set; } = string.Empty;
    }

    /// <summary>
    /// 密碼變更模型
    /// </summary>
    public class ChangePasswordModel
    {
        /// <summary>
        /// 舊密碼
        /// </summary>
        public string OldPassword { get; set; } = string.Empty;
        /// <summary>
        /// 新密碼
        /// </summary>
        public string NewPassword { get; set; } = string.Empty;
    }
} 