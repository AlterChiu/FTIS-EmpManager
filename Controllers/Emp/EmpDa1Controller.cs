using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Emp
{
    public class EmpDa1Controller : Dou.Controllers.AGenericModelController<F22cmmEmpData>
    {
        // GET: EmpDa1 基本資料
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmEmpData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpData>(new DouImp.Models.DouModelContextExt());
        }
    }
}