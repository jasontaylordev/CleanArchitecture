namespace CleanArchitecture.Application.TodoLists.Queries.ExportTodos
{
    public class ExportTodosVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}