using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models.Investment
{

    public class SaldoFijoModel
    {
        public int Id { get; set; }
        public string Titular { get; set; }
        public string Cuenta { get; set; }
        public double SaldoFijo { get; set; }
    }
}
