namespace FCG.Api.Payments.Services;

public interface IPaymentService
{
    Task<(bool Success, string Message)> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        string paymentMethod,
        string cardNumber);
}

public class SimulatedPaymentService : IPaymentService
{
    private readonly ILogger<SimulatedPaymentService> _logger;

    public SimulatedPaymentService(ILogger<SimulatedPaymentService> logger)
    {
        _logger = logger;
    }

    public async Task<(bool Success, string Message)> ProcessPaymentAsync(
        Guid orderId,
        decimal amount,
        string paymentMethod,
        string cardNumber)
    {
        await Task.Delay(500); // Simula latência de processamento

        var lastDigits = cardNumber.Length >= 4 ? cardNumber[^4..] : cardNumber;
        var success = !cardNumber.StartsWith("0000"); // Simula falha se cartão começar com 0000

        var status = success ? "APPROVED" : "DECLINED";
        var message = success
            ? $"Payment approved for order {orderId}"
            : $"Payment declined for order {orderId}";

        _logger.LogInformation(
            "\n========== PAYMENT PROCESSING ==========\n" +
            "Order ID: {OrderId}\n" +
            "Amount: {Amount:C}\n" +
            "Payment Method: {PaymentMethod}\n" +
            "Card (last 4): ****{LastDigits}\n" +
            "Status: {Status}\n" +
            "Message: {Message}\n" +
            "========================================\n",
            orderId, amount, paymentMethod, lastDigits, status, message);

        return (success, message);
    }
}
