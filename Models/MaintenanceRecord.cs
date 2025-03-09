namespace RepairSystem.API.Models
{
    public class MaintenanceRecord
    {
        public int Id { get; set; }
        public int EquipmentId { get; set; }
        public DateTime MaintenanceDate { get; set; }
        public string MaintenanceType { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int PerformedBy { get; set; }
        public string Result { get; set; } = string.Empty;
        public DateTime? NextMaintenanceDate { get; set; }

        public Equipment Equipment { get; set; } = new Equipment();
        public User Performer { get; set; } = new User();
    }
} 