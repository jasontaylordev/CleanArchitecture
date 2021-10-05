namespace CleanArchitecture.Domain.Common
{
    public class CommonEntity : AuditableEntity
    {
        public int Id { get; set; }
        public string Title { get; set; }
    }
}
