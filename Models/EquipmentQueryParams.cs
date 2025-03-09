namespace RepairSystem.API.Models
{
    public class EquipmentQueryParams
    {
        private int _pageSize = 10;
        private int _page = 1;
        
        public int Page 
        { 
            get => _page; 
            set => _page = value < 1 ? 1 : value; 
        }
        
        public int PageNumber 
        { 
            get => Page; 
            set => Page = value; 
        }
        
        public int PageSize 
        { 
            get => _pageSize; 
            set => _pageSize = value > 50 ? 50 : (value < 1 ? 1 : value); 
        }
        
        public string? SearchTerm { get; set; }
        public string? DeviceType { get; set; }
        public string? Status { get; set; }
        public string? Department { get; set; }
        public string? SortBy { get; set; }
        public string? SortOrder { get; set; }
        
        public bool SortDescending 
        { 
            get => SortOrder?.ToLower() == "desc"; 
            set => SortOrder = value ? "desc" : "asc"; 
        }
    }
} 