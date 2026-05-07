using CleanArchitecture.Application.EmailOutbox.Queries.GetEmailOutboxMessages;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class EmailOutboxMessages : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetEmailOutboxMessages);
    }

    public static async Task<Ok<IReadOnlyCollection<EmailOutboxMessageDto>>> GetEmailOutboxMessages(ISender sender)
    {
        var result = await sender.Send(new GetEmailOutboxMessagesQuery());

        return TypedResults.Ok(result);
    }
}
