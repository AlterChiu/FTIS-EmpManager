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
using System.Data;
using DouImp._core;

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
        public ActionResult ExportBasicExcel()
        {
            string Fno = "J11149";

            string folder = FileHelper.GetFileFolder(Code.TempUploadFile.個人員工基本資料表);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            string fileName = "員工基本資料_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls";
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
                   "Report\\RptEmpBasic.rdlc";

                var datas = GetModelEntity().GetAll().Where(a => a.Fno == Fno).First();

                //主表                
                DataTable dtData = new DataTable();
                //dtData.Columns.Add(new DataColumn("xxxx"));
                dtData.Columns.Add(new DataColumn("姓名中"));
                dtData.Columns.Add(new DataColumn("姓名英"));
                dtData.Columns.Add(new DataColumn("部門"));
                dtData.Columns.Add(new DataColumn("職稱"));
                dtData.Columns.Add(new DataColumn("到職日期"));
                dtData.Columns.Add(new DataColumn("出生日期"));
                dtData.Columns.Add(new DataColumn("性別"));
                dtData.Columns.Add(new DataColumn("出生地"));
                dtData.Columns.Add(new DataColumn("身分證字號"));
                dtData.Columns.Add(new DataColumn("婚姻"));
                dtData.Columns.Add(new DataColumn("身高"));
                dtData.Columns.Add(new DataColumn("體重"));
                dtData.Columns.Add(new DataColumn("血型"));
                dtData.Columns.Add(new DataColumn("戶籍地址"));
                dtData.Columns.Add(new DataColumn("戶籍電話"));
                dtData.Columns.Add(new DataColumn("通訊地址"));
                dtData.Columns.Add(new DataColumn("住家電話"));
                dtData.Columns.Add(new DataColumn("行動電話"));
                dtData.Columns.Add(new DataColumn("Email"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人1姓名"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人1關係"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人1電話"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人2姓名"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人2關係"));
                dtData.Columns.Add(new DataColumn("緊急聯絡人2電話"));

                DataRow dr = dtData.NewRow();
                //dr["xxxx"] = "oooooo";
                dr["姓名中"] = datas.Name;
                dr["姓名英"] = datas.En_Name;
                dr["部門"] = FtisHelperV2.DB.Helpe.Department.GetDepartment(datas.DCode).DName;
                dr["職稱"] = "ooo職稱"; //FtisHelperV2.DB.Helper.GetEmployeeTitle(datas.TCode).Title;
                dr["到職日期"] = DateFormat.ToDate4(datas.AD);
                dr["出生日期"] = "ooo出生日期";
                dr["性別"] = "ooo性別";
                dr["出生地"] = "ooo出生地";
                dr["身分證字號"] = "ooo身分證字號";
                dr["婚姻"] = "ooo婚姻";
                dr["身高"] = "ooo身高";
                dr["體重"] = "ooo體重";
                dr["血型"] = "ooo血型";
                dr["戶籍地址"] = "ooo戶籍地址";
                dr["戶籍電話"] = "ooo戶籍電話";
                dr["通訊地址"] = "ooo通訊地址";
                dr["住家電話"] = "ooo住家電話";
                dr["行動電話"] = "ooo行動電話";
                dr["Email"] = "oooEmail";
                dr["緊急聯絡人1姓名"] = "ooo緊急聯絡人1姓名";
                dr["緊急聯絡人1關係"] = "ooo緊急聯絡人1關係";
                dr["緊急聯絡人1電話"] = "ooo緊急聯絡人1電話";
                dr["緊急聯絡人2姓名"] = "ooo緊急聯絡人2姓名";
                dr["緊急聯絡人2關係"] = "ooo緊急聯絡人2關係";
                dr["緊急聯絡人2電話"] = "ooo緊急聯絡人2電話";
                dtData.Rows.Add(dr);

                reportViewer.LocalReport.DataSources.Add(
                    new ReportDataSource("DataSet_Emp", dtData)
                );

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;
                
                byte[] bytes = reportViewer.LocalReport.Render(
                   "Excel", null, out mimeType, out encoding,
                    out extension,
                   out streamids, out warnings);

                FileStream fs = new FileStream(path,
                   FileMode.Create);
                fs.Write(bytes, 0, bytes.Length);
                fs.Close();               
            }
            catch(Exception ex)
            {
                string errorMessage = "匯出員工基本資料失敗" + ex.Message + " " + ex.StackTrace;
                return Json(new { result = false, errorMessage = errorMessage }, JsonRequestBehavior.AllowGet);
            }

            string url = DouImp.Cm.PhysicalToUrl(path);

            return Json(new { result = true, url = url }, JsonRequestBehavior.AllowGet);
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