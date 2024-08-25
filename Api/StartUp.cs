using minimal_api;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Pomelo.EntityFrameworkCore.MySql;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Swashbuckle.AspNetCore.Swagger;
using minimal_api.Domain.DTO;
using minimal_api.Domain.Entities;
using minimal_api.Domain.Enums;
using minimal_api.Domain.ModelViews;
using minimal_api.Domain.Services;
using minimal_api.Infra.Db;
using minimal_api.Infra.Interfaces;

public class StartUp
{
    public StartUp(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?.GetSection("Jwt")?.ToString() ?? string.Empty;
    }

    private string key;
    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdminService, AdminService>();
        services.AddScoped<IVehicleService, VehicleService>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme. IE: \"Authorization: Bearer {token}\""
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
            });
        });


        services.AddDbContext<DatabaseContext>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql")));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            #region Home
            endpoints.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
            #endregion

            #region Admin
            string CreateTokenJWT(Admin admin)
            {
                if (string.IsNullOrEmpty(key)) return string.Empty;

                var claims = new List<Claim>{
                        new Claim ("Email", admin.Email),
                        new Claim (ClaimTypes.Role, admin.Profile),
                        new Claim ("Profile", admin.Profile)
                    };

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                );
                return new JwtSecurityTokenHandler().WriteToken(token);
            }
            endpoints.MapGet("/admin", ([FromQuery] int? page, IAdminService adminService) =>
            {
                var admins = new List<AdminView>();
                var listAdmins = adminService.GetAdmins(page);
                foreach (var admin in listAdmins)
                {
                    admins.Add(new AdminView
                    {
                        Id = admin.Id,
                        Email = admin.Email,
                        Profile = admin.Profile
                    });
                }
                return Results.Ok(admins);
            }).RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithTags("Admin");

            endpoints.MapGet("/admin/{id}", ([FromRoute] int id, IAdminService adminService) =>
            {
                var admin = adminService.GetById(id);
                if (admin == null) return Results.NotFound();
                return Results.Ok(new AdminView
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });
            }).RequireAuthorization()
            //.RequireRole("admin")
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithTags("Admin");

            endpoints.MapPost("/admin", ([FromBody] AdminDTO adminDTO, IAdminService adminService) =>
            {
                var validation = new ValidationErrors
                {
                    Messages = new List<string>()
                };

                if (string.IsNullOrEmpty(adminDTO.Email))
                {
                    validation.Messages.Add("Email is required");
                }
                if (string.IsNullOrEmpty(adminDTO.Password))
                {
                    validation.Messages.Add("Password is required");
                }
                if (adminDTO.Profile == null)
                {
                    validation.Messages.Add("Profile is required");
                }

                if (validation.Messages.Count > 0)
                {
                    return Results.BadRequest(validation);
                }

                var admin = new Admin
                {
                    Email = adminDTO.Email,
                    Password = adminDTO.Password,
                    Profile = adminDTO.Profile.ToString() ?? Profile.User.ToString()
                };

                adminService.Create(admin);
                return Results.Created($"/admin/{admin.Id}", new AdminView
                {
                    Id = admin.Id,
                    Email = admin.Email,
                    Profile = admin.Profile
                });

            }).RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithTags("Admin");

            endpoints.MapPost("/admin/login", ([FromBody] LoginDTO loginDTO, IAdminService adminService) =>
            {
                var adminLogin = adminService.Login(loginDTO);
                if (adminLogin != null)
                {
                    string token = CreateTokenJWT(adminLogin);
                    return Results.Ok(new AdminLogin
                    {
                        Email = adminLogin.Email,
                        Profile = adminLogin.Profile,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }).AllowAnonymous().WithTags("Admin");
            #endregion

            #region  Vehicles

            ValidationErrors validationDTO(VehicleDTO vehicleDTO)
            {
                var validation = new ValidationErrors
                {
                    Messages = new List<string>()
                };
                if (vehicleDTO.Name == null || vehicleDTO.Name == "")
                {
                    validation.Messages.Add("Name is required");
                }
                if (vehicleDTO.Brand == null || vehicleDTO.Brand == "")
                {
                    validation.Messages.Add("Brand is required");
                }
                if (vehicleDTO.Year < 1980)
                {
                    validation.Messages.Add("Year is required and greater than 1980");
                }
                return validation;
            }

            endpoints.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var validation = validationDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                {
                    return Results.BadRequest(validation);
                }

                var vehicle = new Vehicle
                {
                    Name = vehicleDTO.Name,
                    Brand = vehicleDTO.Brand,
                    Year = vehicleDTO.Year
                };

                vehicleService.Insert(vehicle);
                return Results.Created($"/vehicles/{vehicle.Id}", vehicle);
            }).RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole("Admin", "Editor"))
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles/", ([FromQuery] int? page, IVehicleService vehicleService) =>
            {
                var vehicles = vehicleService.GetVehicles(page);
                return Results.Ok(vehicles);
            }).RequireAuthorization()
            .WithTags("Vehicles");

            endpoints.MapGet("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null) return Results.NotFound();
                return Results.Ok(vehicle);
            }).RequireAuthorization().WithTags("Vehicles");

            endpoints.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null) return Results.NotFound();

                var validation = validationDTO(vehicleDTO);
                if (validation.Messages.Count > 0)
                {
                    return Results.BadRequest(validation);
                }

                vehicle.Name = vehicleDTO.Name;
                vehicle.Brand = vehicleDTO.Brand;
                vehicle.Year = vehicleDTO.Year;

                vehicleService.Update(vehicle);
                return Results.Ok(vehicle);
            }).RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole("Admin", "Editor"))
            .WithTags("Vehicles");

            endpoints.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService) =>
            {
                var vehicle = vehicleService.GetById(id);
                if (vehicle == null) return Results.NotFound();
                vehicleService.Delete(vehicle);
                return Results.NoContent();
            }).RequireAuthorization()
            .RequireAuthorization(policy => policy.RequireRole("Admin"))
            .WithTags("Vehicles");

            #endregion

        });
    }

}
