using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
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
    public partial class R_StatisticsCount : BasePage
    {
        protected override void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "ShowProgressBar();", true);
                setTypeDropDownList();
                setFactoryDropDownList();
                setPackageSummary();
            }
        }

        protected void setPackageSummary()
        {
            DataTable dt = getPackageSummary();
            Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");
            int add = 0;
            int scrap = 0;

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["state"].Equals("A"))
                {
                    dt.Rows[i]["state"] = "新增";
                    add++;
                }
                else if (dt.Rows[i]["state"].Equals("N"))
                {
                    dt.Rows[i]["state"] = "報廢";
                    scrap++;
                }
                dt.Rows[i]["MaterialType"] = MaterialTypeDictionary[dt.Rows[i]["MaterialType"].ToString()];
                dt.Rows[i]["Factory"] = factoryDictionary[dt.Rows[i]["Factory"].ToString()];
                dt.Rows[i]["MaterialState"] = MaterialStateDictionary[dt.Rows[i]["MaterialState"].ToString()];
            }

            ListView1.DataSource = dt;
            ListView1.DataBind();
            lbl_sum.Text = "新增量：" + add + " / 報廢量：" + scrap;
        }

        protected DataTable getPackageSummary()
        {
            MaterialSummaryEntity entity = new MaterialSummaryEntity();
            if (!ddl_queryFactory.SelectedValue.Equals("ALL"))
            {
                entity.Factory = ddl_queryFactory.SelectedValue;
            }
            if (!ddl_type.SelectedValue.Equals("ALL"))
            {
                entity.MaterialType = ddl_type.SelectedValue;
            }
            DataTable dt = getStatisticsCountDT(entity, txt_startDate.Text, txt_endDate.Text);

            return dt;
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

        protected void btn_print_Click(object sender, EventArgs e)
        {
            //ScriptManager.RegisterStartupScript(this, this.GetType(), "Pop", "OpenModal();", true);
            //Response.Redirect("PrintQRCode.aspx?code=");
        }

        protected void lbtn_print_Command(object sender, CommandEventArgs e)
        {
            string[] data = e.CommandArgument.ToString().Split('#');
            string MaterialType = data[0];
            string ID = data[1];
            Response.Redirect("PrintQRCode.aspx?code=" + MaterialType + ID);
        }
        protected HashSet<string> getIDSet()
        {
            HashSet<string> idSet = new HashSet<string>();
            string tConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Local"].ToString();
            SqlConnection oleConn = new SqlConnection(tConnectionString);
            try
            {
                if (oleConn.State == ConnectionState.Closed)
                {
                    oleConn.Open();
                }
                SqlCommand oleCmd = new SqlCommand();
                oleCmd.Connection = oleConn;

                StringBuilder sql = new StringBuilder();
                sql.Append(" select ID from Material_Type ");

                oleCmd.CommandText = sql.ToString();
                SqlDataAdapter da = new SqlDataAdapter(oleCmd);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        idSet.Add(dt.Rows[i]["ID"].ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write(ex);
            }
            finally
            {
                oleConn.Close();
            }
            return idSet;
        }

        protected void btn_query_Click(object sender, EventArgs e)
        {
            setPackageSummary();
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

        protected void btn_excel_Click(object sender, EventArgs e)
        {
            exportExcel("新增量報廢量統計表" + DateTime.Now.ToString("yyyyMMdd") + ".xlsx");
        }

        public void exportExcel(string FileName)
        {
            Response.Clear();
            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet u_sheet = workbook.CreateSheet("新增量報廢量統計表");

            string[] titleArray = { "新增/報廢", "類別", "編號", "名稱", "可使用次數", "廠區", "狀態", "建立日", "開始使用日", "報廢日", "備註" };
            string[] fieldArray = { "state", "MaterialType", "ID", "MaterialName", "UseCount", "Factory", "MaterialState", "CreateDate", "StartUseDate", "ScrapDate", "Remark" };

            DataTable dt = getPackageSummary();
            Dictionary<string, string> MaterialTypeDictionary = getOptionDictionary("MaterialType");
            Dictionary<string, string> factoryDictionary = getOptionDictionary("Factory");
            Dictionary<string, string> MaterialStateDictionary = getOptionDictionary("MaterialState");

            IFont font = workbook.CreateFont();
            font.IsBold = true;

            ICellStyle title = workbook.CreateCellStyle();
            title.SetFont(font);

            ICellStyle header = workbook.CreateCellStyle();
            header.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            header.FillPattern = FillPattern.SolidForeground;

            ICellStyle headerBold = workbook.CreateCellStyle();
            headerBold.FillForegroundColor = IndexedColors.Grey25Percent.Index;
            headerBold.FillPattern = FillPattern.SolidForeground;
            headerBold.SetFont(font);

            IRow u_row = u_sheet.CreateRow(0);
            u_row.CreateCell(0).SetCellValue("廠區");
            u_row.CreateCell(1).SetCellValue(ddl_queryFactory.SelectedItem.ToString());
            u_row.CreateCell(2).SetCellValue("類別");
            u_row.CreateCell(3).SetCellValue(ddl_type.SelectedItem.ToString());
            u_row.CreateCell(4).SetCellValue("日期區間");
            //u_row.CreateCell(5).SetCellValue(ddl_dateType.SelectedItem.ToString());
            u_row.CreateCell(5).SetCellValue(txt_startDate.Text);
            u_row.CreateCell(6).SetCellValue("~");
            u_row.CreateCell(7).SetCellValue(txt_endDate.Text);
            u_row.GetCell(0).CellStyle = headerBold;
            u_row.GetCell(1).CellStyle = header;
            u_row.GetCell(2).CellStyle = headerBold;
            u_row.GetCell(3).CellStyle = header;
            u_row.GetCell(4).CellStyle = headerBold;
            u_row.GetCell(5).CellStyle = header;
            u_row.GetCell(6).CellStyle = headerBold;
            u_row.GetCell(7).CellStyle = header;

            for (int i = 0; i < dt.Rows.Count + 1; i++)
            {
                u_row = u_sheet.CreateRow(i + 1);
                if (i == 0)
                {
                    for (int j = 0; j < titleArray.Length; j++)
                    {
                        u_row.CreateCell(j).SetCellValue(titleArray[j]);
                        u_row.GetCell(j).CellStyle = title;
                    }
                }
                else
                {
                    dt.Rows[i - 1]["state"] = dt.Rows[i - 1]["state"].Equals("A") ? "新增" : "報廢";
                    dt.Rows[i - 1]["MaterialType"] = MaterialTypeDictionary[dt.Rows[i - 1]["MaterialType"].ToString()];
                    dt.Rows[i - 1]["Factory"] = factoryDictionary[dt.Rows[i - 1]["Factory"].ToString()];
                    dt.Rows[i - 1]["MaterialState"] = MaterialStateDictionary[dt.Rows[i - 1]["MaterialState"].ToString()];

                    for (int j = 0; j < titleArray.Length; j++)
                    {
                        u_row.CreateCell(j).SetCellValue(dt.Rows[i - 1][fieldArray[j]].ToString());
                    }
                }
            }

            MemoryStream MS = new MemoryStream();
            workbook.Write(MS);

            Response.AddHeader("Content-Disposition", "attachment; filename=" + FileName);
            Response.BinaryWrite(MS.ToArray());

            workbook = null;
            MS.Close();
            MS.Dispose();

            Response.Flush();
            Response.End();
        }

        protected void ListView1_PagePropertiesChanged(object sender, EventArgs e)
        {
            setPackageSummary();
        }
    }
}