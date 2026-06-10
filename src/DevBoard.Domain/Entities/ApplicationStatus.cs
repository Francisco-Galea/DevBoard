using DevBoard.Domain.Common;

namespace DevBoard.Domain.Entities
{
    public class ApplicationStatus : BaseEntity
    {
        public Guid JobApplicationId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime ChangedAt { get; set; }

        public JobApplication JobApplication { get; set; } = null!;
    }
}
