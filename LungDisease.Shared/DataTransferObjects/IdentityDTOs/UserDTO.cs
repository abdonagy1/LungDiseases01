using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LungDisease.Shared.DataTransferObjects.IdentityDTOs
{
    public record UserDTO(string Email, string Display, string Token);
}
