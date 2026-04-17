using System.ComponentModel.DataAnnotations;

namespace PortalGovi.Models.Credito
{
    /// <summary>
    /// Parámetros del diálogo "Cargar saldos" (fechas en ISO yyyy-MM-dd).
    /// </summary>
    public class CargarSaldosCuadroRequest
    {
        [Required]
        public string SaldoDiario { get; set; }

        [Required]
        public string TransferenciasDesde { get; set; }

        [Required]
        public string TransferenciasHasta { get; set; }

        [Required]
        public string DepositosDesde { get; set; }

        [Required]
        public string DepositosHasta { get; set; }
    }
}
