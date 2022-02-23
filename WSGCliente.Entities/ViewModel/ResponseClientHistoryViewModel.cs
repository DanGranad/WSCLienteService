using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.ViewModel
{
    public class ResponseClientHistoryViewModel
    {
        public string P_NCODE { get; set; }
        public string P_SMESSAGE { get; set; }
     
        public List<HistoryInformationViewModel> EListHistoryInformationClient { get; set; }
        public List<HistoryPhoneViewModel> EListHistoryPhoneBeforeClient { get; set; }
        public List<HistoryPhoneViewModel> EListHistoryPhoneNowClient { get; set; }
        public List<HistoryEmailViewModel> EListHistoryEmailBeforeClient { get; set; }
        public List<HistoryEmailViewModel> EListHistoryEmailNowClient { get; set; }
        public List<HistoryAddressViewModel> EListHistoryAddressBeforeClient { get; set; }
        public List<HistoryAddressViewModel> EListHistoryAddressNowClient { get; set; }
        public List<HistoryContactViewModel> EListHistoryContactBeforeClient { get; set; }
        public List<HistoryContactViewModel> EListHistoryContactNowClient { get; set; }
        public List<ListViewErrores> EListErrores { get; set; }
    
    }
}
