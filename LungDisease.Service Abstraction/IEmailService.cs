using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.Common_Result;

namespace Inventra.Service_Abstraction
{
    public interface IEmailService
    {
        Task<Result> SendAsync(string to, string subject, string body);
    }
}
