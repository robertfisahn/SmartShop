using Microsoft.EntityFrameworkCore;

using Moq;

using SmartShopAPI.Data;

namespace SmartShopAPI.Tests.Unit.Shared;

public static class MockDbContext
{
    public static DbSet<T> CreateMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();
        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        return dbSet.Object;
    }

    public static Mock<SmartShopDbContext> CreateMockDbContext()
    {
        var options = new DbContextOptionsBuilder<SmartShopDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;
        var mockDbContext = new Mock<SmartShopDbContext>(options);
        return mockDbContext;
    }
}
