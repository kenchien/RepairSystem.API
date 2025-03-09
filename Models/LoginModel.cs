namespace RepairSystem.API.Models
{
    /// <summary>
    /// 用戶登錄模型，用於提交登錄請求
    /// </summary>
    /// <example>
    /// {
    ///   "username": "john.doe",
    ///   "password": "YourPassword123"
    /// }
    /// </example>
    public class LoginModel
    {
        /// <summary>
        /// 用戶名
        /// </summary>
        /// <example>john.doe</example>
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 用戶密碼
        /// </summary>
        /// <example>YourPassword123</example>
        public string Password { get; set; } = string.Empty;
    }
} 