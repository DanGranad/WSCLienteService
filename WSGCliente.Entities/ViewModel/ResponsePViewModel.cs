using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ResponsePViewModel
    {
        public string P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
        public List<ClientPViewModel> EListClient { get; set; }
    }
}
