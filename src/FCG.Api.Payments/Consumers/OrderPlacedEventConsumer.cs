using FCG.Api.Payments.Services;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;

namespace FCG.Api.Payments.Consumers;

public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ILogger<OrderPlacedEventConsumer> _logger;

    public OrderPlacedEventConsumer(
        IPaymentService paymentService,
        IPublishEndpoint publishEndpoint,
        ILogger<OrderPlacedEventConsumer> logger)
    {
        _paymentService = paymentService;
        _publishEndpoint = publishEndpoint;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var evt = context.Message;
        
        _logger.LogInformation(
            "Recebido evento OrderPlacedEvent para pedido {OrderId} - Valor: {Price:C}",
            evt.OrderId,
            evt.Price);

        // Simular processamento de pagamento (usar cartão fictício)
        var (success, message) = await _paymentService.ProcessPaymentAsync(
            evt.OrderId,
            evt.Price,
            "CreditCard",
            "4111111111111111"); // Cartão de teste que sempre aprova

        // Publicar resultado do pagamento
        await _publishEndpoint.Publish(new PaymentProcessedEvent
        {
            OrderId = evt.OrderId,
            UserId = evt.UserId,
            GameId = evt.GameId,
            GameTitle = evt.GameTitle,
            UserEmail = evt.UserEmail,
            Status = success ? PaymentStatus.Approved : PaymentStatus.Rejected,
            Message = message,
            ProcessedAt = DateTime.UtcNow
        });
    }
}
