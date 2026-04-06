using System.Collections.Generic;

namespace PortalGovi.Models.Configuracion
{
    public class CuentaPrincipalModel
    {
        public string Almacen { get; set; }
        public string RazonSocial { get; set; }
        public string Cuenta { get; set; } // Cuenta CLABE o Principal
        public int Orden { get; set; } // Actúa como ID principal
    }

    public class CuentaDependienteModel
    {
        // Esta clase sirve tanto para USD como para Referenciadas
        // ya que tienen estructura casi idéntica
        public int Id { get; set; } // IdUSD o IdReferencia
        public int OrdenID { get; set; } // FK
        public string Almacen { get; set; }
        public string RazonSocial { get; set; }
        public string Cuenta { get; set; } // CuentaUSD o CuentaReferenciada
        public string Tipo { get; set; } // "USD" o "REF" (Para uso interno)
    }

    // Objeto compuesto para devolver todo junto al seleccionar una cuenta
    public class DependenciasViewModel 
    {
        public List<CuentaDependienteModel> CuentasUSD { get; set; }
        public List<CuentaDependienteModel> CuentasRef { get; set; }
    }
}
