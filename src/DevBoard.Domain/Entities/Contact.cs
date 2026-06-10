using DevBoard.Domain.Common;

namespace DevBoard.Domain.Entities
{
    public class Contact : BaseEntity
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Company { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
        public string? LinkedInUrl { get; set; }
        public string? Notes { get; set; }

        public User User { get; set; } = null!;
        public ICollection<JobApplication> JobApplications { get; set; } = [];
    }
}
