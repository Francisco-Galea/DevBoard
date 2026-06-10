using DevBoard.Domain.Common;

namespace DevBoard.Domain.Entities
{
    public class Interview : BaseEntity
    {
        public Guid JobApplicationId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime ScheduledAt { get; set; }

        public JobApplication JobApplication { get; set; } = null!;
    }
}
