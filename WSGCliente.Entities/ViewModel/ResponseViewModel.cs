using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ResponseViewModel
    {
        public string P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
        public string P_SEMAIL { get; set; }
        public string P_SCOD_CLIENT { get; set; }
        public string P_SURL_SISTEMA { get; set; }
        public int? P_NIDCM { get; set; }
        public List<ClientViewModel> EListClient { get; set; }
        public List<ListViewErrores> EListErrores { get; set; }
        public Object Data { get; set; }
    }
}
