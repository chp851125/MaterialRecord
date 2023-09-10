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
    public partial class P_EstablishType : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setMaterialType();
            }
        }

        protected void setMaterialType()
        {
            DataTable dt = getMaterialTypeDT(txt_condition.Text);
            dt.Columns.Add("allData");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i]["allData"] = dt.Rows[i]["ID"].ToString() + '#' + dt.Rows[i]["TypeName"].ToString() + '#' + dt.Rows[i]["Remark"].ToString();
            }
            ListView1.DataSource = dt;
            ListView1.DataBind();
        }
        
        protected string checkField()
        {
            string msg = "";
            string state = txt_state.Text;
            Dictionary<string, string> fieldDictionary = new Dictionary<string, string>();
            fieldDictionary.Add("編號", txt_ID.Text);
            fieldDictionary.Add("名稱", txt_TypeName.Text);

            foreach (string key in fieldDictionary.Keys)
            {
                if (fieldDictionary[key].Trim().Length <= 0)
                {
                    msg += key + "不可為空<br>";
                }
            }

            if (state.Equals("A")) {
                HashSet<string> idSet = getIDSet();
                if (idSet.Contains(fieldDictionary["編號"]))
                {
                    msg += "輸入編號已存在<br>";
                }
            }
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

        protected void save()
        {
            string state = txt_state.Text;
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string userName = getName(userID);
            string sdatetime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");

            MaterialTypeEntity entity = new MaterialTypeEntity();
            entity.ID = txt_ID.Text;
            entity.TypeName = txt_TypeName.Text;
            entity.Remark = txt_Remark.Text;
            entity.CreateEmpID = userID;
            entity.CreateName = userName;
            entity.CreateTime = sdatetime;
            entity.UpdateEmpID = userID;
            entity.UpdateName = userName;
            entity.UpdateTime = sdatetime;

            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];            
            if (state.Equals("A"))
            {
                WasLogEntity logEntity = setLogData(pageName + "-save", pageName, "類別新增", "卡號 " + userID + " 失敗：", userID, "F");
                insertMaterialType(entity, logEntity);
            }
            else if (state.Equals("U"))
            {
                WasLogEntity logEntity = setLogData(pageName + "-save", pageName, "類別修改", "卡號 " + userID + " 失敗：", userID, "F");
                updateMaterialType(entity, logEntity);
            }
            setMaterialType();
        }

        protected void btn_addSave_Click(object sender, EventArgs e)
        {
            string msg = checkField();
            if (msg.Trim().Length > 0)
            {
                lbl_msg.Text = msg;
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenAddModal();", true);
            }
            else
            {
                save();
            }
        }

        protected void btn_query_Click(object sender, EventArgs e)
        {
            setMaterialType();
        }

        protected void lbtn_edit_Command(object sender, CommandEventArgs e)
        {
            setModal(e.CommandArgument.ToString());
        }

        protected void btn_insert_Click(object sender, EventArgs e)
        {
            setModal("");
        }

        protected void setModal(string allData)
        {
            if (allData.Trim().Length == 0)
            {
                txt_ID.Text = "";
                txt_TypeName.Text = "";
                txt_Remark.Text = "";
                lbl_modalTitle.Text = "新增";
                txt_state.Text = "A";
                txt_ID.ReadOnly = false;
            }
            else
            {
                string[] data = allData.Split('#');
                txt_ID.Text = data[0];
                txt_TypeName.Text = data[1];
                txt_Remark.Text = data[2];
                lbl_modalTitle.Text = "更新";
                txt_state.Text = "U";
                txt_ID.ReadOnly = true;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenAddModal();", true);
        }

        protected void lbtn_delete_Command(object sender, CommandEventArgs e)
        {
            string[] data = e.CommandArgument.ToString().Split('#');
            txt_deleteID.Text = data[0];

            Boolean check = checkIdUsed();
            if (check)
            {
                lbl_delete.Text = "是否要刪除此筆資料: " + data[0] + " " + data[1];
                btn_deleteSave.Visible = true;
            }
            else
            {
                lbl_delete.Text = "編號已使用，不可刪除";
                btn_deleteSave.Visible = false;
            }
            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenDeleteModal();", true);
        }

        protected Boolean checkIdUsed()
        {
            int count = getMaterialTypeCount(txt_deleteID.Text);
            if (count == 0)
            {
                return true;
            }
            return false;
        }

        protected void btn_deleteSave_Click(object sender, EventArgs e)
        {
            string userID = Session["MaterialRecord_UserID"] == null ? "admin" : Session["MaterialRecord_UserID"].ToString();
            string pageName = Request.Url.Segments[Request.Url.Segments.Length - 1];
            WasLogEntity entity = setLogData(pageName + "-btn_deleteSave_Click", pageName, "類別刪除", "卡號 " + userID + " 失敗：", userID, "F");
            deleteMaterialType(txt_deleteID.Text, entity);
            setMaterialType();
            setMaterialType();
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setMaterialType();
        }

    }
}