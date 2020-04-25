using CleanArchitecture.Application.Common.Models;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICacheInvalidatorPostProcessor
    {
       InvalidateCacheForQueries QueriesList { get; set; }
    }
}