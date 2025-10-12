using SmartShopAPI.Models.Dtos;

namespace SmartShopAPI.Tests.IntegrationTests.Helpers
{
    public class ProductTestData
    {
        public static QueryParams QueryParams =>
            new()
            {
                PageNumber = 1,
                PageSize = 10,
                SearchPhrase = "op",
                SortBy = "Price",
                SortOrder = SortOrder.Ascending
            };

        public static IEnumerable<object[]> InvalidQueryParamsList =>
            new List<object[]>
            {
                new object[] { new QueryParams
                {
                    PageNumber = 1,
                    PageSize = 10,
                    SearchPhrase = "op",
                    SortBy = "xxx",
                    SortOrder = SortOrder.Ascending
                } },
                new object[] { new QueryParams
                {
                    PageNumber = -1,
                    PageSize = 10,
                    SearchPhrase = "op",
                    SortBy = "Price",
                    SortOrder = SortOrder.Ascending
                } },
                new object[] { new QueryParams
                {
                    PageNumber = 1,
                    PageSize = -10,
                    SearchPhrase = "op",
                    SortBy = "Price",
                    SortOrder = SortOrder.Ascending
                } },
                new object[] { new QueryParams
                {
                    PageNumber = 1,
                    PageSize = 1,
                    SearchPhrase = "op",
                    SortBy = "Price",
                    SortOrder = SortOrder.Descending
                } }
            };

        public static IEnumerable<object[]> InvalidCreateProductMultiparts =>
            new List<object[]>
            {
                new object[] { new MultipartFormDataContent ()
                {
                    { new StringContent(""), "Name" }, //invalid name
                    { new StringContent("Test Description"), "Description" },
                    { new StringContent("59,999"), "Price" },
                    { new StringContent("100"), "StockQuantity" },
                    { new StringContent("1"), "CategoryId" }
                } },
                new object[] { new MultipartFormDataContent ()
                {
                    { new StringContent("Test Product"), "Name" },
                    { new StringContent("Test Description"), "Description" },
                    { new StringContent("-10"), "Price" }, //invalid price
                    { new StringContent("100"), "StockQuantity" },
                    { new StringContent("1"), "CategoryId" }
                } },
                new object[] { new MultipartFormDataContent ()
                {
                    { new StringContent("Test Product"), "Name" },
                    { new StringContent("Test Description"), "Description" },
                    { new StringContent("1111111111"), "Price" }, //invalid price
                    { new StringContent("100"), "StockQuantity" },
                    { new StringContent("1"), "CategoryId" }
                } }
            };

        public static MultipartFormDataContent CreateProductMultipart =>
            new()
            {
                { new StringContent("Test Product"), "Name" },
                { new StringContent("Test Description"), "Description" },
                { new StringContent("59,99"), "Price" },
                { new StringContent("100"), "StockQuantity" },
                { new StringContent("1"), "CategoryId" }
            };

        public static MultipartFormDataContent UpdateProductMultipart =>
            new()
            {
                { new StringContent("Update Product"), "Name" },
                { new StringContent("Update Description"), "Description" },
                { new StringContent("88,88"), "Price" },
                { new StringContent("88"), "StockQuantity" }
            };

        public static MultipartFormDataContent InvalidUpdateProductMultipart =>
            new()
            {
                { new StringContent(""), "Name" }, //invalid name
                { new StringContent("Update Description"), "Description" },
                { new StringContent("88,88"), "Price" },
                { new StringContent("88"), "StockQuantity" },
            };
    }
}
