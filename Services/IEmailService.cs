using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public interface IEmailService
    {
        Task SendRepairNotificationAsync(RepairTicket ticket);
        Task SendStatusUpdateAsync(RepairTicket ticket, string oldStatus);
        Task SendAssignmentNotificationAsync(RepairTicket ticket, User handler);
    }
} 