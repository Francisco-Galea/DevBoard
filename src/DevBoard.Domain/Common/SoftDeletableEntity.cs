namespace DevBoard.Domain.Common
{
    public abstract class SoftDeletableEntity : BaseEntity
    {
        public bool IsDeleted { get; set; }
    }
}
