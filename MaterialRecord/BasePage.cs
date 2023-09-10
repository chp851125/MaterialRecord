using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.IO;

namespace MaterialRecord
{
    public class BasePage : System.Web.UI.Page
    {
        Query query = new Query();
        Modify modify = new Modify();
        public static string TEST = System.Web.Configuration.WebConfigurationManager.AppSettings["TEST"].Trim().ToUpper();

        protected void InitializeComponent()
        {
            this.Load += new System.EventHandler(this.Page_Load);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeComponent();
        }

        protected virtual void Page_Load(object sender, System.EventArgs e)
        {
        }

        protected string getUrlParameter()
        {
            string url = Request.Url.ToString().Trim();
            string parameter = "";

            if (url.IndexOf("?") >= 0)
            {
                parameter = url.Trim().Substring(url.IndexOf("?"), url.Length - url.IndexOf("?"));
            }

            return parameter.Trim();
        }

        protected Boolean isPathExist(string path)
        {
            bool rtn = true;
            DirectoryInfo dir = new DirectoryInfo(path);
            if (!dir.Exists)
            {
                rtn = false;
            }
            return rtn;
        }

        public Dictionary<string, string> getOptionDictionary(string field)
        {
            Dictionary<string, string> optionDictionary = new Dictionary<string, string>();
            switch (field)
            {
                case "MaterialType":
                    DataTable dt = new DataTable();
                    dt = getMaterialTypeDT("");
                    if (dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            optionDictionary.Add(dt.Rows[i]["ID"].ToString(), dt.Rows[i]["TypeName"].ToString());
                        }
                    }
                    break;
                case "Factory":
                    optionDictionary.Add("I", "廠內");
                    optionDictionary.Add("O", "廠外");
                    break;
                case "MaterialState":
                    optionDictionary.Add("Y", "使用中");
                    optionDictionary.Add("N", "報廢");
                    optionDictionary.Add("X", "失效");
                    optionDictionary.Add("D", "刪除");
                    break;
                case "DoAction":
                    optionDictionary.Add("A", "建檔");
                    optionDictionary.Add("B", "批次建檔");
                    optionDictionary.Add("U", "修改");
                    optionDictionary.Add("D", "刪除");
                    optionDictionary.Add("X", "失效");
                    optionDictionary.Add("N", "報廢");
                    optionDictionary.Add("Y", "取消報廢");
                    optionDictionary.Add("R", "可使用次數維護");
                    optionDictionary.Add("I", "返廠");
                    optionDictionary.Add("O", "出廠");
                    optionDictionary.Add("C", "取消出廠");
                    break;
                case "DateType":
                    optionDictionary.Add("CreateDate", "建立日期");
                    optionDictionary.Add("StartUseDate", "開始使用日");
                    break;
                default:
                    break;
            }

            return optionDictionary;
        }

