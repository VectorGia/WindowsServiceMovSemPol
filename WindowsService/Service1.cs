using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsServiceSemanal.Models;
using WindowsServiceSemanal.Servicio;
using WindowsServiceSemanal.Util;

namespace WindowsServiceSemanal
{
    public partial class Service1 : ServiceBase
    {
        public static bool continua = false;
        ConfiguracionCorreoDA configCorreo = new ConfiguracionCorreoDA();
        ProcesoDA procesoDa = new ProcesoDA();

        public Service1()
        {
            InitializeComponent();
        }


        protected override void OnStart(string[] args)
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appSettings = configuration.AppSettings;
            string timeToRun = appSettings.Settings["timechecking"].Value;
            int timeTo = Convert.ToInt32(timeToRun);
            continua = true;
            iniciaServicio( timeTo);
        }

        public void iniciaServicio(int timeTo)
        {
            while (continua)
            {
                ETLBalanzaDA etl = new ETLBalanzaDA();
                ValidaExtraccion valiExtr = new ValidaExtraccion();
                List<ETLProg> lstExtrProg = new List<ETLProg>();
                lstExtrProg = valiExtr.lstExisteExtr();

                List<ETLProg> lstExtrProg1 = new List<ETLProg>();
                lstExtrProg1 = valiExtr.lstParametros();

                //int id_compania = lstExtrProg1[0].INT_ID_EMPRESA;
           
                Int64 idEmpresa = 0;
                string nombreCompania = "";

                foreach (ETLProg etlProg in lstExtrProg1) {
                    idEmpresa = etlProg.id;
                    List<Empresa> lstCompania = etl.EmpresaConexionETL_List(idEmpresa);
                    if (lstCompania!=null) {
                        if (lstCompania.Count >=1)
                        {
                            nombreCompania = lstCompania[0].nombre;
                        }
                    }

                    try
                    {
                        Thread.Sleep(timeTo);
                        //anterior 
                        ///etl.insertarTabBalanza(idEmpresa,nombreCompania);
                        /// con cvs 
                        this.iniciarETLBalanzaCSV(idEmpresa);
                    }
                    catch (Exception ex)
                    {
                        //string error = ex.Message;
                        //configCorreo.EnviarCorreo("Estimado Usuario : \n\n  La extracción correspondiente a la compania " + idEmpresa +"."+ nombreCompania + " se genero incorrectamente \n\n Mensaje de Error: \n " + ex, "ETL Extracción Balanza");
                        //throw;
                    }
                }



                //try
                //{
                //    Thread.Sleep(timeTo);
                //    Compania compania = new Compania();

                //    if (lstExtrProg.Count() == 0)
                //    {
                //        //etl.CadenaConexionETL(id_compania);
                //        etl.addTAB_BALANZA(id_compania);
                //        //if (lstExtrProg[0].EXISTE == 0)
                //        //if (lstExtrProg[0].EXISTE == 0)
                //        //{
                //        //    new ETL().addTAB_BALANZA(compania);
                //        //}
                //        //else
                //        //{
                //        //    continue;
                //        //}
                //    }
                //    else
                //    {
                        
                //    }

                    
                //}
                //catch (Exception ex)
                //{
                //    string error = ex.Message;
                //    throw;
                //}
            }
        }

        public void onDebug()
        {
            OnStart(null);
        }
        protected override void OnStop()
        {
            continua = false;
        }

        public void iniciarETLBalanzaCSV(Int64 idEmpresa)
        {
            ETLBalanzaDA etlBalanza = new ETLBalanzaDA(); 
            string archivo = string.Empty;

            string ruta = Constantes.CSV_PATH_BALANZA;
            List<Balanza> lstBala = new List<Balanza>();

            DateTime fechaInicioProceso = DateTime.Now;
            Proceso proceso = new Proceso();
            try
            {
                archivo = etlBalanza.generarSalContCC_CSV(idEmpresa, ruta);
                //prueba
                //archivo = "PruebaBalanzaRecrotado.csv";

                int cantRegAfectados = etlBalanza.copy_balanza(archivo, ruta);

                DateTime fechaFinalProceso = DateTime.Now;


                configCorreo.EnviarCorreo("La extracción de Balanza se genero correctamente"
                                           + "\nFecha Inicio : " + fechaInicioProceso + " \n Fecha Final: " + fechaFinalProceso
                                           + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                           , Constantes.MENSAJE_CORREO_ETL);

                proceso.id_empresa = idEmpresa;
                proceso.tipo = Constantes.TIPO_EXT_PROGRAMADA;
                proceso.fecha_inicio = fechaInicioProceso;
                proceso.fecha_fin = fechaFinalProceso;
                proceso.estatus = Constantes.EST_EXT_FIN;
                proceso.mensaje = "";

                procesoDa.AddProceso(proceso);

                etlBalanza.UpdateCuentaUnificada(idEmpresa);// concatencacion de cuentas 

            }
            catch (Exception ex)
            {

                DateTime fechaFinalProceso = DateTime.Now;
                configCorreo.EnviarCorreo("Ha ocurrido un error en la extracción de Balanza"
                                           + "\nFecha Inicio : " + fechaInicioProceso + "\nFecha Final: " + fechaFinalProceso
                                           + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                           + "\nError : " + ex.Message
                                           , Constantes.MENSAJE_CORREO_ETL);
                string error = ex.Message;
                proceso.id_empresa = idEmpresa;
                proceso.tipo = Constantes.TIPO_EXT_PROGRAMADA;
                proceso.fecha_inicio = fechaInicioProceso;
                proceso.fecha_fin = fechaFinalProceso;
                proceso.estatus = Constantes.EST_EXT_ERR;
                proceso.mensaje = ex.Message;
                procesoDa.AddProceso(proceso);
                //etlBalanza.UpdateCuentaUnificada();// concatencacion de cuentas 
                throw;
            }

        }
    }
}
