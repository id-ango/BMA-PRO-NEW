using System;

namespace eSoft.Penjualan.Services
{
    public interface ISalesDocumentNumberService
    {
        string GetNumber();
        string GetNumberTax();
        string GetNumberRetur();
        string GetNumberTaxRetur();
    }
}
