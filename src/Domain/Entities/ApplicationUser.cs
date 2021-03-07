using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities
{
    /// <summary>
    /// Contains user defined propreties for this specific application.
    /// </summary>
    /// <remarks></remarks>
    public class ApplicationUser 
    {
        /// <summary>
        /// Primary key and also a foreign key to IdentityUser.Id
        /// </summary>
        public int Id { get; set; }
        public ThemeColor ThemeColor { get; set; }
    }
}
