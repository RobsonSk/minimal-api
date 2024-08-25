using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Services;
using minimal_api.Infra.Db;

namespace Test.Domain;
[TestClass]
public class AdminServiceTest
{
    private DatabaseContext CreateContext()
    {
        var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        var builder = new ConfigurationBuilder()
            .SetBasePath(path)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = builder.Build();

        return new DatabaseContext(configuration);
    }
    [TestMethod]
    public void TestCreateAdmin()
    {
        var context = CreateContext();
        context.Database.ExecuteSqlRaw("TRUNCATE TABLE Admins");

        var admin = new Admin();
        admin.Id = 1;
        admin.Email = "admin@test.com";
        admin.Password = "admin";
        admin.Profile = "Admin";

        var adminService = new AdminService(context);

        adminService.Create(admin);

        Assert.AreEqual(1, adminService.GetAdmins(1).Count());

    }
}
