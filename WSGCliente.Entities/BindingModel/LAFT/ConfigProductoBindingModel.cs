﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WSGCliente.Entities.BindingModel.LAFT
{
    public class ConfigProductoBindingModel
    {
        public int id { get; set; }
        public string fechaRegistro { get; set; }
        public string usuario { get; set; }
        public bool activo { get; set; }
        public MaestroBindingModel producto { get; set; }

    }
}
