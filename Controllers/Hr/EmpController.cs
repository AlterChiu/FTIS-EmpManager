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
using ZXing;

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

            string fileName = "員工基本資料_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
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
                reportViewer.LocalReport.ReportPath = "Report\\EmpBasic\\Master.rdlc";

                //主表                
                DataTable dtData = GetEmpData(Fno);
                reportViewer.LocalReport.DataSources.Add(new ReportDataSource("MasterEmpData", dtData));

                // 子報表事件
                reportViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_Content_SubreportProcessing);

                //////經歷
                ////Dou.Models.DB.IModelEntity<F22cmmEmpDa5> modelDa5s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa5>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
                ////var da5s = modelDa4s.GetAll().Where(a => a.Fno == Fno).First();

                ////DataTable dt5 = new DataTable();
                //////dt5.Columns.Add(new DataColumn("xxxx"));
                ////dt5.Columns.Add(new DataColumn("部門"));
                ////dt5.Columns.Add(new DataColumn("職務"));
                ////dt5.Columns.Add(new DataColumn("起始年月"));
                ////dt5.Columns.Add(new DataColumn("結束年月"));
                ////dt5.Columns.Add(new DataColumn("年資"));
                ////dt5.Columns.Add(new DataColumn("其他備註"));

                ////DataRow dr5 = dt5.NewRow();
                //////dr5["xxxx"] = "oooooo";
                ////dr5["部門"] = "oooooo";
                ////dr5["職務"] = "oooooo";
                ////dr5["起始年月"] = "oooooo";
                ////dr5["結束年月"] = "oooooo";
                ////dr5["年資"] = "oooooo";
                ////dr5["其他備註"] = "oooooo";

                //////家庭狀況
                ////Dou.Models.DB.IModelEntity<F22cmmEmpDa6> modelDa6s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa6>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
                ////var da6s = modelDa4s.GetAll().Where(a => a.Fno == Fno).First();

                ////DataTable dt6 = new DataTable();
                //////dt6.Columns.Add(new DataColumn("xxxx"));
                ////dt6.Columns.Add(new DataColumn("稱謂"));
                ////dt6.Columns.Add(new DataColumn("姓名"));
                ////dt6.Columns.Add(new DataColumn("生日"));
                ////dt6.Columns.Add(new DataColumn("職務"));
                ////dt6.Columns.Add(new DataColumn("任職公司或在學學校"));

                ////DataRow dr6 = dt6.NewRow();
                //////dr6["xxxx"] = "oooooo";
                ////dr6["稱謂"] = "oooooo";
                ////dr6["姓名"] = "oooooo";
                ////dr6["生日"] = "oooooo";
                ////dr6["職務"] = "oooooo";
                ////dr6["任職公司或在學學校"] = "oooooo";

                //////外語檢定
                ////Dou.Models.DB.IModelEntity<F22cmmEmpDa7> modelDa7s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa7>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
                ////var da7s = modelDa4s.GetAll().Where(a => a.Fno == Fno).First();

                ////DataTable dt7 = new DataTable();
                //////dt7.Columns.Add(new DataColumn("xxxx"));
                ////dt7.Columns.Add(new DataColumn("名稱"));
                ////dt7.Columns.Add(new DataColumn("取得年月"));
                ////dt7.Columns.Add(new DataColumn("測驗成績"));

                ////DataRow dr7 = dt7.NewRow();
                //////dr7["xxxx"] = "oooooo";
                ////dr7["名稱"] = "oooooo";
                ////dr7["取得年月"] = "oooooo";
                ////dr7["測驗成績"] = "oooooo";

                //////專業資格
                ////Dou.Models.DB.IModelEntity<F22cmmEmpDa4> modelDa8s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
                ////var da8s = modelDa4s.GetAll().Where(a => a.Fno == Fno).First();

                ////DataTable dt8 = new DataTable();
                //////dt8.Columns.Add(new DataColumn("xxxx"));
                ////dt8.Columns.Add(new DataColumn("名稱"));
                ////dt8.Columns.Add(new DataColumn("取得年月"));
                ////dt8.Columns.Add(new DataColumn("證照字號"));
                ////dt8.Columns.Add(new DataColumn("xxxx"));

                ////DataRow dr8 = dt8.NewRow();
                //////dr8["xxxx"] = "oooooo";
                ////dr8["名稱"] = "oooooo";
                ////dr8["取得年月"] = "oooooo";
                ////dr8["證照字號"] = "oooooo";

                //-----------------------------

                Microsoft.Reporting.WebForms.Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;
                
                byte[] bytes = reportViewer.LocalReport.Render(
                   "EXCELOPENXML", null, out mimeType, out encoding,
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

        //繫結子報表
        private void LocalReport_Content_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            string Fno = "J11149";
           
            if (e.ReportPath == "Sub1Data")
            {
                //主表
                DataTable dt = GetEmpData(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub1SourceData", dt));
            }
            else if (e.ReportPath == "Sub2Da4s")
            {
                //學歷
                DataTable dtDa4s = GetDa4s(Fno);
                e.DataSources.Add(new Microsoft.Reporting.WebForms.ReportDataSource("Sub2SourceDa4s", dtDa4s));
            }
        }

        //主表
        private DataTable GetEmpData(string Fno)
        {
            Dou.Models.DB.IModelEntity<F22cmmEmpDa1> modelDa1s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa1>(FtisHelperV2.DB.Helper.CreateFtisModelContext());

            var data = GetModelEntity().GetAll().Where(a => a.Fno == Fno).First();
            var da1s = modelDa1s.GetAll().Where(a => a.Fno == Fno).First();

            DataTable dt = new DataTable();
            //dt.Columns.Add(new DataColumn("xxxx"));
            dt.Columns.Add(new DataColumn("姓名中"));
            dt.Columns.Add(new DataColumn("姓名英"));
            dt.Columns.Add(new DataColumn("部門"));
            dt.Columns.Add(new DataColumn("職稱"));
            dt.Columns.Add(new DataColumn("到職日期"));
            dt.Columns.Add(new DataColumn("出生日期"));
            dt.Columns.Add(new DataColumn("性別"));
            dt.Columns.Add(new DataColumn("出生地"));
            dt.Columns.Add(new DataColumn("身分證字號"));
            dt.Columns.Add(new DataColumn("婚姻"));
            dt.Columns.Add(new DataColumn("身高"));
            dt.Columns.Add(new DataColumn("體重"));
            dt.Columns.Add(new DataColumn("血型"));
            dt.Columns.Add(new DataColumn("戶籍地址"));
            dt.Columns.Add(new DataColumn("戶籍電話"));
            dt.Columns.Add(new DataColumn("通訊地址"));
            dt.Columns.Add(new DataColumn("住家電話"));
            dt.Columns.Add(new DataColumn("行動電話"));
            dt.Columns.Add(new DataColumn("Email"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1姓名"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1關係"));
            dt.Columns.Add(new DataColumn("緊急聯絡人1電話"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2姓名"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2關係"));
            dt.Columns.Add(new DataColumn("緊急聯絡人2電話"));

            DataRow dr = dt.NewRow();
            //dr["xxxx"] = "oooooo";
            dr["姓名中"] = data.Name;
            dr["姓名英"] = data.En_Name;
            dr["部門"] = FtisHelperV2.DB.Helpe.Department.GetDepartment(data.DCode).DName;
            dr["職稱"] = "無?"; //FtisHelperV2.DB.Helper.GetEmployeeTitle(data.TCode).Title;
            dr["到職日期"] = DateFormat.ToDate4(data.AD);
            dr["出生日期"] = DateFormat.ToDate4((DateTime)da1s.da03);
            dr["性別"] = data.Sex;
            dr["出生地"] = da1s.da04;
            dr["身分證字號"] = da1s.da05;
            dr["婚姻"] = "無?";
            dr["身高"] = da1s.da06;
            dr["體重"] = da1s.da07;
            dr["血型"] = da1s.da08;
            dr["戶籍地址"] = da1s.da10;
            dr["戶籍電話"] = da1s.da11;
            dr["通訊地址"] = da1s.da13;
            dr["住家電話"] = da1s.da14;
            dr["行動電話"] = da1s.da15;
            dr["Email"] = data.EMail;
            dr["緊急聯絡人1姓名"] = da1s.da17;
            dr["緊急聯絡人1關係"] = da1s.da18;
            dr["緊急聯絡人1電話"] = da1s.da19;
            dr["緊急聯絡人2姓名"] = da1s.da20;
            dr["緊急聯絡人2關係"] = da1s.da21;
            dr["緊急聯絡人2電話"] = da1s.da22;
            dt.Rows.Add(dr);

            return dt;
        }

        //學歷
        private DataTable GetDa4s(string Fno)
        {            
            Dou.Models.DB.IModelEntity<F22cmmEmpDa4> modelDa4s = new Dou.Models.DB.ModelEntity<F22cmmEmpDa4>(FtisHelperV2.DB.Helper.CreateFtisModelContext());
            var da4s = modelDa4s.GetAll().Where(a => a.Fno == Fno);

            DataTable dt = new DataTable();
            //dt4.Columns.Add(new DataColumn("xxxx"));
            dt.Columns.Add(new DataColumn("學校"));
            dt.Columns.Add(new DataColumn("學院"));
            dt.Columns.Add(new DataColumn("科系"));
            dt.Columns.Add(new DataColumn("入學年月"));
            dt.Columns.Add(new DataColumn("畢業年月"));
            dt.Columns.Add(new DataColumn("學位"));
            dt.Columns.Add(new DataColumn("指導教授"));

            foreach (var v in da4s)
            {
                DataRow dr = dt.NewRow();
                //dr4["xxxx"] = "oooooo";
                dr["學校"] = v.da401;
                dr["學院"] = v.da402;
                dr["科系"] = v.da403;
                dr["入學年月"] = v.da404;
                dr["畢業年月"] = v.da405;
                dr["學位"] = v.da406;
                dr["指導教授"] = v.da407;
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }
}