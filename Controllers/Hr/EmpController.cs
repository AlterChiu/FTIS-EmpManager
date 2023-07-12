using Dou.Models.DB;
using DouImp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FtisHelperV2.DB.Model;
using Dou.Misc;
using System.Threading.Tasks;
using Dou.Controllers;
using System.Data.Entity;
using System.Collections;
using System.Threading;

namespace DouImp.Controllers
{   
    [Dou.Misc.Attr.MenuDef(Id = "EmpAll", Name = "員工資料總表", MenuPath = "人資專區", Action = "Index", Index = 1, Func = Dou.Misc.Attr.FuncEnum.ALL, AllowAnonymous = false)]
    public class EmpController : Dou.Controllers.APaginationModelController<F22cmmEmpData>
    {
        // GET: Disposals
        public ActionResult Index()
        {
            return View();
        }

        ////public override Task<ActionResult> GetData(params KeyValueParams[] paras)
        ////{
        ////    //Mvc MaxJsonLength序列化長度問題
        ////    var datas = base.GetData(paras);
        ////    (datas.Result as JsonResult).MaxJsonLength = Int32.MaxValue;

        ////    return datas;
        ////}

        protected override IQueryable<F22cmmEmpData> BeforeIQueryToPagedList(IQueryable<F22cmmEmpData> iquery, params KeyValueParams[] paras)
        {
            ////////Test Left Join
            ////Dou.Models.DB.IModelEntity<F22cmmEmpData> data = new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            ////Dou.Models.DB.IModelEntity<F22cmmEmpDa1> da1s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            //////( e1 ) . GroupJoin( e2 , x1 => k1 , x2 => k2 , ( x1 , g ) => v )
            ////var z = data.GetAll().GroupJoin(da1s.GetAll(), a => a.Fno, b => b.Fno, (a, b) => new { a, b });

            ////foreach (var item in z)
            ////{
            ////    string ss = "Abc";
            ////}

            ////return iquery;
            return base.BeforeIQueryToPagedList(iquery, paras);
        }

        public override DataManagerOptions GetDataManagerOptions()
        {
            var options = base.GetDataManagerOptions();
            options.ctrlFieldAlign = "left";
            options.editformWindowStyle = "modal";
            options.editformWindowClasses = "modal-xl";
            options.editformSize.height = "fixed";
            options.editformSize.width = "auto";
            //options.useMutiDelete = true;
            options.GetFiled("DCode_").visibleEdit = false;
            options.GetFiled("UpdateTime").visibleEdit = false;
            options.GetFiled("UpdateMan").visibleEdit = false;
            options.GetFiled("SeatNo").visibleEdit = false;

            options.GetFiled("Da1s").visibleEdit = false;

            //共用頁面
            options.editformWindowStyle = "showEditformOnly";

            return options;
        }

        /// <summary>
        /// 新增異動者與異動時間
        /// </summary>
        /// <param name="dbEntity"></param>
        /// <param name="objs"></param>
        protected override void AddDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            foreach (var obj in objs)
            {
                obj.UpdateMan = Dou.Context.CurrentUser<User>().Id;
                obj.UpdateTime = DateTime.Now;
            }
            dbEntity.Add(objs);
        }

        /// <summary>
        /// 更新異動者與異動時間
        /// </summary>
        /// <param name="dbEntity"></param>
        /// <param name="objs"></param>
        protected override void UpdateDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            foreach (var obj in objs)
            {
                obj.UpdateMan = Dou.Context.CurrentUser<User>().Id;
                obj.UpdateTime = DateTime.Now;
            }
            dbEntity.Update(objs);
        }
        protected override IModelEntity<F22cmmEmpData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            //return new Dou.Models.DB.ModelEntity<AssetDisposals>(new DouImp.Models.DouModelContextExt());
        }
    }
}