using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eSoft.Hutang.Data;
using eSoft.Hutang.Model;
using eSoft.Hutang.View;
using Microsoft.EntityFrameworkCore;

namespace eSoft.Hutang.Services
{
    public interface IPayableServices
    {
        bool CekKdSupplier(string supplier);
        List<ApSuppl> GetSupplier();
        ApSuppl GetSupplierId(int id);
        ApSuppl GetSupplierKode(string kode);
        bool AddSupplier(SupplierView suppliers);
        Task<bool> EditSupplier(SupplierView suppliers);
        Task<bool> DelSupplier(int suppliers);
        bool CekAcctSet(string supplier);
        List<ApAcct> GetApAkunSet();
        ApAcct GetApAkunSetId(int id);
        bool AddAkunSet(ApAcctView codeview);
        Task<bool> EditAkunSet(ApAcctView codeview);
        Task<bool> DelAkunSet(int codeview);
        bool CekDistCode(string distcode);
        List<ApDist> GetDist();
        ApDist GetDistId(int id);
        bool AddDist(ApDistView codeview);
        Task<bool> EditDist(ApDistView codeview);
        Task<bool> DelDist(int codeview);
        ApTransH GetTrans(int id);
        List<ApTransH> GetTransH();
        List<ApTransH> Get3TransH();
        List<ApTransD> GetTransD();
        ApTransH AddTransH(ApTransHView transH);
        ApTransH EditTransH(ApTransHView transH);
        Task<bool> DelTransH(int id);
        bool CekAlreadyPayment(string dokumen);
        ApHutang GetHutang(string bukti);
        bool CekBeli(string noLpb);
        List<ApHutangView> GetBayarDimuka();
        List<ApHutang> Detail1(string xKdHeader);
        List<ApAgingView> GetAgingSchedule();
        Task ProsesHutang();

    }
}
