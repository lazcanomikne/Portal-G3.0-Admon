using Newtonsoft.Json;

namespace PortalGovi.Models.Cancelacion
{
    public class Credentials
    {
        public string id { get; set; }
        public string token { get; set; }
    }

    public class Document
    {
        [JsonProperty("certificate-number")]
        public string certificatenumber { get; set; }
        public string rfc_receptor { get; set; }
        public decimal? total_cfdi { get; set; }
        public string motive { get; set; }
    }

    public class Issuer
    {
        public string rfc { get; set; }
    }

    public class CancelationRequest
    {
        public Credentials credentials { get; set; }
        public Issuer issuer { get; set; }
        public Document document { get; set; }
    }

    public class CancelationStatus
    {
        public string validacion_efos { get; set; }
        public string estatus_cancelacion { get; set; }
        public string estado { get; set; }
        public string es_cancelable { get; set; }
        public string codigo_estatus { get; set; }
    }

}
