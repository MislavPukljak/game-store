using Business.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/notifications")]
[ApiController]
[Authorize(Roles = "Administrator")]
public class NotificationController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    // GET: api/notifications
    [HttpPost]
    public async Task<ActionResult> ChangeNotification(bool turnOffEmail, bool turnOffSms, bool turnOffPushNotification, CancellationToken cancellationToken = default)
    {
        await _notificationService.ChangeIsTurnedOffAsync(turnOffEmail, turnOffSms, turnOffPushNotification, cancellationToken);

        return Ok();
    }
}
