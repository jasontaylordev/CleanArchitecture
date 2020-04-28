namespace CleanArchitecture.Application.Common.Models
{
    public class PaginationQuery
    {
        public int PageNumber { get; set; } = 0;
        public int? PageSize { get; set; } = 10;

        public int SkipPageCount => (PageNumber - 1) * PageSize.GetValueOrDefault();
    }
}
