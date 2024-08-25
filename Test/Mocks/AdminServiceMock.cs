using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Infra.Interfaces;

namespace Test.Mocks;

public class AdminServiceMock : IAdminService
{
    private static List<Admin> admins = new List<Admin>();
    public Admin Create(Admin admin)
    {
        admin.Id = admins.Count() + 1;
        admins.Add(admin);
        return admin;
    }

    public List<Admin> GetAdmins(int? page)
    {
        return admins;
    }

    public Admin? GetById(int id)
    {
        return admins.Find(i => i.Id == id);
    }

    public Admin? Login(LoginDTO loginDTO)
    {
       return admins.Find(i => i.Email == loginDTO.Email && i.Password == loginDTO.Password);
    }
}