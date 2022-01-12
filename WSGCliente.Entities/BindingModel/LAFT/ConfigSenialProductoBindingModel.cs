using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class ConfigSenialProductoBindingModel
    {

        public int id { get; set; }
        public int idConfigSenial { get; set; }
        public MaestroBindingModel producto { get; set; }
        public string fechaRegistro { get; set; }
        public string usuario { get; set; }
        public bool activo { get; set; }
    }
}
