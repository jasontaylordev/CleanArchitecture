using FluentValidation;

namespace CleanArchitecture.Application.Common.Validations;

public static class ValidationsExtensions
{
    // When case for nullable structs
    public static IRuleBuilderOptions<T, TProperty?> RangeWithWhen<T, TProperty>(this IRuleBuilder<T, TProperty?> ruleBuilder, string entity, TProperty min, TProperty max, Func<T, bool> expression) where TProperty : struct, IComparable<TProperty>, IComparable => ruleBuilder.InclusiveBetween(min,max).WithMessage($"{entity} should be in range {min} - {max}").When(expression);

    // When case for non nullable structs 
    public static IRuleBuilderOptions<T, TProperty> RangeWithWhen<T, TProperty>(this IRuleBuilder<T, TProperty> ruleBuilder, string entity, TProperty min, TProperty max, Func<T, bool> expression) where TProperty : IComparable<TProperty>, IComparable => ruleBuilder.InclusiveBetween(min,max).WithMessage($"{entity} should be in range {min} - {max}").When(expression);
}