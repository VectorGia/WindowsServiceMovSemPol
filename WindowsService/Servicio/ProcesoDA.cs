using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsService1.Models;

namespace WindowsService1.Servicio
{
   public class ProcesoDA
    {
        NpgsqlConnection conP;
        Conexion.Conexion conex = new Conexion.Conexion();

        public ProcesoDA()
        {
            conP = conex.ConnexionDB();
        }

        public IEnumerable<Proceso> GetAllProcesos()
        {

            string consulta = "SELECT id,empresa,estatus,fecha_fin,fecha_inicio,mensaje,tipo,id_empresa FROM proceso ";
            try
            {
                List<Proceso> lstProcesos = new List<Proceso>();
                {

                    conP.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(consulta, conP);


                    NpgsqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {
                        Proceso proceso = new Proceso();

                        proceso.id = Convert.ToInt64(rdr["id"]);
                        proceso.empresa = Convert.ToString(rdr["empresa"]);
                        proceso.estatus = Convert.ToString(rdr["estatus"]);
                        proceso.fecha_fin = Convert.ToDateTime(rdr["fecha_fin"]);
                        proceso.fecha_inicio = Convert.ToDateTime(rdr["fecha_inicio"]);
                        proceso.mensaje = Convert.ToString(rdr["mensaje"]);
                        proceso.tipo = Convert.ToString(rdr["tipo"]);
                        proceso.id_empresa = Convert.ToInt64(rdr["id_empresa"]);
                        lstProcesos.Add(proceso);
                    }
                    conP.Close();
                }
                return lstProcesos;
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
        //Obtiene las cuentas de cada compañia 
        public Proceso GetProcesoData(long id)
        {
            string consulta = "SELECT id,empresa,estatus,fecha_fin,fecha_inicio,mensaje,tipo,id_empresa FROM proceso where id= " + id;
            try
            {
                Proceso proceso = new Proceso();
                {
                    conP.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(consulta, conP);
                    NpgsqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                    {

                        proceso.id = Convert.ToInt64(rdr["id"]);
                        proceso.empresa = Convert.ToString(rdr["empresa"]);
                        proceso.estatus = Convert.ToString(rdr["estatus"]);
                        proceso.fecha_fin = Convert.ToDateTime(rdr["fecha_fin"]);
                        proceso.fecha_inicio = Convert.ToDateTime(rdr["fecha_inicio"]);
                        proceso.mensaje = Convert.ToString(rdr["mensaje"]);
                        proceso.tipo = Convert.ToString(rdr["tipo"]);
                        proceso.id_empresa = Convert.ToInt64(rdr["id_empresa"]);

                    }

                    conP.Close();
                }
                return proceso;
            }
            catch
            {
                conP.Close();
                throw;

            }
            finally
            {
                conP.Close();
            }
        }
        public int AddProceso(Proceso proceso)
        {
            string add = "INSERT INTO proceso ("
                + " id,"
                //+ " empresa,"
                + " estatus,"
                + " fecha_fin,"
                + " fecha_inicio,"
                + " mensaje,"
                + " tipo,"
                + " id_empresa "
                + " ) "
                + " VALUES ("
                + " @nextval('seq_proceso'),"
                //+ " @empresa,"
                + " @estatus,"
                + " @fecha_fin,"
                + " @fecha_inicio,"
                + " @mensaje,"
                + " @tipo, "
                + " @id_empresa )";

            try
            {
                {
                    conP.Open();
                    NpgsqlCommand cmd = new NpgsqlCommand(add, conP);
                    // cmd.Parameters.AddWithValue("@empresa", NpgsqlTypes.NpgsqlDbType.Text, proceso.empresa);
                    cmd.Parameters.AddWithValue("@estatus", NpgsqlTypes.NpgsqlDbType.Text, proceso.estatus);
                    cmd.Parameters.AddWithValue("@fecha_fin", NpgsqlTypes.NpgsqlDbType.Date, proceso.fecha_fin);
                    cmd.Parameters.AddWithValue("@fecha_inicio", NpgsqlTypes.NpgsqlDbType.Date, proceso.fecha_inicio);
                    cmd.Parameters.AddWithValue("@mensaje", NpgsqlTypes.NpgsqlDbType.Text, proceso.mensaje);
                    cmd.Parameters.AddWithValue("@tipo", NpgsqlTypes.NpgsqlDbType.Text, proceso.tipo);
                    cmd.Parameters.AddWithValue("@id_empresa", NpgsqlTypes.NpgsqlDbType.Bigint, proceso.id_empresa);

                    int cantFilAfec = cmd.ExecuteNonQuery();
                    conP.Close();
                    return cantFilAfec;
                }
            }
            catch
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
