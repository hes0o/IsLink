using IsLink.API.Data;
using IsLink.API.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IsLink.API.Tests;

public sealed class DbSeederTests
{
    private static ApplicationDbContext NewContext() =>
        new(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase($"seeder-{Guid.NewGuid()}")
            .Options);

    [Fact]
    public async Task SeedAsync_EmptyDatabase_AddsDemoData()
    {
        await using var context = NewContext();

        await DbSeeder.SeedAsync(context);

        Assert.True(await context.Users.AnyAsync());
        Assert.True(await context.Categories.AnyAsync());
    }

    [Fact]
    public async Task SeedAsync_ExistingData_LeavesItUntouched()
    {
        await using var context = NewContext();
        var realUser = new User
        {
            Username = "real_user",
            Email = "real@example.com",
            PasswordHash = "hash",
            FullName = "Real User",
        };
        context.Users.Add(realUser);
        await context.SaveChangesAsync();

        await DbSeeder.SeedAsync(context);

        var users = await context.Users.ToListAsync();
        Assert.Single(users);
        Assert.Equal(realUser.Id, users[0].Id);
        Assert.False(await context.Categories.AnyAsync());
    }
}
