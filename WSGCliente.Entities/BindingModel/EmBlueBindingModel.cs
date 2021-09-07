using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel
{
    public class EmBlueBindingModel
    {
        public dynamic Emails { get; set; }
        public string ActionId { get; set; } = "";
        public string Message { get; set; } = "";
        public string Token { get; set; } = "";


    }
}
