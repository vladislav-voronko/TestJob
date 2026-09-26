using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestJob.Constants;
using TestJob.Factories;
using TestJob.Interfaces;
using TestJob.Models;

namespace TestJob.Controllers
{
    [ApiController]

    [Route("api/testJob")]
    public class TestJobContoller : ControllerBase
    {
        private readonly ITestJobService _testJobService;
        private readonly IValidator<TestJobRequest> _validator;
        public TestJobContoller(ITestJobService testJobService, IValidator<TestJobRequest> validator)
        {
            _testJobService = testJobService;
            _validator = validator;
        }

        [HttpPost]
        [ProducesResponseType(typeof(TestJobResponse), 200)]
        [ProducesResponseType(typeof(TestJobResponse), 400)]
        public async Task<IActionResult> Parse([FromBody] TestJobRequest model, CancellationToken cancellationToken)
        {
            if (model is null)
            {
                return BadRequest(
                    TestJobResponseFactory.Error(
                        TestJobErrorCodes.RequestBodyRequired,
                        "Request body is required."));
            }

            var validationResult = await _validator.ValidateAsync(model, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(
                    "; ",
                    validationResult.Errors
                        .Select(error => error.ErrorMessage)
                        .Distinct());

                return BadRequest(
                    TestJobResponseFactory.Error(
                        TestJobErrorCodes.ValidationFailed,
                        errorMessage));
            }

            var response = await _testJobService.Parse(model, cancellationToken);

            if (response.IsError == 0)
            {
                return Ok(response);
            }

            return response.ErrorCode switch
            {
                TestJobErrorCodes.InvalidUrlBase64 or
                TestJobErrorCodes.InvalidPageBase64 or
                TestJobErrorCodes.InvalidKeyBase64 or
                TestJobErrorCodes.InvalidEncryptedTextBase64 or
                TestJobErrorCodes.InvalidAesKey or
                TestJobErrorCodes.InvalidEncryptedTextLength or
                TestJobErrorCodes.DecryptionError =>
                    BadRequest(response),

                _ =>
                    StatusCode(
                        StatusCodes.Status500InternalServerError,
                        response)
            };
        }
    }
}
