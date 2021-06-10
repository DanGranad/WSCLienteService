using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;

namespace WSGCliente.DataAccess
{
    [OracleCustomTypeMapping("C##USER.MYNUMBER_TYPE")]
    public class MyCustomClass : IOracleCustomType
    {

        [OracleObjectMappingAttribute("SCAMPO")]
        public virtual int SCAMPO { get; set; }

        [OracleObjectMappingAttribute("SVALOR")]
        public virtual int SVALOR { get; set; }

        [OracleObjectMappingAttribute("SMENSAJE")]
        public virtual int SMENSAJE { get; set; }

        [OracleObjectMappingAttribute("SGRUPO")]
        public virtual int SGRUPO { get; set; }


        public void FromCustomObject(OracleConnection con, IntPtr pUdt)
        {
           // OracleUdt.SetValue(conn, object, "MyNumber", this.cNumber);
        }

        public void ToCustomObject(OracleConnection con, IntPtr pUdt)
        {
            throw new NotImplementedException();
        }
    }
}
