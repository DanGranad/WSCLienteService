using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ResponseSegmentoViewModel
    {
        public string P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
        public List<SegmentoViewModel> EListSegmento { get; set; }
     //   public List<ListViewErrores> EListErrores { get; set; }
       // public Object Data { get; set; }
    }
}
