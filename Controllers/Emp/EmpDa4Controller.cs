using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers.Emp
{
    public class EmpDa4Controller : Dou.Controllers.AGenericModelController<F22cmmEmpDa4>
    {
        // GET: EmpDa4 學歷
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmEmpDa4> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(new DouImp.Models.DouModelContextExt());
        }
    }
}