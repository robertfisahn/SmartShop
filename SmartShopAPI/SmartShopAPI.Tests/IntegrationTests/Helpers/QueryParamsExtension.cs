using SmartShopAPI.Models.Dtos;

namespace SmartShopAPI.Tests.IntegrationTests.Helpers
{
    public static class QueryParamsExtensions
    {
        public static string ToQueryString(this QueryParams queryParams)
        {
            var queryString = new List<string>
            {
                $"pageNumber={queryParams.PageNumber}",
                $"pageSize={queryParams.PageSize}",
                $"searchPhrase={queryParams.SearchPhrase}",
                $"sortBy={queryParams.SortBy}",
                $"sortOrder={queryParams.SortOrder}"
            };
            return string.Join("&", queryString);
        }
    }
}
