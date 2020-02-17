using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsService1.Models;
using WindowsService1.Util;

namespace WindowsService1.Servicio
{
    public class ETLBalanzaDA
    {
        ConfiguracionCorreoDA configCorreo = new ConfiguracionCorreoDA();
        Conexion.Conexion conex = new Conexion.Conexion();

        NpgsqlConnection conP = new NpgsqlConnection();
        NpgsqlCommand comP = new NpgsqlCommand();
        char cod = '"';

        SqlConnection conSQLETL = new SqlConnection();
        SqlCommand comSQLETL = new SqlCommand();
        DSNConfig dsnConfig = new DSNConfig();
        OdbcConnection odbcCon;

        public ETLBalanzaDA()
        {
            //Constructor
      
            conP = conex.ConnexionDB();
        }

        /// <summary>
        /// Metodo para obtener los valores de la cadena de conexion de la empresa
        /// </summary>
        /// <param name="compania"></param>
        /// <returns></returns>
        public DataTable EmpresaConexionETL(Int64 idEmpresa)
        {
             string add = "SELECT  id ,"
             + "  usuario_etl ,"
             + " nombre ,"
             //+ " contrasenia_etl ,"
             + " host ,"
             + " puerto_compania ,"
             + " bd_name ,"
             + " contra_bytes ,"
             + " llave ,"
             + " apuntador "
             + " FROM empresa"
             + " WHERE  id  = " + idEmpresa;
            try
            {
                conP.Open();
                comP = new Npgsql.NpgsqlCommand(add, conP);
                Npgsql.NpgsqlDataAdapter daP = new Npgsql.NpgsqlDataAdapter(comP);

                DataTable dt = new DataTable();
                daP.Fill(dt);
                return dt;
            }
            catch (Exception ex)
            {
                conP.Close();
                string error = ex.Message;
                throw;
            }
        }

        /// <summary>
        /// Se convierte el DataTable en una Lista Generica
        /// </summary>
        /// <param name="compania"></param>
        /// <returns>Lista del tipo Compania</returns>
        public List<Empresa> EmpresaConexionETL_List(Int64 idEmpresa)
        {
            List<Empresa> lst = new List<Empresa>();
            DataTable dt = new DataTable();
            dt = EmpresaConexionETL(idEmpresa);

            foreach (DataRow r in dt.Rows)
            {
                Empresa cia = new Empresa();
                cia.usuario_etl = r["usuario_etl"].ToString();
                //cia.contrasenia_etl = r["contrasenia_etl"].ToString();
                cia.host = r["host"].ToString();
                cia.puerto_compania = Convert.ToInt32(r["puerto_compania"]);
                cia.bd_name = r["bd_name"].ToString();
                cia.id = Convert.ToInt32(r["id"]);
                cia.nombre = Convert.ToString(r["nombre"]);
                cia.contra_bytes = r["contra_bytes"] as byte[];
                cia.llave = r["llave"] as byte[];
                cia.apuntador = r["apuntador"] as byte[];
                lst.Add(cia);
            }
            return lst;
        }

        /// <summary>
        /// Crea la cadena de conexion segun lo guardado en la tabla compania
        /// realiza el select a la tabla Balanza
        /// </summary>
        /// <param name="compania"></param>
        /// <returns>Extracción del SQL</returns>
        public DataTable cadena_conexion_extrac(int id_compania)
        {
            string UserID = string.Empty;
            string Password = string.Empty;
            string Host = string.Empty;
            string Port = string.Empty;
            string DataBase = string.Empty;
            string Cadena = string.Empty;

            List<Empresa> lstCadena = new List<Empresa>();
            lstCadena = EmpresaConexionETL_List(id_compania);
            UserID = lstCadena[0].usuario_etl;
            Password = lstCadena[0].contrasenia_etl;
            Host = lstCadena[0].host;
            Port = lstCadena[0].puerto_compania.ToString();
           
            DataBase = lstCadena[0].bd_name;

            /*Cadena de Postegres
            Cadena = "USER ID=" + UserID + ";" + "Password=" + Password + ";" + "Host=" + Host + ";" + "Port =" + Port + ";" + "DataBase=" + DataBase + ";" + "Pooling=true;";*/
            //conPETL = new NpgsqlConnection(Cadena);

            /*Cadena de SQL*/
            Cadena = "Data Source =" + Host +","+ Port+";" + "Initial Catalog=" + DataBase + ";" + "Persist Security Info=True;" + "User ID=" + UserID + ";Password=" + Password;
            conSQLETL = new SqlConnection(Cadena);

            try
            {
                string add = "SELECT[INT_IDBALANZA], [VARCHAR_CTA],"
                            + "[VARCHAR_SCTA], [VARCHAR_SSCTA],"
                            + "[INT_YEAR], [DECI_SALINI],"
                            + "[DECI_ENECARGOS], [DECI_ENEABONOS],"
                            + "[DECI_FEBCARGOS], [DECI_FEBABONOS],"
                            + "[DECI_MARCARGOS], [DECI_MARABONOS],"
                            + "[DECI_ABRCARGOS], [DECI_ABRABONOS],"
                            + "[DECI_MAYCARGOS], [DECI_MAYABONOS],"
                            + "[DECI_JUNCARGOS], [DECI_JUNABONOS],"
                            + "[DECI_JULCARGOS], [DECI_JULABONOS],"
                            + "[DECI_AGOCARGOS], [DECI_AGOABONOS],"
                            + "[DECI_SEPCARGOS], [DECI_SEPABONOS],"
                            + "[DECI_OCTCARGOS], [DECI_OCTABONOS],"
                            + "[DECI_NOVCARGOS], [DECI_NOVABONOS],"
                            + "[DECI_DICCARGOS], [DECI_DICABONOS],"
                            + "[INT_CC],"
                            + "[VARCHAR_DESCRIPCION],"
                            + "[VARCHAR_DESCRIPCION2],"
                            + "[DECI_INCLUIR_SUMA]"
                            + " FROM [TAB_BALANZA_SQL]";

                comSQLETL = new SqlCommand(add, conSQLETL);
                SqlDataAdapter da = new SqlDataAdapter(comSQLETL);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;

            }
            catch (Exception ex)
            {
                string error = ex.Message;
                throw;
            }
            finally
            {
                conSQLETL.Close();
            }
        }

