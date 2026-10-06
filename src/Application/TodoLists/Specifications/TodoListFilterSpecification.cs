using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.TodoLists.Specifications;

public class TodoListFilterSpecification : Specification<TodoList>
{
    public TodoListFilterSpecification(string? search = null, string? colour = null, int? priority = null)
    {
        var criteria = PredicateBuilder.True<TodoList>();

        if (!string.IsNullOrWhiteSpace(search))
        {
            criteria = criteria.And(list => list.Title != null && list.Title.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(colour))
        {
            criteria = criteria.And(list => list.Colour.Code == colour);
        }

        if (priority.HasValue)
        {
            criteria = criteria.And(list => list.Items.Any(item => (int)item.Priority == priority.Value));
        }

        Criteria = criteria;
        ApplyOrderBy(list => list.Title!);
    }
}