        public DataTable summaryToDataTable(List<MaterialSummaryEntity> listEntity)
        {
            DataTable dt = new DataTable();
            dt.TableName = "summary";
            DataRow row;
            string[] columns = { "MaterialType", "ID", "MaterialName", "UseCount", "Factory",
                                 "MaterialState", "CreateDate", "StartUseDate", "ScrapDate", "RefNumber",
                                 "CancelFactory", "Remark", "CreateEmpID", "CreateName", "CreateTime",
                                 "UpdateEmpID", "UpdateName", "UpdateTime" };
            for (int i = 0; i < columns.Length; i++)
            {
                dt.Columns.Add(columns[i]);
            }

            for (int i = 0; i < listEntity.Count; i++)
            {
                row = dt.NewRow();
                row["MaterialType"] = listEntity.ElementAt(i).MaterialType;
                row["ID"] = listEntity.ElementAt(i).ID;
                row["MaterialName"] = listEntity.ElementAt(i).MaterialName;
                row["UseCount"] = listEntity.ElementAt(i).UseCount;
                row["Factory"] = listEntity.ElementAt(i).Factory;
                row["MaterialState"] = listEntity.ElementAt(i).MaterialState;
                row["CreateDate"] = listEntity.ElementAt(i).CreateDate;
                row["StartUseDate"] = listEntity.ElementAt(i).StartUseDate;
                row["ScrapDate"] = listEntity.ElementAt(i).ScrapDate;
                row["RefNumber"] = listEntity.ElementAt(i).RefNumber;
                row["CancelFactory"] = listEntity.ElementAt(i).CancelFactory;
                row["Remark"] = listEntity.ElementAt(i).Remark;
                row["CreateEmpID"] = listEntity.ElementAt(i).CreateEmpID;
                row["CreateName"] = listEntity.ElementAt(i).CreateName;
                row["CreateTime"] = listEntity.ElementAt(i).CreateTime;
                row["UpdateEmpID"] = listEntity.ElementAt(i).UpdateEmpID;
                row["UpdateName"] = listEntity.ElementAt(i).UpdateName;
                row["UpdateTime"] = listEntity.ElementAt(i).UpdateTime;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public DataTable historyToDataTable(List<MaterialHistoryEntity> listEntity)
        {
            DataTable dt = new DataTable();
            dt.TableName = "history";
            DataRow row;
            string[] columns = { "GUID", "Identifier", "DoAction", "MaterialType", "ID",
                                 "MaterialName", "UseCount", "EditCount", "Factory", "MaterialState",
                                 "RefNumber", "Remark", "CreateEmpID", "CreateName", "CreateTime",
                                 "UpdateEmpID", "UpdateName", "UpdateTime" };
            for (int i = 0; i < columns.Length; i++)
            {
                dt.Columns.Add(columns[i]);
            }

            for (int i = 0; i < listEntity.Count; i++)
            {
                row = dt.NewRow();
                row["GUID"] = listEntity.ElementAt(i).GUID;
                row["Identifier"] = listEntity.ElementAt(i).Identifier;
                row["DoAction"] = listEntity.ElementAt(i).DoAction;
                row["MaterialType"] = listEntity.ElementAt(i).MaterialType;
                row["ID"] = listEntity.ElementAt(i).ID;
                row["MaterialName"] = listEntity.ElementAt(i).MaterialName;
                row["UseCount"] = listEntity.ElementAt(i).UseCount;
                row["EditCount"] = listEntity.ElementAt(i).EditCount;
                row["Factory"] = listEntity.ElementAt(i).Factory;
                row["MaterialState"] = listEntity.ElementAt(i).MaterialState;
                row["RefNumber"] = listEntity.ElementAt(i).RefNumber;
                row["Remark"] = listEntity.ElementAt(i).Remark;
                row["CreateEmpID"] = listEntity.ElementAt(i).CreateEmpID;
                row["CreateName"] = listEntity.ElementAt(i).CreateName;
                row["CreateTime"] = listEntity.ElementAt(i).CreateTime;
                row["UpdateEmpID"] = listEntity.ElementAt(i).UpdateEmpID;
                row["UpdateName"] = listEntity.ElementAt(i).UpdateName;
                row["UpdateTime"] = listEntity.ElementAt(i).UpdateTime;
                dt.Rows.Add(row);
            }
            return dt;
        }

        public string[] listToArray(List<string> list)
        {
            string[] columns = new string[list.Count];
            for (int i = 0; i < list.Count; i++)
            {
                columns[i] = list.ElementAt(i);
            }
            return columns;
        }

        public DataTable getSummaryDT(MaterialSummaryEntity entity, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getSummary(entity, condition);
            }
            catch (Exception ex)
            {
                string userID = HttpContext.Current.Session["MaterialRecord_UserID"] == null ? "" : HttpContext.Current.Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getSummaryDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public DataTable getPrintIdDT(string startid, string endid, string MaterialState)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getPrintId(startid, endid, MaterialState);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getPrintIdDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public string getPackageID(string MaterialType)
        {
            string ID = "";
            try
            {
                ID = query.getPackageID(MaterialType);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getPackageID", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return ID;
        }

        public DataTable getMaterialTypeDT(string ID)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getMaterialType(ID);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getMaterialTypeDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public DataTable getHistoryDT(string ID)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getHistory(ID);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getHistoryDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public int getMaterialTypeCount(string MaterialType)
        {
            int count = -1;
            try
            {
                count = query.getMaterialTypeCount(MaterialType);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getMaterialTypeCount", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return count;
        }

        public DataTable getStatisticsCountDT(MaterialSummaryEntity entity, string startDate, string endDate)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getStatisticsCount(entity, startDate, endDate);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getStatisticsCountDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public DataTable getStatisticsRecordDT(MaterialSummaryEntity entity, string dateColumn, string startDate, string endDate, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getStatisticsRecord(entity, dateColumn, startDate, endDate, condition);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getStatisticsRecordDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public DataTable getStatisticsUsedDT(MaterialSummaryEntity entity, string dateColumn, string startDate, string endDate, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                dt = query.getStatisticsUsed(entity, dateColumn, startDate, endDate, condition);
            }
            catch (Exception ex)
            {
                string userID = Session["MaterialRecord_UserID"] == null ? "" : Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getStatisticsUsedDT", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return dt;
        }

        public string getName(string ID)
        {
            string name = ID;
            try
            {
                //name = query.getPersonName(ID);
                name = "admin";
            }
            catch (Exception ex)
            {
                string userID = HttpContext.Current.Session["MaterialRecord_UserID"] == null ? "" : HttpContext.Current.Session["MaterialRecord_UserID"].ToString();
                WasLogEntity logEntity = setLogData("BasePage-getName", "BasePage", "查詢", "查詢失敗：" + ex.ToString(), userID, "F");
                insertToLog(logEntity);
            }
            return name;
        }

        public Boolean insertSummary(DataTable summaryList, DataTable historyList, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.insertSummary(summaryList, historyList);
            }
            catch (Exception ex)
            {
                //Response.Write(ex);
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateSummary(string[] summaryColumns, DataTable summaryList, DataTable historyList, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateSummary(summaryColumns, summaryList, historyList);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean insertMaterialType(MaterialTypeEntity entity, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.insertMaterialType(entity);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateMaterialType(MaterialTypeEntity entity, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateMaterialType(entity);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean deleteMaterialType(string ID, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.deleteMaterialType(ID);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateSummaryInFactory(string state, MaterialSummaryEntity summaryEntity, MaterialHistoryEntity historyEntity, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateSummaryInFactory(state, summaryEntity, historyEntity);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateSummaryOutFactory(string state, MaterialSummaryEntity summaryEntity, MaterialHistoryEntity historyEntity, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateSummaryOutFactory(state, summaryEntity, historyEntity);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateSummaryRecycling(DataTable summaryList, DataTable historyList, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateSummaryRecycling(summaryList, historyList);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean updateSummaryScrap(DataTable summaryList, DataTable historyList, WasLogEntity logEntity)
        {
            Boolean success = false;
            try
            {
                success = modify.updateSummaryScrap(summaryList, historyList);
            }
            catch (Exception ex)
            {
                logEntity.ExeMsg = logEntity.ExeMsg + ex.ToString();
                insertToLog(logEntity);
            }
            return success;
        }

        public Boolean insertToLog(WasLogEntity entity)
        {
            Boolean success = false;
            try
            {
                success = modify.insertToLog(entity);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            return success;
        }

        public static WasLogEntity setLogData(string exeMethod, string exeScreen, string exeButton, string exeMsg, string empId, string status)
        {
            WasLogEntity entity = new WasLogEntity();
            entity.ExeMethod = exeMethod.Trim();
            entity.ExeScreen = exeScreen.Trim();
            entity.ExeButton = exeButton.Trim();
            entity.ExeMsg = exeMsg.Trim();
            entity.Memo = "";
            entity.UpdateEmp = empId.Trim();
            entity.UpdateDate = DateTime.Now.ToString("yyyyMMdd").Trim();
            entity.UpdateTime = DateTime.Now.ToString("HH:mm:ss").Trim();
            entity.Status = status.Trim();
            return entity;
        }

        protected internal static string addQuery()
        {
            string str = HttpContext.Current.Request.QueryString.ToString();
            string[] parmlist = str.Split('&');
            string URLpara = "";
            foreach (string param in parmlist)
            {
                if (param.IndexOf("platform") > -1 || param.IndexOf("embed_type") > -1 || param.IndexOf("Lang") > -1)
                {
                    if (URLpara != "")
                        URLpara += "&";
                    URLpara += param;
                }
            }
            return URLpara;
        }
    }
}