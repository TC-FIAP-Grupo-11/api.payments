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

        var paymentId = Guid.NewGuid();
        var response = new PaymentResponse(
            paymentId,
            request.OrderId,
            success ? "APPROVED" : "DECLINED",
            message,
            DateTime.UtcNow);

        if (!success)
            return BadRequest(response);

        return Ok(response);
    }

    [HttpGet("{paymentId:guid}")]
    public IActionResult GetPaymentStatus(Guid paymentId)
    {
        // Simula consulta de status
        return Ok(new
        {
            paymentId,
            status = "APPROVED",
            message = "Payment processed successfully"
        });
    }
}
