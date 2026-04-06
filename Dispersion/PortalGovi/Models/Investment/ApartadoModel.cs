using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models.Investment
{
    public class ApartadoModel
    {
        public int ID { get; set; }
        public string Cuenta { get; set; }
        public decimal MontoApartar { get; set; }
        public DateTime Fecha { get; set; }
    }
}
