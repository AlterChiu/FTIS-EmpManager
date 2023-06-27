using Dou.Controllers;
using Dou.Misc;
using Dou.Models.DB;
using FtisHelperV2.DB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DouImp.Controllers
{
    [Dou.Misc.Attr.MenuDef(Name = "專案", MenuPath = "員工基本資料", Action = "Index", Index = 76, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class PjController : Dou.Controllers.APaginationModelController<F22cmmProjectData>
    {
        // GET: Country
        public ActionResult Index()
        {
            return View();
        }
        //protected override IEnumerable<F22cmmProjectData> GetDataDBObject(IModelEntity<F22cmmProjectData> dbEntity, params KeyValueParams[] paras)
        //{
        //    var iquery = base.GetDataDBObject(dbEntity, paras);
        //    //if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
        //    //    iquery = iquery.OrderBy(s => s.PrjID);
        //    return iquery;
        //}
        protected override IQueryable<F22cmmProjectData> BeforeIQueryToPagedList(IQueryable<F22cmmProjectData> iquery, params KeyValueParams[] paras)
        {
            iquery = base.BeforeIQueryToPagedList(iquery, paras);
            return iquery;
        }
        //public override DataManagerOptions GetDataManagerOptions()
        //{
        //    var options = base.GetDataManagerOptions();
        //    options.editformWindowStyle = "modal";
        //    return options;
        //}
        //protected override IQueryable<F22cmmEmpData> BeforeIQueryToPagedList(IQueryable<F22cmmEmpData> iquery, params KeyValueParams[] paras)
        //{
        //    iquery = base.BeforeIQueryToPagedList(iquery, paras);
        //    var filters = paras.FirstOrDefault(s => s.key == "filter");

        //    var fno = Dou.Misc.HelperUtilities.GetFilterParaValue(paras, "Fno");
        //    if (fno == null)//有選員編就不考慮部門
        //    {
        //        var dep = Dou.Misc.HelperUtilities.GetFilterParaValue(paras, "Dep");
        //        if (dep != null)
        //        {
        //            var us = FtisHelperV2.DB.Helper.GetAllEmployee().Where(s => s.DCode == dep).Select(s => s.Fno);
        //            iquery = iquery.Where(s => us.Contains(s.Fno));
        //        }                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                              
        //    }
        //    if (string.IsNullOrEmpty(paras.FirstOrDefault(s => s.key == "sort").value + ""))
        //        iquery = iquery.OrderByDescending(s => s.UpdateTime);
        //    return iquery;
        //}
        protected override IModelEntity<F22cmmProjectData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmProjectData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
        }
    }
}