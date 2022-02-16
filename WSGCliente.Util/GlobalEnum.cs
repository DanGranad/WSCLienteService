using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Util
{
    public static class GlobalEnum
    {
        public enum Status
        {
            Registrate = 0,
            Initial = 1,
            correct = 2,
            error = 3
        }
        public enum Service
        {
            
            RENIEC = 0,
            SUNAT = 1,
            GESTORCLIENTE = 2,
            LAFT = 3,
            RegistroLAFT=4,
                UrlCE = 5
        }
        public enum Method
        {
            POST = 1,
            GET = 0
        }
        public enum Operation
        {
            [Description("CON")]
            CONSULT,
            [Description("INS")]
            MODIFY,
        }


        public static string GetDescription<T>(this T e) where T : IConvertible
        {
            if (e is Enum)
            {
                Type type = e.GetType();
                Array values = System.Enum.GetValues(type);

                foreach (int val in values)
                {
                    if (val == e.ToInt32(CultureInfo.InvariantCulture))
                    {
                        var memInfo = type.GetMember(type.GetEnumName(val));
                        var descriptionAttribute = memInfo[0]
                            .GetCustomAttributes(typeof(DescriptionAttribute), false)
                            .FirstOrDefault() as DescriptionAttribute;

                        if (descriptionAttribute != null)
                        {
                            return descriptionAttribute.Description;
                        }
                    }
                }
            }
            return null; // could also return string.Empty
        }



    }
}
