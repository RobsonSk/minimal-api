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

    public class VehicleService : IVehicleService
    {
        private readonly DatabaseContext _context;
        public VehicleService(DatabaseContext context)
        {
            _context = context;
        }

        public List<Vehicle> GetVehicles(int? page = 1, string? name = null, string? brand = null)
        {
            var query = _context.Vehicles.AsQueryable();
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(x => x.Brand.Contains(brand));
            }
            int perPageItens = 10;
            if (page != null)
            {
                query = query.Skip(((int)page - 1) * perPageItens).Take(perPageItens);
            }
            return query.ToList();
        }

        public Vehicle? GetById(int id)
        {
            return _context.Vehicles
                .Where(x => x.Id == id)
                .FirstOrDefault();
        }

        public void Insert(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            _context.SaveChanges();
        }

        public void Update(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            _context.SaveChanges();
        }

        public void Delete(Vehicle vehicle)
        {
            _context.Vehicles.Remove(vehicle);
            _context.SaveChanges();
        }
    }
}