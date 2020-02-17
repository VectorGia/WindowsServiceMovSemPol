using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Servicio
{
    public class Empresa
    {
        //public Empresa()
        //{
        //    //Constructor
        //}

        //public string STR_NOMBRE_COMPANIA { get; set; }

        //public string STR_ABREV_COMPANIA { get; set; }

        //public string STR_IDCOMPANIA { get; set; }

        //public bool BOOL_ETL_COMPANIA { get; set; }

        //public string STR_HOST_COMPANIA { get; set; }

        //public string STR_USUARIO_ETL { get; set; }

        //public string STR_CONTRASENIA_ETL { get; set; }

        //public string STR_PUERTO_COMPANIA { get; set; }

        //public string STR_MONEDA_COMPANIA { get; set; }

        //public string STR_BD_COMPANIA { get; set; }

        //public bool BOOL_ESTATUS_LOGICO_COMPANIA { get; set; }

        //public int INT_IDCOMPANIA_P { get; set; }

        //public int INT_IDPROYECTO_F { get; set; }

        //public int INT_IDCENTROCOSTO_F { get; set; }

        //public DateTime FEC_MODIF_COMPANIA { get; set; }

        public Int64 id { get; set; }
        public int id_modelo_neg { get; set; }
        public int id_centro_costo { get; set; }
        public int moneda_id { get; set; }
        public bool activo { get; set; }
        public bool estatus { get; set; }
        public bool etl { get; set; }
        public string nombre { get; set; }
        public string desc_id { get; set; }
        public string abrev { get; set; }
        public string bd_name { get; set; }
        public string contrasenia_etl { get; set; }
        public DateTime fec_modif { get; set; }
        public string host { get; set; }
        public int puerto_compania { get; set; }
        public string usuario_etl { get; set; }
        public byte[] contra_bytes { get; set; }
        public byte[] llave { get; set; }
        public byte[] apuntador { get; set; }

    }
}
