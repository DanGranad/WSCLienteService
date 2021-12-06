using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.Report
{
    public class Ticket
    {
        public string Codigo { get; set; }
        public string CodigoJIRA { get; set; }
        public string Nombre { get; set; }
        public string Contacto { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        public string TipoDocumentoCli { get; set; }
        public string DocumentoCli { get; set; }
        public string Dias { get; set; }
        public string Canal { get; set; }
        public string FecRecepcion { get; set; }
        public string FecRegistro { get; set; }
        public string ViaRecepcion { get; set; }
        public string ViaRespuesta { get; set; }
        public string Ramo { get; set; }
        public string Producto { get; set; }
        public string Poliza { get; set; }
        public string Estado { get; set; }
        public string Direccion { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string Motivo { get; set; }
        public string MotivoREC { get; set; }
        public string MotivoSOL { get; set; }
        public string SubMotivo { get; set; }
        public string SubMotivoREC { get; set; }
        public string SubMotivoSOL { get; set; }
        public string Monto { get; set; }
        public string Reconsideracion { get; set; }
        public string Descripcion { get; set; }
        public string Vinculo { get; set; }
        public string TipoCaso { get; set; }
        public string EmailCli { get; set; }
        public string Aplicacion { get; set; }
        public string Ejecutivo { get; set; }
        public string Tipo { get; set; }
        public string Proyecto { get; set; }
        public string Summary { get; set; }
        public string Reporter { get; set; }
        public List<Archivo> Adjuntos { get; set; }
        public List<string> sustentatorios { get; set; }
        public List<string> respuestasoluciones { get; set; }
        public List<string> respuestaderivacion { get; set; }
        public List<string> comprobantes { get; set; }
        public string Carta { get; set; }
        public List<Archivo> Enviados { get; set; }
        public string DiasAtencion { get; set; }
        public string UsuarioEnvio { get; set; }
        public string FechaEnvio { get; set; }
        public string Absolucion { get; set; }
        public string tipocierre { get; set; }
        public string referencia { get; set; }
        public string direccioncli { get; set; }
        public string FechaCierre { get; set; }
        public string Usuario { get; set; }
    }
}
