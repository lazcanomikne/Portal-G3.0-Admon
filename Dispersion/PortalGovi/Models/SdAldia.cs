using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class SdAldia
    {
        public string Empresa { get; set; }
        public string Cuenta { get; set; }
        public string NombreCuenta { get; set; }
        public decimal SaldoDiario { get; set; }
        public string Fecha { get; set; }
    }
    public class Fechas
    {
        public string FechaIni { get; set; }
        public string FechaFin { get; set; }
    }
}
