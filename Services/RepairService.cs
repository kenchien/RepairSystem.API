using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RepairSystem.API.Data;
using RepairSystem.API.Models;

namespace RepairSystem.API.Services
{
    public class RepairService : IRepairService
    {
        private readonly RepairDbContext _context;
        private readonly ILogger<RepairService> _logger;

        public RepairService(RepairDbContext context, ILogger<RepairService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<RepairTicket>> GetAllTicketsAsync()
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .ToListAsync();
        }

        public async Task<RepairTicket?> GetTicketByIdAsync(int id)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Include(t => t.Equipment)
                .Include(t => t.Attachments)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<RepairTicket> CreateTicketAsync(RepairTicket ticket)
        {
            _context.RepairTickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;
        }

        public async Task<RepairTicket?> UpdateTicketAsync(int id, RepairTicket ticket)
        {
            var existingTicket = await _context.RepairTickets.FindAsync(id);
            if (existingTicket == null)
                return null;

            // 更新屬性，考慮到屬性名稱的變化
            existingTicket.Title = ticket.Title;
            existingTicket.Description = ticket.Description;
            existingTicket.Status = ticket.Status;
            existingTicket.Priority = ticket.Priority;
            existingTicket.Location = ticket.Location;
            existingTicket.HandledBy = ticket.HandledBy;
            
            // 處理可能缺失的屬性
            if (ticket.DeviceType != null)
                existingTicket.DeviceType = ticket.DeviceType;
                
            if (ticket.DeviceNumber != null)
                existingTicket.DeviceNumber = ticket.DeviceNumber;
                
            if (ticket.Problem != null)
                existingTicket.Problem = ticket.Problem;
                
            if (ticket.Solution != null)
                existingTicket.Solution = ticket.Solution;

            await _context.SaveChangesAsync();
            return existingTicket;
        }

        public async Task<bool> DeleteTicketAsync(int id)
        {
            var ticket = await _context.RepairTickets.FindAsync(id);
            if (ticket == null)
                return false;

            _context.RepairTickets.Remove(ticket);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RepairTicket>> GetUserTicketsAsync(int userId)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Handler)
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<RepairTicket>> GetTechnicianTicketsAsync(int technicianId)
        {
            return await _context.RepairTickets
                .Include(t => t.User)
                .Include(t => t.Equipment)
                .Where(t => t.HandledBy == technicianId)
                .ToListAsync();
        }
    }
} 