using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LungDisease.Shared.DataTransferObjects.IdentityDTOs
{
    public record LoginDTO([EmailAddress] string Email, string Password);
}
