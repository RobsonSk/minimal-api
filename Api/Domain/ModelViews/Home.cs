using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minimal_api.Domain.ModelViews
{
    public struct Home
    {
        public string Message { get => "Welcome to - Minimal API"; }
        public string Documentation { get => "/swagger"; }
    }
}