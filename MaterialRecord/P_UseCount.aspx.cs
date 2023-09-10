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
    public partial class P_UseCount : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setTypeDropDownList();
                setFactoryDropDownList();
                setStateDropDownList();
                hf_mode.Value = "summary";
                setPackageSummary();
            }
        }

        protected void setPackageSummary()
        {
            DataTable dt = getPackageSummary();
            Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["MaterialType"] = MaterialTypeDictionary[dt.Rows[i]["MaterialType"].ToString()];
                dt.Rows[i]["Factory"] = factoryDictionary[dt.Rows[i]["Factory"].ToString()];
                dt.Rows[i]["MaterialState"] = MaterialStateDictionary[dt.Rows[i]["MaterialState"].ToString()];
            }

            ListView1.DataSource = dt;
            ListView1.DataBind();
        }

        protected DataTable getPackageSummary()
        {
            MaterialSummaryEntity entity = new MaterialSummaryEntity();
            entity.MaterialState = ddl_state.SelectedValue;
            if (!ddl_queryFactory.SelectedValue.Equals("ALL"))
            {
                entity.Factory = ddl_queryFactory.SelectedValue;
            }
            if (!ddl_type.SelectedValue.Equals("ALL"))
            {
                entity.MaterialType = ddl_type.SelectedValue;
            }
            string condition = txt_condition.Text;
            DataTable dt = getSummaryDT(entity, condition);
            return dt;
        }

        protected void btn_query_Click(object sender, EventArgs e)
        {
            setPackageSummary();
        }

        protected void setTypeDropDownList()
        {
            Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            ddl_type.Items.Clear();

            ddl_type.Items.Add(new ListItem("所有類別", "ALL"));
            foreach (string key in MaterialTypeDictionary.Keys)
            {
                ddl_type.Items.Add(new ListItem(MaterialTypeDictionary[key], key));
            }
        }

        protected void setFactoryDropDownList()
        {
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            ddl_queryFactory.Items.Clear();

            ddl_queryFactory.Items.Add(new ListItem("所有廠區", "ALL"));
            foreach (string key in factoryDictionary.Keys)
            {
                ddl_queryFactory.Items.Add(new ListItem(factoryDictionary[key], key));
            }
        }

        protected void setStateDropDownList()
        {
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
            ddl_state.Items.Clear();

            foreach (string key in MaterialStateDictionary.Keys)
            {
                ddl_state.Items.Add(new ListItem(MaterialStateDictionary[key], key));
            }
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
            string allData = txt_check.Text;
            if (allData.Contains('#'))
            {
                string[] data = allData.Split('#');
                for (int i = 0; i < data.Length; i++)
                {
                    if (data[i].Trim().Length > 0 && !idList.Contains(data[i]))
                    {
                        idList.Add(data[i]);
                    }
                }
            }
            return idList;
        }

        protected void updateRecycling(ArrayList idList)
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();

            string GUID;
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string runDate = DateTime.Now.ToString("yyyyMMdd");

            for (int i = 0; i < idList.Count; i++)
            {
                MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
                MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();
                GUID = System.Guid.NewGuid().ToString();

                historyEntity.GUID = GUID;
                historyEntity.Identifier = Identifier;
                historyEntity.DoAction = "R";
                historyEntity.ID = idList[i].ToString();
                historyEntity.EditCount = Convert.ToInt32(txt_UseCount.Text);
                historyEntity.Remark = txt_Remark.Text;
                historyEntity.CreateEmpID = userID;
                historyEntity.CreateName = userName;
                historyEntity.CreateTime = sdatetime;
                historyEntity.UpdateEmpID = userID;
                historyEntity.UpdateName = userName;
                historyEntity.UpdateTime = sdatetime;
                historyList.Add(historyEntity);

                summaryEntity.UseCount = Convert.ToInt32(txt_UseCount.Text);
                summaryEntity.StartUseDate = runDate;
                summaryEntity.Remark = txt_Remark.Text;
                summaryEntity.UpdateEmpID = userID;
                summaryEntity.UpdateName = userName;
                summaryEntity.UpdateTime = sdatetime;
                summaryEntity.ID = idList[i].ToString();
                summaryList.Add(summaryEntity);
            }

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            WasLogEntity entity = setLogData(pageName + "-updateRecycling", pageName, "可使用次數修改", "卡號 " + userID + " 失敗：", userID, "F");
            updateSummaryRecycling(summaryToDataTable(summaryList), historyToDataTable(historyList), entity);
        }

        protected void setModal()
        {
            btn_hintConfirm.Visible = true;
            txt_UseCount.Text = "";
            txt_Remark.Text = "";
            lbl_hint.Text = "請輸入欲修改可使用次數";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHintModal();", true);
        }

        protected void btn_hintConfirm_Click(object sender, EventArgs e)
        {
            string allData = txt_hintData.Text;

            if (allData.Contains('#')) {
                string[] data = txt_hintData.Text.Split('#');
                ArrayList idList = new ArrayList();
                idList.Add(data[1]);
                updateRecycling(idList);
                setPackageSummary();
            }
            else
            {
                ArrayList idList = checkDataToArrayList();
                if (idList.Count > 0)
                {
                    updateRecycling(idList);
                    setPackageSummary();
                }
            }
        }

        protected void btn_summary_Click(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "summaryMode();", true);
        }

        protected void lbtn_recycling_Command(object sender, CommandEventArgs e)
        {
            txt_hintData.Text = e.CommandArgument.ToString();
            setModal();
        }

        protected void btn_batchMode_Click(object sender, EventArgs e)
        {
            btn_batchEdit.Visible = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "batchMode();", true);
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setPackageSummary();
            if (hf_mode.Value.Equals("batch"))
            {
                string checkData = txt_check.Text;
                int count = 0;
                foreach (ListViewItem tempItem in ListView1.Items)
                {
                    CheckBox cb_row = tempItem.FindControl("chk_row") as CheckBox;
                    Label lbl_id = tempItem.FindControl("IDLabel") as Label;

                    if (checkData.Contains(lbl_id.Text))
                    {
                        cb_row.Checked = true;
                        count++;
                    }
                }
                Boolean allcheck = false;
                int currentPage = DataPager1.StartRowIndex / DataPager1.PageSize + 1;
                int totalPage = DataPager1.TotalRowCount / DataPager1.PageSize + 1;
                int lastPageDataRow = DataPager1.TotalRowCount % DataPager1.PageSize;
                if (count != 0)
                {
                    if (count == DataPager1.PageSize)
                    {
                        allcheck = true;
                    }
                    else if (currentPage == totalPage && count == lastPageDataRow)
                    {
                        allcheck = true;
                    }
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "pageCheck('" + allcheck + "');", true);
            }
        }

        protected void chk_row_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = (CheckBox)sender;
            ListViewItem row = (ListViewItem)cb.NamingContainer;
            Label lbl_id = row.FindControl("IDLabel") as Label;
            if (cb.Checked)
            {
                txt_check.Text += lbl_id.Text + '#';
            }
            else
            {
                txt_check.Text = txt_check.Text.Replace(lbl_id.Text + '#', "");
            }
        }
    }
}