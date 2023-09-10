using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaterialRecord
{
    public partial class scanqrcode : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public string checkID(string id, string factory)
        {
            string message = "";
            try
            {                
                MaterialSummaryEntity entity = new MaterialSummaryEntity();
                entity.ID = id;
                DataTable dt = getSummaryDT(entity, "");
                if (dt.Rows.Count <= 0)
                {
                    message = "編號(" + id + ")不存在";
                }
                else if (!dt.Rows[0]["MaterialState"].Equals("Y"))
                {
                    message = "狀態非使用中";
                }
                else if (dt.Rows[0]["Factory"].Equals(factory))
                {
                    if (factory.Equals("I"))
                    {
                        message = "已於廠內，不可執行返廠";
                    }
                    else if (factory.Equals("O"))
                    {
                        message = "已於廠外，不可執行出廠";
                    }
                    else
                    {
                        message = "無法取得作業代號，請聯絡管理員";
                    }
                }
                else
                {
                    update(id, factory);
                }
            }
            catch (Exception ex)
            {
                message = ex.ToString();
            }            
            return message;
        }

        public void update(string id, string factory)
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();
            MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
            MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();

            string GUID = System.Guid.NewGuid().ToString(); ;
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string userID = HttpContext.Current.Session["MaterialRecord_UserID"] == null ? "admin" : HttpContext.Current.Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string RefNumber = "";
            string Remark = "刷讀自動出入廠";
            string StartUseDate = sdatetime.Replace("-", "").Substring(0, 8);

            historyEntity.GUID = GUID;
            historyEntity.Identifier = Identifier;
            historyEntity.DoAction = factory;
            historyEntity.ID = id;
            historyEntity.Factory = factory;
            historyEntity.RefNumber = RefNumber;
            historyEntity.Remark = Remark;
            historyEntity.CreateEmpID = userID;
            historyEntity.CreateName = userName;
            historyEntity.CreateTime = sdatetime;
            historyEntity.UpdateEmpID = userID;
            historyEntity.UpdateName = userName;
            historyEntity.UpdateTime = sdatetime;
            historyList.Add(historyEntity);

            summaryEntity.Factory = factory;
            summaryEntity.StartUseDate = StartUseDate;
            summaryEntity.RefNumber = RefNumber;
            summaryEntity.Remark = Remark;
            summaryEntity.UpdateEmpID = userID;
            summaryEntity.UpdateName = userName;
            summaryEntity.UpdateTime = sdatetime;
            summaryEntity.ID = id;

            WasLogEntity logEntity = setLogData("scanqrcode-update", "scanqrcode", "自動出入廠更新", "卡號 " + userID + " 失敗：", userID, "F");
            if (factory.Equals("O"))
            {
                updateSummaryOutFactory(factory, summaryEntity, historyEntity, logEntity);
            }
            else if (factory.Equals("I"))
            {
                updateSummaryInFactory(factory, summaryEntity, historyEntity, logEntity);
            }
        }
    }
}