
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using Dominio;

namespace Negocio
{
    public class UsuarioNegocio
    {
        public List<Usuario> listar()
        {
            List<Usuario> lista = new List<Usuario>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, NombreUsuario, Clave, Nombre, Apellido, Email, Perfil, Activo FROM USUARIOS");

                datos.ejecutarLectura();

                while (datos.Lector.Read())
                {
                    Usuario usuario = new Usuario();

                    usuario.Id = (int)datos.Lector["Id"];
                    usuario.NombreUsuario = (string)datos.Lector["NombreUsuario"];
                    usuario.Clave = (string)datos.Lector["Clave"];
                    usuario.Nombre = (string)datos.Lector["Nombre"];
                    usuario.Apellido = (string)datos.Lector["Apellido"];
                    usuario.Email = (string)datos.Lector["Email"];
                    usuario.Perfil = (string)datos.Lector["Perfil"];
                    usuario.Activo = (bool)datos.Lector["Activo"];

                    lista.Add(usuario);
                }

                return lista;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Usuario obtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, NombreUsuario, Clave, Nombre, Apellido, Email, Perfil, Activo FROM USUARIOS WHERE Id = @Id");
                datos.setearParametro("@Id", id);

                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Usuario usuario = new Usuario();

                    usuario.Id = (int)datos.Lector["Id"];
                    usuario.NombreUsuario = (string)datos.Lector["NombreUsuario"];
                    usuario.Clave = (string)datos.Lector["Clave"];
                    usuario.Nombre = (string)datos.Lector["Nombre"];
                    usuario.Apellido = (string)datos.Lector["Apellido"];
                    usuario.Email = (string)datos.Lector["Email"];
                    usuario.Perfil = (string)datos.Lector["Perfil"];
                    usuario.Activo = (bool)datos.Lector["Activo"];

                    return usuario;
                }

                return null;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void agregar(Usuario usuario)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("INSERT INTO USUARIOS (NombreUsuario, Clave, Nombre, Apellido, Email, Perfil, Activo) VALUES (@NombreUsuario, @Clave, @Nombre, @Apellido, @Email, @Perfil, 1)");

                datos.setearParametro("@NombreUsuario", usuario.NombreUsuario);
                datos.setearParametro("@Clave", usuario.Clave);
                datos.setearParametro("@Nombre", usuario.Nombre);
                datos.setearParametro("@Apellido", usuario.Apellido);
                datos.setearParametro("@Email", usuario.Email);
                datos.setearParametro("@Perfil", usuario.Perfil);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void modificar(Usuario usuario)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE USUARIOS SET NombreUsuario = @NombreUsuario, Clave = @Clave, Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Perfil = @Perfil WHERE Id = @Id");

                datos.setearParametro("@Id", usuario.Id);
                datos.setearParametro("@NombreUsuario", usuario.NombreUsuario);
                datos.setearParametro("@Clave", usuario.Clave);
                datos.setearParametro("@Nombre", usuario.Nombre);
                datos.setearParametro("@Apellido", usuario.Apellido);
                datos.setearParametro("@Email", usuario.Email);
                datos.setearParametro("@Perfil", usuario.Perfil);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public void cambiarEstado(int id, bool activo)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("UPDATE USUARIOS SET Activo = @Activo WHERE Id = @Id");
                datos.setearParametro("@Id", id);
                datos.setearParametro("@Activo", activo);

                datos.ejecutarAccion();
            }
            finally
            {
                datos.cerrarConexion();
            }
        }

        public Usuario validarUsuario(string nombreUsuario, string clave)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.setearConsulta("SELECT Id, NombreUsuario, Nombre, Apellido, Email, Perfil, Activo FROM USUARIOS WHERE NombreUsuario = @NombreUsuario AND Clave = @Clave AND Activo = 1");

                datos.setearParametro("@NombreUsuario", nombreUsuario);
                datos.setearParametro("@Clave", clave);

                datos.ejecutarLectura();

                if (datos.Lector.Read())
                {
                    Usuario usuario = new Usuario();

                    usuario.Id = (int)datos.Lector["Id"];
                    usuario.NombreUsuario = (string)datos.Lector["NombreUsuario"];
                    usuario.Nombre = (string)datos.Lector["Nombre"];
                    usuario.Apellido = (string)datos.Lector["Apellido"];
                    usuario.Email = (string)datos.Lector["Email"];
                    usuario.Perfil = (string)datos.Lector["Perfil"];
                    usuario.Activo = (bool)datos.Lector["Activo"];

                    return usuario;
                }

                return null;
            }
            finally
            {
                datos.cerrarConexion();
            }
        }
    }
}
