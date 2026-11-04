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
        public void agregar(Libro libro)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = @"
            SET NOCOUNT ON;

            BEGIN TRY
                BEGIN TRANSACTION;

                INSERT INTO LIBROS (Titulo, ISBN, IdEditorial, AnioEdicion)
                VALUES (@titulo, @isbn, @idEditorial, @anioEdicion);

                DECLARE @idLibro INT;
                SET @idLibro = CAST(SCOPE_IDENTITY() AS INT);
        ";

                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    consulta += $@"
                INSERT INTO LIBROS_AUTORES (IdLibro, IdAutor)
                VALUES (@idLibro, @idAutor{i});
            ";
                }

                for (int i = 0; i < libro.Categorias.Count; i++)
                {
                    consulta += $@"
                INSERT INTO LIBROS_CATEGORIAS (IdLibro, IdCategoria)
                VALUES (@idLibro, @idCategoria{i});
            ";
                }

                for (int i = 0; i < libro.Ejemplares.Count; i++)
                {
                    consulta += $@"
                INSERT INTO EJEMPLARES
                (IdLibro, CodigoInventario, Estado, Observaciones)
                VALUES (@idLibro, @codigo{i}, @estado{i}, @observaciones{i});
            ";
                }

                consulta += @"
                COMMIT TRANSACTION;

                SELECT @idLibro;

            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;

                THROW;
            END CATCH;
        ";

                datos.setearConsulta(consulta);

                datos.setearParametro("@titulo", libro.Titulo);
                datos.setearParametro("@isbn", libro.ISBN);
                datos.setearParametro("@idEditorial", libro.Editorial.Id);
                datos.setearParametro("@anioEdicion", libro.AnioEdicion);

                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    datos.setearParametro("@idAutor" + i, libro.Autores[i].Id);
                }

                for (int i = 0; i < libro.Categorias.Count; i++)
                {
                    datos.setearParametro("@idCategoria" + i, libro.Categorias[i].Id);
                }

                for (int i = 0; i < libro.Ejemplares.Count; i++)
                {
                    Ejemplar ejemplar = libro.Ejemplares[i];

                    datos.setearParametro("@codigo" + i, ejemplar.CodigoInventario);
                    datos.setearParametro("@estado" + i,
                        string.IsNullOrWhiteSpace(ejemplar.Estado) ? "Disponible" : ejemplar.Estado);
                    datos.setearParametro("@observaciones" + i, ejemplar.Observaciones);
                }

                libro.Id = datos.ejecutarAccionScalar();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
        public void modificar(Libro libro)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = @"
            SET NOCOUNT ON;

            BEGIN TRY
                BEGIN TRANSACTION;

                UPDATE LIBROS
                SET Titulo = @titulo,
                    ISBN = @isbn,
                    IdEditorial = @idEditorial,
                    AnioEdicion = @anioEdicion
                WHERE Id = @idLibro;

                IF @@ROWCOUNT = 0
                    THROW 50001, 'El libro no existe.', 1;

                DELETE FROM LIBROS_AUTORES
                WHERE IdLibro = @idLibro;

                DELETE FROM LIBROS_CATEGORIAS
                WHERE IdLibro = @idLibro;
        ";

                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    consulta += $@"
                INSERT INTO LIBROS_AUTORES (IdLibro, IdAutor)
                VALUES (@idLibro, @idAutor{i});
            ";
                }

                for (int i = 0; i < libro.Categorias.Count; i++)
                {
                    consulta += $@"
                INSERT INTO LIBROS_CATEGORIAS (IdLibro, IdCategoria)
                VALUES (@idLibro, @idCategoria{i});
            ";
                }

                consulta += @"
                COMMIT TRANSACTION;

                SELECT @idLibro;

            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;

                THROW;
            END CATCH;
        ";

                datos.setearConsulta(consulta);

                datos.setearParametro("@idLibro", libro.Id);
                datos.setearParametro("@titulo", libro.Titulo);
                datos.setearParametro("@isbn", libro.ISBN);
                datos.setearParametro("@idEditorial", libro.Editorial.Id);
                datos.setearParametro("@anioEdicion", libro.AnioEdicion);

                for (int i = 0; i < libro.Autores.Count; i++)
                {
                    datos.setearParametro(
                        "@idAutor" + i,
                        libro.Autores[i].Id
                    );
                }

                for (int i = 0; i < libro.Categorias.Count; i++)
                {
                    datos.setearParametro(
                        "@idCategoria" + i,
                        libro.Categorias[i].Id
                    );
                }

                datos.ejecutarAccionScalar();
            }
            catch (Exception)
            {
                throw;
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
            BEGIN TRY
                BEGIN TRANSACTION;

                IF EXISTS (
                    SELECT 1
                    FROM PRESTAMOS p
                    INNER JOIN EJEMPLARES e ON e.Id = p.IdEjemplar
                    WHERE e.IdLibro = @id
                )
                BEGIN
                    THROW 50001, 'No se puede eliminar el libro porque tiene ejemplares asociados a prestamos.', 1;
                END;

                DELETE FROM LIBROS_AUTORES
                WHERE IdLibro = @id;

                DELETE FROM LIBROS_CATEGORIAS
                WHERE IdLibro = @id;

                DELETE FROM EJEMPLARES
                WHERE IdLibro = @id;

                DELETE FROM LIBROS
                WHERE Id = @id;

                COMMIT TRANSACTION;
            END TRY
            BEGIN CATCH
                IF @@TRANCOUNT > 0
                    ROLLBACK TRANSACTION;

                THROW;
            END CATCH;
        ");

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