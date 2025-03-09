namespace RepairSystem.API.Models
{
    public class FileStorageSettings
    {
        public string StoragePath { get; set; } = string.Empty;
        public long MaxFileSize { get; set; } = 10 * 1024 * 1024; // 預設最大10MB
        public string[] AllowedExtensions { get; set; } = { ".jpg", ".jpeg", ".png", ".pdf", ".doc", ".docx", ".xls", ".xlsx" };
    }
} 