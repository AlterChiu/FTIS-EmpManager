using Dou.Misc;
using Dou.Models.DB;
using DouImp.Models;
using FtisHelperV2.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Emp
{
    public class EmpDa1Controller : Dou.Controllers.AGenericModelController<F22cmmEmpDa1>
    {
        // GET: EmpDa1 基本資料
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmEmpDa1> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }

        protected override void AddDBObject(IModelEntity<F22cmmEmpDa1> dbEntity, IEnumerable<F22cmmEmpDa1> objs)
        {
            var f = objs.First();            
            f.mno = f.Fno;
            f.UpdateTime = DateTime.Now;
            f.Updateman = Dou.Context.CurrentUserBase.Name;

            base.AddDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa1();
        }

        protected override void UpdateDBObject(IModelEntity<F22cmmEmpDa1> dbEntity, IEnumerable<F22cmmEmpDa1> objs)
        {
            var f = objs.First();
            f.mno = f.Fno;
            f.UpdateTime = DateTime.Now;
            f.Updateman = Dou.Context.CurrentUserBase.Name;

            base.UpdateDBObject(dbEntity, objs);
            FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa1();
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var opts = base.GetDataManagerOptions();

            opts.GetFiled("mno").visibleEdit = false;            
            opts.GetFiled("UpdateTime").visibleEdit = false;
            opts.GetFiled("Updateman").visibleEdit = false;

            ////opts.singleDataEdit = true;
            ////string userid = Dou.Context.CurrentUser<User>().Id;
            ////var dbe = GetModelEntity();
            ////opts.datas = new F22cmmEmpDa1[] { dbe.FirstOrDefault(s => s.Fno == userid) };
            ////opts.editformWindowStyle = "showEditformOnly";

            return opts;
        }

        public virtual ActionResult AddDB2(List<F22cmmEmpDa1> objs)
        {
            bool success = false;
            string desc = "";

            try
            {
                var f = objs.First();
                f.mno = f.Fno;
                f.UpdateTime = DateTime.Now;
                f.Updateman = Dou.Context.CurrentUserBase.Name;

                base.AddDBObject(GetModelEntity(), objs);
                FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa1();

                success = true;
                desc = "新增成功";
            }
            catch (Exception ex)
            {
                desc = "新增失敗：" + ex.Message + " " + ex.InnerException;
            }

            return Json(new { Success = success, Desc = desc, data = objs }, JsonRequestBehavior.AllowGet);
        }

        public virtual ActionResult UpdateDB2(List<F22cmmEmpDa1> objs)
        {
            bool success = false;
            string desc = "";

            try
            {
                var f = objs.First();
                f.mno = f.Fno;
                f.UpdateTime = DateTime.Now;
                f.Updateman = Dou.Context.CurrentUserBase.Name;

                base.UpdateDBObject(GetModelEntity(), objs);
                FtisHelperV2.DB.Helpe.Employee.ResetGetAllF22cmmEmpDa1();

                success = true;
                desc = "修改成功";
            }
            catch (Exception ex)
            {
                desc = "修改失敗：" + ex.Message + " " + ex.InnerException;
            }

            return Json(new { Success = success, Desc = desc, data = objs }, JsonRequestBehavior.AllowGet);
        }
    }
}