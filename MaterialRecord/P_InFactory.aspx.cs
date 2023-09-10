using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaterialRecord
{
    public partial class P_InFactory : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            //string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setPackageHistory();

                // https或localhost才顯示掃描按鈕
                btn_code.Visible = (Request.Url.Scheme == "https" || Request.Url.Host == "localhost") ? true : false;
            }

            if (Request.Form["fanum"] != null) //QRCode內容
            {
                if (Request.Form["fanum"].ToString().Trim().Length > 0)
                {
                    txt_condition.Text = Request.Form["fanum"].ToString().Trim();
                }
            }
            txt_condition.Focus();
        }

        protected void setPackageHistory()
        {
            string condition = hf_id.Value;
            DataTable dt = getHistoryDT(condition);
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
            Dictionary<string, string> actionDictionary = getOptionDictionary("DoAction");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["Factory"] = factoryDictionary[dt.Rows[i]["Factory"].ToString()];
                dt.Rows[i]["MaterialState"] = MaterialStateDictionary[dt.Rows[i]["MaterialState"].ToString()];
                dt.Rows[i]["DoAction"] = actionDictionary[dt.Rows[i]["DoAction"].ToString()];
            }
            
            ListView1.DataSource = dt;
            ListView1.DataBind();
            if (condition.Length > 0)
            {
                if (dt.Rows.Count == 0)
                {
                    Label label = ListView1.Controls[0].FindControl("lbl_msg") as Label;
                    label.Text = "無此包裝材編號，請確認輸入編號是否正確";
                }
                else
                {
                    dt = getSummaryDT(new MaterialSummaryEntity(), condition);
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["Factory"].Equals("I"))
                        {
                            btn_inFactory.Visible = false;
                        }
                        else
                        {
                            btn_inFactory.Visible = true;
                        }
                    }
                }
            }
        }

        protected void btn_query_Click(object sender, EventArgs e)
        {
            hf_id.Value = txt_condition.Text;
            setPackageHistory();
            txt_condition.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "autoFactory();", true);
        }

        protected void btn_batchEdit_Click(object sender, EventArgs e)
        {
            ArrayList idList = checkDataToArrayList();
            if (idList.Count == 0)
            {
                lbl_hint.Text = "未勾選資料";
                btn_hintConfirm.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
            }
            else
            {
                setModal();
            }
        }

        protected ArrayList checkDataToArrayList()
        {
            ArrayList idList = new ArrayList();
            foreach (ListViewItem tempItem in ListView1.Items)
            {
                CheckBox cb_row = tempItem.FindControl("chk_row") as CheckBox;
                Label lbl_id = tempItem.FindControl("IDLabel") as Label;
                if (cb_row.Checked)
                {
                    idList.Add(lbl_id.Text);
                }
            }
            return idList;
        }
        
        protected void setModal()
        {
            MaterialSummaryEntity entity = new MaterialSummaryEntity();
            entity.ID = hf_id.Value;
            DataTable dt = getSummaryDT(entity, "");
            if (hf_id.Value.Trim().Length <= 0 || dt.Rows.Count <= 0)
            {
                lbl_hintModalTitle.Text = "提示";
                lbl_hint.Text = "請輸入查詢編號或掃描條碼";
                btn_hintConfirm.Visible = false;
                btn_hintClose.Text = "關閉";
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                return;
            }
            else
            {
                if (!dt.Rows[0]["MaterialState"].Equals("Y"))
                {
                    lbl_hintModalTitle.Text = "提示";
                    lbl_hint.Text = hf_id.Value + " 已報廢";
                    btn_hintConfirm.Visible = false;
                    btn_hintClose.Text = "關閉";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                    return;
                }
            }
            string state = txt_hintState.Text;
            txt_Remark.Text = "";
            btn_hintConfirm.Visible = true;
            switch (state) 
            {
                case "I":
                    Boolean factory = false;
                    if (dt.Rows.Count > 0)
                    {
                        if (!dt.Rows[0]["Factory"].Equals(state))
                        {
                            factory = true;
                        }
                    }
                    if (factory)
                    {
                        if (hf_mode.Value.Equals("true"))
                        {
                            update(state);
                            return;
                        }
                        else
                        {
                            lbl_hintModalTitle.Text = "返廠";
                            lbl_hint.Text = "確定返廠？";
                        }
                    }
                    else
                    {
                        lbl_hintModalTitle.Text = "返廠";
                        lbl_hint.Text = hf_id.Value + " 已於廠內，不可執行返廠";
                        btn_hintConfirm.Visible = false;
                        btn_hintClose.Text = "關閉";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                        return;
                    }
                    break;
                case "N":
                    lbl_hintModalTitle.Text = "報廢";
                    lbl_hint.Text = "確定報廢？";
                    break;
                default:
                    break;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHintModal();", true);
        }

        protected void btn_hintConfirm_Click(object sender, EventArgs e)
        {
            update(txt_hintState.Text);
        }

        protected void lbtn_inFactory_Command(object sender, CommandEventArgs e)
        {
            txt_hintData.Text = e.CommandArgument.ToString();
            setModal();
        }

        protected void btn_scrap_Click(object sender, EventArgs e)
        {
            txt_hintState.Text = "N";
            setModal();
        }

        protected void update(string state)
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();
            MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
            MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();

            string GUID = System.Guid.NewGuid().ToString(); ;
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            switch (state)
            {
                case "I":
                    summaryEntity.Factory = state;
                    summaryEntity.StartUseDate = sdatetime.Replace("-", "").Substring(0, 8);
                    break;
                case "Y":
                    summaryEntity.MaterialState = state;
                    summaryEntity.ScrapDate = "";
                    break;
                case "N":
                    summaryEntity.MaterialState = state;
                    summaryEntity.ScrapDate = sdatetime.Replace("-", "").Substring(0, 8);
                    break;
                default:
                    break;
            }
            summaryEntity.Remark = txt_Remark.Text;
            summaryEntity.UpdateEmpID = userID;
            summaryEntity.UpdateName = userName;
            summaryEntity.UpdateTime = sdatetime;
            summaryEntity.ID = hf_id.Value;

            historyEntity.GUID = GUID;
            historyEntity.Identifier = Identifier;
            historyEntity.DoAction = state;
            historyEntity.ID = hf_id.Value;
            if (state.Equals("I"))
            {
                historyEntity.Factory = state;
            }
            else
            {
                historyEntity.MaterialState = state;
            }
            historyEntity.Remark = txt_Remark.Text;
            historyEntity.CreateEmpID = userID;
            historyEntity.CreateName = userName;
            historyEntity.CreateTime = sdatetime;
            historyEntity.UpdateEmpID = userID;
            historyEntity.UpdateName = userName;
            historyEntity.UpdateTime = sdatetime;
            historyList.Add(historyEntity);

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            WasLogEntity logEntity = setLogData(pageName + "-update", pageName, "返廠更新", "卡號 " + userID + " 失敗：", userID, "F");
            updateSummaryInFactory(state, summaryEntity, historyEntity, logEntity);

            setPackageHistory();
        }

        protected void btn_inFactory_Click(object sender, EventArgs e)
        {
            txt_hintState.Text = "I";
            setModal();
        }

        protected void btn_code_Click(object sender, EventArgs e)
        {
            string param = addQuery();
            if (param != "")
            {
                Server.Transfer("scanqrcode.aspx?" + param);//導到掃描頁
            }
            else
            {
                Server.Transfer("scanqrcode.aspx");//導到掃描頁
            }
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setPackageHistory();
        }
    }
}