        public List<Balanza> lstBalanzaETL(int id_compania)
        {
            List<Balanza> lstBalanza = new List<Balanza>();
            DataTable dt = new DataTable();
            dt = cadena_conexion_extrac(id_compania);
            foreach (DataRow r in dt.Rows)
            {
                Balanza balanza = new Balanza();
                balanza.id = Convert.ToInt32(r["INT_IDBALANZA"]);
                balanza.cta = r["VARCHAR_CTA"].ToString();
                balanza.scta = r["VARCHAR_SCTA"].ToString();
                balanza.sscta = r["VARCHAR_SSCTA"].ToString();
                balanza.year = Convert.ToInt32(r["INT_YEAR"]);
                balanza.salini = Convert.ToDouble(r["DECI_SALINI"]);
                balanza.enecargos = Convert.ToDouble(r["DECI_ENECARGOS"]);
                balanza.eneabonos = Convert.ToDouble(r["DECI_ENEABONOS"]);
                balanza.febcargos = Convert.ToDouble(r["DECI_FEBCARGOS"]);
                balanza.febabonos = Convert.ToDouble(r["DECI_FEBABONOS"]);
                balanza.marcargos = Convert.ToDouble(r["DECI_MARCARGOS"]);
                balanza.marabonos = Convert.ToDouble(r["DECI_MARABONOS"]);
                balanza.abrcargos = Convert.ToDouble(r["DECI_ABRCARGOS"]);
                balanza.abrabonos = Convert.ToDouble(r["DECI_ABRABONOS"]);
                balanza.maycargos = Convert.ToDouble(r["DECI_ABRABONOS"]);
                balanza.mayabonos = Convert.ToDouble(r["DECI_MAYABONOS"]);
                balanza.juncargos = Convert.ToDouble(r["DECI_JUNCARGOS"]);
                balanza.junabonos = Convert.ToDouble(r["DECI_JUNABONOS"]);
                balanza.julcargos = Convert.ToDouble(r["DECI_JULCARGOS"]);
                balanza.julabonos = Convert.ToDouble(r["DECI_JULABONOS"]);
                balanza.agocargos = Convert.ToDouble(r["DECI_AGOCARGOS"]);
                balanza.agoabonos = Convert.ToDouble(r["DECI_AGOABONOS"]);
                balanza.sepcargos = Convert.ToDouble(r["DECI_AGOABONOS"]);
                balanza.sepabonos = Convert.ToDouble(r["DECI_SEPABONOS"]);
                balanza.octcargos = Convert.ToDouble(r["DECI_OCTCARGOS"]);
                balanza.octabonos = Convert.ToDouble(r["DECI_OCTABONOS"]);
                balanza.novcargos = Convert.ToDouble(r["DECI_NOVCARGOS"]);
                balanza.novabonos = Convert.ToDouble(r["DECI_NOVABONOS"]);
                balanza.diccargos = Convert.ToDouble(r["DECI_DICCARGOS"]);
                balanza.dicabonos = Convert.ToDouble(r["DECI_DICABONOS"]);
                balanza.cc = Convert.ToString(r["INT_CC"]);
                balanza.incluir_suma = Convert.ToInt32(r["DECI_INCLUIR_SUMA"]);
                lstBalanza.Add(balanza);
            }
            return lstBalanza;
        }

