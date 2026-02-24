using System.Text.Json.Serialization;

namespace CleanArchitecture.Domain.Enums;

// You can add the Serialization https://learn.microsoft.com/en-us/aspnet/core/fundamentals/openapi/include-metadata?view=aspnetcore-10.0&tabs=minimal-apis#enum
//[JsonConverter(typeof(JsonStringEnumConverter<PriorityLevel>))]
public enum PriorityLevel
{
    None = 0,
    Low = 1,
    Medium = 2,
    High = 3
}
