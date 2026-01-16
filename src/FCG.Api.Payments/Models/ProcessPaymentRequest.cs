namespace FCG.Api.Payments.Models;

public record ProcessPaymentRequest(
    Guid OrderId,
    decimal Amount,
    string PaymentMethod,
    string CardNumber,
    string CardHolderName,
    string ExpiryDate,
    string Cvv);
