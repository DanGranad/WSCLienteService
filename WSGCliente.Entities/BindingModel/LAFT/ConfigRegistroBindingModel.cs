using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class ConfigRegistroBindingModel
    {
        public int idRegistro { get; set; }
        public List<ConfigAplicacionBindingModel> aplicaciones { get; set; }
    }
}
