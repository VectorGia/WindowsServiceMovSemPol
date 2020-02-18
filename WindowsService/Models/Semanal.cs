using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Models
{
    public class Semanal

    {
        public long id { get; set; }
        public int year { get; set; }
        public int mes { get; set; }
        public int poliza { get; set; }
        public string tp { get; set; }
        public int linea { get; set; }
        public int cta { get; set; }
        public int scta { get; set; }
        public int sscta { get; set; }
        public string concepto { get; set; }
        public double monto { get; set; }
        public string folio_imp { get; set; }
        public int itm { get; set; }
        public int tm { get; set; }
        public string numpro { get; set; }
        public string cc { get; set; }
        public string referencia { get; set; }
        public string orden_compra { get; set; }
        public string fechapol { get; set; }
        public int id_empresa { get; set; }
        public int id_version { get; set; }
        public string cfd_ruta_pdf { get; set; }
        public string cfd_ruta_xml { get; set; }
        public string uuid { get; set; }
    }
}
