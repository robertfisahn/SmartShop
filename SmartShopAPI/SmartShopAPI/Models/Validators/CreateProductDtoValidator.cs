using FluentValidation;

using SmartShopAPI.Data;
using SmartShopAPI.Models.Dtos.Product;

namespace SmartShopAPI.Models.Validators
{
    public class CreateProductDtoValidator : AbstractValidator<UpsertProductDto>
    {
        public CreateProductDtoValidator()
        {
            RuleFor(x => x.Price).PrecisionScale(8, 2, false)
                .WithMessage("Price must not be more than 8 digits in total and 2 decimal");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero");

            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name cannot be empty")
                .MaximumLength(50).WithMessage("Name cannot be more than 50 characters long");
        }
    }
}
