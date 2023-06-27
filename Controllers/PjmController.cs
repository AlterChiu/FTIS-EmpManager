using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{
    [Dou.Misc.Attr.MenuDef(Name = "專案對應", MenuPath = "員工基本資料", Action = "Index", Index = 77, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class PjmController : Dou.Controllers.APaginationModelController<F22cmmProjectDataMap>
    {
        // GET: Pjm
        public ActionResult Index()
        {
            return View();
        }
        protected override IModelEntity<F22cmmProjectDataMap> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmProjectDataMap>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}