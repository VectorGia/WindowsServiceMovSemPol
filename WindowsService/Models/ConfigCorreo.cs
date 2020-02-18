using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceSemanal.Models
{
    public class ConfigCorreo
    {        //public int INT_ID_CORREO { get; set; }
        //public string TEXT_FROM { get; set; }
        //public string TEXT_PASSWORD { get; set; }
        //public int INT_PORT { get; set; }
        //public string TEXT_HOST { get; set; }

        public int id { get; set; }
        public string remitente { get; set; }
        public string password { get; set; }
        public int puerto { get; set; }
        public string host { get; set; }
    }
}
