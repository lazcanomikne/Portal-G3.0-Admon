using System;
using System.Collections.Generic;

namespace PortalGovi.Models
{
    public class PaymentInvoice
    {
        public int DocEntry { get; set; }
        public double SumApplied { get; set; }
    }

    public class PasivoModel
    {
        public string CardCode { get; set; }
        public int BPLID { get; set; }
        public string BPLName { get; set; }
        public int Series { get; set; }
        public string JournalRemarks { get; set; }
        public string Remarks { get; set; }
        public List<PaymentInvoice> PaymentInvoices { get; set; }
        public string TransferAccount { get; set; }
        public DateTime TransferDate { get; } = DateTime.Now;
        public double TransferSum { get; set; }
    }
}
