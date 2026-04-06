namespace PortalGovi.Models.Cancelacion
{
    public class CancelacionMasiva
    {
        public int id { get; set; }
        public string RFC_Emisor { get; set; }
        public string RFC_Receptor { get; set; }
        public decimal? Total_UUID { get; set; }
        public string UUID { get; set; }
        public string Estatus { get; set; }
        public string Motivo { get; set; }
        public string Usuario { get; set; }
    }

    public class Credencial {
        public int id { get; set; }
        public string RFC_Emisor { get; set; }
        public string id_Empresa { get; set; }
        public string token { get; set; }
        public string Certificado { get; set; }
    }
}
