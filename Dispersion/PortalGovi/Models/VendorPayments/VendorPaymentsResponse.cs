using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalGovi.Models.VendorPayments
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class BillOfExchange
    {
    }

    public class ElectronicProtocol
    {
        public string ProtocolCode { get; set; }
        public string GenerationType { get; set; }
        public int MappingID { get; set; }
        public string TestingMode { get; set; }
        public object Confirmation { get; set; }
        public object EDocType { get; set; }
        public object CFDiCancellationReason { get; set; }
        public object CFDiCancellationResponse { get; set; }
        public object CFDiCancellationReference { get; set; }
        public List<object> RelatedDocuments { get; set; }
    }

    public class PaymentInvoice
    {
        public int LineNum { get; set; }
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public double SumApplied { get; set; }
        public double AppliedFC { get; set; }
        public double AppliedSys { get; set; }
        public double DocRate { get; set; }
        public int DocLine { get; set; }
        public string InvoiceType { get; set; }
        public double DiscountPercent { get; set; }
        public double PaidSum { get; set; }
        public int InstallmentId { get; set; }
        public double WitholdingTaxApplied { get; set; }
        public double WitholdingTaxAppliedFC { get; set; }
        public double WitholdingTaxAppliedSC { get; set; }
        public object LinkDate { get; set; }
        public object DistributionRule { get; set; }
        public object DistributionRule2 { get; set; }
        public object DistributionRule3 { get; set; }
        public object DistributionRule4 { get; set; }
        public object DistributionRule5 { get; set; }
        public double TotalDiscount { get; set; }
        public double TotalDiscountFC { get; set; }
        public double TotalDiscountSC { get; set; }
    }

    public class Root
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }

        [JsonProperty("odata.etag")]
        public string odataetag { get; set; }
        public int DocNum { get; set; }
        public string DocType { get; set; }
        public string HandWritten { get; set; }
        public string Printed { get; set; }
        public DateTime DocDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string Address { get; set; }
        public object CashAccount { get; set; }
        public string DocCurrency { get; set; }
        public double CashSum { get; set; }
        public object CheckAccount { get; set; }
        public string TransferAccount { get; set; }
        public double TransferSum { get; set; }
        public DateTime TransferDate { get; set; }
        public object TransferReference { get; set; }
        public string LocalCurrency { get; set; }
        public double DocRate { get; set; }
        public string Reference1 { get; set; }
        public object Reference2 { get; set; }
        public object CounterReference { get; set; }
        public string Remarks { get; set; }
        public string JournalRemarks { get; set; }
        public string SplitTransaction { get; set; }
        public int ContactPersonCode { get; set; }
        public string ApplyVAT { get; set; }
        public DateTime TaxDate { get; set; }
        public int Series { get; set; }
        public string BankCode { get; set; }
        public string BankAccount { get; set; }
        public double DiscountPercent { get; set; }
        public object ProjectCode { get; set; }
        public string CurrencyIsLocal { get; set; }
        public double DeductionPercent { get; set; }
        public double DeductionSum { get; set; }
        public double CashSumFC { get; set; }
        public double CashSumSys { get; set; }
        public object BoeAccount { get; set; }
        public double BillOfExchangeAmount { get; set; }
        public object BillofExchangeStatus { get; set; }
        public double BillOfExchangeAmountFC { get; set; }
        public double BillOfExchangeAmountSC { get; set; }
        public object BillOfExchangeAgent { get; set; }
        public object WTCode { get; set; }
        public double WTAmount { get; set; }
        public double WTAmountFC { get; set; }
        public double WTAmountSC { get; set; }
        public object WTAccount { get; set; }
        public double WTTaxableAmount { get; set; }
        public string Proforma { get; set; }
        public object PayToBankCode { get; set; }
        public object PayToBankBranch { get; set; }
        public object PayToBankAccountNo { get; set; }
        public string PayToCode { get; set; }
        public object PayToBankCountry { get; set; }
        public string IsPayToBank { get; set; }
        public int DocEntry { get; set; }
        public string PaymentPriority { get; set; }
        public object TaxGroup { get; set; }
        public double BankChargeAmount { get; set; }
        public double BankChargeAmountInFC { get; set; }
        public double BankChargeAmountInSC { get; set; }
        public double UnderOverpaymentdifference { get; set; }
        public double UnderOverpaymentdiffSC { get; set; }
        public double WtBaseSum { get; set; }
        public double WtBaseSumFC { get; set; }
        public double WtBaseSumSC { get; set; }
        public object VatDate { get; set; }
        public object TransactionCode { get; set; }
        public string PaymentType { get; set; }
        public double TransferRealAmount { get; set; }
        public string DocObjectCode { get; set; }
        public string DocTypte { get; set; }
        public DateTime DueDate { get; set; }
        public object LocationCode { get; set; }
        public string Cancelled { get; set; }
        public string ControlAccount { get; set; }
        public double UnderOverpaymentdiffFC { get; set; }
        public string AuthorizationStatus { get; set; }
        public int BPLID { get; set; }
        public string BPLName { get; set; }
        public string VATRegNum { get; set; }
        public object BlanketAgreement { get; set; }
        public string PaymentByWTCertif { get; set; }
        public object Cig { get; set; }
        public object Cup { get; set; }
        public object AttachmentEntry { get; set; }
        public object SignatureInputMessage { get; set; }
        public object SignatureDigest { get; set; }
        public object CertificationNumber { get; set; }
        public object PrivateKeyVersion { get; set; }
        public object U_cuenta { get; set; }
        public object U_idCuenta { get; set; }
        public object U_LugarExpedicion { get; set; }
        public object U_TipoRelacion { get; set; }
        public object U_UUIDRel { get; set; }
        public string U_B1SYS_PmntMethod { get; set; }
        public object U_B1SYS_Confirm { get; set; }
        public string U_dispersion { get; set; }
        public string U_Domiciliado { get; set; }
        public object U_Referencia { get; set; }
        public object U_CuentaMayor { get; set; }
        public object U_FechaTrans { get; set; }
        public string U_EnviarCFDi { get; set; }
        public List<object> PaymentChecks { get; set; }
        public List<PaymentInvoice> PaymentInvoices { get; set; }
        public List<object> PaymentCreditCards { get; set; }
        public List<object> PaymentAccounts { get; set; }
        public List<object> PaymentDocumentReferencesCollection { get; set; }
        public BillOfExchange BillOfExchange { get; set; }
        public List<object> WithholdingTaxCertificatesCollection { get; set; }
        public List<ElectronicProtocol> ElectronicProtocols { get; set; }
        public List<object> CashFlowAssignments { get; set; }
        public List<object> Payments_ApprovalRequests { get; set; }
        public List<object> WithholdingTaxDataWTXCollection { get; set; }
    }


}
