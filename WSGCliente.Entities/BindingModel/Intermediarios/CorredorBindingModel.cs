using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.Intermediarios
{
    public class CorredorBindingModel
    {
       
        
        public List<PhoneBindingModel> phoneBindingModels { get; set; }
        public List<EmailBindingModel> emailBindingModels { get; set; }
        public List<AddressBindingModel> addressBindingModels{ get; set; }
        public List<ContactBindingModel> contactBindingModels { get; set; }
    }
}
