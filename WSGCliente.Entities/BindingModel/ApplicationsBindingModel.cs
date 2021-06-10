using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel
{
    public class ApplicationsBindingModel
    {
        public Decimal NID_APPLICATION { get; set; }
        public string SDESCRIPTION { get; set; }
        public string SCOD_APPLICATION { get; set; }
        public string SESTADO { get; set; }
        public Decimal NID_SISTEMA_LAFT { get; set; }
        public string NIND_USE_LAFT { get; set; }

    }
}
