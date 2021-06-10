using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class SenialBindingModel
    {
        public int id { get; set; }
        public string descripcion { get; set; }
        public string color { get; set; }
        public bool activo { get; set; }
        public bool indAlert { get; set; }
        public bool indError { get; set; }
    }
}
