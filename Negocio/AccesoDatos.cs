
using System;
using System.Data;
using System.Data.SqlClient;

namespace Negocio
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;
        private SqlTransaction transaccion;

        public SqlDataReader Lector
        {
            get { return lector; }
        }

        public AccesoDatos()
        {
            conexion = new SqlConnection("server=.\\SQLEXPRESS; database=BIBLIOTECA_DB; integrated security=true;");
            comando = new SqlCommand();
        }

        public void setearConsulta(string consulta)
        {
            comando.Parameters.Clear();
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
        }

        public void setearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        // Inicia una transacción
        public void iniciarTransaccion()
        {
            if (conexion.State == ConnectionState.Closed)
                conexion.Open();

            transaccion = conexion.BeginTransaction();
            comando.Connection = conexion;
            comando.Transaction = transaccion;
        }

        // Confirma una transacción
        public void confirmarTransaccion()
        {
            if (transaccion != null)
            {
                transaccion.Commit();
                transaccion.Dispose();
                transaccion = null;
                comando.Transaction = null;
            }
        }

        // Cancela una transacción
        public void cancelarTransaccion()
        {
            if (transaccion != null)
            {
                try
                {
                    transaccion.Rollback();
                }
                finally
                {
                    transaccion.Dispose();
                    transaccion = null;
                    comando.Transaction = null;
                }
            }
        }

        public void ejecutarLectura()
        {
            comando.Connection = conexion;
            comando.Transaction = transaccion;

            try
            {
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();

                lector = comando.ExecuteReader();
            }
            catch
            {
                throw;
            }
        }

        public void ejecutarAccion()
        {
            comando.Connection = conexion;
            comando.Transaction = transaccion;

            try
            {
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();

                comando.ExecuteNonQuery();
            }
            catch
            {
                throw;
            }
        }

        public int ejecutarAccionScalar()
        {
            comando.Connection = conexion;
            comando.Transaction = transaccion;

            try
            {
                if (conexion.State == ConnectionState.Closed)
                    conexion.Open();

                return Convert.ToInt32(comando.ExecuteScalar());
            }
            catch
            {
                throw;
            }
        }

        public void cerrarConexion()
        {
            if (lector != null)
            {
                if (!lector.IsClosed)
                    lector.Close();

                lector = null;
            }

            // Si quedó una transacción pendiente, se cancela.
            if (transaccion != null)
                cancelarTransaccion();

            if (conexion != null && conexion.State == ConnectionState.Open)
                conexion.Close();
        }
    }
}
