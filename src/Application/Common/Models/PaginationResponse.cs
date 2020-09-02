namespace CleanArchitecture.Application.Common.Models
{
    using System.Collections.Generic;
    using AutoMapper;
    using Mappings;

    public class PaginationResponse<T>
    {
        public List<T> Items { get; set; }

        public int PageIndex { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

    }
}
