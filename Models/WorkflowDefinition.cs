namespace RepairSystem.API.Models
{
    public class WorkflowDefinition
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<WorkflowStep> Steps { get; set; } = new List<WorkflowStep>();
        public bool IsActive { get; set; }
        public string RequiredRole { get; set; } = string.Empty;
        public List<string> Actions { get; set; } = new List<string>();
    }

    public class WorkflowStep
    {
        public int Id { get; set; }
        public int WorkflowDefinitionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Order { get; set; }
        public string RequiredRole { get; set; } = string.Empty;
        public bool RequiresApproval { get; set; }
        public List<string> Actions { get; set; } = new List<string>();
    } 
}