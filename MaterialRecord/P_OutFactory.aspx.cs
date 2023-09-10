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
    public partial class P_OutFactory : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setPackageHistory();

                // https或localhost才顯示掃描按鈕
                btn_code.Visible = (Request.Url.Scheme == "https" || Request.Url.Host == "localhost") ? true : false;
            }

            if (Request.Form["fanum"] != null)//post取法
            {//QRCode內容
                if (Request.Form["fanum"].ToString().Trim().Length > 0)
                {
                    txt_condition.Text = Request.Form["fanum"].ToString().Trim();
                    //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "autoQuery();", true);
                }
            }
            txt_condition.Focus();
        }

        protected void setPackageHistory()
        {
            string condition = hf_id.Value;
            DataTable dt = getHistoryDT(condition);
            //Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
            Dictionary<string, string> actionDictionary = getOptionDictionary("DoAction");            

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //dt.Rows[i]["MaterialType"] = MaterialTypeDictionary[dt.Rows[i]["MaterialType"].ToString()];
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
                        if (dt.Rows[0]["Factory"].Equals("O"))
                        {
                            btn_outFactory.Visible = false;
                        }
                        else
                        {
                            btn_outFactory.Visible = true;
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
                case "O":
                    Boolean factory = false;
                    if (dt.Rows.Count > 0)
                    {
                        if (!dt.Rows[0]["Factory"].Equals(state))
                        {
                            factory = true;
                        }
                        else if (Convert.ToInt32(dt.Rows[0]["UseCount"]) <= 0)
                        {
                            lbl_hint.Text = hf_id.Value + " 可使用次數為0，不可執行出廠";
                            btn_hintConfirm.Visible = false;
                            btn_hintClose.Text = "關閉";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                            return;
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

                            lbl_hintModalTitle.Text = "出廠";
                            lbl_hint.Text = "確定出廠？";
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideRecyclingDiv();OpenHintModal();", true);
                            return;
                        }
                    }
                    else
                    {
                        lbl_hintModalTitle.Text = "返廠";
                        lbl_hint.Text = hf_id.Value + " 已於廠外，不可執行出廠";
                        btn_hintConfirm.Visible = false;
                        btn_hintClose.Text = "關閉";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                        return;
                    }
                    //break;
                case "N":
                    lbl_hintModalTitle.Text = "報廢";
                    lbl_hint.Text = "確定報廢？";
                    break;
                case "R":
                    lbl_hintModalTitle.Text = "可使用次數維護";
                    lbl_hint.Text = "";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideRefNumberDiv();OpenHintModal();", true);
                    return;
                case "C":
                    lbl_hintModalTitle.Text = "取消出廠";
                    factory = false;
                    if (dt.Rows.Count > 0)
                    {
                        if (dt.Rows[0]["Factory"].Equals("O"))
                        {
                            factory = true;
                        }
                    }
                    if (factory)
                    {
                        lbl_hint.Text = "確定取消出廠？";
                    }
                    else
                    {
                        lbl_hint.Text = hf_id.Value + " 已於廠內，不可執行取消出廠";
                        btn_hintConfirm.Visible = false;
                        btn_hintClose.Text = "關閉";
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                        return;
                    }
                    break;
                default:
                    break;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideRecyclingDiv();hideRefNumberDiv();OpenHintModal();", true);
        }

        protected void btn_hintConfirm_Click(object sender, EventArgs e)
        {
            update(txt_hintState.Text);
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
            //int UseCount = Convert.ToInt32(txt_UseCount.Text);
            string RefNumber = txt_RefNumber.Text;
            string Remark = txt_Remark.Text;
            string StartUseDate = sdatetime.Replace("-", "").Substring(0, 8);

            historyEntity.GUID = GUID;
            historyEntity.Identifier = Identifier;
            historyEntity.DoAction = state;
            historyEntity.ID = hf_id.Value;
            switch (state)
            {
                case "R":
                    historyEntity.EditCount = Convert.ToInt32(txt_UseCount.Text); ;
                    break;
                case "O":
                    historyEntity.Factory = state;
                    break;
                case "C":
                    historyEntity.Factory = "I";
                    break;
                default:
                    historyEntity.MaterialState = state;
                    break;
            }
            historyEntity.RefNumber = RefNumber;
            historyEntity.Remark = Remark;
            historyEntity.CreateEmpID = userID;
            historyEntity.CreateName = userName;
            historyEntity.CreateTime = sdatetime;
            historyEntity.UpdateEmpID = userID;
            historyEntity.UpdateName = userName;
            historyEntity.UpdateTime = sdatetime;
            historyList.Add(historyEntity);

            switch (state)
            {
                case "O":
                    summaryEntity.Factory = state;
                    summaryEntity.StartUseDate = StartUseDate;
                    break;
                case "Y":
                    summaryEntity.MaterialState = state;
                    summaryEntity.ScrapDate = "";
                    break;
                case "N":
                    summaryEntity.MaterialState = state;
                    summaryEntity.ScrapDate = StartUseDate;
                    break;
                case "R":
                    summaryEntity.UseCount = Convert.ToInt32(txt_UseCount.Text);;
                    summaryEntity.StartUseDate = StartUseDate;
                    break;
                case "C":
                    summaryEntity.Factory = "I";
                    break;
            }
            summaryEntity.RefNumber = RefNumber;
            summaryEntity.Remark = Remark;
            summaryEntity.UpdateEmpID = userID;
            summaryEntity.UpdateName = userName;
            summaryEntity.UpdateTime = sdatetime;
            summaryEntity.ID = hf_id.Value;

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            WasLogEntity logEntity = setLogData(pageName + "-update", pageName, "出廠更新", "卡號 " + userID + " 失敗：", userID, "F");
            updateSummaryOutFactory(state, summaryEntity, historyEntity, logEntity);

            setPackageHistory();
        }
        protected void btn_outFactory_Click(object sender, EventArgs e)
        {
            txt_hintState.Text = "O";
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

        protected void btn_edit_Click(object sender, EventArgs e)
        {
            txt_hintState.Text = "R";
            setModal();
        }

        public bool IsNumeric(String strNumber)
        {
            Regex NumberPattern = new Regex("^\\d+$"); //非負整數（正整數 + 0）
            return NumberPattern.IsMatch(strNumber);
        }

        protected void btn_cancelFactory_Click(object sender, EventArgs e)
        {
            txt_hintState.Text = "C";
            setModal();
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setPackageHistory();
        }
    }
}