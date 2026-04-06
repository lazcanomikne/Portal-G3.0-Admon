using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class FiliarModel
    {

        public int Documento { get; set; }
        public int Referencia { get; set; }
        public double Total { get; set; }
        public string Prefectuado { get; set; }
        public string Precibido { get; set; }
        public int Tipo { get; set; }

    }
}
