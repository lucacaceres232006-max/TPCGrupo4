using System;
using System.Collections.Generic;
using Dominio;

namespace Negocio
{
    public class AutorNegocio
    {
        public List<Autor> listar()
        {
            List<Autor> lista = new List<Autor>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Nombre, Apellido, Biografia, Activo FROM AUTORES ORDER BY Apellido, Nombre");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Autor aux = new Autor();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.Activo = (bool)datos.Lector["Activo"];

                    if (!(datos.Lector["Biografia"] is DBNull))
                        aux.Biografia = (string)datos.Lector["Biografia"];

                    lista.Add(aux);
                }

                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public List<Autor> listarActivos()
        {
            List<Autor> lista = new List<Autor>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Nombre, Apellido, Biografia, Activo FROM AUTORES WHERE Activo = 1 ORDER BY Apellido, Nombre");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Autor aux = new Autor();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.Activo = (bool)datos.Lector["Activo"];

                    if (!(datos.Lector["Biografia"] is DBNull))
                        aux.Biografia = (string)datos.Lector["Biografia"];

                    lista.Add(aux);
                }

                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Autor obtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, Nombre, Apellido, Biografia, Activo FROM AUTORES WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Autor aux = new Autor();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Nombre = (string)datos.Lector["Nombre"];
                    aux.Apellido = (string)datos.Lector["Apellido"];
                    aux.Activo = (bool)datos.Lector["Activo"];

                    if (!(datos.Lector["Biografia"] is DBNull))
                        aux.Biografia = (string)datos.Lector["Biografia"];

                    return aux;
                }

                return null;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Autor nuevo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO AUTORES (Nombre, Apellido, Biografia) VALUES (@nombre, @apellido, @biografia)");
                datos.setearParametro("@nombre", nuevo.Nombre);
                datos.setearParametro("@apellido", nuevo.Apellido);
                datos.setearParametro("@biografia", nuevo.Biografia);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Autor autor)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE AUTORES SET Nombre = @nombre, Apellido = @apellido, Biografia = @biografia WHERE Id = @id");
                datos.setearParametro("@nombre", autor.Nombre);
                datos.setearParametro("@apellido", autor.Apellido);
                datos.setearParametro("@biografia", autor.Biografia);
                datos.setearParametro("@id", autor.Id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void eliminar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta(@"
                    IF EXISTS (
                        SELECT 1
                        FROM PRESTAMOS P
                        INNER JOIN EJEMPLARES E ON P.IdEjemplar = E.Id
                        INNER JOIN LIBROS_AUTORES LA ON E.IdLibro = LA.IdLibro
                        WHERE LA.IdAutor = @id
                          AND P.Estado = 'Activo'
                    )
                        THROW 50001, 'No se puede desactivar el autor porque tiene libros con préstamos activos.', 1;

                    UPDATE AUTORES SET Activo = 0 WHERE Id = @id;");

                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void activar(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE AUTORES SET Activo = 1 WHERE Id = @id");
                datos.setearParametro("@id", id);
                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}