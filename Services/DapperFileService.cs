using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    /// <summary>
    /// 使用Dapper實現的文件服務
    /// </summary>
    public class DapperFileService : IFileService
    {
        private readonly IRepairRepository _repository;
        private readonly FileStorageSettings _fileSettings;

        /// <summary>
        /// 構建DapperFileService的新實例
        /// </summary>
        /// <param name="repository">數據訪問存儲庫</param>
        /// <param name="fileSettings">文件存儲設置</param>
        public DapperFileService(
            IRepairRepository repository,
            IOptions<FileStorageSettings> fileSettings)
        {
            _repository = repository;
            _fileSettings = fileSettings.Value;
            
            // 確保存儲目錄存在
            if (!Directory.Exists(_fileSettings.StoragePath))
            {
                Directory.CreateDirectory(_fileSettings.StoragePath);
            }
        }

        /// <summary>
        /// 上傳附件
        /// </summary>
        /// <param name="file">上傳的文件</param>
        /// <param name="ticketId">維修工單ID</param>
        /// <returns>附件信息</returns>
        public async Task<AttachmentFile> UploadFileAsync(IFormFile file, int ticketId)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("未提供有效的文件");

            var ticket = await _repository.GetTicketByIdAsync(ticketId);
            if (ticket == null)
                throw new KeyNotFoundException($"未找到ID為{ticketId}的維修工單");

            // 獲取當前用戶ID（在實際應用中，這通常從HttpContext中獲取）
            int userId = ticket.UserId; // 使用報修人ID

            // 生成唯一文件名
            var fileName = file.FileName;
            var uniqueFileName = $"{Guid.NewGuid()}_{fileName}";
            var filePath = Path.Combine(_fileSettings.StoragePath, uniqueFileName);

            // 將文件保存到磁盤
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // 創建附件記錄
            var attachment = new AttachmentFile
            {
                FileName = fileName,
                FilePath = uniqueFileName, // 存儲相對路徑
                FileSize = file.Length,
                TicketId = ticketId,
                UploadTime = DateTime.Now,
                UploadedBy = userId
            };

            // 保存到數據庫 - 這裡需要擴展 IRepairRepository 接口
            // 考慮到接口可能尚未擴展，這裡暫時返回未保存的附件
            // 在實際應用中，應該實現並調用 _repository.AddAttachmentAsync(attachment);

            return attachment;
        }

        /// <summary>
        /// 獲取附件
        /// </summary>
        /// <param name="fileId">附件ID</param>
        /// <returns>附件信息</returns>
        public async Task<AttachmentFile?> GetFileAsync(int fileId)
        {
            // 實際應用中，應該實現並調用 _repository.GetAttachmentByIdAsync(fileId);
            // 由於接口可能尚未擴展，這裡返回 null
            return null;
        }

        /// <summary>
        /// 刪除附件
        /// </summary>
        /// <param name="fileId">附件ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteFileAsync(int fileId)
        {
            // 實際應用中，應該實現並調用 _repository.GetAttachmentByIdAsync(fileId);
            var attachment = await GetFileAsync(fileId);
            if (attachment == null)
                return false;

            var filePath = GetPhysicalFilePath(attachment.FilePath);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            // 從數據庫中刪除記錄
            // 實際應用中，應該實現並調用 _repository.DeleteAttachmentAsync(fileId);
            
            return true;
        }

        /// <summary>
        /// 獲取維修工單的所有附件
        /// </summary>
        /// <param name="ticketId">維修工單ID</param>
        /// <returns>附件集合</returns>
        public async Task<IEnumerable<AttachmentFile>> GetFilesByTicketIdAsync(int ticketId)
        {
            // 實際應用中，應該實現並調用 _repository.GetTicketAttachmentsAsync(ticketId);
            // 由於接口可能尚未擴展，這裡返回空集合
            return new List<AttachmentFile>();
        }

        /// <summary>
        /// 獲取附件的物理文件路徑
        /// </summary>
        /// <param name="filePath">存儲的文件路徑</param>
        /// <returns>文件的完整物理路徑</returns>
        public string GetPhysicalFilePath(string filePath)
        {
            return Path.Combine(_fileSettings.StoragePath, filePath);
        }

        /// <summary>
        /// 舊接口的實現方法，調用新方法
        /// </summary>
        public async Task<AttachmentFile> UploadFileAsync(IFormFile file, int ticketId, int userId)
        {
            // 此方法不再需要，但為了向後兼容性，我們保留它
            return await UploadFileAsync(file, ticketId);
        }

        /// <summary>
        /// 舊接口的實現方法，調用新方法
        /// </summary>
        public async Task<AttachmentFile?> GetAttachmentAsync(int fileId)
        {
            return await GetFileAsync(fileId);
        }

        /// <summary>
        /// 舊接口的實現方法，調用新方法
        /// </summary>
        public async Task<bool> DeleteAttachmentAsync(int fileId)
        {
            return await DeleteFileAsync(fileId);
        }

        /// <summary>
        /// 舊接口的實現方法，調用新方法
        /// </summary>
        public async Task<IEnumerable<AttachmentFile>> GetTicketAttachmentsAsync(int ticketId)
        {
            return await GetFilesByTicketIdAsync(ticketId);
        }
    }
} 