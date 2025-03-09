using System.Collections.Generic;
using System.Threading.Tasks;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public interface IRepairService
    {
        Task<IEnumerable<RepairTicket>> GetAllTicketsAsync();
        Task<RepairTicket?> GetTicketByIdAsync(int id);
        Task<RepairTicket> CreateTicketAsync(RepairTicket ticket);
        Task<RepairTicket?> UpdateTicketAsync(int id, RepairTicket ticket);
        Task<bool> DeleteTicketAsync(int id);
        Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId);
        Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId);
    }
} 