using CleanArchitecture.Application.Notifications.Commands.MarkNotificationRead;
using CleanArchitecture.Application.Notifications.Queries.GetNotifications;
using Microsoft.AspNetCore.Http.HttpResults;

namespace CleanArchitecture.Web.Endpoints;

public class Notifications : IEndpointGroup
{
    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();

        groupBuilder.MapGet(GetNotifications);
        groupBuilder.MapPut(MarkRead, "{id}/Read");
    }

    public static async Task<Ok<IReadOnlyCollection<NotificationDto>>> GetNotifications(ISender sender)
    {
        var result = await sender.Send(new GetNotificationsQuery());

        return TypedResults.Ok(result);
    }

    public static async Task<NoContent> MarkRead(ISender sender, int id)
    {
        await sender.Send(new MarkNotificationReadCommand(id));

        return TypedResults.NoContent();
    }
}
