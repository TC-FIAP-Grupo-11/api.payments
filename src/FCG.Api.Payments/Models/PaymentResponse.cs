namespace FCG.Api.Payments.Models;

public record PaymentResponse(
    Guid PaymentId,
    Guid OrderId,
    string Status,
    string Message,
    DateTime ProcessedAt);
