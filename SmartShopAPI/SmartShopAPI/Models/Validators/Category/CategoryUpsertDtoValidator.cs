using FluentValidation;

using SmartShopAPI.Interfaces.Repositories;
using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Models.Validators.Category
{
    public class CategoryUpsertDtoValidator : AbstractValidator<CategoryUpsertDto>
    {
        public CategoryUpsertDtoValidator(ICategoryRepository categoryRepository)
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required")
                .MinimumLength(2).WithMessage("Name must be at least 2 characters long")
                .MaximumLength(50).WithMessage("Name must not exceed 50 characters")
                .MustAsync(async (name, _) => !await categoryRepository.ExistsByNameAsync(name))
                    .WithMessage("Category with the same name already exists.");
        }
    }
}
