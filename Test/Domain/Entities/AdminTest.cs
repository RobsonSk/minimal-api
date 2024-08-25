using minimal_api.Domain.Entities;

namespace Test.Domain;

[TestClass]
public class AdminTest
{
    [TestMethod]
    public void TestGetSetProps()
    {
        var admin = new Admin();

        admin.Id = 1;
        admin.Email = "admin@test.com";
        admin.Password = "admin";
        admin.Profile = "Admin";

        Assert.AreEqual(1, admin.Id);
        Assert.AreEqual("admin@test.com", admin.Email);
        Assert.AreEqual("admin", admin.Password);
        Assert.AreEqual("Admin", admin.Profile);

        Console.WriteLine($"Executed Test");
    }
}