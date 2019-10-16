using CleanArchitecture.Application.Common.Interfaces;
using CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile;
using CleanArchitecture.Infrastructure.Files.Maps;
using CsvHelper;
using System.Collections.Generic;
using System.IO;

namespace CleanArchitecture.Infrastructure.Files
{
    public class CsvFileBuilder : ICsvFileBuilder
    {
        public byte[] BuildTodoItemsFile(IEnumerable<TodoItemFileRecord> records)
        {
            using var memoryStream = new MemoryStream();
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                using var csvWriter = new CsvWriter(streamWriter);

                csvWriter.Configuration.RegisterClassMap<TodoItemFileRecordMap>();
                csvWriter.WriteRecords(records);
            }

            return memoryStream.ToArray();
        }
    }
}
