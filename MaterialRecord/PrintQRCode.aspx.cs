using QRCoder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MaterialRecord
{
    public partial class PrintQRCode : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack) {
                //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();HideProgressBar();", true);
            }
            print();
        }

        protected void ListView1_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            TextBox code1 = (TextBox)e.Item.FindControl("code1");
            PlaceHolder PlaceHolder1 = (PlaceHolder)e.Item.FindControl("PlaceHolder1");
            Label Label1 = (Label)e.Item.FindControl("Label1");
            string code = code1.Text;
            if (code != "")
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                QRCodeData qrCodeData = qrGenerator.CreateQrCode(code, QRCodeGenerator.ECCLevel.Q);
                QRCode qrCode = new QRCode(qrCodeData);
                System.Web.UI.WebControls.Image imgBarCode = new System.Web.UI.WebControls.Image();
                imgBarCode.Height = 53;
                imgBarCode.Width = 53;
                using (Bitmap bitMap = qrCode.GetGraphic(20))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitMap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                        byte[] byteImage = ms.ToArray();
                        imgBarCode.ImageUrl = "data:image/png;base64," + Convert.ToBase64String(byteImage);
                    }
                    PlaceHolder1.Controls.Add(imgBarCode);
                    Label1.Text = code;
                }

            }
        }

        protected void print()
        {
            string state = Request.QueryString["state"];
            string id = Session["MaterialRecord_PRINTID"] == null? "" : Session["MaterialRecord_PRINTID"].ToString();
            if (state != "" && id != "")
            {
                string startId = id.Substring(0, id.IndexOf('-'));
                string endId = id.Substring(id.IndexOf('-') + 1);

                if (state.Equals("ALL"))
                {
                    state = "";
                }
                DataTable summary = getPrintIdDT(startId, endId, state);

                DataTable dt = new DataTable();
                System.Data.DataColumn newColumn = new System.Data.DataColumn("code", typeof(System.String));
                newColumn.DefaultValue = "";
                dt.Columns.Add(newColumn);

                for (int i = 0; i < summary.Rows.Count; i++)
                {
                    DataRow row = dt.NewRow();
                    row["code"] = summary.Rows[i]["ID"].ToString();
                    dt.Rows.Add(row);
                }

                ListView1.DataSource = dt;
                ListView1.DataBind();
            }

            ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "Printiframe();", true);
        }

    }
}