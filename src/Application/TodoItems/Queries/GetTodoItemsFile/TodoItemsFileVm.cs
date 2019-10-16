namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile
{
    public class TodoItemsFileVm
    {
        public string FileName { get; set; }

        public string ContentType { get; set; }

        public byte[] Content { get; set; }
    }
}