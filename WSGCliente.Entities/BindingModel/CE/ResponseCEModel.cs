using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel
{
    public  class ResponseCEModel
    {
        public int estado { get; set; }
        public string mensaje { get; set; }
        public string nombre { get; set; }
        public string nacionalidad { get; set; }
        public string nacimiento { get; set; }
        public string calidad { get; set; }
        public string vencresidencia { get; set; }
        public string caducce { get; set; }
        public string emitCE { get; set; }

    }
}
