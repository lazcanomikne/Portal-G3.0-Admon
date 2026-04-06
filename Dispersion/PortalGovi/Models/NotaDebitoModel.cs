using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class NotaDebitoModel
    {
        public int Documento { get; set; }
        public string Cliente { get; set; }
        public string Concepto { get; set; }
        public int Cantidad { get; set; }
        public double Precio { get; set; }
        public string Comentarios{ get; set; }

    }
}