        public int addTAB_BALANZA(int id_compania)
        {
            List<Balanza> lstBala = new List<Balanza>();
            lstBala = lstBalanzaETL(id_compania);

            string addBalanza = "INSERT INTO"
                    + cod + "TAB_BALANZA" + cod + "("
                    //+ cod + "INT_IDBALANZA" + cod + ","
                    + cod + "TEXT_CTA" + cod + ","
                    + cod + "TEXT_SCTA" + cod + ","
                    + cod + "TEXT_SSCTA" + cod + ","
                    + cod + "INT_YEAR" + cod + ","
                    + cod + "DECI_SALINI" + cod + ","
                    + cod + "DECI_ENECARGOS" + cod + ","
                    + cod + "DECI_ENEABONOS" + cod + ","
                    + cod + "DECI_FEBCARGOS" + cod + ","
                    + cod + "DECI_FEBABONOS" + cod + ","
                    + cod + "DECI_MARCARGOS" + cod + ","
                    + cod + "DECI_MARABONOS" + cod + ","
                    + cod + "DECI_ABRCARGOS" + cod + ","
                    + cod + "DECI_ABRABONOS" + cod + ","
                    + cod + "DECI_MAYCARGOS" + cod + ","
                    + cod + "DECI_MAYABONOS" + cod + ","
                    + cod + "DECI_JUNCARGOS" + cod + ","
                    + cod + "DECI_JUNABONOS" + cod + ","
                    + cod + "DECI_JULCARGOS" + cod + ","
                    + cod + "DECI_JULABONOS" + cod + ","
                    + cod + "DECI_AGOCARGOS" + cod + ","
                    + cod + "DECI_AGOABONOS" + cod + ","
                    + cod + "DECI_SEPCARGOS" + cod + ","
                    + cod + "DECI_SEPABONOS" + cod + ","
                    + cod + "DECI_OCTCARGOS" + cod + ","
                    + cod + "DECI_OCTABONOS" + cod + ","
                    + cod + "DECI_NOVCARGOS" + cod + ","
                    + cod + "DECI_NOVABONOS" + cod + ","
                    + cod + "DECI_DICCARGOS" + cod + ","
                    + cod + "DECI_DICABONOS" + cod + ","
                    + cod + "INT_CC" + cod + ","
                    + cod + "TEXT_DESCRIPCION" + cod + ","
                    + cod + "TEXT_DESCRIPCION2" + cod + ","
                    + cod + "INT_INCLUIR_SUMA" + cod + ")"
                        + "VALUES "
                            //+ "(@INT_IDBALANZA,"
                            + "(@TEXT_CTA,"
                            + "@TEXT_SCTA,"
                            + "@TEXT_SSCTA,"
                            + "@INT_YEAR,"
                            + "@DECI_SALINI,"
                            + "@DECI_ENECARGOS,"
                            + "@DECI_ENEABONOS,"
                            + "@DECI_FEBCARGOS,"
                            + "@DECI_FEBABONOS,"
                            + "@DECI_MARCARGOS,"
                            + "@DECI_MARABONOS,"
                            + "@DECI_ABRCARGOS,"
                            + "@DECI_ABRABONOS,"
                            + "@DECI_MAYCARGOS,"
                            + "@DECI_MAYABONOS,"
                            + "@DECI_JUNCARGOS,"
                            + "@DECI_JUNABONOS,"
                            + "@DECI_JULCARGOS,"
                            + "@DECI_JULABONOS,"
                            + "@DECI_AGOCARGOS,"
                            + "@DECI_AGOABONOS,"
                            + "@DECI_SEPCARGOS,"
                            + "@DECI_SEPABONOS,"
                            + "@DECI_OCTCARGOS,"
                            + "@DECI_OCTABONOS,"
                            + "@DECI_NOVCARGOS,"
                            + "@DECI_NOVABONOS,"
                            + "@DECI_DICCARGOS,"
                            + "@DECI_DICABONOS,"
                            + "@INT_CC,"
                            + "@TEXT_DESCRIPCION,"
                            + "@TEXT_DESCRIPCION2,"
                            + "@INT_INCLUIR_SUMA)";

            try
            {
                {
                    NpgsqlCommand cmd = new NpgsqlCommand(addBalanza, conP);
                    //cmd.Parameters.AddWithValue("@INT_IDBALANZA", NpgsqlTypes.NpgsqlDbType.Integer, lstBala[0].INT_IDBALANZA);
                    cmd.Parameters.AddWithValue("@TEXT_CTA", NpgsqlTypes.NpgsqlDbType.Text, lstBala[0].cta);
                    cmd.Parameters.AddWithValue("@TEXT_SCTA", NpgsqlTypes.NpgsqlDbType.Text, lstBala[0].scta);
                    cmd.Parameters.AddWithValue("@TEXT_SSCTA", NpgsqlTypes.NpgsqlDbType.Text, lstBala[0].sscta);
                    cmd.Parameters.AddWithValue("@INT_YEAR", NpgsqlTypes.NpgsqlDbType.Integer, lstBala[0].year);
                    cmd.Parameters.AddWithValue("@DECI_SALINI", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].salini);
                    cmd.Parameters.AddWithValue("@DECI_ENECARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].enecargos);
                    cmd.Parameters.AddWithValue("@DECI_ENEABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].eneabonos);
                    cmd.Parameters.AddWithValue("@DECI_FEBCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].febcargos);
                    cmd.Parameters.AddWithValue("@DECI_FEBABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].febabonos);
                    cmd.Parameters.AddWithValue("@DECI_MARCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].marcargos);
                    cmd.Parameters.AddWithValue("@DECI_MARABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].marabonos);
                    cmd.Parameters.AddWithValue("@DECI_ABRCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].abrcargos);
                    cmd.Parameters.AddWithValue("@DECI_ABRABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].abrabonos);
                    cmd.Parameters.AddWithValue("@DECI_MAYCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].maycargos);
                    cmd.Parameters.AddWithValue("@DECI_MAYABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].mayabonos);
                    cmd.Parameters.AddWithValue("@DECI_JUNCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].juncargos);
                    cmd.Parameters.AddWithValue("@DECI_JUNABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].junabonos);
                    cmd.Parameters.AddWithValue("@DECI_JULCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].julcargos);
                    cmd.Parameters.AddWithValue("@DECI_JULABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].julabonos);
                    cmd.Parameters.AddWithValue("@DECI_AGOCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].agocargos);
                    cmd.Parameters.AddWithValue("@DECI_AGOABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].agoabonos);
                    cmd.Parameters.AddWithValue("@DECI_SEPCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].sepcargos);
                    cmd.Parameters.AddWithValue("@DECI_SEPABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].sepabonos);
                    cmd.Parameters.AddWithValue("@DECI_OCTCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].octcargos);
                    cmd.Parameters.AddWithValue("@DECI_OCTABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].octabonos);
                    cmd.Parameters.AddWithValue("@DECI_NOVCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].novcargos);
                    cmd.Parameters.AddWithValue("@DECI_NOVABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].novabonos);
                    cmd.Parameters.AddWithValue("@DECI_DICCARGOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].diccargos);
                    cmd.Parameters.AddWithValue("@DECI_DICABONOS", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].dicabonos);
                    cmd.Parameters.AddWithValue("@INT_CC", NpgsqlTypes.NpgsqlDbType.Integer, lstBala[0].cc);
                    //cmd.Parameters.AddWithValue("@TEXT_DESCRIPCION", NpgsqlTypes.NpgsqlDbType.Text, lstBala[0].TEXT_DESCRIPCION);
                    //cmd.Parameters.AddWithValue("@TEXT_DESCRIPCION2", NpgsqlTypes.NpgsqlDbType.Text, lstBala[0].TEXT_DESCRIPCION2);
                    cmd.Parameters.AddWithValue("@INT_INCLUIR_SUMA", NpgsqlTypes.NpgsqlDbType.Double, lstBala[0].incluir_suma);
                    //conP.Open();
                    int cantFilaAfect = Convert.ToInt32(cmd.ExecuteNonQuery());
                    conP.Close();
                    return cantFilaAfect;
                }
            }
            catch (Exception ex)
            {
                conP.Close();
                string error = ex.Message;
                throw;
            }
        }

        #region Sybase Extraccion
        public List<ScSaldoConCc> obtenerSalContCC(Int64 idEmpresa)
        {
            /// creacion de odbc 
            DSN dsn = new DSN();
            dsn = dsnConfig.crearDSN(idEmpresa);

            if (dsn.creado)
            {
                /// obtener conexion de Odbc creado 
                odbcCon = conex.ConexionSybaseodbc(dsn.nombreDSN);

                try
                {
                  //// eventual SYBASE GIA EN SITIO 
                    string consulta = " SELECT "
                                + "year,"
                                + "cta,"
                                + "scta,"
                                + "sscta,"
                                + "salini,"
                                + "enecargos,"
                                + "eneabonos,"
                                + "febcargos,"
                                + "febabonos,"
                                + "marcargos,"
                                + "marabonos,"
                                + "abrcargos,"
                                + "abrabonos,"
                                + "maycargos,"
                                + "mayabonos,"
                                + "juncargos,"
                                + "junabonos,"
                                + "julcargos,"
                                + "julabonos,"
                                + "agocargos,"
                                + "agoabonos,"
                                + "sepcargos,"
                                + "sepabonos,"
                                + "octcargos,"
                                + "octabonos,"
                                + "novcargos,"
                                + "novabonos,"
                                + "diccargos,"
                                + "dicabonos,"
                                + "cierreabonos,"
                                + "cierrecargos,"
                                + "acta,"
                                + "cc"
                                + " FROM sc_salcont_cc";

                    OdbcCommand cmd = new OdbcCommand(consulta, odbcCon);
                    odbcCon.Open();
                    OdbcDataReader rdr = cmd.ExecuteReader();
                    List<ScSaldoConCc> listaSaldo = new List<ScSaldoConCc>();
                    while (rdr.Read())
                    {
                        ScSaldoConCc saldo = new ScSaldoConCc();
                        saldo.year = Convert.ToInt32(rdr["year"]);
                        saldo.cta = Convert.ToInt32(rdr["cta"]);
                        saldo.scta = Convert.ToInt32(rdr["scta"]);
                        saldo.sscta = Convert.ToInt32(rdr["sscta"]);
                        saldo.salini = Convert.ToInt32(rdr["salini"]);
                        saldo.enecargos = Convert.ToInt32(rdr["enecargos"]);
                        saldo.eneabonos = Convert.ToInt32(rdr["eneabonos"]);
                        saldo.febcargos = Convert.ToInt32(rdr["febcargos"]);
                        saldo.febabonos = Convert.ToInt32(rdr["febabonos"]);
                        saldo.marcargos = Convert.ToInt32(rdr["marcargos"]);
                        saldo.marabonos = Convert.ToInt32(rdr["marabonos"]);
                        saldo.abrcargos = Convert.ToInt32(rdr["abrcargos"]);
                        saldo.abrabonos = Convert.ToInt32(rdr["abrabonos"]);
                        saldo.maycargos = Convert.ToInt32(rdr["maycargos"]);
                        saldo.mayabonos = Convert.ToInt32(rdr["mayabonos"]);
                        saldo.juncargos = Convert.ToInt32(rdr["juncargos"]);
                        saldo.junabonos = Convert.ToInt32(rdr["junabonos"]);
                        saldo.julcargos = Convert.ToInt32(rdr["julcargos"]);
                        saldo.julabonos = Convert.ToInt32(rdr["julabonos"]);
                        saldo.agocargos = Convert.ToInt32(rdr["agocargos"]);
                        saldo.agoabonos = Convert.ToInt32(rdr["agoabonos"]);
                        saldo.sepcargos = Convert.ToInt32(rdr["sepcargos"]);
                        saldo.sepabonos = Convert.ToInt32(rdr["sepabonos"]);
                        saldo.octcargos = Convert.ToInt32(rdr["octcargos"]);
                        saldo.octabonos = Convert.ToInt32(rdr["octabonos"]);
                        saldo.novcargos = Convert.ToInt32(rdr["novcargos"]);
                        saldo.novabonos = Convert.ToInt32(rdr["novabonos"]);
                        saldo.diccargos = Convert.ToInt32(rdr["diccargos"]);
                        saldo.dicabonos = Convert.ToInt32(rdr["dicabonos"]);
                        saldo.cierreabonos = Convert.ToInt32(rdr["cierreabonos"]);
                        saldo.cierrecargos = Convert.ToInt32(rdr["cierrecargos"]);
                        saldo.acta = Convert.ToInt32(rdr["acta"]);
                        saldo.cc = Convert.ToString(rdr["cc"]);

                        listaSaldo.Add(saldo);

                    }
                    return listaSaldo;
                }
                catch (Exception ex)
                {
                   

                    throw ex;
                }
                finally
                {
                    odbcCon.Close();
                }
            }
            else
            {
                return null;
            }
        }

        public List<Balanza> convertirTabBalanza(Int64 idEmpresa)
        {
            List<Balanza> lstBalanza = new List<Balanza>();

          

            List<ScSaldoConCc> lstSaldoCC = new List<ScSaldoConCc>();
            lstSaldoCC = obtenerSalContCC(idEmpresa);

            foreach (ScSaldoConCc saldoCC in lstSaldoCC)
            {
                Balanza balanza = new Balanza();
                balanza.year = saldoCC.year;
                balanza.cta = saldoCC.cta.ToString();
                balanza.scta = saldoCC.scta.ToString();
                balanza.sscta = saldoCC.sscta.ToString();
                balanza.salini = saldoCC.salini;
                balanza.enecargos = saldoCC.enecargos;
                balanza.eneabonos = saldoCC.eneabonos;
                balanza.febabonos = saldoCC.febabonos;
                balanza.febcargos = saldoCC.febcargos;
                balanza.marabonos = saldoCC.marabonos;
                balanza.marcargos = saldoCC.marcargos;
                balanza.abrabonos = saldoCC.abrabonos;
                balanza.abrcargos = saldoCC.abrcargos;
                balanza.mayabonos = saldoCC.mayabonos;
                balanza.maycargos = saldoCC.maycargos;
                balanza.junabonos = saldoCC.junabonos;
                balanza.juncargos = saldoCC.juncargos;
                balanza.julabonos = saldoCC.julabonos;
                balanza.julcargos = saldoCC.julcargos;
                balanza.agoabonos = saldoCC.agoabonos;
                balanza.agocargos = saldoCC.agocargos;
                balanza.sepabonos = saldoCC.sepabonos;
                balanza.sepcargos = saldoCC.sepcargos;
                balanza.octabonos = saldoCC.octabonos;
                balanza.octcargos = saldoCC.octcargos;
                balanza.novabonos = saldoCC.novabonos;
                balanza.novcargos = saldoCC.novcargos;
                balanza.dicabonos = saldoCC.dicabonos;
                balanza.diccargos = saldoCC.diccargos;
                //balanza.TEXT_DESCRIPCION = "";
                //balanza.TEXT_DESCRIPCION2 = "";
                balanza.cierre_abonos = saldoCC.cierreabonos;
                balanza.cierre_cargos = saldoCC.cierrecargos;
                balanza.acta = saldoCC.acta;
                balanza.cc = saldoCC.cc;


                lstBalanza.Add(balanza);


            }


            return lstBalanza;
        }

        public int insertarTabBalanza(Int64 idEmpresa,string nombreCompania)
        {
            //conP.Open();
            DateTime fechaInicioProceso = DateTime.Now;
            var transaction = conP.BeginTransaction();
            List<Balanza> lstBala = new List<Balanza>();


            lstBala = convertirTabBalanza(idEmpresa);

            int cantFilaAfect = 0;
            try
            {

                string addBalanza = "insert into "
                 + "balanza("
                 + "id,"
                 + "cta,"
                 + "scta,"
                 + "sscta,"
                 + "year,"
                 + "salini,"
                 + "enecargos,"
                 + "eneabonos,"
                 + "febcargos,"
                 + "febabonos,"
                 + "marcargos,"
                 + "marabonos,"
                 + "abrcargos,"
                 + "abrabonos,"
                 + "maycargos,"
                 + "mayabonos,"
                 + "juncargos,"
                 + "junabonos,"
                 + "julcargos,"
                 + "julabonos,"
                 + "agocargos,"
                 + "agoabonos,"
                 + "sepcargos,"
                 + "sepabonos,"
                 + "octcargos,"
                 + "octabonos,"
                 + "novcargos,"
                 + "novabonos,"
                 + "diccargos,"
                 + "dicabonos,"
                 //+"cc,"
                 //+"descripcion,"
                 //+"descripcion2,"
                 + "incluir_suma,"
                 + "tipo_extraccion,"
                 + "fecha_carga,"
                 + "hora_carga,"
                 + "id_empresa,"
                 + "cierre_cargos,"
                 + "cierre_abonos,"
                 + "acta,"
                 + "cc" + ")"
                     + "values "
                         + "(NEXTVAL('seq_balanza'),"
                         + "@CTA,"
                         + "@SCTA,"
                         + "@SSCTA,"
                         + "@YEAR,"
                         + "@SALINI,"
                         + "@ENECARGOS,"
                         + "@ENEABONOS,"
                         + "@FEBCARGOS,"
                         + "@FEBABONOS,"
                         + "@MARCARGOS,"
                         + "@MARABONOS,"
                         + "@ABRCARGOS,"
                         + "@ABRABONOS,"
                         + "@MAYCARGOS,"
                         + "@MAYABONOS,"
                         + "@JUNCARGOS,"
                         + "@JUNABONOS,"
                         + "@JULCARGOS,"
                         + "@JULABONOS,"
                         + "@AGOCARGOS,"
                         + "@AGOABONOS,"
                         + "@SEPCARGOS,"
                         + "@SEPABONOS,"
                         + "@OCTCARGOS,"
                         + "@OCTABONOS,"
                         + "@NOVCARGOS,"
                         + "@NOVABONOS,"
                         + "@DICCARGOS,"
                         + "@DICABONOS,"
                         //+ "@CC,"
                         //+ "@DESCRIPCION,"
                         //+ "@DESCRIPCION2,"
                         + "@INCLUIR_SUMA,"
                         + "@TIPO_EXTRACCION,"
                         + "@FECHA_CARGA,"
                         + "@HORA_CARGA,"
                         + "@ID_EMPRESA,"
                         + "@CIERRE_CARGOS,"
                         + "@CIERRE_ABONOS,"
                         + "@ACTA,"
                         + "@CC)";

                {
                    foreach (Balanza balanza in lstBala)
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand(addBalanza, conP);
                        //cmd.Parameters.AddWithValue("@INT_IDBALANZA", NpgsqlTypes.NpgsqlDbType.Integer, balanza.INT_IDBALANZA);
                        cmd.Parameters.AddWithValue("@CTA", NpgsqlTypes.NpgsqlDbType.Text, balanza.cta);
                        cmd.Parameters.AddWithValue("@SCTA", NpgsqlTypes.NpgsqlDbType.Text, balanza.scta);
                        cmd.Parameters.AddWithValue("@SSCTA", NpgsqlTypes.NpgsqlDbType.Text, balanza.sscta);
                        cmd.Parameters.AddWithValue("@YEAR", NpgsqlTypes.NpgsqlDbType.Integer, balanza.year);
                        cmd.Parameters.AddWithValue("@SALINI", NpgsqlTypes.NpgsqlDbType.Double, balanza.salini);
                        cmd.Parameters.AddWithValue("@ENECARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.enecargos);
                        cmd.Parameters.AddWithValue("@ENEABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.eneabonos);
                        cmd.Parameters.AddWithValue("@FEBCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.febcargos);
                        cmd.Parameters.AddWithValue("@FEBABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.febabonos);
                        cmd.Parameters.AddWithValue("@MARCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.marcargos);
                        cmd.Parameters.AddWithValue("@MARABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.marabonos);
                        cmd.Parameters.AddWithValue("@ABRCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.abrcargos);
                        cmd.Parameters.AddWithValue("@ABRABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.abrabonos);
                        cmd.Parameters.AddWithValue("@MAYCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.maycargos);
                        cmd.Parameters.AddWithValue("@MAYABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.mayabonos);
                        cmd.Parameters.AddWithValue("@JUNCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.juncargos);
                        cmd.Parameters.AddWithValue("@JUNABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.junabonos);
                        cmd.Parameters.AddWithValue("@JULCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.julcargos);
                        cmd.Parameters.AddWithValue("@JULABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.julabonos);
                        cmd.Parameters.AddWithValue("@AGOCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.agocargos);
                        cmd.Parameters.AddWithValue("@AGOABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.agoabonos);
                        cmd.Parameters.AddWithValue("@SEPCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.sepcargos);
                        cmd.Parameters.AddWithValue("@SEPABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.sepabonos);
                        cmd.Parameters.AddWithValue("@OCTCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.octcargos);
                        cmd.Parameters.AddWithValue("@OCTABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.octabonos);
                        cmd.Parameters.AddWithValue("@NOVCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.novcargos);
                        cmd.Parameters.AddWithValue("@NOVABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.novabonos);
                        cmd.Parameters.AddWithValue("@DICCARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.diccargos);
                        cmd.Parameters.AddWithValue("@DICABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.dicabonos);
                        //cmd.Parameters.AddWithValue("@CC", NpgsqlTypes.NpgsqlDbType.Integer, balanza.CC);
                        //cmd.Parameters.AddWithValue("@DESCRIPCION", NpgsqlTypes.NpgsqlDbType.Text, balanza.DESCRIPCION);
                        //cmd.Parameters.AddWithValue("@DESCRIPCION2", NpgsqlTypes.NpgsqlDbType.Text, balanza.DESCRIPCION2);
                        cmd.Parameters.AddWithValue("@INCLUIR_SUMA", NpgsqlTypes.NpgsqlDbType.Integer, balanza.incluir_suma);
                        cmd.Parameters.AddWithValue("@TIPO_EXTRACCION", NpgsqlTypes.NpgsqlDbType.Integer, Constantes.EXTRACCION_PROGRAMADA);
                        ////cmd.Parameters.AddWithValue("@FECHA_CARGA", NpgsqlTypes.NpgsqlDbType.Text, DateTime.Now.ToString("dd/MM/yyyy"));
                        cmd.Parameters.AddWithValue("@FECHA_CARGA", NpgsqlTypes.NpgsqlDbType.Date, DateTime.Now);
                        //cmd.Parameters.AddWithValue("@HORA_CARGA", NpgsqlTypes.NpgsqlDbType.Text, DateTime.Now.ToString("h:mm tt"));
                        cmd.Parameters.AddWithValue("@HORA_CARGA", NpgsqlTypes.NpgsqlDbType.Text, DateTime.Now.ToString("h:mm tt"));
                        cmd.Parameters.AddWithValue("@ID_EMPRESA", NpgsqlTypes.NpgsqlDbType.Bigint, idEmpresa);
                        cmd.Parameters.AddWithValue("@CIERRE_CARGOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.cierre_cargos);
                        cmd.Parameters.AddWithValue("@CIERRE_ABONOS", NpgsqlTypes.NpgsqlDbType.Double, balanza.cierre_abonos);
                        cmd.Parameters.AddWithValue("@ACTA", NpgsqlTypes.NpgsqlDbType.Integer, balanza.acta);
                        cmd.Parameters.AddWithValue("@CC", NpgsqlTypes.NpgsqlDbType.Text, balanza.cc);

                        //con.Open();
                        // int cantFilaAfect = Convert.ToInt32(cmd.ExecuteNonQuery());
                        cantFilaAfect = cantFilaAfect + Convert.ToInt32(cmd.ExecuteNonQuery());
                    }
                    transaction.Commit();

                    conP.Close();
                    DateTime fechaFinalProceso = DateTime.Now;
                    configCorreo.EnviarCorreo("La extracción de Balanza se genero correctamente"
                                               + "\nFecha Inicio : " + fechaInicioProceso + " \n Fecha Final: " + fechaFinalProceso
                                               + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                               , "ETL Balanza");
                    return cantFilaAfect;
                }
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                conP.Close();
                DateTime fechaFinalProceso = DateTime.Now;
                configCorreo.EnviarCorreo("Ha ocurrido un error en la extracción de Balanza"
                                           + "\nFecha Inicio : " + fechaInicioProceso + "\nFecha Final: " + fechaFinalProceso
                                           + "\nTiempo de ejecucion : " + (fechaFinalProceso - fechaInicioProceso).TotalMinutes + " mins"
                                           + "\nError : " + ex.Message
                                           , "ETL Balanza");
                string error = ex.Message;
                throw;
            }
            finally
            {
                conP.Close();
            }

            //return cantFilaAfect;
        }
        #endregion  Sybase Extraccion

        public string generarSalContCC_CSV(Int64 idEmpresa, string ruta)
        {
            string nombreArchivo = string.Empty;
            string registros = string.Empty;
            string cabecera = string.Empty;
            nombreArchivo = Constantes.NOMBRE_ARCHIVO_BALANZA + "_" + idEmpresa + DateTime.Now.ToString("ddMMyyyy") + DateTime.Now.ToString("HHmmSS") + ".csv";
            StreamWriter layout;
            //layout = File.AppendText(@"C:\Users\Omnisys\Desktop\txtWinConnector\" + "cvsBalanza"+idEmpresa+DateTime.Now + ".csv");
            layout = File.AppendText(ruta + nombreArchivo);

            try
            {
                /// creacion de odbc 
                DSN dsn = new DSN();
                dsn = dsnConfig.crearDSN(idEmpresa); //regresar
                if (dsn.creado) //regresar// if(true)
                {
                    /// obtener conexion de Odbc creado
                    /// 
                    //dsn.nombreDSN = "2_GRUPO_ INGENIERIA"; /// quitar 
                    //dsn.nombreDSN =  "4_CONSTRUCTORA_Y_EDIFICADORA";///quitar
                    odbcCon = conex.ConexionSybaseodbc(dsn.nombreDSN);

                    try
                    {
                        odbcCon.Open();
                        layout.WriteLine(Constantes.HEADER_BALANZA_CSV);

                        string consulta = " SELECT "
                                    + "year,"
                                    + "cta,"
                                    + "scta,"
                                    + "sscta,"
                                    + "salini,"
                                    + "enecargos,"
                                    + "eneabonos,"
                                    + "febcargos,"
                                    + "febabonos,"
                                    + "marcargos,"
                                    + "marabonos,"
                                    + "abrcargos,"
                                    + "abrabonos,"
                                    + "maycargos,"
                                    + "mayabonos,"
                                    + "juncargos,"
                                    + "junabonos,"
                                    + "julcargos,"
                                    + "julabonos,"
                                    + "agocargos,"
                                    + "agoabonos,"
                                    + "sepcargos,"
                                    + "sepabonos,"
                                    + "octcargos,"
                                    + "octabonos,"
                                    + "novcargos,"
                                    + "novabonos,"
                                    + "diccargos,"
                                    + "dicabonos,"
                                    + "cierreabonos,"
                                    + "cierrecargos,"
                                    + "acta,"
                                    + "cc"
                                    + " FROM sc_salcont_cc";

                        OdbcCommand cmd = new OdbcCommand(consulta, odbcCon);

                        OdbcDataReader rdr = cmd.ExecuteReader();

                        while (rdr.Read())
                        {

                            registros =
                             Convert.ToString(rdr["cta"].ToString()) + ","
                            + Convert.ToString(rdr["scta"].ToString()) + ","
                            + Convert.ToString(rdr["sscta"].ToString()) + ","
                            + Convert.ToInt32(rdr["year"]) + ","
                            + Convert.ToDouble(rdr["salini"]) + ","
                            + Convert.ToDouble(rdr["enecargos"]) + ","
                            + Convert.ToDouble(rdr["eneabonos"]) + ","
                            + Convert.ToDouble(rdr["febcargos"]) + ","
                            + Convert.ToDouble(rdr["febabonos"]) + ","
                            + Convert.ToDouble(rdr["marcargos"]) + ","
                            + Convert.ToDouble(rdr["marabonos"]) + ","
                            + Convert.ToDouble(rdr["abrcargos"]) + ","
                            + Convert.ToDouble(rdr["abrabonos"]) + ","
                            + Convert.ToDouble(rdr["maycargos"]) + ","
                            + Convert.ToDouble(rdr["mayabonos"]) + ","
                            + Convert.ToDouble(rdr["juncargos"]) + ","
                            + Convert.ToDouble(rdr["junabonos"]) + ","
                            + Convert.ToDouble(rdr["julcargos"]) + ","
                            + Convert.ToDouble(rdr["julabonos"]) + ","
                            + Convert.ToDouble(rdr["agocargos"]) + ","
                            + Convert.ToDouble(rdr["agoabonos"]) + ","
                            + Convert.ToDouble(rdr["sepcargos"]) + ","
                            + Convert.ToDouble(rdr["sepabonos"]) + ","
                            + Convert.ToDouble(rdr["octcargos"]) + ","
                            + Convert.ToDouble(rdr["octabonos"]) + ","
                            + Convert.ToDouble(rdr["novcargos"]) + ","
                            + Convert.ToDouble(rdr["novabonos"]) + ","
                            + Convert.ToDouble(rdr["diccargos"]) + ","
                            + Convert.ToDouble(rdr["dicabonos"]) + ","
                            + " 0,"
                            + Constantes.EXTRACCION_MANUAL + ","
                            + idEmpresa + ","
                            + Convert.ToDouble(rdr["cierrecargos"]) + ","
                            + Convert.ToDouble(rdr["cierreabonos"]) + ","
                            + Convert.ToInt32(rdr["acta"]) + ","
                            + Convert.ToString(rdr["cc"]) + ","
                            + "'" + DateTime.Now.ToString("HH:mm:ss") + "'"
                            + "'" + DateTime.Now.ToString("dd/MM/yyy") + "',";

                            layout.WriteLine(registros, Environment.NewLine);

                        }

                        layout.Close();
                        odbcCon.Close();
                        return nombreArchivo;

                    }
                    catch (Exception ex)
                    {
                        string error = ex.Message;
                        layout.Close();
                        odbcCon.Close();
                        throw;
                    }
                    finally
                    {
                        layout.Close();
                        odbcCon.Close();

                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {

                throw;

            }
            finally
            {
                layout.Close();
                odbcCon.Close();
            }
        }

        public int copy_balanza(string nombre_archivo, string ruta_archivo)
        {
            int resultado;
            string script_copy = string.Empty;

            // pruebas 
            //script_copy = " copy tmp_balanza (" + Constantes.headerBalanzaCSV + ") from '" + ruta_archivo + nombre_archivo + "'" + " delimiter ',' csv header ";
            script_copy = " copy balanza (" + Constantes.HEADER_BALANZA_CSV + ") from '" + ruta_archivo + nombre_archivo + "'" + " delimiter ',' csv header ";

            try
            {
                conP.Open();
                NpgsqlCommand cmd = new NpgsqlCommand(script_copy, conP);
                resultado = Convert.ToInt32(cmd.ExecuteNonQuery());
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                conP.Close();
                throw;
            }
            finally
            {
                conP.Close();
            }

            return resultado;
        }

        public int UpdateCuentaUnificada(Int64 idEmpresa)
        {
            string update = "   update balanza "
                            + " set "
                            + " cuenta_unificada=LPAD(cta,4,'0')||LPAD(scta,4,'0')||LPAD(sscta,4,'0') "
                            + " where id_empresa = " + idEmpresa
                            + "  and cuenta_unificada is null";

            try
            {
                {
                    conP.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(update, conP);

                    int cantFilAfec = cmd.ExecuteNonQuery();
                    conP.Close();
                    return cantFilAfec;
                }
            }
            catch (Exception ex)
            {
                conP.Close();
                throw;
            }
            finally
            {
                conP.Close();
            }
        }






    }
}
