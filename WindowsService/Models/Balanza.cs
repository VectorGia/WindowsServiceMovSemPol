using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsService1.Servicio
{
    public class Balanza
    {

            //Constructor;
            //}
            //public int INT_IDBALANZA { get; set; }
            //public string TEXT_CTA { get; set; }
            //public string TEXT_SCTA { get; set; }
            //public string TEXT_SSCTA { get; set; }
            //public int INT_YEAR { get; set; }
            //public double DECI_ENECARGOS { get; set; }
            //public double DECI_SALINI { get; set; }
            //public double DECI_ENEABONOS { get; set; }
            //public double DECI_FEBCARGOS { get; set; }
            //public double DECI_FEBABONOS { get; set; }
            //public double DECI_MARCARGOS { get; set; }
            //public double DECI_MARABONOS { get; set; }
            //public double DECI_ABRCARGOS { get; set; }
            //public double DECI_ABRABONOS { get; set; }
            //public double DECI_MAYCARGOS { get; set; }
            //public double DECI_MAYABONOS { get; set; }
            //public double DECI_JUNCARGOS { get; set; }
            //public double DECI_JUNABONOS { get; set; }
            //public double DECI_JULCARGOS { get; set; }
            //public double DECI_JULABONOS { get; set; }
            //public double DECI_AGOCARGOS { get; set; }
            //public double DECI_AGOABONOS { get; set; }
            //public double DECI_SEPCARGOS { get; set; }
            //public double DECI_SEPABONOS { get; set; }
            //public double DECI_OCTCARGOS { get; set; }
            //public double DECI_OCTABONOS { get; set; }
            //public double DECI_NOVCARGOS { get; set; }
            //public double DECI_NOVABONOS { get; set; }
            //public double DECI_DICCARGOS { get; set; }
            //public double DECI_DICABONOS { get; set; }
            //public int INT_CC { get; set; }
            //public string TEXT_DESCRIPCION { get; set; }
            //public string TEXT_DESCRIPCION2 { get; set; }
            //public int INT_INCLUIR_SUMA { get; set; }
            //public int INT_TIPO_EXTRACCION { get; set; }
            //public string TEXT_FECH_EXTR { get; set; }
            //public string TEXT_HORA { get; set; }
            //public int INT_ID_EMPRESA { get; set; }
            //public double DECI_CIERRE_CARGOS { get; set; }
            //public double DECI_CIERRE_ABONOS { get; set; }
            //public int INT_ACTA { get; set; }
            //public string TEXT_CC { get; set; }


        public int id { get; set; }
        public string cta { get; set; }
        public string scta { get; set; }
        public string sscta { get; set; }
        public int year { get; set; }
        public string concepto { get; set; }
        public double enecargos { get; set; }
        public double salini { get; set; }
        public double eneabonos { get; set; }
        public double febcargos { get; set; }
        public double febabonos { get; set; }
        public double marcargos { get; set; }
        public double marabonos { get; set; }
        public double abrcargos { get; set; }
        public double abrabonos { get; set; }
        public double maycargos { get; set; }
        public double mayabonos { get; set; }
        public double juncargos { get; set; }
        public double junabonos { get; set; }
        public double julcargos { get; set; }
        public double julabonos { get; set; }
        public double agocargos { get; set; }
        public double agoabonos { get; set; }
        public double sepcargos { get; set; }
        public double sepabonos { get; set; }
        public double octcargos { get; set; }
        public double octabonos { get; set; }
        public double novcargos { get; set; }
        public double novabonos { get; set; }
        public double diccargos { get; set; }
        public double dicabonos { get; set; }
        public int incluir_suma { get; set; }
        public int tipo_extraccion { get; set; }
        public string fecha_carga { get; set; }
        public string hora_carga { get; set; }
        public int id_empresa { get; set; }
        public double cierre_cargos { get; set; }
        public double cierre_abonos { get; set; }
        public int acta { get; set; }
        public string cc { get; set; }
        public string cuenta_unificada { get; set; }

    }
}
