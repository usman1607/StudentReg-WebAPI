using Application.Dtos.RequestDto;
using Application.Services.Interfaces;
using Asp.Versioning;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Text;

namespace WebAPI.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Produces("application/json")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get the bank account details for manual payments
        /// </summary>
        [HttpGet("bank-account")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult GetBankAccount()
        {
            return Ok(_paymentService.GetBankAccountDetails());
        }

        /// <summary>
        /// Initiate a Paystack payment (Student only)
        /// </summary>
        [HttpPost("paystack/initiate")]
        [Authorize(Policy = "StudentOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitiatePaystack([FromBody] InitiatePaystackPaymentRequest request)
        {
            var (studentId, email) = GetStudentClaims();
            if (studentId == null)
                return Unauthorized(new { message = "Student identity not found in token." });

            _logger.LogInformation("Student {StudentId} initiating Paystack payment", studentId);
            var result = await _paymentService.InitiatePaystackPaymentAsync(studentId.Value, email!, request);
            return Ok(result);
        }

        /// <summary>
        /// Paystack webhook — called by Paystack after a successful charge
        /// </summary>
        [HttpPost("paystack/webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> PaystackWebhook()
        {
            var signature = Request.Headers["x-paystack-signature"].FirstOrDefault() ?? "";

            Request.EnableBuffering();
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
            var rawBody = await reader.ReadToEndAsync();
            Request.Body.Position = 0;

            _logger.LogInformation("Received Paystack webhook, sig: {Sig}", signature);

            await _paymentService.HandlePaystackWebhookAsync(rawBody, signature);
            return Ok();
        }

        /// <summary>
        /// Submit a manual payment (Student only)
        /// </summary>
        [HttpPost("manual")]
        [Authorize(Policy = "StudentOnly")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SubmitManualPayment([FromBody] ManualPaymentRequest request)
        {
            var (studentId, _) = GetStudentClaims();
            if (studentId == null)
                return Unauthorized(new { message = "Student identity not found in token." });

            _logger.LogInformation("Student {StudentId} submitting manual payment", studentId);
            var result = await _paymentService.SubmitManualPaymentAsync(studentId.Value, request);
            return StatusCode(StatusCodes.Status201Created, result);
        }

        /// <summary>
        /// Get all payments for the authenticated student (Student only)
        /// </summary>
        [HttpGet("my-payments")]
        [Authorize(Policy = "StudentOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyPayments()
        {
            var (studentId, _) = GetStudentClaims();
            if (studentId == null)
                return Unauthorized(new { message = "Student identity not found in token." });

            var payments = await _paymentService.GetStudentPaymentsAsync(studentId.Value);
            return Ok(payments);
        }

        /// <summary>
        /// Get all payments with optional filtering (Admin only)
        /// </summary>
        [HttpGet]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] PaymentStatus? status = null,
            [FromQuery] PaymentType? type = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var result = await _paymentService.GetAllPaymentsAsync(status, type, page, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Get a payment by ID (Admin only)
        /// </summary>
        [HttpGet("{id:guid}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var payment = await _paymentService.GetPaymentByIdAsync(id);
            if (payment == null)
                return NotFound(new { message = $"Payment with ID '{id}' was not found." });

            return Ok(payment);
        }

        /// <summary>
        /// Confirm a manual payment (Admin only)
        /// </summary>
        [HttpPost("{id:guid}/confirm")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ConfirmPayment([FromRoute] Guid id, [FromBody] ConfirmPaymentRequest request)
        {
            var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin";
            _logger.LogInformation("Admin {Email} confirming payment {PaymentId}", adminEmail, id);
            var result = await _paymentService.ConfirmManualPaymentAsync(id, adminEmail);
            return Ok(result);
        }

        /// <summary>
        /// Reject a manual payment (Admin only)
        /// </summary>
        [HttpPost("{id:guid}/reject")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectPayment([FromRoute] Guid id, [FromBody] RejectPaymentRequest request)
        {
            var adminEmail = User.FindFirstValue(ClaimTypes.Email) ?? "admin";
            _logger.LogInformation("Admin {Email} rejecting payment {PaymentId}", adminEmail, id);
            var result = await _paymentService.RejectPaymentAsync(id, adminEmail, request.Reason);
            return Ok(result);
        }

        private (Guid? id, string? email) GetStudentClaims()
        {
            var idValue = User.FindFirstValue("userId") ?? User.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = User.FindFirstValue(ClaimTypes.Email);
            return Guid.TryParse(idValue, out var id) ? (id, email) : (null, null);
        }
    }
}
