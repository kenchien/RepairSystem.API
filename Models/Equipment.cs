namespace RepairSystem.API.Models
{
    /// <summary>
    /// 設備實體類，用於記錄各類設備的基本信息
    /// </summary>
    /// <example>
    /// {
    ///   "equipmentId": 1,
    ///   "name": "Dell Latitude 7420",
    ///   "deviceType": "筆記本電腦",
    ///   "serialNumber": "SN12345678",
    ///   "status": "正常使用",
    ///   "department": "資訊部",
    ///   "location": "行政大樓 301 室"
    /// }
    /// </example>
    public class Equipment
    {
        /// <summary>
        /// 設備唯一標識符
        /// </summary>
        /// <example>1</example>
        public int EquipmentId { get; set; }
        
        /// <summary>
        /// 設備名稱
        /// </summary>
        /// <example>Dell Latitude 7420</example>
        public string Name { get; set; } = string.Empty;
        
        /// <summary>
        /// 設備類型
        /// </summary>
        /// <example>筆記本電腦</example>
        public string DeviceType { get; set; } = string.Empty;
        
        /// <summary>
        /// 設備序列號
        /// </summary>
        /// <example>SN12345678</example>
        public string SerialNumber { get; set; } = string.Empty;
        
        /// <summary>
        /// 設備狀態
        /// </summary>
        /// <example>正常使用</example>
        public string Status { get; set; } = string.Empty;
        
        /// <summary>
        /// 所屬部門
        /// </summary>
        /// <example>資訊部</example>
        public string Department { get; set; } = string.Empty;
        
        /// <summary>
        /// 設備位置
        /// </summary>
        /// <example>行政大樓 301 室</example>
        public string Location { get; set; } = string.Empty;
        
        /// <summary>
        /// 購買日期
        /// </summary>
        /// <example>2023-01-15</example>
        public DateTime PurchaseDate { get; set; }
        
        /// <summary>
        /// 最後維護日期
        /// </summary>
        /// <example>2023-06-20</example>
        public DateTime? LastMaintenanceDate { get; set; }
        
        /// <summary>
        /// 備註信息
        /// </summary>
        /// <example>附帶原廠電源適配器和滑鼠</example>
        public string? Notes { get; set; }
        
        /// <summary>
        /// 設備圖片URL
        /// </summary>
        /// <example>/images/equipment/laptop-001.jpg</example>
        public string? ImageUrl { get; set; }
        
        /// <summary>
        /// 創建時間
        /// </summary>
        public DateTime CreateTime { get; set; }
        
        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime UpdateTime { get; set; }
    }
} 