namespace FCG.Api.Payments.Models;

public record ProcessPaymentRequest(
    Guid OrderId,
    Guid UserId,
    Guid GameId,
    string GameTitle,
    string UserEmail,
    decimal Amount,
    string PaymentMethod,
    string CardNumber);
