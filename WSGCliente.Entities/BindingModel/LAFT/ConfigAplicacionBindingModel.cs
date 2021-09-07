using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class ConfigAplicacionBindingModel
    {
        public int id { get; set; }
        public string fechaRegistro { get; set; }
        public string usuario { get; set; }
        public bool activo { get; set; }
        public MaestroBindingModel aplicacion { get; set; }
        public List<ConfigProductoBindingModel> productos { get; set; }

    }
}
