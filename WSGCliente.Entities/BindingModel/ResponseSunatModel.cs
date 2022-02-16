using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel
{
     public class ResponseSunatModel
    {
        public Boolean Exito { get; set; }
        public string Mensaje { get; set; }
        public string Pila { get; set; }
        public string CodigoHash { get; set; }
        public ClientSunat Data { get; set; }

        
        
    }

    public class ClientSunat {
        public string RazonSocial { get; set; }
        public string Direcion { get; set; }
        public string Ruc { get; set; }
        public string EstadoContr { get; set; }
        public string TipoContr { get; set; }
        public string SistemaEmisionComprobante { get; set; }
        public string Departamento { get; set; }
        public string Provincia { get; set; }
        public string Distrito { get; set; }
    }


    
}
