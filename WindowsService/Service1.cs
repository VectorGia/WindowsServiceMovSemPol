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
using WindowsService1.Models;
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
                ETLMovPolizaSemanalDA etlMovPoliza = new ETLMovPolizaSemanalDA();
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
                    List<Empresa> lstCompania = etlMovPoliza.EmpresaConexionETL_List(idEmpresa);
                    if (lstCompania!=null) {
                        if (lstCompania.Count >=1)
                        {
                            nombreCompania = lstCompania[0].nombre;
                        }
                    }

                    try
                    {
                        Thread.Sleep(timeTo);
                       
                        this.inicarEtlMovPolizaCSV(idEmpresa);
                    }
                    catch (Exception ex)
                    {
                        //string error = ex.Message;
                        //configCorreo.EnviarCorreo("Estimado Usuario : \n\n  La extracción correspondiente a la compania " + idEmpresa +"."+ nombreCompania + " se genero incorrectamente \n\n Mensaje de Error: \n " + ex, "ETL Extracción Balanza");
                        //throw;
                    }
                }


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

        public void inicarEtlMovPolizaCSV(Int64 idEmpresa)
        {

            ETLMovPolizaSemanalDA etlMovSemanal = new ETLMovPolizaSemanalDA();
            string archivo = string.Empty;
            string anio = string.Empty;
            string ruta = Constantes.CSV_PATH_SEMANAL;
            List<Semanal> lstSemanal = new List<Semanal>();

            DateTime fechaInicioProceso = DateTime.Now;
            Proceso proceso = new Proceso();

            try
            {

                //archivo = etlMovSemanal.generarScMOV_CSV(idEmpresa,ruta,anio);
                //Prueba 
                archivo = "PruebaSemanal.csv";

                int cantRegAfectados = etlMovSemanal.copy_semanal(archivo, ruta);

                DateTime fechaFinalProceso = DateTime.Now;
                configCorreo.EnviarCorreo("La extracción de Movimientos de Polizas Semanal se genero correctamente"
                                           + "\nFecha Inicio : " + fechaInicioProceso + " \n Fecha Final: " + fechaFinalProceso
                                           + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                           , "ETL Movimiento de Polizas Semanal Manual ");


                proceso.id_empresa = idEmpresa;
                proceso.tipo = Constantes.TIPO_EXT_PROGRAMADA;
                proceso.fecha_inicio = fechaInicioProceso;
                proceso.fecha_fin = fechaFinalProceso;
                proceso.estatus = Constantes.EST_EXT_FIN;
                proceso.mensaje = "";

                procesoDa.AddProceso(proceso);

            }

            catch (Exception ex)
            {
                DateTime fechaFinalProceso = DateTime.Now;
                configCorreo.EnviarCorreo("Ha ocurrido un error en la extracción de Movimientos de Polizas Semanal"
                                           + "\nFecha Inicio : " + fechaInicioProceso + "\nFecha Final: " + fechaFinalProceso
                                           + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                           + "\nError : " + ex.Message
                                           , "ETL Movimiento de Polizas Semanal Manual ");
                string error = ex.Message;
                proceso.id_empresa = idEmpresa;
                proceso.tipo = Constantes.TIPO_EXT_PROGRAMADA;
                proceso.fecha_inicio = fechaInicioProceso;
                proceso.fecha_fin = fechaFinalProceso;
                proceso.estatus = Constantes.EST_EXT_ERR;
                proceso.mensaje = ex.Message;

                procesoDa.AddProceso(proceso);

                throw;
            }


        }
    }
}
