using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using RepairSystem.API.Data;
using RepairSystem.API.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace RepairSystem.API.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IRepairRepository _repairRepository;
        private readonly string _uploadDirectory;
        private readonly RepairDbContext _context;
        private readonly ILogger<FileService> _logger;
        private readonly FileStorageSettings _fileSettings;

        public FileService(IWebHostEnvironment environment, IRepairRepository repairRepository, RepairDbContext context, ILogger<FileService> logger, IOptions<FileStorageSettings> fileSettings)
        {
            _environment = environment;
            _repairRepository = repairRepository;
            _uploadDirectory = Path.Combine(_environment.WebRootPath, "uploads");
            _context = context;
            _logger = logger;
            _fileSettings = fileSettings.Value;
        }

        public async Task<AttachmentFile> SaveFileAsync(IFormFile file, int ticketId, int userId)
        {
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }

            var uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(_uploadDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var attachment = new AttachmentFile
            {
                TicketId = ticketId,
                FileName = file.FileName,
                FilePath = uniqueFileName,
                ContentType = file.ContentType,
                FileSize = file.Length,
                UploadTime = DateTime.Now,
                UploadedBy = userId
            };

            _context.AttachmentFiles.Add(attachment);
            await _context.SaveChangesAsync();

            return attachment;
        }

        public async Task<AttachmentFile> UploadFileAsync(IFormFile file, int ticketId)
        {
            // 最小化實現，先讓建置通過
            var attachment = new AttachmentFile
            {
                FileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                RepairTicketId = ticketId
            };
            
            return await Task.FromResult(attachment);
        }

        public async Task<AttachmentFile?> GetFileAsync(int fileId)
        {
            // 最小化實現，先讓建置通過
            return await Task.FromResult<AttachmentFile?>(null);
        }

        public async Task<bool> DeleteFileAsync(int fileId)
        {
            // 最小化實現，先讓建置通過
            return await Task.FromResult(true);
        }

        /// <summary>
        /// 獲取報修單的所有附件文件
        /// </summary>
        /// <param name="ticketId">報修單ID</param>
        /// <returns>附件集合</returns>
        public async Task<IEnumerable<AttachmentFile>> GetFilesByTicketIdAsync(int ticketId)
        {
            try
            {
                var ticket = await _repairRepository.GetTicketByIdAsync(ticketId);
                if (ticket == null || ticket.Attachments == null)
                {
                    return new List<AttachmentFile>();
                }
                
                return ticket.Attachments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"獲取報修單ID:{ticketId}的附件時發生錯誤");
                return new List<AttachmentFile>();
            }
        }
    }
} 