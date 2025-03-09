using System.Collections.Generic;
using System.Threading.Tasks;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    /// <summary>
    /// 使用Dapper實現的維修服務
    /// </summary>
    public class DapperRepairService : IRepairService
    {
        private readonly IRepairRepository _repository;
        private readonly IEmailService _emailService;

        /// <summary>
        /// 構建DapperRepairService的新實例
        /// </summary>
        /// <param name="repository">數據訪問存儲庫</param>
        /// <param name="emailService">電子郵件服務</param>
        public DapperRepairService(IRepairRepository repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        /// <summary>
        /// 獲取所有維修工單
        /// </summary>
        /// <returns>維修工單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetAllTicketsAsync()
        {
            return await _repository.GetAllTicketsAsync();
        }

        /// <summary>
        /// 根據ID獲取維修工單
        /// </summary>
        /// <param name="id">工單ID</param>
        /// <returns>維修工單對象</returns>
        public async Task<RepairTicket?> GetTicketByIdAsync(int id)
        {
            return await _repository.GetTicketByIdAsync(id);
        }

        /// <summary>
        /// 創建新維修工單
        /// </summary>
        /// <param name="ticket">維修工單對象</param>
        /// <returns>創建後的維修工單</returns>
        public async Task<RepairTicket> CreateTicketAsync(RepairTicket ticket)
        {
            var createdTicket = await _repository.CreateTicketAsync(ticket);

            // 發送電子郵件通知
            if (ticket.HandledBy.HasValue)
            {
                var technician = await _repository.GetUserByIdAsync(ticket.HandledBy.Value);
                if (technician != null)
                {
                    await _emailService.SendEmailAsync(
                        to: technician.Email,
                        subject: "新維修工單分配通知",
                        body: $"您有一個新的維修工單（ID: {createdTicket.Id}，標題：{createdTicket.Title}）已分配給您。"
                    );
                }
            }

            return createdTicket;
        }

        /// <summary>
        /// 更新維修工單
        /// </summary>
        /// <param name="ticket">維修工單對象</param>
        /// <returns>更新後的維修工單</returns>
        public async Task<RepairTicket> UpdateTicketAsync(RepairTicket ticket)
        {
            var existingTicket = await _repository.GetTicketByIdAsync(ticket.Id);
            if (existingTicket == null)
            {
                throw new KeyNotFoundException($"未找到ID為{ticket.Id}的維修工單");
            }

            var updatedTicket = await _repository.UpdateTicketAsync(ticket);

            // 如果狀態變更為已完成，發送郵件通知報修人
            if (existingTicket.Status != "已完成" && ticket.Status == "已完成")
            {
                var reporter = await _repository.GetUserByIdAsync(existingTicket.CreatedBy);
                if (reporter != null)
                {
                    await _emailService.SendEmailAsync(
                        to: reporter.Email,
                        subject: "維修工單完成通知",
                        body: $"您的維修工單（ID: {ticket.Id}，標題：{ticket.Title}）已經完成處理。"
                    );
                }
            }

            // 如果分配了新的技術人員，發送郵件通知
            if (existingTicket.HandledBy != ticket.HandledBy && ticket.HandledBy.HasValue)
            {
                var technician = await _repository.GetUserByIdAsync(ticket.HandledBy.Value);
                if (technician != null)
                {
                    await _emailService.SendEmailAsync(
                        to: technician.Email,
                        subject: "維修工單分配通知",
                        body: $"您有一個維修工單（ID: {ticket.Id}，標題：{ticket.Title}）已分配給您處理。"
                    );
                }
            }

            return updatedTicket;
        }

        /// <summary>
        /// 刪除維修工單
        /// </summary>
        /// <param name="id">工單ID</param>
        /// <returns>刪除是否成功</returns>
        public async Task<bool> DeleteTicketAsync(int id)
        {
            return await _repository.DeleteTicketAsync(id);
        }

        /// <summary>
        /// 獲取特定用戶的所有維修工單
        /// </summary>
        /// <param name="userId">用戶ID</param>
        /// <returns>維修工單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId)
        {
            return await _repository.GetUserTicketsAsync(userId);
        }

        /// <summary>
        /// 獲取指定技術人員負責的所有維修工單
        /// </summary>
        /// <param name="technicianId">技術人員ID</param>
        /// <returns>維修工單集合</returns>
        public async Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId)
        {
            return await _repository.GetTechnicianTicketsAsync(technicianId);
        }
    }
} 