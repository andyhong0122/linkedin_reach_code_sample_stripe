using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Models.Domain;
using Models.Requests.Payments;
using Services;
using Services.Interfaces;
using Web.Controllers;
using Web.Models.Responses;
using System;

namespace Web.Api.Controllers
{
    [Route("api/payments")]
    [ApiController]
    public class PaymentApiController : BaseApiController
    {
        private IPaymentService _service = null;
        private IAuthenticationService<int> _authService = null;

        public PaymentApiController(
            IAuthenticationService<int> authService
            , IPaymentService service
            , ILogger<PaymentApiController> logger) : base(logger)
        {
            _service = service;
            _authService = authService;
        }


        [HttpPost]
        public ActionResult<ItemResponse<string>> Create(ChargeCreateRequest model)
        {
            BaseResponse response = null;
            int code = 200;

            try
            {
                StripeCharge _charge = _service.CreateCharge(model, _authService.GetCurrentUserId());
                response = new ItemResponse<StripeCharge> { Item = _charge };
            }
            catch (Exception ex)
            {
                code = 500;
                response = new ErrorResponse(ex.Message);
            }
            return StatusCode(code, response);
        }

    }
}