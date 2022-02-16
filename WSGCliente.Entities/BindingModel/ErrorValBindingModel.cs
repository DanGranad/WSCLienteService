using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel
{
    public class ErrorValBindingModel
    {
        public string P_SNOPROCESO { get; set; }
        public Int64 P_NNUMREG { get; set; }
        public string P_SFILENAME { get; set; }
        public string P_SDESERROR { get; set; }
        public string P_SCOLUMNA { get; set; }
        public Int64 P_NUSERCODE { get; set; }
        public Int64 P_NIDDOC_TYPE { get; set; }
        public string P_SIDDOC { get; set; }
    }
}
