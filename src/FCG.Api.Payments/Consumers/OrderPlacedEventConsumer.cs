using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using FCG.Lib.Shared.Messaging.Contracts;
using MassTransit;

namespace FCG.Api.Payments.Consumers;

public class OrderPlacedEventConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly IAmazonLambda _lambdaClient;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly string _functionName;
    private readonly ILogger<OrderPlacedEventConsumer> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public OrderPlacedEventConsumer(
        IAmazonLambda lambdaClient,
        IPublishEndpoint publishEndpoint,
        IConfiguration configuration,
        ILogger<OrderPlacedEventConsumer> logger)
    {
        _lambdaClient = lambdaClient;
        _publishEndpoint = publishEndpoint;
        _functionName = configuration["AWS:PaymentLambdaName"] ?? "fcg-payment-processor";
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var order = context.Message;

        _logger.LogInformation("Invoking payment Lambda for order {OrderId}", order.OrderId);

        var invokeRequest = new InvokeRequest
        {
            FunctionName = _functionName,
            InvocationType = InvocationType.RequestResponse,
            Payload = JsonSerializer.Serialize(order, JsonOptions)
        };

        var response = await _lambdaClient.InvokeAsync(invokeRequest, context.CancellationToken);

        var paymentEvent = await JsonSerializer.DeserializeAsync<PaymentProcessedEvent>(
            response.Payload, JsonOptions, context.CancellationToken);

        if (paymentEvent is null)
        {
            _logger.LogError("Null response from Lambda for order {OrderId}", order.OrderId);
            return;
        }

        _logger.LogInformation("Payment {Status} for order {OrderId}", paymentEvent.Status, paymentEvent.OrderId);

        await _publishEndpoint.Publish(paymentEvent, context.CancellationToken);
    }
}
