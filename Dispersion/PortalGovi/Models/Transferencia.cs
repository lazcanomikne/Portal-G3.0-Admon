using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models
{
    public class Transferencia
    {

        public string Sociedad { get; set; }
        public string VATRegNum { get; set; }
        public string BankCtlKey { get; set; }
        public string MandateID { get; set; }
        public string Account { get; set; }
        public string DflAccount { get; set; }
        public decimal DocTotal { get; set; }
        public int DocNum { get; set; }
        public string JrnlMemo { get; set; }
        public string County { get; set; }
        public string LicTradNum { get; set; }
        public string IVA { get; set; }
        public string E_Mail { get; set; }
        public DateTime DocDate { get; set; }
        public string CardName { get; set; }
        public int DocEntry { get; set; }
    }

    public class Servicio
    {

        public string Sociedad { get; set; }
        public string VATRegNum { get; set; }
        public string BankCtlKey { get; set; }
        public string MandateID { get; set; }
        public string Account { get; set; }
        public string DflAccount { get; set; }
        public decimal DocTotal { get; set; }
        public int DocNum { get; set; }
        public string JrnlMemo { get; set; }
        public string County { get; set; }
        public string LicTradNum { get; set; }
        public string IVA { get; set; }
        public string E_Mail { get; set; }
        public DateTime DocDate { get; set; }
        public string CardName { get; set; }
        public int DocNum2 { get; set; }
        public int DocEntry { get; set; }
        public string BPLName { get; set; }
        public string GLAccount { get; set; }
        public string REF01 { get; set; }
        public string REF02 { get; set; }
        public string REF03 { get; set; }
        public string REF04 { get; set; }
        public string REF05 { get; set; }
        public string REF06 { get; set; }
        public string FILLER { get; set; }
        public string FECVEN { get; set; }
    }
}
