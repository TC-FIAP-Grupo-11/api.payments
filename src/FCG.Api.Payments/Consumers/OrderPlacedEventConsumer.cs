using FCG.Api.Payments.Services;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;

namespace FCG.Api.Payments.Consumers;

public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IPaymentService _paymentService;
    private readonly ILambdaNotificationService _lambdaNotificationService;
    private readonly ILogger<OrderPlacedEventConsumer> _logger;

    public OrderPlacedEventConsumer(
        IPaymentService paymentService,
        ILambdaNotificationService lambdaNotificationService,
        ILogger<OrderPlacedEventConsumer> logger)
    {
        _paymentService = paymentService;
        _lambdaNotificationService = lambdaNotificationService;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation("Processing payment for order {OrderId}", order.OrderId);

        var (success, message) = await _paymentService.ProcessPaymentAsync(
            order.OrderId,
            order.Price,
            "card",
            "4111111111111111");

        var paymentEvent = new PaymentProcessedEvent
        {
            OrderId = order.OrderId,
            UserId = order.UserId,
            GameId = order.GameId,
            GameTitle = order.GameTitle,
            UserEmail = order.UserEmail,
            Status = success ? PaymentStatus.Approved : PaymentStatus.Rejected,
            Message = message,
            ProcessedAt = DateTime.UtcNow
        };

        await _lambdaNotificationService.InvokeAsync("PaymentProcessed", paymentEvent, context.CancellationToken);
    }
}
