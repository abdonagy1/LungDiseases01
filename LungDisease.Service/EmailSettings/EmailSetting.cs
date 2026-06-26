using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LungDisease.Service.EmailSettings
{
    public class EmailSetting
    {
        public string Host { get; set; } = default!;
        public int Port { get; set; }
        public string FromEmail { get; set; } = default!;
        public string UserName { get; set; } = default!;
        public string Password { get; set; } = default!;
    }
}
