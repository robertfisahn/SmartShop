using FluentValidation;

using SmartShopAPI.Models.Dtos;

namespace SmartShopAPI.Models.Validators
{
    public class QueryParamsValidator : AbstractValidator<QueryParams>
    {
        private int[] allowedPageSizes = new[] { 10, 20, 30 };
        private string[] allowedSortBy = new[] { "Name", "Price" };
        public QueryParamsValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PagesSizes", $"Page size must be one of the following values [{string.Join(", ", allowedPageSizes)}]");
                }
            });
            RuleFor(x => x.SortBy).Custom((value, context) =>
            {
                if (!allowedSortBy.Contains(value))
                {
                    context.AddFailure($"Sort by must be one of the following values [{string.Join(", ", allowedSortBy)}]");
                }
            });
        }
    }
}
