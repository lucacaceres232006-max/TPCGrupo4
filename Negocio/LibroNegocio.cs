using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dominio;

namespace Negocio
{
    public class LibroNegocio
    {
        public List<Libro> listar()
        {
            List<Libro> lista = new List<Libro>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT L.Id, L.Titulo, L.ISBN, L.AnioEdicion, E.Id AS IdEditorial, E.Nombre AS Editorial FROM LIBROS L INNER JOIN EDITORIALES E ON L.IdEditorial = E.Id");
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Libro aux = new Libro();
                    aux.Id = (int)datos.Lector["Id"];
                    aux.Titulo = (string)datos.Lector["Titulo"];
                    aux.ISBN = (string)datos.Lector["ISBN"];
                    aux.AnioEdicion = (int)datos.Lector["AnioEdicion"];

                    aux.Editorial = new Editorial();
                    aux.Editorial.Id = (int)datos.Lector["IdEditorial"];
                    aux.Editorial.Nombre = (string)datos.Lector["Editorial"];

                    cargarAutores(aux);
                    cargarCategorias(aux);
                    cargarEjemplares(aux);

                    lista.Add(aux);
                }

                return lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private void cargarAutores(Libro libro)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT A.Id, A.Nombre, A.Apellido, A.Biografia FROM AUTORES A INNER JOIN LIBROS_AUTORES LA ON A.Id = LA.IdAutor WHERE LA.IdLibro = @idLibro");
                datos.setearParametro("@idLibro", libro.Id);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Autor autor = new Autor();
                    autor.Id = (int)datos.Lector["Id"];
                    autor.Nombre = (string)datos.Lector["Nombre"];
                    autor.Apellido = (string)datos.Lector["Apellido"];

                    if (!(datos.Lector["Biografia"] is DBNull))
                        autor.Biografia = (string)datos.Lector["Biografia"];

                    libro.Autores.Add(autor);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private void cargarCategorias(Libro libro)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT C.Id, C.Descripcion FROM CATEGORIAS C INNER JOIN LIBROS_CATEGORIAS LC ON C.Id = LC.IdCategoria WHERE LC.IdLibro = @idLibro");
                datos.setearParametro("@idLibro", libro.Id);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Categoria categoria = new Categoria();
                    categoria.Id = (int)datos.Lector["Id"];
                    categoria.Descripcion = (string)datos.Lector["Descripcion"];

                    libro.Categorias.Add(categoria);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        private void cargarEjemplares(Libro libro)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.setearConsulta("SELECT E.Id, E.IdLibro, E.CodigoInventario, E.Estado, E.Observaciones FROM EJEMPLARES E INNER JOIN LIBROS L ON E.IdLibro = L.Id WHERE E.IdLibro = @idLibro");
                datos.setearParametro("@idLibro", libro.Id);
                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Ejemplar ejemplar = new Ejemplar();
                    ejemplar.Id = (int)datos.Lector["Id"];
                    ejemplar.IdLibro = (int)datos.Lector["IdLibro"];
                    ejemplar.CodigoInventario = (string)datos.Lector["CodigoInventario"];
                    ejemplar.Estado = (string)datos.Lector["Estado"];

                    if (!(datos.Lector["Observaciones"] is DBNull))
                        ejemplar.Observaciones = (string)datos.Lector["Observaciones"];

                    libro.Ejemplares.Add(ejemplar);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}