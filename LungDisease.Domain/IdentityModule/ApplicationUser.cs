using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LungDisease.Domain.IdentityModule
{
    public class ApplicationUser:IdentityUser
    {
        public string DisplayName { get; set; } = default!;
    }
}
