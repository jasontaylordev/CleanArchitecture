using CleanArchitecture.Domain.Enums;

namespace CleanArchitecture.Domain.Entities
{
    /// <summary>
    /// Contains user defined propreties for this specific application.
    /// </summary>
    public class ApplicationUser 
    {
        /// <summary>
        /// Primary key and also a foreign key to the user's identity id.
        /// </summary>
        public string Id { get; set; }

        public ThemeColor ThemeColor { get; set; }
    }
}
