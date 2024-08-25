using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minimal_api.Domain.Enums;

namespace minimal_api.Domain.ModelViews
{
    public record AdminLogin
    {
        public string Email{ get; set; } = default!;
        public string Profile{ get; set; } = default!;
        public string Token{ get; set; } = default!;
    }
}