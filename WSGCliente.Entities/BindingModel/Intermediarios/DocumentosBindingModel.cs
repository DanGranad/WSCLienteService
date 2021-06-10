using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.Intermediarios
{
    public class DocumentosBindingModel
    {
        public string P_TipOper { get; set; }
        public string P_NROW { get; set; }
        public string P_SCLIENT { get; set; }
        public Int32 P_NID_TIPO_DOC_ADJUNTO { get; set; } 
        public string P_SNAME_TIPO_DOC_ADJUNT { get; set; }
        public string P_SFORMAT_FILE { get; set; }
        public string P_SNAME_FILE { get; set; }
        public string P_DDATE_LOAD { get; set; }
        public Int32 P_NUSERCODE { get; set; }
        public string P_SNOMUSUARIO { get; set; }
        public string P_DCOMPDATE { get; set; }

    }
}
