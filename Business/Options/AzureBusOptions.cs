namespace Business.Options;

public class AzureBusOptions
{
    public string AzureServiceBusConnection { get; set; }

    public string EmailQueue { get; set; }

    public string SmsQueue { get; set; }

    public string PushNotificationQueue { get; set; }
}
