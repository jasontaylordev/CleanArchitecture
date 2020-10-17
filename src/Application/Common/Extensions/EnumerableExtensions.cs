using System.Collections.Generic;
using Newtonsoft.Json;

namespace CleanArchitecture.Application.Common.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> DeepClone<T>(this IEnumerable<T> list)
        {
            string serializedData = JsonConvert.SerializeObject(list);
            var newList = JsonConvert.DeserializeObject<IEnumerable<T>>(serializedData);
            return newList;
        }
    }
}
