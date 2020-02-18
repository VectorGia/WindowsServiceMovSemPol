using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Odbc;
using System.Linq;
using System.Text;

namespace WindowsServiceSemanal.Conexion
{
    public class Conexion
    {

  
        NpgsqlConnection con;
        OdbcConnection odbcCon;



        public NpgsqlConnection ConnexionDB()

        {
            con = new NpgsqlConnection("User ID=postgres;Password=omnisys;Host=127.0.0.1;Port=5432;Database=GIA31;Pooling=true;");
            return con;
        }

      
        public OdbcConnection ConexionSybaseodbc(string DsnName)

        {
            try
            {

                odbcCon = new OdbcConnection("DSN=" + DsnName);
                // odbcCon = new OdbcConnection("DSN=GIAODBCPRUEBAS"); ////ODBCGIA

                return odbcCon;
            }
            catch (InvalidOperationException ex)
            {
                string error = ex.Message;
                return null;
            }
        }

    }
}
