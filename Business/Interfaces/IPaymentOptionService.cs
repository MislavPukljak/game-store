using Business.DTO;

namespace Business.Interfaces;

public interface IPaymentOptionService
{
    Task<object> HandlePaymentAsync(int orderId, string paymentOptionTitle, VisaDto visaInformation);
}
