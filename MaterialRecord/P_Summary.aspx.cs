using System;
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
    public partial class P_Summary : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setTypeDropDownList();
                setFactoryDropDownList();
                setStateDropDownList();
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


        protected void setTypeDropDownList()
        {
            Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            ddl_MaterialType.Items.Clear();
            ddl_type.Items.Clear();
            ddl_printMaterialType.Items.Clear();

            ddl_type.Items.Add(new ListItem("所有類別", "ALL"));
            foreach (string key in MaterialTypeDictionary.Keys)
            {
                ddl_MaterialType.Items.Add(new ListItem(MaterialTypeDictionary[key], key));
                ddl_type.Items.Add(new ListItem(MaterialTypeDictionary[key], key));
                ddl_printMaterialType.Items.Add(new ListItem(MaterialTypeDictionary[key], key));
            }
        }

        protected void setFactoryDropDownList()
        {
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            ddl_queryFactory.Items.Clear();
            ddl_Factory.Items.Clear();

            ddl_queryFactory.Items.Add(new ListItem("所有廠區", "ALL"));
            foreach (string key in factoryDictionary.Keys)
            {
                ddl_queryFactory.Items.Add(new ListItem(factoryDictionary[key], key));
                ddl_Factory.Items.Add(new ListItem(factoryDictionary[key], key));
            }
        }

        protected void setStateDropDownList()
        {
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
            ddl_state.Items.Clear();
            ddl_MaterialState.Items.Clear();
            ddl_printMaterialState.Items.Clear();

            ddl_printMaterialState.Items.Add(new ListItem("所有狀態", "ALL"));
            foreach (string key in MaterialStateDictionary.Keys)
            {
                ddl_state.Items.Add(new ListItem(MaterialStateDictionary[key], key));
                ddl_MaterialState.Items.Add(new ListItem(MaterialStateDictionary[key], key));
                ddl_printMaterialState.Items.Add(new ListItem(MaterialStateDictionary[key], key));
            }
        }

        protected void btn_insert_Click(object sender, EventArgs e)
        {
            lbl_msg.Text = "";
            txt_state.Text = "A";
            setAddModal("");
        }

        protected void lbtn_edit_Command(object sender, CommandEventArgs e)
        {
            txt_state.Text = "U";
            setAddModal(e.CommandArgument.ToString());
        }

        protected void setAddModal(string allData)
        {
            ddl_MaterialType.Enabled = true;
            txt_ID.Enabled = true;
            txt_MaterialName.Enabled = true;
            txt_UseCount.Enabled = true;
            ddl_Factory.Enabled = true;
            ddl_MaterialState.Enabled = true;
            txt_Remark.Enabled = true;

            if (allData.Contains("#"))
            {
                string[] data = allData.Split('#');
                ddl_MaterialType.Text = data[0];
                txt_ID.Text = data[1];
                txt_MaterialName.Text = data[2];
                txt_UseCount.Text = data[3];
                ddl_Factory.Text = data[4];
                ddl_MaterialState.Text = data[5];
                string CreateDate = data[6];
                string StartUseDate = data[7];
                string ScrapDate = data[8];
                txt_Remark.Text = data[11];
                lbl_modalTitle.Text = "更新";

                ddl_MaterialType.Enabled = false;
                txt_ID.Enabled = false;
                if (StartUseDate.Trim().Length > 0)
                {
                    txt_MaterialName.Enabled = false;
                    txt_UseCount.Enabled = false;
                    ddl_Factory.Enabled = false;
                    ddl_MaterialState.Enabled = false;
                }
                else if (!ddl_MaterialState.Text.Equals("Y"))
                {
                    txt_UseCount.Enabled = false;
                    ddl_Factory.Enabled = false;
                }
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideAddModalNumber();OpenAddModal();", true);
            }
            else
            {
                if (ddl_MaterialType.Items.Count <= 0)
                {
                    lbl_hintModalTitle.Text = "建檔作業";
                    lbl_hint.Text = "請先建立包裝類別";
                    btn_hintConfirm.Visible = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHintModal();", true);
                    return;
                }
                ddl_MaterialType.SelectedIndex = 0;
                txt_ID.Text = "";
                txt_MaterialName.Text = "";
                txt_UseCount.Text = "";
                ddl_Factory.SelectedIndex = 0;
                ddl_MaterialState.SelectedIndex = 0;
                txt_Remark.Text = "";
                ddl_Factory.Enabled = false;
                ddl_MaterialState.Enabled = false;

                if (txt_state.Text.Equals("B"))
                {
                    lbl_modalTitle.Text = "批次建檔作業";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideAddModalID();OpenAddModal();", true);
                }
                else
                {
                    lbl_modalTitle.Text = "建檔作業";
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideAddModalNumber();HideAddModalID();OpenAddModal();", true);
                }
            }
        }

        protected void lbtn_print_Command(object sender, CommandEventArgs e)
        {
            string[] data = e.CommandArgument.ToString().Split('#');
            string MaterialType = data[0];
            string ID = data[1];
            //Response.Redirect("PrintQRCode.aspx?code=" + MaterialType + ID);
            string state = data[5];
            Session["MaterialRecord_PRINTID"] = ID + "~" + ID;
            string url = "PrintQRCode.aspx?state=" + state;
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + url + "'); location.href='P_Summary.aspx';", true);
        }

        protected string checkField()
        {
            Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
            string msg = "";
            string state = txt_state.Text;

            fieldDictionary.Add("類別", ddl_MaterialType.SelectedValue);
            //fieldDictionary.Add("編號", txt_ID.Text);
            fieldDictionary.Add("名稱", txt_MaterialName.Text);
            fieldDictionary.Add("可使用次數", txt_UseCount.Text);
            fieldDictionary.Add("廠區", ddl_Factory.SelectedValue);
            fieldDictionary.Add("狀態", ddl_MaterialState.SelectedValue);


            if (state.Equals("B"))
            {
                if (!IsNumeric(txt_number.Text))
                {
                    msg += "批次新增數量格式錯誤<br>";
                }
            }

            foreach (string key in fieldDictionary.Keys)
            {
                if (fieldDictionary[key].Trim().Length <= 0)
                {
                    msg += key + "不可為空<br>";
                }
                else if (key.Equals("可使用次數"))
                {
                    if (!IsNumeric(fieldDictionary[key]))
                    {
                        msg += key + "格式錯誤<br>";
                    }
                }
            }

            //if (state.Equals("A"))
            //{
            //    HashSet<string> idSet = getIDSet();
            //    if (idSet.Contains(fieldDictionary["編號"]))
            //    {
            //        msg += "編號已存在<br>";
            //    }
            //}
            return msg;
        }

        protected HashSet<string> getIDSet()
        {
            HashSet<string> idSet = new HashSet<string>();
            DataTable dt = getMaterialTypeDT("");
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    idSet.Add(dt.Rows[i]["ID"].ToString());
                }
            }           
            return idSet;
        }

        public bool IsNumeric(String strNumber)
        {
            Regex NumberPattern = new Regex("^\\d+$"); //非負整數（正整數 + 0）
            return NumberPattern.IsMatch(strNumber);
        }

        protected void save()
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();
            
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);            
            string state = txt_state.Text;
            string MaterialType = ddl_MaterialType.SelectedValue;
            string ID = txt_ID.Text;
            //string ID = getID(MaterialType);
            string MaterialName = txt_MaterialName.Text;
            string UseCount = txt_UseCount.Text;
            string Factory = ddl_Factory.SelectedValue;
            string MaterialState = ddl_MaterialState.SelectedValue;
            string RefNumber = "";
            string Remark = txt_Remark.Text;
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string GUID = System.Guid.NewGuid().ToString();
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");

            if (state.Equals("U"))
            {
                MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
                MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();

                string[] summaryColumns = { "MaterialType", "MaterialName", "UseCount", "Factory", "MaterialState",
                                            "RefNumber", "Remark", "UpdateEmpID", "UpdateName", "UpdateTime" };

                summaryEntity.ID = ID;
                summaryEntity.MaterialType = MaterialType;
                summaryEntity.MaterialName = MaterialName;
                summaryEntity.UseCount = Convert.ToInt32(UseCount);
                summaryEntity.Factory = Factory;
                summaryEntity.MaterialState = MaterialState;
                summaryEntity.Remark = Remark;
                summaryEntity.UpdateEmpID = userID;
                summaryEntity.UpdateName = userName;
                summaryEntity.UpdateTime = sdatetime;
                summaryList.Add(summaryEntity);

                historyEntity.GUID = GUID;
                historyEntity.Identifier = Identifier;
                historyEntity.MaterialType = MaterialType;
                historyEntity.ID = ID;
                historyEntity.MaterialName = MaterialName;
                historyEntity.UseCount = Convert.ToInt32(UseCount);
                historyEntity.EditCount = Convert.ToInt32(UseCount);
                historyEntity.Factory = Factory;
                historyEntity.MaterialState = MaterialState;
                historyEntity.Remark = Remark;
                historyEntity.DoAction = state;
                historyEntity.CreateEmpID = userID;
                historyEntity.CreateName = userName;
                historyEntity.CreateTime = sdatetime;
                historyEntity.UpdateEmpID = userID;
                historyEntity.UpdateName = userName;
                historyEntity.UpdateTime = sdatetime;
                historyList.Add(historyEntity);

                string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
                WasLogEntity entity = setLogData(pageName + "-save", pageName, "總表修改", "卡號 " + userID + " 失敗：", userID, "F");
                updateSummary(summaryColumns, summaryToDataTable(summaryList), historyToDataTable(historyList), entity);
            }
            else
            {
                int serno;
                string initial = getPackageID(MaterialType);
                int number = state.Equals("A") ? 1 : Convert.ToInt32(txt_number.Text);
                for (int i = 0; i < number; i++)
                {
                    MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
                    MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();

                    GUID = System.Guid.NewGuid().ToString();
                    serno = 100000 + Convert.ToInt32(initial) + i;
                    ID = MaterialType + serno.ToString().Substring(1);

                    summaryEntity.MaterialType = MaterialType;
                    summaryEntity.ID = ID;
                    summaryEntity.MaterialName = MaterialName;
                    summaryEntity.UseCount = Convert.ToInt32(UseCount);
                    summaryEntity.Factory = Factory;
                    summaryEntity.MaterialState = MaterialState;
                    summaryEntity.CreateDate = sdatetime.Replace("-", "").Substring(0, 8);
                    summaryEntity.StartUseDate = "";
                    summaryEntity.ScrapDate = "";
                    summaryEntity.RefNumber = RefNumber;
                    summaryEntity.CancelFactory = 0;
                    summaryEntity.Remark = Remark;
                    summaryEntity.CreateEmpID = userID;
                    summaryEntity.CreateName = userName;
                    summaryEntity.CreateTime = sdatetime;
                    summaryEntity.UpdateEmpID = userID;
                    summaryEntity.UpdateName = userName;
                    summaryEntity.UpdateTime = sdatetime;
                    summaryList.Add(summaryEntity);

                    historyEntity.GUID = GUID;
                    historyEntity.Identifier = Identifier;
                    historyEntity.MaterialType = MaterialType;
                    historyEntity.ID = ID;
                    historyEntity.MaterialName = MaterialName;
                    historyEntity.UseCount = Convert.ToInt32(UseCount);
                    historyEntity.EditCount = Convert.ToInt32(UseCount);
                    historyEntity.Factory = Factory;
                    historyEntity.MaterialState = MaterialState;
                    historyEntity.RefNumber = RefNumber;
                    historyEntity.Remark = Remark;
                    historyEntity.DoAction = state;
                    historyEntity.CreateEmpID = userID;
                    historyEntity.CreateName = userName;
                    historyEntity.CreateTime = sdatetime;
                    historyEntity.UpdateEmpID = userID;
                    historyEntity.UpdateName = userName;
                    historyEntity.UpdateTime = sdatetime;
                    historyList.Add(historyEntity);
                }
                string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
                WasLogEntity entity = setLogData(pageName + "-save", pageName, "總表新增", "卡號 " + userID + " 失敗：", userID, "F");
                insertSummary(summaryToDataTable(summaryList), historyToDataTable(historyList), entity);
            }
        }

        protected void btn_query_Click(object sender, EventArgs e)
        {
            setPackageSummary();
        }

        protected void btn_addSave_Click(object sender, EventArgs e)
        {
            string msg = checkField();
            if (msg.Trim().Length > 0)
            {
                lbl_msg.Text = msg;
                if (txt_state.Text.Equals("B"))
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideAddModalID();OpenAddModal();", true);
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideAddModalNumber();HideAddModalID();OpenAddModal();", true);
                }
            }
            else
            {
                save();
                setPackageSummary();
            }
        }

        protected void lbtn_scrap_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "N";
            setHintModal(e.CommandArgument.ToString());
        }

        protected void lbtn_inFactory_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "I";
            setHintModal(e.CommandArgument.ToString());
        }

        protected void lbtn_outFactory_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "O";
            setHintModal(e.CommandArgument.ToString());
        }

        protected void lbtn_invalid_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "X";
            setHintModal(e.CommandArgument.ToString());
        }

        protected void lbtn_delete_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "D";
            setHintModal(e.CommandArgument.ToString());
        }

        protected void btn_hintConfirm_Click(object sender, EventArgs e)
        {
            update();
            setPackageSummary();
            setPackageSummary();
        }

        protected void setHintModal(string allData)
        {
            txt_hintData.Text = allData;
            string[] data = allData.Split('#');
            string MaterialType = data[0];
            string ID = data[1];
            string MaterialName = data[2];
            string UseCount = data[3];
            string Factory = data[4];
            string MaterialState = data[5];
            string CreateDate = data[6];
            string StartUseDate = data[7];
            string ScrapDate = data[8];
            string Remark = data[11];

            string state = txt_hintState.Text;
            string hintMessage;

            btn_hintConfirm.Visible = true;
            btn_hintConfirm.CssClass = "btn btn-success";
            //btn_hintConfirm.Text = "確定";
            switch (state)
            {
                case "N":
                    if (MaterialState.Equals(state))
                    {
                        hintMessage = "此筆資料已報廢";
                        btn_hintConfirm.Visible = false;
                    }
                    else
                    {
                        hintMessage = "是否要報廢此筆資料: " + ID + " " + MaterialName;
                        btn_hintConfirm.CssClass = "btn btn-danger";
                        //btn_hintConfirm.Text = "報廢";
                    }
                    break;
                case "I":
                    if (!MaterialState.Equals("Y"))
                    {
                        hintMessage = "此筆資料已報廢或失效";
                        btn_hintConfirm.Visible = false;
                    }
                    else if (Factory.Equals(state))
                    {
                        hintMessage = "此筆資料已返廠";
                        btn_hintConfirm.Visible = false;
                    }
                    else
                    {
                        hintMessage = "是否要將此筆資料執行返廠: " + ID + " " + MaterialName;
                    }                    
                    break;
                case "O":
                    if (!MaterialState.Equals("Y"))
                    {
                        hintMessage = "此筆資料已報廢或失效";
                        btn_hintConfirm.Visible = false;
                    }
                    else if (Factory.Equals(state))
                    {
                        hintMessage = "此筆資料已出廠";
                        btn_hintConfirm.Visible = false;
                    }
                    else
                    {
                        hintMessage = "是否要將此筆資料執行出廠: " + ID + " " + MaterialName;
                    }
                    break;
                case "X":
                    if (MaterialState.Equals(state))
                    {
                        hintMessage = "此筆資料已失效";
                        btn_hintConfirm.Visible = false;
                    }
                    else
                    {
                        hintMessage = "是否要設定此筆資料失效: " + ID + " " + MaterialName;
                        btn_hintConfirm.CssClass = "btn btn-danger";
                        //btn_hintConfirm.Text = "失效";
                    }
                    break;
                case "D":
                    lbl_hintModalTitle.Text = "刪除";
                    if (StartUseDate.Trim().Length > 0)
                    {
                        hintMessage = "資料已使用，不可刪除";
                        btn_hintConfirm.Visible = false;
                    }
                    else
                    {
                        hintMessage = "是否要刪除此資料: " + ID + " " + MaterialName;
                        btn_hintConfirm.CssClass = "btn btn-danger";
                    }
                    break;
                default:
                    hintMessage = "";
                    break;
            }
            lbl_hint.Text = hintMessage;            
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHintModal();", true);
        }

        protected void update()
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();
            MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
            MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();
            List<string> columnsArray = new List<string>();

            string state = txt_hintState.Text;
            string[] data = txt_hintData.Text.Split('#');
            string MaterialType = data[0];
            string ID = data[1];
            string MaterialName = data[2];
            string UseCount = data[3];
            string Factory = data[4];
            string MaterialState = data[5];
            string Remark = data[11];

            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            string GUID = System.Guid.NewGuid().ToString();
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string runDate = sdatetime.Replace("-", "").Substring(0, 8);
                        
            switch (state)
            {
                case "N":
                    columnsArray.Add("MaterialState");
                    columnsArray.Add("ScrapDate");
                    summaryEntity.MaterialState = "N";
                    summaryEntity.ScrapDate = runDate;
                break;
                case "I":
                    columnsArray.Add("Factory");
                    columnsArray.Add("UseCount");
                    columnsArray.Add("StartUseDate");
                    summaryEntity.Factory = state;
                    summaryEntity.UseCount = Convert.ToInt32(UseCount) - 1;
                    summaryEntity.StartUseDate = runDate;
                    break;
                case "O":
                    columnsArray.Add("Factory");
                    columnsArray.Add("StartUseDate");
                    summaryEntity.Factory = state;
                    summaryEntity.StartUseDate = runDate;
                    //sql.Append(" StartUseDate = (case when StartUseDate = '' then '").Append(runDate).Append("' else StartUseDate end), ");
                    break;
                case "X":
                    columnsArray.Add("MaterialState");
                    summaryEntity.MaterialState = "X";
                    break;
                case "D":
                    columnsArray.Add("MaterialState");
                    summaryEntity.MaterialState = "D";
                    break;
            default:
                    break;
            }

            columnsArray.Add("UpdateEmpID");
            columnsArray.Add("UpdateName");
            columnsArray.Add("UpdateTime");
            summaryEntity.UpdateEmpID = userID;
            summaryEntity.UpdateName = userName;
            summaryEntity.UpdateTime = sdatetime;
            summaryEntity.ID = ID;            
            summaryList.Add(summaryEntity);

            historyEntity.GUID = GUID;
            historyEntity.Identifier = Identifier;
            historyEntity.DoAction = state;
            historyEntity.MaterialType = MaterialType;
            historyEntity.ID = ID;
            historyEntity.MaterialName = MaterialName;
            historyEntity.UseCount = Convert.ToInt32(UseCount);
            historyEntity.EditCount = Convert.ToInt32(UseCount);
            historyEntity.Factory = Factory;
            historyEntity.MaterialState = MaterialState;
            historyEntity.RefNumber = "";
            historyEntity.Remark = Remark;
            historyEntity.CreateEmpID = userID;
            historyEntity.CreateName = userName;
            historyEntity.CreateTime = sdatetime;
            historyEntity.UpdateEmpID = userID;
            historyEntity.UpdateName = userName;
            historyEntity.UpdateTime = sdatetime;
            historyList.Add(historyEntity);

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            //WS_Modify.WasLogEntity entity = setLogData(pageName + "-update", pageName, "總表更新", "卡號 " + userID + " 失敗：", userID, "F");
            //updateSummary(listToArray(columnsArray), summaryToDataTable(summaryList), historyToDataTable(historyList), entity);
        }

        protected void lbtn_history_Command(object sender, CommandEventArgs e)
        {
            setDetail(e.CommandArgument.ToString());
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHistoryModal();", true);
        }

        protected void setDetail(string ID)
        {
            DataTable dt = getHistoryDT(ID);
            if (dt.Rows.Count > 0)
            {
                Dictionary<string, string> typeDictionary = getOptionDictionary("MaterialType");
                Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
                Dictionary<string, string> stateDictionary = getOptionDictionary("MaterialState");
                Dictionary<string, string> actionDictionary = getOptionDictionary("DoAction");
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    dt.Rows[i]["MaterialType"] = typeDictionary[dt.Rows[i]["MaterialType"].ToString()];
                    dt.Rows[i]["Factory"] = factoryDictionary[dt.Rows[i]["Factory"].ToString()];
                    dt.Rows[i]["MaterialState"] = stateDictionary[dt.Rows[i]["MaterialState"].ToString()];
                    dt.Rows[i]["DoAction"] = actionDictionary[dt.Rows[i]["DoAction"].ToString()];
                }
            }
            lv_detail.DataSource = dt;
            lv_detail.DataBind();
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setPackageSummary();
        }

        protected void btn_batchInsert_Click(object sender, EventArgs e)
        {
            lbl_msg.Text = "";
            txt_state.Text = "B";
            setAddModal("");
        }

        protected void btn_print_Click(object sender, EventArgs e)
        {
            ddl_printMaterialState.SelectedValue = "Y";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenPrintModal();", true);
        }

        protected string printCheckInput()
        {
            string message = "";
            string startID = txt_startID.Text;
            string endID = txt_endID.Text;
            if (startID.Length <= 0 || endID.Length <= 0)
            {
                message = "編號不可為空";
                return message;
            }
            if (!IsNumeric(startID) || !IsNumeric(endID))
            {
                message = "編號輸入格式錯誤，請輸入數字";
                return message;
            }

            int istartID = Convert.ToInt32(startID);
            int iendID = Convert.ToInt32(endID);
            if (istartID <= 0 || istartID > 99999 || iendID <= 0 || iendID > 99999)
            {
                message = "編號輸入區間為1-99999";
            }

            if (istartID > iendID)
            {
                message = "編號起始值不可大於結束值";
            }
            return message;
        }

        protected void btn_printConfirm_Click(object sender, EventArgs e)
        {
            string message = printCheckInput();
            if (message.Length > 0)
            {
                lbl_printMsg.Text = message;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenPrintModal();", true);
            }
            else
            {
                //string url = "PrintQRCode.aspx?code=" + ID;
                string startID = ddl_printMaterialType.SelectedValue + txt_startID.Text.PadLeft(5, '0');
                string endID = ddl_printMaterialType.SelectedValue + txt_endID.Text.PadLeft(5, '0'); ;
                Session["MaterialRecord_PRINTID"] = startID + "~" + endID;
                string url = "PrintQRCode.aspx?state=" + ddl_printMaterialState.SelectedValue;
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "test", "window.open('" + url + "'); location.href='P_Summary.aspx';", true);
            }
        }
    }
}