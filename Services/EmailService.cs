using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using RepairSystem.API.Data;
using RepairSystem.API.Models;
using Microsoft.Extensions.Options;

namespace RepairSystem.API.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// 電子郵件服務的構造函數
        /// </summary>
        /// <param name="configuration">應用程序配置</param>
        /// <param name="logger">日誌記錄器</param>
        /// <param name="emailSettings">電子郵件設置選項</param>
        public EmailService(IConfiguration configuration, ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _configuration = configuration;
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// 發送報修單通知郵件
        /// </summary>
        /// <param name="ticket">報修單信息</param>
        /// <param name="recipientEmail">收件人郵箱地址</param>
        /// <returns>異步任務</returns>
        public async Task SendTicketNotificationAsync(RepairTicket ticket, string recipientEmail)
        {
            var emailSettings = _configuration.GetSection("EmailSettings");
            var client = new SmtpClient(emailSettings["SmtpServer"])
            {
                Port = int.Parse(emailSettings["SmtpPort"]),
                Credentials = new NetworkCredential(emailSettings["Username"], emailSettings["Password"]),
                EnableSsl = bool.Parse(emailSettings["EnableSsl"])
            };

            var message = new MailMessage
            {
                From = new MailAddress(emailSettings["FromAddress"]),
                Subject = $"報修單狀態更新 - #{ticket.TicketId}",
                Body = GenerateEmailBody(ticket),
                IsBodyHtml = true
            };
            message.To.Add(recipientEmail);

            try
            {
                await client.SendMailAsync(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "發送郵件失敗");
                throw;
            }
        }

        /// <summary>
        /// 生成郵件正文內容
        /// </summary>
        /// <param name="ticket">報修單信息</param>
        /// <returns>格式化的HTML郵件內容</returns>
        private string GenerateEmailBody(RepairTicket ticket)
        {
            string deviceType = ticket.DeviceType ?? "未指定設備類型";
            string problem = ticket.Problem ?? "無問題描述";
            string solution = ticket.Solution ?? "尚未解決";
            string solutionInfo = string.IsNullOrEmpty(ticket.Solution) ? "" : $"解決方案: {ticket.Solution}";

            return $@"
                <h2>報修單狀態更新</h2>
                <p>報修單號：{ticket.TicketId}</p>
                <p>設備類型：{deviceType}</p>
                <p>當前狀態：{ticket.Status}</p>
                <p>更新時間：{ticket.UpdateTime}</p>
                <p>問題描述：{problem}</p>
                {solutionInfo}
            ";
        }

        /// <summary>
        /// 發送維修通知郵件
        /// </summary>
        /// <param name="ticket">報修單信息</param>
        /// <returns>異步任務</returns>
        public async Task SendRepairNotificationAsync(RepairTicket ticket)
        {
            // 最小化實現，先讓建置通過
            await Task.CompletedTask;
        }

        /// <summary>
        /// 發送狀態更新通知郵件
        /// </summary>
        /// <param name="ticket">報修單信息</param>
        /// <param name="oldStatus">舊狀態</param>
        /// <returns>異步任務</returns>
        public async Task SendStatusUpdateAsync(RepairTicket ticket, string oldStatus)
        {
            // 最小化實現，先讓建置通過
            await Task.CompletedTask;
        }

        /// <summary>
        /// 發送分配通知郵件
        /// </summary>
        /// <param name="ticket">報修單信息</param>
        /// <param name="handler">處理人員信息</param>
        /// <returns>異步任務</returns>
        public async Task SendAssignmentNotificationAsync(RepairTicket ticket, User handler)
        {
            // 最小化實現，先讓建置通過
            await Task.CompletedTask;
        }

        /// <summary>
        /// 發送電子郵件的通用方法
        /// </summary>
        /// <param name="to">收件人郵箱地址</param>
        /// <param name="subject">郵件主題</param>
        /// <param name="body">郵件正文</param>
        /// <returns>異步任務</returns>
        private async Task SendEmailAsync(string to, string subject, string body)
        {
            using var client = new SmtpClient(_emailSettings.SmtpServer, _emailSettings.SmtpPort)
            {
                EnableSsl = _emailSettings.EnableSsl,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password)
            };
            
            using var message = new MailMessage
            {
                From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            
            message.To.Add(to);
            await client.SendMailAsync(message);
        }
    }
} 