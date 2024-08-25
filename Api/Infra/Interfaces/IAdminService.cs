using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Infra.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Domain.Entities;
using minimal_api.Infra.Db;
using minimal_api.Domain.DTO;

namespace minimal_api.Infra.Interfaces
{
    public interface IAdminService
    {
        Admin? Login(LoginDTO loginDTO);
        Admin Create(Admin admin);
        Admin? GetById(int id);
        List<Admin> GetAdmins(int? page);
    }
}