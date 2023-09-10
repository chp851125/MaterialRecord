using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Dapper;

namespace MaterialRecord
{
    public class Query
    {
        /// <summary>
        /// 取得包裝材總表
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getSummary(MaterialSummaryEntity entity, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select MaterialType, ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime, ");
                sql.Append(" MaterialType + '#' + ID + '#' + MaterialName + '#' + convert(varchar(10), UseCount) + '#' + Factory + '#' + MaterialState + '#' + CreateDate + '#' + StartUseDate + '#' + ScrapDate + '#' + RefNumber + '#' + convert(varchar(10), CancelFactory) + '#' + Remark as allData ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where 1 = 1 ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                if (entity.ID != null)
                {
                    sql.Append(" and ID = @ID ");
                    SqlParameter parameter = new SqlParameter("@ID", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.ID.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialState != null)
                {
                    sql.Append(" and MaterialState = @MaterialState ");
                    SqlParameter parameter = new SqlParameter("@MaterialState", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialState.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.Factory != null)
                {
                    sql.Append(" and Factory = @Factory ");
                    SqlParameter parameter = new SqlParameter("@Factory", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.Factory.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialType != null)
                {
                    sql.Append(" and MaterialType = @MaterialType ");
                    SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialType.Trim();
                    SqlParameters.Add(parameter);
                }
                if (condition != null && condition.Trim().Length > 0)
                {
                    sql.Append(" and (charindex(@condition, ID) > 0 ");
                    sql.Append(" or charindex(@condition, MaterialName) > 0) ");
                    SqlParameter parameter = new SqlParameter("@condition", System.Data.SqlDbType.NVarChar);
                    parameter.Value = condition.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 列印標籤
        /// </summary>
        /// <param name="startid"></param>
        /// <param name="endid"></param>
        /// <param name="MaterialState"></param>
        /// <returns></returns>
        public DataTable getPrintId(string startid, string endid, string MaterialState)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select MaterialType, ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where ID between @startid and @endid ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@startid", System.Data.SqlDbType.NVarChar);
                parameter.Value = startid.Trim();
                SqlParameters.Add(parameter);

                parameter = new SqlParameter("@endid", System.Data.SqlDbType.NVarChar);
                parameter.Value = endid.Trim();
                SqlParameters.Add(parameter);

                if (MaterialState.Length > 0)
                {
                    sql.Append(" and MaterialState = @MaterialState ");
                    parameter = new SqlParameter("@MaterialState", System.Data.SqlDbType.NVarChar);
                    parameter.Value = MaterialState.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 取得ID編號
        /// </summary>
        /// <param name="MaterialType"></param>
        /// <returns></returns>
        public string getPackageID(string MaterialType)
        {
            string ID = null;
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select isnull(max(substring(ID, 2, len(ID) - 1)), 0) + 1 as serno ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where ID like @MaterialType ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                parameter.Value = MaterialType.Trim() + '%';
                SqlParameters.Add(parameter);

                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                if (dt.Rows.Count > 0)
                {
                    ID = dt.Rows[0]["serno"].ToString();
                }
                else
                {
                    throw new Exception("無法取得編號，請聯絡管理員");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ID;
        }

        /// <summary>
        /// 取得包裝材類別
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getMaterialType(string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select ID, TypeName, Remark, CreateEmpID, CreateName, ");
                sql.Append(" CreateTime, UpdateEmpID, UpdateName, UpdateTime ");
                sql.Append(" from Material_Type ");
                sql.Append(" where 1 = 1 ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();

                if (condition.Trim().Length > 0)
                {
                    sql.Append(" and (charindex('").Append(condition).Append("', ID) > 0 ");
                    sql.Append(" or charindex('").Append(condition).Append("', TypeName) > 0) ");
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 取得包裝材類別編號是否使用
        /// </summary>
        /// <param name="MaterialType"></param>
        /// <returns></returns>
        public int getMaterialTypeCount(string MaterialType)
        {
            int count = -1;
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select count(*) ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where MaterialType = @MaterialType ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                parameter.Value = MaterialType.Trim();
                SqlParameters.Add(parameter);

                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                if (dt.Rows.Count > 0)
                {
                    count = Convert.ToInt32(dt.Rows[0][0].ToString());
                }
                else
                {
                    throw new Exception("無法取得編號使用數，請聯絡管理員");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return count;
        }

        /// <summary>
        /// 取得ID編號
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public DataTable getHistory(string ID)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select GUID, Identifier, DoAction, MaterialType, ID, ");
                sql.Append(" MaterialName, UseCount, EditCount, Factory, MaterialState, ");
                sql.Append(" RefNumber, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime ");
                sql.Append(" from Material_History ");
                sql.Append(" where ID = @ID ");
                sql.Append(" order by UpdateTime desc ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@ID", System.Data.SqlDbType.NVarChar);
                parameter.Value = ID.Trim();
                SqlParameters.Add(parameter);
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 新增量/報廢量統計表
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public DataTable getStatisticsCount(MaterialSummaryEntity entity, string startDate, string endDate)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select 'A' as state, MaterialType, ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime, ");
                sql.Append(" MaterialType + '#' + ID + '#' + MaterialName + '#' + convert(varchar(10), UseCount) + '#' + Factory + '#' + MaterialState + '#' + CreateDate + '#' + StartUseDate + '#' + ScrapDate + '#' + RefNumber + '#' + convert(varchar(10), CancelFactory) + '#' + Remark as allData ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where 1 = 1 ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                //if (entity.MaterialState != null)
                //{
                //    sql.Append(" and MaterialState = @MaterialState ");
                //    SqlParameter parameter = new SqlParameter("@MaterialState", System.Data.SqlDbType.NVarChar);
                //    parameter.Value = entity.MaterialState.Trim();
                //    SqlParameters.Add(parameter);
                //}
                if (entity.Factory != null)
                {
                    sql.Append(" and Factory = @Factory ");
                    SqlParameter parameter = new SqlParameter("@Factory", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.Factory.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialType != null)
                {
                    sql.Append(" and MaterialType = @MaterialType ");
                    SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialType.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" and CreateDate  != '' ");
                if (startDate.Length > 0)
                {
                    sql.Append(" and CreateDate >= '").Append(startDate).Append("'");
                }

                if (endDate.Length > 0)
                {
                    sql.Append(" and CreateDate <= '").Append(endDate).Append("'");
                }

                sql.Append(" UNION ALL ");

                sql.Append(" select 'N' as state, MaterialType, ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime, ");
                sql.Append(" MaterialType + '#' + ID + '#' + MaterialName + '#' + convert(varchar(10), UseCount) + '#' + Factory + '#' + MaterialState + '#' + CreateDate + '#' + StartUseDate + '#' + ScrapDate + '#' + RefNumber + '#' + convert(varchar(10), CancelFactory) + '#' + Remark as allData ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where 1 = 1 ");

                if (entity.Factory != null)
                {
                    sql.Append(" and Factory = @Factory ");
                    SqlParameter parameter = new SqlParameter("@Factory", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.Factory.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialType != null)
                {
                    sql.Append(" and MaterialType = @MaterialType ");
                    SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialType.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" and ScrapDate  != '' ");
                if (startDate.Length > 0)
                {
                    sql.Append(" and ScrapDate >= '").Append(startDate).Append("'");
                }

                if (endDate.Length > 0)
                {
                    sql.Append(" and ScrapDate <= '").Append(endDate).Append("'");
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 使用次數/使用紀錄統計表
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dateColumn"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getStatisticsRecord(MaterialSummaryEntity entity, string dateColumn, string startDate, string endDate, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select MaterialType, Material_Summary.ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, Remark, ");
                sql.Append(" isnull(TotalCount, 0) - CancelFactory as TotalCount ");
                sql.Append(" from Material_Summary ");
                sql.Append(" left join (select ID, count(ID) as TotalCount from Material_History where DoAction = 'O' group by ID) Material_History ");
                sql.Append(" on Material_Summary.ID = Material_History.ID ");
                sql.Append(" where 1 = 1 ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                if (entity.Factory != null)
                {
                    sql.Append(" and Factory = @Factory ");
                    SqlParameter parameter = new SqlParameter("@Factory", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.Factory.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialType != null)
                {
                    sql.Append(" and MaterialType = @MaterialType ");
                    SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialType.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" and ").Append(dateColumn).Append("  != '' ");
                if (startDate.Length > 0)
                {
                    sql.Append(" and ").Append(dateColumn).Append(" >= '").Append(startDate).Append("'");
                }

                if (endDate.Length > 0)
                {
                    sql.Append(" and ").Append(dateColumn).Append(" <= '").Append(endDate).Append("'");
                }

                if (condition != null && condition.Trim().Length > 0)
                {
                    sql.Append(" and (charindex(@condition, Material_Summary.ID) > 0 ");
                    sql.Append(" or charindex(@condition, MaterialName) > 0) ");
                    SqlParameter parameter = new SqlParameter("@condition", System.Data.SqlDbType.NVarChar);
                    parameter.Value = condition.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 現有包裝材統計表
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="dateColumn"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public DataTable getStatisticsUsed(MaterialSummaryEntity entity, string dateColumn, string startDate, string endDate, string condition)
        {
            DataTable dt = new DataTable();
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" select MaterialType, ID, MaterialName, UseCount, Factory, ");
                sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                sql.Append(" UpdateEmpID, UpdateName, UpdateTime, ");
                sql.Append(" MaterialType + '#' + ID + '#' + MaterialName + '#' + convert(varchar(10), UseCount) + '#' + Factory + '#' + MaterialState + '#' + CreateDate + '#' + StartUseDate + '#' + ScrapDate + '#' + RefNumber + '#' + convert(varchar(10), CancelFactory) + '#' + Remark as allData ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where 1 = 1 ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                if (entity.MaterialState != null)
                {
                    sql.Append(" and MaterialState = @MaterialState ");
                    SqlParameter parameter = new SqlParameter("@MaterialState", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialState.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.Factory != null)
                {
                    sql.Append(" and Factory = @Factory ");
                    SqlParameter parameter = new SqlParameter("@Factory", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.Factory.Trim();
                    SqlParameters.Add(parameter);
                }
                if (entity.MaterialType != null)
                {
                    sql.Append(" and MaterialType = @MaterialType ");
                    SqlParameter parameter = new SqlParameter("@MaterialType", System.Data.SqlDbType.NVarChar);
                    parameter.Value = entity.MaterialType.Trim();
                    SqlParameters.Add(parameter);
                }

                sql.Append(" and ").Append(dateColumn).Append("  != '' ");
                if (startDate.Length > 0)
                {
                    sql.Append(" and ").Append(dateColumn).Append(" >= '").Append(startDate).Append("'");
                }

                if (endDate.Length > 0)
                {
                    sql.Append(" and ").Append(dateColumn).Append(" <= '").Append(endDate).Append("'");
                }

                if (condition != null && condition.Trim().Length > 0)
                {
                    sql.Append(" and (charindex(@condition, ID) > 0 ");
                    sql.Append(" or charindex(@condition, MaterialName) > 0) ");
                    SqlParameter parameter = new SqlParameter("@condition", System.Data.SqlDbType.NVarChar);
                    parameter.Value = condition.Trim();
                    SqlParameters.Add(parameter);
                }
                sql.Append(" order by ID ");
                dt = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return dt;
        }

        /// <summary>
        /// 執行MSSQL
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        private DataTable ExecuteSql(string dbType, string sqlText, List<SqlParameter> parameters, string cs)
        {
            try
            {
                Boolean test = true;
                if (test)
                {
                    DataSet ds = new DataSet();
                    DataTable dt = new DataTable();

                    ds = GetDataSourceMSSQL(sqlText, parameters, cs);

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        dt = ds.Tables[0];
                    }
                    return dt;
                }
                else
                {
                    throw new Exception("驗證失敗");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 取得回傳的資料集
        /// </summary>
        /// <param name="sSqlCmd"></param>
        /// <param name="parameters"></param>
        /// <param name="connectionStringCfg"></param>
        /// <returns></returns>
        private DataSet GetDataSourceMSSQL(string sSqlCmd, List<SqlParameter> parameters, string connectionStringCfg)
        {
            DataSet dataSource = new DataSet();
            SqlConnection conn = new SqlConnection(connectionStringCfg);
            try
            {
                using (conn)
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandText = sSqlCmd;
                    cmd.Connection = conn;
                    if (parameters != null)
                    {
                        foreach (var parameter in parameters)
                        {
                            cmd.Parameters.Add(parameter);
                        }
                    }
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(cmd);
                    sqlDataAdapter.Fill(dataSource, "DATA");                    
                }
                return dataSource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                conn.Close();
            }
        }

        /// <summary>
        /// 取得Local連線字串,依系統環境切換
        /// </summary>
        /// <returns></returns>
        private static string getLocal()
        {
            string tStr = "";
            //測試區
            if (System.Web.Configuration.WebConfigurationManager.AppSettings["TEST"].Trim().ToUpper() == "Y")
            {
                tStr = @"Data Source=127.0.0.1;Initial Catalog=PackageRecycle;User ID=sa;Password=sa;Connect Timeout=5";
            }
            else
            {
                tStr = @"Data Source=127.0.0.1;Initial Catalog=PackageRecycle;User ID=sa;Password=sa;Connect Timeout=5";
            }
            return tStr;
        }
    }
}