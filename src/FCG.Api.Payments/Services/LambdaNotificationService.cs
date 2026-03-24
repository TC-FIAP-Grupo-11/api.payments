using System.Text.Json;
using Amazon.Lambda;
using Amazon.Lambda.Model;

namespace FCG.Api.Payments.Services;

public interface ILambdaNotificationService
{
    Task InvokeAsync<T>(string eventType, T payload, CancellationToken cancellationToken = default);
}

public class LambdaNotificationService : ILambdaNotificationService
{
    private readonly IAmazonLambda _lambdaClient;
    private readonly string _functionName;
    private readonly ILogger<LambdaNotificationService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public LambdaNotificationService(
        IAmazonLambda lambdaClient,
        IConfiguration configuration,
        ILogger<LambdaNotificationService> logger)
    {
        _lambdaClient = lambdaClient;
        _functionName = configuration["AWS:NotificationLambdaName"] ?? "fcg-notification-sender";
        _logger = logger;
    }

    public async Task InvokeAsync<T>(string eventType, T payload, CancellationToken cancellationToken = default)
    {
        var request = new { EventType = eventType, Payload = payload };

        var invokeRequest = new InvokeRequest
        {
            FunctionName = _functionName,
            InvocationType = InvocationType.Event,
            Payload = JsonSerializer.Serialize(request, JsonOptions)
        };

        _logger.LogInformation("Invoking Lambda {FunctionName} with event type {EventType}", _functionName, eventType);

        await _lambdaClient.InvokeAsync(invokeRequest, cancellationToken);
    }
}
