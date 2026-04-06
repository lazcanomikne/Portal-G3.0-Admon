using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models.Investment
{
    public class Detalle
    {
        public string Fecha { get; set; }
        public object Clabe { get; set; }
        public int Cuenta { get; set; }
        public string Moneda { get; set; }
        public double SaldoActual { get; set; }
        public double SaldoDisponible { get; set; }
        public double SaldoRetenido { get; set; }
        public string Titular { get; set; }
    }

    public class SaldoDisponibleModel
    {
        public string Fecha { get; set; }
        public string Archivo { get; set; }
        public string Usuario { get; set; }
        public List<Detalle> Detalle { get; set; }
    }
}
