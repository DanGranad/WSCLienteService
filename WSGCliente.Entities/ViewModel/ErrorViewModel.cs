using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ErrorViewModel
    {
        public string SNOPROCESO { get; set; }
        public Decimal NNUMREG { get; set; }
        public string SFILENAME { get; set; }
        public string SDESERROR { get; set; }
        public string SCOLUMNA { get; set; }
        public Int32 NUSERCODE { get; set; }
        public DateTime DCOMPDATE { get; set; }
        public Int64 NIDDOC_TYPE { get; set; }
        public string SIDDOC { get; set; }
    }
}
