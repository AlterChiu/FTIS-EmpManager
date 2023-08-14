﻿using Dou.Models.DB;
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
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using FtisHelperV2.DB.Helpe;
using System.IO;
using Microsoft.Reporting.WebForms;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using Microsoft.Reporting;
using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using System.Dynamic;
using DouImp._core;
using ZXing;
using Microsoft.Ajax.Utilities;
using System.Drawing.Drawing2D;

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

            options.GetFiled("Da1s").visible = false;
            options.GetFiled("Da1s").visibleEdit = false;

            //正祥
            options.GetFiled("IsOT2V").defaultvalue = "";  //資料轉入(null)，預設'N',視為有異動

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

        protected override void DeleteDBObject(IModelEntity<F22cmmEmpData> dbEntity, IEnumerable<F22cmmEmpData> objs)
        {
            var obj = objs.FirstOrDefault();

            //DB有關聯
            ////if (obj.Da1s != null)
            ////    dbEntity.SetEntityState<F22cmmEmpDa1>(obj.Da1s, System.Data.Entity.EntityState.Deleted);
            ////if (obj.Da4s != null)
            ////    dbEntity.SetEntityState<F22cmmEmpDa4>(obj.Da4s, System.Data.Entity.EntityState.Deleted);

            //DB沒關聯
            var dbContext = FtisHelperV2.DB.Helper.CreateFtisModelContext();

            ////Employee.ResetGetAllF22cmmEmpDa1();
            ////Employee.ResetGetAllF22cmmEmpDa4();

            if (obj.Da1s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa1> da1 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(dbContext);
                da1.Delete(obj.Da1s);
                Employee.ResetGetAllF22cmmEmpDa1();
            }
            if (obj.Da4s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa4> da4 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(dbContext);
                da4.Delete(obj.Da4s);
                Employee.ResetGetAllF22cmmEmpDa4();
            }
            if (obj.Da5s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa5> da5 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(dbContext);
                da5.Delete(obj.Da5s);
                Employee.ResetGetAllF22cmmEmpDa5();
            }
            if (obj.Da6s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa6> da6 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa6>(dbContext);
                da6.Delete(obj.Da6s);
                Employee.ResetGetAllF22cmmEmpDa6();
            }
            if (obj.Da7s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa7> da7 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa7>(dbContext);
                da7.Delete(obj.Da7s);
                Employee.ResetGetAllF22cmmEmpDa7();
            }
            if (obj.Da8s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa8> da8 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa8>(dbContext);
                da8.Delete(obj.Da8s);
                Employee.ResetGetAllF22cmmEmpDa8();
            }
            if (obj.Da9s != null)
            {
                Dou.Models.DB.IModelEntity<F22cmmEmpDa9> da9 = new Dou.Models.DB.ModelEntity<F22cmmEmpDa9>(dbContext);
                da9.Delete(obj.Da9s);
                Employee.ResetGetAllF22cmmEmpDa9();
            }

            base.DeleteDBObject(dbEntity, objs);
        }

        protected override IModelEntity<F22cmmEmpData> GetModelEntity()
        {
            return new Dou.Models.DB.ModelEntity<F22cmmEmpData>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            //return new Dou.Models.DB.ModelEntity<AssetDisposals>(new DouImp.Models.DouModelContextExt());
        }

        //匯出基本資料表
        public ActionResult ExportBasicExcel(string fno)
        {
            ReportEmp rep = new ReportEmp();
            string url = rep.ExportExcel(fno);            

            if (url == "")
            {
                return Json(new { result = false, errorMessage = rep.ErrorMessage }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
            }
        }

        //匯出履歷表
        public ActionResult ExportCVExcel()
        {
            string Fno = "J11149";

            string folder = FileHelper.GetFileFolder(Code.TempUploadFile.個人員工基本資料表);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "履歷表_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".doc";
            string path = folder + fileName;

            //產出檔案
            try
            {
                ReportViewer reportViewer = new ReportViewer();
                reportViewer.ProcessingMode = ProcessingMode.Local;

                // 設定報表 iFrame Full Width
                reportViewer.SizeToReportContent = true;
                reportViewer.Width = Unit.Percentage(100);
                reportViewer.Height = Unit.Percentage(100);

                // Load Report File From Local Path
                reportViewer.LocalReport.ReportPath =
                   "Report\\RptEmpCV.rdlc";

                var datas = GetModelEntity().GetAll().Where(a => a.Fno == Fno);

                reportViewer.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet_Emp", datas)
                );

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = reportViewer.LocalReport.Render(
                   "Word", null, out mimeType, out encoding,
                    out extension,
                   out streamids, out warnings);

                FileStream fs = new FileStream(path,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();
            }
            catch (Exception ex)
            {
                string errorMessage = "匯出履歷表失敗" + ex.Message + " " + ex.StackTrace;
                return Json(new { result = false, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            string url = DouImp.Cm.PhysicalToUrl(path);

            return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
        }

        
    }
}