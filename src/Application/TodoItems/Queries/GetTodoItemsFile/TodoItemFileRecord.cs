using AutoMapper;
using CleanArchitecture.Application.Common.Mappings;
using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoItems.Queries.GetTodoItemsFile
{
    public class TodoItemFileRecord : IMapFrom<TodoItem>
    {
        public string Name { get; set; }

        public bool Done { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<TodoItem, TodoItemFileRecord>()
                .ForMember(d => d.Done, opt => opt.MapFrom(s => s.IsComplete));
        }
    }
}
