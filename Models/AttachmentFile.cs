using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RepairSystem.API.Models
{
    public class AttachmentFile
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(255)]
        public string FileName { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string StoredFileName { get; set; } = string.Empty;
        
        [Required]
        public string FilePath { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string ContentType { get; set; } = string.Empty;
        
        [Required]
        public long Size { get; set; }
        
        public long FileSize { get; set; }
        public DateTime UploadTime { get; set; } = DateTime.Now;
        public int? UploadedBy { get; set; }
        public int TicketId { get; set; }
        
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public int RepairTicketId { get; set; }
        
        [ForeignKey("RepairTicketId")]
        public RepairTicket? RepairTicket { get; set; }
    }
} 