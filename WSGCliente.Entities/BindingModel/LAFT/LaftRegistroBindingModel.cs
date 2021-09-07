using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class LaftRegistroBindingModel
    {
        public int id { get; set; }
        public int secuencia { get; set; }
        public string numero { get; set; }
        public int idCarga { get; set; }
        public MaestroBindingModel persona { get; set; }
        public MaestroBindingModel pais { get; set; }
        public SenialBindingModel senial { get; set; }
        public MaestroBindingModel documento { get; set; }
        public ConfigRegistroBindingModel configRegistro { get; set; }
        public List<ConfigAplicacionBindingModel> aplicaciones { get; set; }
        public string numeroDocumento { get; set; }
        public string apellidoPaterno { get; set; }
        public string apellidoMaterno { get; set; }
        public string nombre { get; set; }
        public string observacion { get; set; }
        public string fechaRegistro { get; set; }
        public string usuario { get; set; }
        public bool activo { get; set; }
        public bool liberado { get; set; }
        public bool locked_App { get; set; } 
        public string locked_Message { get; set; }


    }
}
