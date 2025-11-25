using System.Data.Common;

namespace Cubido.Template.Application.FunctionalTests;

public interface ITestDatabase
{
    Task InitializeAsync();

    DbConnection GetConnection();

    Task ResetAsync();

    Task DisposeAsync();
}
