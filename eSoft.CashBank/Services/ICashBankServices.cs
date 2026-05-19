using eSoft.CashBank.Model;
using eSoft.CashBank.View;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace eSoft.CashBank.Services
{
    public interface ICashBankServices
    {
        List<CbBank> GetBank();
        CbBank GetBankId(int id);
        CbBank GetBankKd(string id);
        bool CekKdBank(string kodeBank);
        bool AddBank(BankView banks);
        Task<bool> EditBank(BankView banks);
        Task<bool> DelBank(int banks);

        List<CbGrp> GetSrcGroup();

        bool CekSrcGroup(string kodeBank);
        CbGrp GetSrcGroupId(int id);
        CbGrp GetSrcGroupKd(string id);
        bool AddSrcGroup(SrcGroupView codeview);
        Task<bool> EditSrcGroup(SrcGroupView codeview);
        Task<bool> DelSrcGroup(int codeview);
        List<CbSrcCode> GetSrcCode();
        bool CekSrcCode(string kodeBank);
        CbSrcCode GetSrcCodeId(int id);

        CbSrcCode GetSrcCodeKd(string id);
        bool AddSrcCode(SrcCodeView codeview);
        Task<bool> EditSrcCode(SrcCodeView codeview);
        Task<bool> DelSrcCode(int codeview);
        CbTransH GetTrans(int id);
        List<SearchTransHView> GetTransHSearch();
        List<CbTransH> GetTransH();
        List<CbTransH> Get3TransH(DateTime tgl1, DateTime tgl2);
        List<CbTransD> GetTransD();
        List<CbTransD> GetTransDdetail(int Id);
        CbTransH AddTransH(TransHView transH);
        CbTransH EditTransH(TransHView transH);
        Task<bool> DelTransH(int id);
        CbTransfer GetTransferId(int id);
        List<CbTransfer> GetTransfer();
        CbTransfer AddTransfer(TransferView transfer);
        CbTransfer EditTransfer(TransferView transH);
        Task<bool> DelTransfer(int id);
        List<RekeningView> CetakMutasi(DateTime Tanggal1, DateTime Tanggal2, string KodeBank);
        List<RekeningView> CetakSourceBank(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode, string[] kodeBank);
        void prosesCashBank();
        List<RekeningView> CetakSourceRekapBank(DateTime Tanggal1, DateTime Tanggal2, string[] sourceCode, string[] kodeBanks);
        Task SaveTransactionsAsync(List<BankTransactionView> transactions, DateTime formDate, string kodeBank, string tambah, string kurang);


    }
}