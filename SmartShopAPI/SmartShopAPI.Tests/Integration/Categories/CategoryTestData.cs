using SmartShopAPI.Models.Dtos.Category;

namespace SmartShopAPI.Tests.Integration.Categories;


    public class CategoryTestData
    {
        public static CategoryUpsertDto ValidCategory =>
            new() { Name = "New Test Category" };

        public static CategoryUpsertDto UpdateCategory =>
            new() { Name = "Updated Category" };

        public static CategoryUpsertDto InvalidCategory =>
            new() { Name = "" };

        public static IEnumerable<object[]> InvalidCategories =>
            [
                new object[] { new CategoryUpsertDto { Name = "" } },
                new object[] { new CategoryUpsertDto { Name = " " } },
                new object[] { new CategoryUpsertDto { Name = null! } }
            ];
    }

