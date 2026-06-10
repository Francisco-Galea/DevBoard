using DevBoard.Domain.Common;

namespace DevBoard.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        public ICollection<JobApplication> JobApplications { get; set; } = [];
        public ICollection<Contact> Contacts { get; set; } = [];
    }
}
