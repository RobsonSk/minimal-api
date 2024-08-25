using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Infra.Interfaces;
using minimal_api.Domain.Services;
using minimal_api.Domain.Entities;
using minimal_api.Infra.Db;
using minimal_api.Domain.DTO;

namespace minimal_api.Domain.Services
{

    public class AdminService : IAdminService
    {
        private readonly DatabaseContext _context;
        public AdminService(DatabaseContext context)
        {
            _context = context;
        }

        public Admin? Login(LoginDTO loginDTO)
        {
            var adm = _context.Admins.Where(x => x.Email == loginDTO.Email && x.Password == loginDTO.Password).FirstOrDefault();
            return adm;
        }
        public Admin Create(Admin admin)
        {
            _context.Admins.Add(admin);
            _context.SaveChanges();
            return admin;
        }

        public Admin? GetById(int id)
        {
            return _context.Admins.Where(x => x.Id == id).FirstOrDefault();
        }

        public List<Admin> GetAdmins(int? page)
        {
            var query = _context.Admins.AsQueryable();
            int perPageItens = 10;
            if (page != null)
            {
                query = query.Skip(((int)page - 1) * perPageItens).Take(perPageItens);
            }
            return query.ToList();
        }
    }
}