using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsServiceSemanal.Util;

namespace WindowsServiceSemanal.Servicio
{
    public class ValidaExtraccion
    {
        NpgsqlConnection con = new NpgsqlConnection();
        Conexion.Conexion conex = new Conexion.Conexion();
        NpgsqlCommand com = new NpgsqlCommand();
        char cod = '"';

        public ValidaExtraccion()
        {
            //Constructor

            con = conex.ConnexionDB();
        }

        /// <summary>
        /// Busca en la tabla y recupera todos lo registros de la tabla de extracción programada
        /// </summary>
        /// <returns>Data Table TAB_ETL_PROG</returns> 
        public DataTable FechaExtra()
        {
            ////string consulta = "SELECT " + cod + "INT_ID_ETL_PROG" + cod + ","
            ////    + cod + "TEXT_FECH_EXTR" + cod + ","
            ////    + cod + "TEXT_HORA_EXTR" + cod + ","
            ////    + cod + "INT_ID_EMPRESA" + cod
            ////    + " FROM " + cod + "TAB_ETL_PROG" + cod;
            //////+ " WHERE " + cod + "INT_ID_EMPRESA" + cod + " = " + id_empresa;

            string consulta = " select  id ,"
                + " fecha_extraccion ," 
                + " hora_extraccion ," 
                + " id_empresa , " 
                + " modulo "
                + " from  etl_prog " 
                //+ " where  id_empresa  = " + idEmpresa
                + " where modulo =  '"+ Constantes.MODULO_SEMANAL+"'" ;

            try
            {
                con.Open();
                com = new Npgsql.NpgsqlCommand(consulta, con);
                Npgsql.NpgsqlDataAdapter daP = new Npgsql.NpgsqlDataAdapter(com);

                DataTable dt = new DataTable();
                daP.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                con.Close();
                string error = ex.Message;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public List<ETLProg> lstParametros()
        {
            List<ETLProg> lstEtlP = new List<ETLProg>();
            DataTable dt = new DataTable();
            dt = FechaExtra();
            foreach (DataRow r in dt.Rows)
            {
                ETLProg etlProg = new ETLProg();
                etlProg.id = Convert.ToInt32(r["id"]);
                etlProg.fecha_extraccion = r["fecha_extraccion"].ToString();
                etlProg.hora_extraccion = r["hora_extraccion"].ToString();
                etlProg.id_empresa = Convert.ToInt32(r["id_empresa"]);

                lstEtlP.Add(etlProg);
            }
            return lstEtlP;
        }

        /// <summary>
        /// Metodo para revisar si ya existe una extracion PREVIA
        /// El tipo de extraccion es Programada
        /// debe recibir el ID de la empresa
        /// </summary>
        /// <returns></returns>
        public DataTable existeExtr()
        {
            ETLProg tab_etl_prog = new ETLProg();
            List<ETLProg> lstPara = lstParametros();

            string consulta = " select 1 as existe, tipo_extraccion, "
                + " fecha_carga, "
                + " hora_carga, "
                + " id_empresa "
                + " from balanza "
                + " where id_empresa = " + lstPara[0].id
                + " and tipo_extraccion = " + 2;

            //string consulta = "SELECT 1 as EXISTE, " + cod + "INT_TIPO_EXTRACCION" + cod + ","
            //       + cod + "TEXT_FECH_EXTR" + cod + ","
            //       + cod + "TEXT_HORA" + cod + ","
            //       + cod + "INT_ID_EMPRESA" + cod
            //       + " FROM " + cod + "TAB_BALANZA" + cod
            //       + " WHERE" + cod + "INT_ID_EMPRESA" + cod + " = " + lstPara[0].id //tab_etl_prog.INT_ID_EMPRESA
            //       + " AND " + cod + "INT_TIPO_EXTRACCION" + cod +" = 2";
            try
            {
                con.Open();
                com = new Npgsql.NpgsqlCommand(consulta, con);
                Npgsql.NpgsqlDataAdapter daP = new Npgsql.NpgsqlDataAdapter(com);
                con.Close();
                DataTable dt = new DataTable();
                daP.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                con.Close();
                string error = ex.Message;
                throw;
            }
            finally
            {
                con.Close();
            }
        }

        public List<ETLProg> lstExisteExtr()
        {
            List<ETLProg> lstEtlP = new List<ETLProg>();
            DataTable dt = new DataTable();
            dt = existeExtr();
            foreach (DataRow r in dt.Rows)
            {
                ETLProg etlProg = new ETLProg();
                etlProg.id = Convert.ToInt32(r["id"]);
                etlProg.fecha_extraccion = r["fecha_extraccion"].ToString();
                etlProg.hora_extraccion = r["hora_extraccion"].ToString();
                etlProg.id_empresa = Convert.ToInt32(r["id_empresa"]);
                //etlProg.EXISTE = Convert.ToInt32(r["EXISTE"]);
                lstEtlP.Add(etlProg);
            }
            return lstEtlP;
        }

        /// <summary>
        /// Metodo para obtener fecha y hora del sistema comparar contra los valores de BD
        /// indica si se debe o no realizar la extraccion
        /// </summary>
        /// <returns></returns>
        //public int compararParametros()
        //{
        //    int realizarExtr;
        //    string fechaSistema = DateTime.Now.ToShortDateString();
        //    string hora = DateTime.Now.ToShortTimeString();
        //    TAB_ETL_PROG tab_etl_prog = new TAB_ETL_PROG();
        //    List<TAB_ETL_PROG> lstEtlP = new List<TAB_ETL_PROG>();
        //    lstEtlP = lstParametros();
            
        //    int extrProg = lstEtlP.Count();

        //    for (int i = 0; i < extrProg; i++)
        //    {                
        //        /*Si se requiere comparar fecha - hora se puede agregar en este if*/
        //        if (lstEtlP[i].EXISTE == 1) /*No hace la estracción*/
        //        {
        //             realizarExtr = 1;
        //        }
        //        else
        //        {
        //             realizarExtr = 0;
        //        }
        //        return realizarExtr;
        //    }            
        //}
    }
}
