namespace CleanArchitecture.Application.TodoLists.Queries.ExportTodos;

public class ExportTodosVm
{
    public ExportTodosVm(string fileName, string contentType, byte[] content)
    {
        FileName = fileName;
        ContentType = contentType;
        Content = content;
    }

    public string FileName { get; init; }

    public string ContentType { get; init; }

    public byte[] Content { get; init; }
}
