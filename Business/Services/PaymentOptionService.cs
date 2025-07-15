using AutoMapper;
using Business.DTO;
using Business.Exceptions;
using Business.Interfaces;
using Data.SQL.Entities;
using Data.SQL.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.Extensions.Logging;

namespace Business.Services;

public class PaymentOptionService : IPaymentOptionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<PaymentOptionService> _logger;

    public PaymentOptionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PaymentOptionService> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<object> HandlePaymentAsync(int orderId, string paymentOptionTitle, VisaDto visaInformation)
    {
#pragma warning disable IDE0046 // Convert to conditional expression
        if (paymentOptionTitle == "Bank")
        {
            return await GetInvoicePdfAsync(orderId);
        }
        else if (paymentOptionTitle == "IBox")
        {
            return await GetIBoxInvoiceAsync(orderId);
        }
        else if (paymentOptionTitle == "Visa")
        {
            return await CreateVisaOrderAsync(orderId, visaInformation);
        }
        else
        {
            throw new OrderException("Payment option not found.");
        }
#pragma warning restore IDE0046 // Convert to conditional expression
    }

    private async Task<VisaDto> CreateVisaOrderAsync(int orderId, VisaDto visa)
    {
        _logger.LogInformation("Creating Visa order.");

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, CancellationToken.None);

        if (order is not null)
        {
            var visaEntity = _mapper.Map<Visa>(visa);

            await _unitOfWork.VisaRepository.AddAsync(visaEntity, CancellationToken.None);

            order.Status = OrderStatus.Paid;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            _logger.LogInformation($"Visa order created with id: {visaEntity.Id}");

            return visa;
        }

        _logger.LogError($"Order with id: {orderId} not found");

        throw new OrderException("Order not found.");
    }

    private async Task<PlatformIBoxDto> GetIBoxInvoiceAsync(int orderId)
    {
        _logger.LogInformation("Creating IBox order.");

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, CancellationToken.None);

        var newOrder = new PlatformIBoxDto()
        {
            CustomerId = order.CustomerId,
            OrderId = order.Id,
            Sum = order.Sum,
        };

        order.Status = OrderStatus.Paid;
        _unitOfWork.OrderRepository.Update(order);
        await _unitOfWork.SaveAsync();

        return newOrder;
    }

    private async Task<byte[]> GetInvoicePdfAsync(int orderId)
    {
        _logger.LogInformation("Creating Bank order.");

        var order = await _unitOfWork.OrderRepository.GetByIdAsync(orderId, CancellationToken.None);

        if (order is not null)
        {
            using MemoryStream stream = new();

            PdfWriter writer = new(stream);
            PdfDocument pdf = new(writer);
            Document document = new(pdf);

            Paragraph header = new Paragraph("Invoice")
                .SetFontSize(20);

            document.Add(header);

            Table table = new(2, false);
            Cell cell21 = new Cell(1, 2)
               .SetTextAlignment(TextAlignment.RIGHT)
               .Add(new Paragraph("User ID: " + order.CustomerId.ToString()));
            table.AddCell(cell21);

            Cell cell22 = new Cell(1, 2)
               .SetTextAlignment(TextAlignment.RIGHT)
               .Add(new Paragraph("Order ID: " + order.Id.ToString()));
            table.AddCell(cell22);

            Cell cell23 = new Cell(1, 2)
               .SetTextAlignment(TextAlignment.RIGHT)
               .Add(new Paragraph("Invoice Validity: " + DateTime.UtcNow.AddDays(3).ToShortDateString()));
            table.AddCell(cell23);

            Cell cell24 = new Cell(1, 2)
               .SetTextAlignment(TextAlignment.RIGHT)
               .Add(new Paragraph("Sum: $" + order.Sum.ToString()));
            table.AddCell(cell24);

            document.Add(table);
            document.Close();

            order.Status = OrderStatus.CheckedOut;
            _unitOfWork.OrderRepository.Update(order);
            await _unitOfWork.SaveAsync();

            return stream.ToArray();
        }

        throw new OrderException("Order not found.");
    }
}
