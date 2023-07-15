using Dou.Misc;
using Dou.Models.DB;
using FtisHelperV2.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Hr
{
    public class EmpDa6Controller : Dou.Controllers.AGenericModelController<F22cmmEmpDa6>
    {
        // GET: EmpDa6 家庭狀況
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmEmpDa6> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpDa6>(new FtisModelContext());
        }

        protected override void AddDBObject(IModelEntity<F22cmmEmpDa6> dbEntity, IEnumerable<F22cmmEmpDa6> objs)
        {
            var f = objs.First();

            Dou.Models.DB.IModelEntity<FtisHelperV2.DB.Model.F22cmmEmpDa6> models = new Dou.Models.DB.ModelEntity<FtisHelperV2.DB.Model.F22cmmEmpDa6>(new FtisHelperV2.DB.FtisModelContext());
            var snos = models.GetAll(a => a.Fno == f.Fno).Select(a => a.sno).ToList();
            int max = 100;
            byte sno = 1;
            for (; sno < max; sno++)
                if (!snos.Contains(sno)) break;

            f.mno = f.Fno;
            f.sno = sno;
            f.UpdateTime = DateTime.Now;
            f.UpdateMan = Dou.Context.CurrentUserBase.Name;

            base.AddDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa6();
        }

        protected override void UpdateDBObject(IModelEntity<F22cmmEmpDa6> dbEntity, IEnumerable<F22cmmEmpDa6> objs)
        {
            var f = objs.First();
            f.mno = f.Fno;
            f.UpdateTime = DateTime.Now;
            f.UpdateMan = Dou.Context.CurrentUserBase.Name;

            base.UpdateDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa6();
        }

        protected override void DeleteDBObject(IModelEntity<F22cmmEmpDa6> dbEntity, IEnumerable<F22cmmEmpDa6> objs)
        {
            base.DeleteDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa6();
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var opts = base.GetDataManagerOptions();

            opts.GetFiled("mno").visibleEdit = false;
            opts.GetFiled("sno").visibleEdit = false;
            opts.GetFiled("UpdateTime").visibleEdit = false;
            opts.GetFiled("UpdateMan").visibleEdit = false;

            return opts;
        }
    }
}