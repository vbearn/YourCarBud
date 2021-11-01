using System.ComponentModel.DataAnnotations;
using FluentValidation;

namespace YourCarBud.WebApi.Modules.OrderModule.Dtos
{
    public class OrderCreateDto
    {
        [Required] public decimal TotalAmount { get; set; }
    }

    public class OrderCreateValidator : AbstractValidator<OrderCreateDto>
    {
        public OrderCreateValidator()
        {
            RuleFor(x => x.TotalAmount).NotEmpty();
        }
    }
}