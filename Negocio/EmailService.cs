using System;
using System.Net.Mail;
using Dominio;

namespace Negocio
{
    public class EmailService
    {
        public void enviarComprobantePrestamo(Prestamo prestamo)
        {
            if (prestamo == null || prestamo.Socio == null || string.IsNullOrWhiteSpace(prestamo.Socio.Email))
                return;

            MailMessage mail = new MailMessage();
            mail.To.Add(prestamo.Socio.Email);
            mail.Subject = "Comprobante de préstamo " + prestamo.NumeroComprobante + " - Biblioteca";
            mail.IsBodyHtml = true;

            string tituloLibro = prestamo.Ejemplar != null && !string.IsNullOrWhiteSpace(prestamo.Ejemplar.TituloLibro)
                ? prestamo.Ejemplar.TituloLibro
                : "Ejemplar de biblioteca";

            string codigoInventario = prestamo.Ejemplar != null
                ? prestamo.Ejemplar.CodigoInventario
                : "";

            mail.Body = $@"
                <h2>Confirmación de Préstamo</h2>
                <p>Hola <strong>{prestamo.Socio.Nombre} {prestamo.Socio.Apellido}</strong>,</p>
                <p>Se ha registrado el retiro del siguiente material bibliográfico a tu nombre:</p>
                <ul>
                    <li><strong>Comprobante:</strong> {prestamo.NumeroComprobante}</li>
                    <li><strong>Libro:</strong> {tituloLibro}</li>
                    <li><strong>Código de inventario:</strong> {codigoInventario}</li>
                    <li><strong>Fecha de inicio:</strong> {prestamo.FechaInicio:dd/MM/yyyy}</li>
                    <li><strong>Fecha prevista de devolución:</strong> {prestamo.FechaPrevistaDevolucion:dd/MM/yyyy}</li>
                </ul>
                <p>Recordá devolver o renovar el ejemplar antes de la fecha prevista para evitar suspensiones.</p>
                <p>Saludos,<br/>Equipo de Biblioteca</p>";

            SmtpClient cliente = new SmtpClient();
            cliente.Send(mail);
        }
    }
}