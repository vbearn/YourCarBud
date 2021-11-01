using System.Linq;
using System.Text.Json;
using FluentValidation;

namespace YourCarBud.WebApi.Modules.OrderStepModule.Dtos
{
    // dto is used in api layer
    public class OrderStepStatusUpdateDto
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public JsonElement DetailCreationArgs{ get; set; }
    }

    public class OrderStepStatusUpdateValidator : AbstractValidator<OrderStepStatusUpdateDto>
    {
        public OrderStepStatusUpdateValidator()
        {
            RuleFor(x => x.Status).NotEmpty();

            RuleFor(x => x.Status)
                .Must(status => new[] { "start", "success", "fail" }.Contains(status?.ToLower()))
                .WithMessage("Status should be either Start, Success or Fail");

            RuleFor(x => x.Message)
                .Must((dto, message) => dto.Status == "Fail" || string.IsNullOrEmpty(message))
                .WithMessage("Message can only be provided for Error status");

        }
    }
}