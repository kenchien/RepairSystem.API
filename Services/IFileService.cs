using RepairSystem.API.Models;
using Microsoft.AspNetCore.Http;

namespace RepairSystem.API.Services
{
    public interface IFileService
    {
        Task<AttachmentFile> UploadFileAsync(IFormFile file, int ticketId);
        Task<AttachmentFile?> GetFileAsync(int fileId);
        Task<bool> DeleteFileAsync(int fileId);
        
        // 添加獲取報修單所有附件的方法
        Task<IEnumerable<AttachmentFile>> GetFilesByTicketIdAsync(int ticketId);
    }
} 