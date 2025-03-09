using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairSystem.API.Models
{
    /// <summary>
    /// 報修單實體類，用於記錄維修請求的詳細信息
    /// </summary>
    /// <example>
    /// {
    ///   "id": 1,
    ///   "title": "電腦無法開機",
    ///   "description": "按下電源按鈕後無反應",
    ///   "deviceType": "筆記本電腦",
    ///   "status": "待處理",
    ///   "priority": "高",
    ///   "location": "行政大樓 201 室",
    ///   "userId": 5,
    ///   "equipmentId": 10
    /// }
    /// </example>
    public class RepairTicket
    {
        /// <summary>
        /// 報修單唯一標識符
        /// </summary>
        [Key]
        public int Id { get; set; }
        
        /// <summary>
        /// 報修單號，為了兼容性提供的別名屬性
        /// </summary>
        public int TicketId => Id;
        
        /// <summary>
        /// 報修單標題，不可為空
        /// </summary>
        /// <example>電腦無法開機</example>
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 報修單詳細描述，不可為空
        /// </summary>
        /// <example>按下電源按鈕後無反應，指示燈不亮</example>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 設備類型
        /// </summary>
        /// <example>筆記本電腦</example>
        public string? DeviceType { get; set; }
        
        /// <summary>
        /// 設備編號
        /// </summary>
        /// <example>IT-NB-2023-0042</example>
        public string? DeviceNumber { get; set; }
        
        /// <summary>
        /// 問題描述
        /// </summary>
        /// <example>電源問題</example>
        public string? Problem { get; set; }
        
        /// <summary>
        /// 解決方案
        /// </summary>
        /// <example>更換電源適配器</example>
        public string? Solution { get; set; }
        
        /// <summary>
        /// 創建時間，默認為當前時間
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        /// <summary>
        /// 最後更新時間
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
        
        /// <summary>
        /// 創建時間（舊版屬性名稱，提供向後兼容）
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 更新時間（舊版屬性名稱，提供向後兼容）
        /// </summary>
        public DateTime? UpdateTime { get; set; }
        
        /// <summary>
        /// 報修單狀態，不可為空，默認為"待處理"
        /// </summary>
        /// <example>待處理</example>
        [Required]
        public string Status { get; set; } = "待處理";
        
        /// <summary>
        /// 優先級
        /// </summary>
        /// <example>高</example>
        public string? Priority { get; set; }
        
        /// <summary>
        /// 設備位置
        /// </summary>
        /// <example>行政大樓 201 室</example>
        public string? Location { get; set; }
        
        /// <summary>
        /// 關聯設備 ID
        /// </summary>
        public int? EquipmentId { get; set; }
        
        /// <summary>
        /// 關聯設備對象
        /// </summary>
        [ForeignKey("EquipmentId")]
        public Equipment? Equipment { get; set; }
        
        /// <summary>
        /// 報修人 ID
        /// </summary>
        public int UserId { get; set; }
        
        /// <summary>
        /// 報修人用戶對象
        /// </summary>
        [ForeignKey("UserId")]
        public User? User { get; set; }
        
        /// <summary>
        /// 處理人 ID
        /// </summary>
        public int? HandledBy { get; set; }
        
        /// <summary>
        /// 處理人用戶對象
        /// </summary>
        [ForeignKey("HandledBy")]
        public User? Handler { get; set; }
        
        /// <summary>
        /// 報修單附件集合
        /// </summary>
        public virtual ICollection<AttachmentFile>? Attachments { get; set; }
    }
} 