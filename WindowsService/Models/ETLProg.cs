using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Servicio
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

        public Int64 id { get; set; }
        public string fecha_extraccion { get; set; }
        public string hora_extraccion { get; set; }
        public Int64 id_empresa { get; set; }



    }
}
