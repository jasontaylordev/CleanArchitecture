using System.Collections.Generic;

namespace CleanArchitecture.Application.Common.Interfaces
{
    public interface ICsvFileBuilder
    {
        byte[] BuildTodoItemsFile<T>(IEnumerable<T> records);
    }
}
