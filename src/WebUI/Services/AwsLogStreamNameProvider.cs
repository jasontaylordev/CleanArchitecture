using Serilog.Sinks.AwsCloudWatch;

namespace CleanArchitecture.WebUI.Services;

//TODO: this currently only returns the same GUID for all requests.
// Need to figure out how to generate a new GUID per request
public class AwsLogStreamNameProvider : ILogStreamNameProvider
{
    private readonly string traceIdentifier;
    // public AwsLogStreamNameProvider(IHttpContextAccessor? httpContextAccessor)
    // {
    //     if(httpContextAccessor?.HttpContext == null)
    //         traceIdentifier = Guid.NewGuid().ToString();
    //     else
    //         traceIdentifier = httpContextAccessor.HttpContext.TraceIdentifier;
    // }

    public AwsLogStreamNameProvider()
    {
        traceIdentifier = Guid.NewGuid().ToString();
    }

    /// <inheritdoc cref="ILogStreamNameProvider"/>
    public string GetLogStreamName()
    {
        return $"{DateTime.Now.ToString("yyyy/MM/dd")}/{traceIdentifier}"; ;
    }
}