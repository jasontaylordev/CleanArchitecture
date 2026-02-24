using CleanArchitecture.Domain.Entities;

namespace CleanArchitecture.Application.Common.Models;

public record UserDto
{
    public string? Id { get; init; }
    public string? DisplayName { get; init; }
    public string? Email { get; init; }
    public string? PhoneNumber { get; init; }
    public bool EmailConfirmed { get; init; }
    public bool PhoneNumberConfirmed { get; init; }
    public IReadOnlyCollection<string> Roles { get; init; } = [];
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<User, UserDto>()
                .ForMember(dest => dest.Roles, o => o.MapFrom(src => src.UserRoles.Select(r => r.Role.Name).ToList()));
        }
    }
}
