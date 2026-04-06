using System;
using System.Collections.Generic;

public class CYC_PagosEncabezado
{
    public int FolioPago { get; set; }
    public DateTime Fecha { get; set; }
    public DateTime FechaPago { get; set; }
    public string Sociedad { get; set; }
    public string Sucursal { get; set; }
    public int BPLId { get; set; }
    public string IdCuenta { get; set; }
    public string Cuenta { get; set; }
    public string IdEdoCta { get; set; }
    public string CardCode { get; set; }
    public string CardName { get; set; }
    public decimal Monto { get; set; }
    public string Referencia { get; set; }
    public string Descuento1 { get; set; }
    public decimal PorcDesc1 { get; set; }
    public string Descuento2 { get; set; }
    public decimal PorcDesc2 { get; set; }
    public string Descuento3 { get; set; }
    public decimal PorcDesc3 { get; set; }
    public string Descuento4 { get; set; }
    public decimal PorcDesc4 { get; set; }
    public decimal TotalAPagar { get; set; }
    public decimal PagoaCuenta { get; set; }
    public decimal SaldoaFavor { get; set; }
    public string Estatus { get; set; }
    public string Comentarios { get; set; }
    public int TipoOp { get; set; }
    public string Usuario { get; set; }
    public string FidValue { get; set; }
    public List<CYC_PagosDetalle> Detalles { get; set; }
}

public class CYC_PagosDetalle
{
    public int IdDetalle { get; set; }
    public int FolioPago { get; set; }
    public int DocEntry { get; set; }
    public int DocNum { get; set; }
    public decimal SaldoVencido { get; set; }
    public decimal RebjDev { get; set; }
    public decimal MontoDcto1 { get; set; }
    public decimal TDcto1 { get; set; }
    public decimal MontoDcto2 { get; set; }
    public decimal TDcto2 { get; set; }
    public decimal MontoDcto3 { get; set; }
    public decimal TDcto3 { get; set; }
    public decimal MontoDcto4 { get; set; }
    public decimal TDcto4 { get; set; }
    public decimal TotalPago { get; set; }
    public int Manual { get; set; }
    public string Tipo { get; set; }
    public string Estatus { get; set; }
    public string Comentarios { get; set; }
    public string UUID { get; set; }
    public string TransId { get; set; }
}