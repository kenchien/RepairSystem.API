using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RepairSystem.API.Models
{
    /// <summary>
    /// 報修單創建模型，用於提交新的維修請求
    /// </summary>
    /// <example>
    /// {
    ///   "title": "電腦無法開機",
    ///   "description": "按下電源按鈕後無反應，指示燈不亮",
    ///   "equipmentId": 10,
    ///   "location": "行政大樓 201 室",
    ///   "priority": "高"
    /// }
    /// </example>
    public class RepairTicketCreateModel
    {
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
        /// <example>按下電源按鈕後無反應，指示燈不亮，嘗試更換插座仍然無法開機</example>
        [Required]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// 關聯設備ID，不可為空
        /// </summary>
        /// <example>10</example>
        [Required]
        public int EquipmentId { get; set; }
        
        /// <summary>
        /// 故障設備所在位置
        /// </summary>
        /// <example>行政大樓 201 室</example>
        public string? Location { get; set; }
        
        /// <summary>
        /// 維修優先級
        /// </summary>
        /// <example>高</example>
        public string? Priority { get; set; }
        
        /// <summary>
        /// 報修單附件文件列表
        /// </summary>
        public List<IFormFile>? Attachments { get; set; }
    }
} 