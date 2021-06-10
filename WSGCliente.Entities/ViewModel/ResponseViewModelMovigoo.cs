using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ResponseViewModelMovigoo
    {
        public string P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
        public string EListClient_P_DESDOC_TYPE { get; set; } = "";
        public string EListClient_P_SIDDOC { get; set; } = "";
        public string EListClient_P_SFIRSTNAME { get; set; } = "";
        public string EListClient_P_SLASTNAME { get; set; } = "";
        public string EListClient_P_SLASTNAME2 { get; set; } = "";
        public string EListClient_P_DESSEXCLIEN { get; set; } = "";
        public string EListClient_P_DESCIVILSTA { get; set; } = "";
        public string EListClient_P_DESPRODUCTO { get; set; } = "";
        public string EListClient_P_DESPRODUCTO_Finicio { get; set; } = "";
        public string EListClient_P_DESPRODUCTO_Ffin { get; set; } = "";
        public string EListClient_P_NPOLICY { get; set; } = "";
        public string EListClient_P_NCERTIF { get; set; } = "";
    }
}
