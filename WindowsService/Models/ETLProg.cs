using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsServiceSemanal.Servicio
{
    public class ETLProg
    {
        public ETLProg()
        {
            //Constructor
        }

        //public int INT_ID_ETL_PROG { get; set; } 
        //public string TEXT_FECH_EXTR { get; set; }
        //public string TEXT_HORA_EXTR { get; set; }
        //public int INT_ID_EMPRESA { get; set; }
        //public int EXISTE { get; set; }

        public Int64 id { set; get; }
        public string fecha_extraccion { set; get; }
        public string hora_extraccion { set; get; }
        public Int64 id_empresa { get; set; }
        public string modulo { get; set; }
        public int anio_inicio { get; set; }
        public int anio_fin { get; set; }
        public bool activo { get; set; }
    }
}
