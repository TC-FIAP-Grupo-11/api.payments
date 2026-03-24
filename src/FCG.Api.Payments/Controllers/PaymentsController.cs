using FCG.Api.Payments.Models;
using FCG.Api.Payments.Services;
using Microsoft.AspNetCore.Mvc;

namespace FCG.Api.Payments.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessPayment([FromBody] ProcessPaymentRequest request)
    {
        var (success, message) = await _paymentService.ProcessPaymentAsync(
            request.OrderId,
            request.Amount,
            request.PaymentMethod,
            request.CardNumber);

        var response = new PaymentResponse(
            PaymentId: Guid.NewGuid(),
            OrderId: request.OrderId,
            Status: success ? "Approved" : "Rejected",
            Message: message,
            ProcessedAt: DateTime.UtcNow);

        return success ? Ok(response) : UnprocessableEntity(response);
    }
}
