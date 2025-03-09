using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairSystem.API.Models
{
    /// <summary>
    /// 用戶實體類，表示系統中的用戶信息
    /// </summary>
    public class User
    {
        /// <summary>
        /// 用戶唯一標識符
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 用戶登錄名，不可為空
        /// </summary>
        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;
        
        /// <summary>
        /// 用戶真實姓名，不可為空
        /// </summary>
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 用戶電子郵件地址，可為空
        /// </summary>
        [EmailAddress]
        [StringLength(100)]
        public string? Email { get; set; }
        
        /// <summary>
        /// 用戶角色，默認為"User"
        /// </summary>
        [Required]
        [StringLength(20)]
        public string Role { get; set; } = "User";
        
        /// <summary>
        /// 用戶電話號碼，可為空
        /// </summary>
        [Phone]
        [StringLength(20)]
        public string? Phone { get; set; }
        
        /// <summary>
        /// 用戶所屬部門，可為空
        /// </summary>
        public string? Department { get; set; }
        
        /// <summary>
        /// 密碼哈希值，用於安全存儲密碼
        /// </summary>
        public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
        
        /// <summary>
        /// 密碼鹽值，用於加強密碼安全性
        /// </summary>
        public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
        
        /// <summary>
        /// 明文密碼，僅用於初始化數據，不存儲到數據庫
        /// </summary>
        [NotMapped] // 不映射到數據庫
        public string? Password { get; set; }
        
        /// <summary>
        /// 用戶創建時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 用戶最後登錄時間，可為空
        /// </summary>
        public DateTime? LastLoginAt { get; set; }
        
        /// <summary>
        /// 創建時間的別名，用於向後兼容
        /// </summary>
        public DateTime CreateTime => CreatedAt;
    }
} 