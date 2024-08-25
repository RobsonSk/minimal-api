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
    public interface IVehicleService
    {
        List<Vehicle> GetVehicles(int? page = 1, string? name = null, string? brand = null);
        Vehicle? GetById(int id);
        void Insert(Vehicle vehicle);
        void Update(Vehicle vehicle);
        void Delete(Vehicle vehicle);
    }
}