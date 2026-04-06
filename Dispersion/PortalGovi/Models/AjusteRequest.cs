using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class AjusteRequest
    {
        public List<Ajuste> Ajustes { get; set; }
        public Sucursal Sucursal { get; set; }
        public string Motivo { get; set; }
    }
}
