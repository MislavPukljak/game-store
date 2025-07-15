using AutoMapper;
using Business.DTO;
using Business.Interfaces;
using Business.Options;
using Data.SQL.Interfaces;
using Microsoft.Extensions.Options;

namespace Business.Services;

public class NotificationService : INotificationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IAzureBusService _azureBusService;
    private readonly string _emailQueueName;
    private readonly string _smsQueueName;
    private readonly string _pushNotificationQueueName;

    public NotificationService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IAzureBusService azureBusService,
        IOptions<AzureBusOptions> options)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _azureBusService = azureBusService;
        _emailQueueName = options.Value.EmailQueue;
        _smsQueueName = options.Value.SmsQueue;
        _pushNotificationQueueName = options.Value.PushNotificationQueue;
    }

    public async Task SendOrderEmailAsync(OrderDto order, CancellationToken cancellationToken)
    {
        var email = await _unitOfWork.NotificationRepository.GetNotification(_emailQueueName, cancellationToken);

        if (email.IsTurnedOff)
        {
            return;
        }

        var notificationDto = _mapper.Map<NotificationDto>(email);
        await _azureBusService.SendMessageAsync(order, notificationDto);
    }

    public async Task SendOrderSmsAsync(OrderDto order, CancellationToken cancellationToken)
    {
        var sms = await _unitOfWork.NotificationRepository.GetNotification(_smsQueueName, cancellationToken);

        if (sms.IsTurnedOff)
        {
            return;
        }

        var notificationDto = _mapper.Map<NotificationDto>(sms);
        await _azureBusService.SendMessageAsync(order, notificationDto);
    }

    public async Task SendOrderNotificationAsync(OrderDto order, CancellationToken cancellationToken)
    {
        var pushNotification = await _unitOfWork.NotificationRepository.GetNotification(_pushNotificationQueueName, cancellationToken);

        if (pushNotification.IsTurnedOff)
        {
            return;
        }

        var notificationDto = _mapper.Map<NotificationDto>(pushNotification);
        await _azureBusService.SendMessageAsync(order, notificationDto);
    }

    public async Task ChangeIsTurnedOffAsync(bool email, bool sms, bool notification, CancellationToken cancellationToken)
    {
        await EmailIsTurnedOffAsync(email, cancellationToken);
        await SmsIsTurnedOffAsync(sms, cancellationToken);
        await PushNotificationIsTurnedOffAsync(notification, cancellationToken);

        await _unitOfWork.SaveAsync();
    }

    private async Task EmailIsTurnedOffAsync(bool email, CancellationToken cancellationToken)
    {
        var emailNotification = await _unitOfWork.NotificationRepository.GetNotification(_emailQueueName, cancellationToken);

        if (emailNotification is not null)
        {
            emailNotification.IsTurnedOff = email;
            _unitOfWork.NotificationRepository.Update(emailNotification);
        }
    }

    private async Task SmsIsTurnedOffAsync(bool sms, CancellationToken cancellationToken)
    {
        var smsNotification = await _unitOfWork.NotificationRepository.GetNotification(_smsQueueName, cancellationToken);

        if (smsNotification is not null)
        {
            smsNotification.IsTurnedOff = sms;
            _unitOfWork.NotificationRepository.Update(smsNotification);
        }
    }

    private async Task PushNotificationIsTurnedOffAsync(bool notification, CancellationToken cancellationToken)
    {
        var pushNotification = await _unitOfWork.NotificationRepository.GetNotification(_pushNotificationQueueName, cancellationToken);

        if (pushNotification is not null)
        {
            pushNotification.IsTurnedOff = notification;
            _unitOfWork.NotificationRepository.Update(pushNotification);
        }
    }
}
