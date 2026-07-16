using DevBoard.Domain.Common;

namespace DevBoard.Domain.Entities
{
    public class JobApplication : BaseEntity
    {
        public Guid UserId { get; set; }
        public Guid? ContactId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string? ContactName { get; set; }
        public string Position { get; set; } = string.Empty;
        public string? JobUrl { get; set; }
        public string? Notes { get; set; }
        public string CurrentStatus { get; set; } = string.Empty;
        public DateTime AppliedAt { get; set; }

        public User User { get; set; } = null!;
        public Contact? Contact { get; set; }
        public ICollection<ApplicationStatus> StatusHistory { get; set; } = [];
        public ICollection<Interview> Interviews { get; set; } = [];
    }
}
