namespace Cubido.Template.Application.FunctionalTests.TestServices;

public class TestTimeProvider : TimeProvider
{
    public DateTimeOffset? DateTimeOverride { get; set; }
    public override DateTimeOffset GetUtcNow() => DateTimeOverride ?? DateTimeOffset.UtcNow;
}
