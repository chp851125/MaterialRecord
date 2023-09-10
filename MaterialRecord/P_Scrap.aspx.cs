using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaterialRecord
{
    public partial class P_Scrap : BasePage
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
            if (hf_mode.Value.Equals("batch"))
            {
                hf_mode.Value = "summary";
            }
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

        protected void btn_batchScrap_Click(object sender, EventArgs e)
        {
            ArrayList idList = checkDataToArrayList();
            if (idList.Count == 0)
            {
                lbl_hintModalTitle.Text = "提示";
                lbl_hint.Text = "未勾選資料";
                btn_hintConfirm.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
            }
            else
            {
                txt_hintState.Text = "N";
                setModal();
            }
        }

        protected void btn_batchUsed_Click(object sender, EventArgs e)
        {
            ArrayList idList = checkDataToArrayList();
            if (idList.Count == 0)
            {
                lbl_hintModalTitle.Text = "提示";
                lbl_hint.Text = "未勾選資料";
                btn_hintConfirm.Visible = false;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
            }
            else
            {
                txt_hintState.Text = "Y";
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

        protected void updateScrap(ArrayList idList, string MaterialState)
        {
            List<MaterialSummaryEntity> summaryList = new List<MaterialSummaryEntity>();
            List<MaterialHistoryEntity> historyList = new List<MaterialHistoryEntity>();

            string GUID;
            string Identifier = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            for (int i = 0; i < idList.Count; i++)
            {
                MaterialSummaryEntity summaryEntity = new MaterialSummaryEntity();
                MaterialHistoryEntity historyEntity = new MaterialHistoryEntity();
                GUID = System.Guid.NewGuid().ToString();

                summaryEntity.MaterialState = MaterialState;
                summaryEntity.ScrapDate = MaterialState.Equals("N") ? sdatetime.Replace("-", "").Substring(0, 8) : "";
                summaryEntity.Remark = txt_Remark.Text;
                summaryEntity.UpdateEmpID = userID;
                summaryEntity.UpdateName = userName;
                summaryEntity.UpdateTime = sdatetime;
                summaryEntity.ID = idList[i].ToString();
                summaryList.Add(summaryEntity);

                historyEntity.GUID = GUID;
                historyEntity.Identifier = Identifier;
                historyEntity.DoAction = MaterialState;
                historyEntity.ID = idList[i].ToString();
                historyEntity.MaterialState = MaterialState;
                historyEntity.Remark = txt_Remark.Text;
                historyEntity.CreateEmpID = userID;
                historyEntity.CreateName = userName;
                historyEntity.CreateTime = sdatetime;
                historyEntity.UpdateEmpID = userID;
                historyEntity.UpdateName = userName;
                historyEntity.UpdateTime = sdatetime;
                historyList.Add(historyEntity);
            }

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            WasLogEntity entity = setLogData(pageName + "-updateScrap", pageName, "報廢修改", "卡號 " + userID + " 失敗：", userID, "F");
            updateSummaryScrap(summaryToDataTable(summaryList), historyToDataTable(historyList), entity);
        }

        protected void lbtn_scrap_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "N";
            txt_hintData.Text = e.CommandArgument.ToString();
            setModal();
        }

        protected void lbtn_used_Command(object sender, CommandEventArgs e)
        {
            txt_hintState.Text = "Y";
            txt_hintData.Text = e.CommandArgument.ToString();
            setModal();
        }

        protected void setModal()
        {
            string state = txt_hintState.Text;
            string allData = txt_hintData.Text;
            string hintText = "異常，請聯絡管理員";

            if (allData.Contains('#'))
            {
                string[] data = allData.Split('#');
                Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
                btn_hintConfirm.Visible = true;
                if (state.Equals(data[5]))
                {
                    lbl_hintModalTitle.Text = "提示";
                    lbl_hint.Text = "資料狀態已為 " + MaterialStateDictionary[state];
                    btn_hintConfirm.Visible = false;
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
                    return;
                }
                else if (state.Equals("N"))
                {
                    lbl_hintModalTitle.Text = "報廢";
                    hintText = "確定報廢？";
                }
                else if (state.Equals("Y"))
                {
                    lbl_hintModalTitle.Text = "取消報廢";
                    hintText = "確定取消報廢？";
                }
                lbl_hint.Text = hintText;
            }
            else
            {
                btn_hintConfirm.Visible = true;
                if (state.Equals("N"))
                {
                    lbl_hintModalTitle.Text = "報廢";
                    hintText = "確定報廢？";
                }
                else if (state.Equals("Y"))
                {
                    lbl_hintModalTitle.Text = "取消報廢";
                    hintText = "確定取消報廢？";
                }
                lbl_hint.Text = hintText;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenHintModal();", true);
        }

        protected void btn_hintConfirm_Click(object sender, EventArgs e)
        {
            string state = txt_hintState.Text;
            string allData = txt_hintData.Text;

            if (state.Equals("file"))
            {
                fileDelete(allData);
            }
            else if (allData.Contains('#'))
            {
                string[] data = txt_hintData.Text.Split('#');
                ArrayList idList = new ArrayList();
                idList.Add(data[1]);
                updateScrap(idList, state);
                setPackageSummary();
            }
            else
            {
                ArrayList idList = checkDataToArrayList();
                if (idList.Count > 0)
                {
                    updateScrap(idList, state);
                    setPackageSummary();
                }
            }
        }

        protected void btn_batchScrapMode_Click(object sender, EventArgs e)
        {
            ddl_state.SelectedValue = "Y";
            setPackageSummary();
            btn_batchScrap.Visible = true;
            btn_batchUsed.Visible = false;
            txt_check.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "batchMode();", true);
        }

        protected void btn_batchUsedMode_Click(object sender, EventArgs e)
        {
            ddl_state.SelectedValue = "N";
            setPackageSummary();
            btn_batchScrap.Visible = false;
            btn_batchUsed.Visible = true;
            txt_check.Text = "";
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "batchMode();", true);
        }

        protected void btn_summary_Click(object sender, EventArgs e)
        {
            ddl_state.SelectedValue = "Y";
            setPackageSummary();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "summaryMode();", true);
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

        protected void btn_import_Click(object sender, EventArgs e)
        {
            lbl_importMsg.Text = "";
            txt_importID.Text = "";
            txt_importID.ReadOnly = false;
            setAttachmentList(txt_importID.Text);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImportModal();", true);
        }

        protected void import(string ID)
        {
            if (FileUpload1.HasFiles)
            {
                SubmitFile(ID, FileUpload1.PostedFiles);
            }
        }

        protected void SubmitFile(string ID, IList<HttpPostedFile> FileList)//上傳檔案及回傳檔案清單
        {
            foreach (HttpPostedFile postedFile in FileList)
            {
                String saveDir = "\\files\\" + ID + "\\";
                String appPath = Request.PhysicalApplicationPath;//根目錄之實體路徑

                if (!Directory.Exists(appPath + saveDir))
                {
                    Directory.CreateDirectory(appPath + saveDir);
                }

                String fileName, checkPath;
                fileName = postedFile.FileName;
                string tempfileName = fileName;
                checkPath = appPath + saveDir + fileName;
                if (System.IO.File.Exists(checkPath))//避免檔案重複儲存
                {
                    int counter = 2;
                    while (System.IO.File.Exists(checkPath))
                    {
                        tempfileName = "(" + counter.ToString() + ")" + fileName;
                        checkPath = appPath + saveDir + tempfileName;
                        counter = counter + 1;
                    }
                    fileName = tempfileName;
                }
                string filePathName = appPath + saveDir + tempfileName;
                postedFile.SaveAs(filePathName);
                lbl_importMsg.Text = "上傳成功";
            }
        }

        protected void btn_importUpdate_Click(object sender, EventArgs e)
        {
            string ID = txt_importID.Text;
            import(ID);
            setAttachmentList(ID);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImportModal();", true);
        }

        protected void setAttachmentList(string ID)
        {

            String saveDir = "files\\";
            if (ID.Trim().Length > 0)
            {
                saveDir += ID + "\\";
            }
            String appPath = Request.PhysicalApplicationPath;
            string sdate = DateTime.Now.ToString();
            string fileName, fileType;
            DataTable dt = new DataTable();

            if (Directory.Exists(appPath + saveDir))
            {
                string[] files = System.IO.Directory.GetFiles(appPath + saveDir);
                List<string> pictureList = new List<string>();
                pictureList.Add("jpg");
                pictureList.Add("jpeg");
                pictureList.Add("png");
                pictureList.Add("gif");

                DataRow row;
                dt.Columns.Add("serno");
                dt.Columns.Add("fileName");
                dt.Columns.Add("filaPath");
                dt.Columns.Add("picture");
                dt.Columns.Add("display");
                dt.Columns.Add("imageData");

                int count = 1;
                foreach (string s in files)
                {
                    row = dt.NewRow();
                    fileName = System.IO.Path.GetFileName(s);
                    fileType = fileName.Substring(fileName.IndexOf('.') + 1);
                    row["serno"] = count;
                    row["fileName"] = fileName;
                    row["filaPath"] = saveDir + fileName;
                    row["picture"] = saveDir + fileName + "?ver='" + sdate + "'";
                    row["display"] = pictureList.Contains(fileType) ? "Y" : "N";
                    row["imageData"] = ID + "#" + row["filaPath"];
                    dt.Rows.Add(row);
                    count++;
                }
            }
            lv_file.DataSource = dt;
            lv_file.DataBind();
        }

        protected void txt_importID_TextChanged(object sender, EventArgs e)
        {
            setAttachmentList(txt_importID.Text);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImportModal();", true);
        }

        protected void lbtn_import_Command(object sender, CommandEventArgs e)
        {
            lbl_importMsg.Text = "";
            txt_importID.Text = e.CommandArgument.ToString();
            setAttachmentList(txt_importID.Text);
            txt_importID.ReadOnly = true;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImportModal();", true);
        }

        protected void btn_importDelete_Command(object sender, CommandEventArgs e)
        {
            string filePath = e.CommandArgument.ToString();
            lbl_hintModalTitle.Text = "刪除檔案";
            lbl_hint.Text = "確定刪除 " + filePath.Substring(filePath.LastIndexOf(@"\") + 1) + " ？";
            txt_hintState.Text = "file";
            txt_hintData.Text = filePath;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "hideHintDiv();OpenHintModal();", true);
        }

        protected void fileDelete(string filePath)
        {
            String appPath = Request.PhysicalApplicationPath;
            string path = appPath + filePath;
            System.IO.FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                file.Delete();
                lbl_importMsg.Text = "刪除成功";
            }
            else
            {
                lbl_importMsg.Text = "檔案不存在，請重新載入";
            }
            setAttachmentList(txt_importID.Text);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImportModal();", true);
        }

        protected void ibtn_picture_Command(object sender, CommandEventArgs e)
        {
            string imageData = e.CommandArgument.ToString();
            txt_imageID.Text = imageData.Substring(0, imageData.IndexOf('#'));
            img_picture.ImageUrl = imageData.Substring(imageData.IndexOf('#') + 1);
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenImageModal();", true);
        }

        protected void btn_ImageClose_Click(object sender, EventArgs e)
        {
            txt_importID.Text = txt_imageID.Text;
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "HideImageModal();OpenImportModal();", true);
        }
    }
}