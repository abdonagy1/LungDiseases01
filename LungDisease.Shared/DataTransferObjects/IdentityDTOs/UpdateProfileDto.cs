using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LungDisease.Shared.DataTransferObjects.IdentityDTOs
{
    public class UpdateProfileDto
    {
        public string? UserName { get; set; } = default!;
        public string? DisplayName { get; set; } = default!;
        public string? Email { get; set; } = default!;
    }
}
