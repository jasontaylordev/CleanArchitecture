namespace CleanArchitecture.Application.Common.Models
{
    public class PaginationResponse
    {
        public object Data { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }

        public string NextPage { get; set; }
        public string PreviousPage { get; set; }
    }
}
