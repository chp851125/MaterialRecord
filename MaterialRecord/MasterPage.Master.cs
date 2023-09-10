using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaterialRecord
{
    public partial class MasterPage : System.Web.UI.MasterPage
    {
        public string gUserName = "";
        public static string TEST = System.Web.Configuration.WebConfigurationManager.AppSettings["TEST"].Trim().ToUpper();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!TEST.Equals("Y"))
            {
                //gUserName = getPersonName();
            }
        }

        protected void LogOut()
        {
            Session["MaterialRecord_UserID"] = null;
            Session["MaterialRecord_UserIP"] = null;
            Response.Redirect("Login.aspx");
        }

        protected void lbtn_logout_Click(object sender, EventArgs e)
        {
            LogOut();
        }

        public Boolean insertToLog(WasLogEntity entity)
        {
            Boolean success = false;
            try
            {
                Modify modify = new Modify();
                success = modify.insertToLog(entity);
            }
            catch (Exception ex)
            {
                Response.Write(ex.ToString());
            }
            return success;
        }

        public WasLogEntity setLogData(string exeMethod, string exeScreen, string exeButton, string exeMsg, string empId, string status)
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
    }
}