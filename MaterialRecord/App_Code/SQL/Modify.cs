using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace MaterialRecord
{
    public class Modify
    {
        /// <summary>
        /// 新增包裝材總表/歷史紀錄
        /// </summary>
        /// <param name="summaryList"></param>
        /// <param name="historyList"></param>
        /// <returns></returns>
        public Boolean insertSummary(DataTable summaryList, DataTable historyList)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");
                for (int i = 0; i < summaryList.Rows.Count; i++)
                {
                    //Material_Summary
                    sql.Append(" insert into Material_Summary(MaterialType, ID, MaterialName, UseCount, Factory, ");
                    sql.Append(" MaterialState, CreateDate, StartUseDate, ScrapDate, RefNumber, ");
                    sql.Append(" CancelFactory, Remark, CreateEmpID, CreateName, CreateTime, ");
                    sql.Append(" UpdateEmpID, UpdateName, UpdateTime) ");
                    sql.Append(" values(");
                    for (int j = 0; j < summaryList.Columns.Count; j++)
                    {
                        sql.Append("'").Append(summaryList.Rows[i][j]).Append("',");
                    }
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append("); ");

                    //Material_History
                    sql.Append(" insert into Material_History(GUID, Identifier, DoAction, MaterialType, ID, ");
                    sql.Append(" MaterialName, UseCount, EditCount, Factory, MaterialState, ");
                    sql.Append(" RefNumber, Remark, CreateEmpID, CreateName, CreateTime, ");
                    sql.Append(" UpdateEmpID, UpdateName, UpdateTime) ");
                    sql.Append(" values(");
                    for (int j = 0; j < historyList.Columns.Count; j++)
                    {
                        sql.Append("'").Append(historyList.Rows[i][j]).Append("',");
                    }
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append("); ");
                }
                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新包裝材總表/新增歷史紀錄
        /// </summary>
        /// <param name="summaryColumns"></param>
        /// <param name="summaryList"></param>
        /// <param name="historyList"></param>
        /// <returns></returns>
        public Boolean updateSummary(string[] summaryColumns, DataTable summaryList, DataTable historyList)
        {
            string[] historyColumns = { "GUID", "Identifier", "DoAction", "MaterialType", "ID",
                             "MaterialName", "UseCount", "EditCount", "Factory", "MaterialState",
                             "RefNumber", "Remark", "CreateEmpID", "CreateName", "CreateTime",
                             "UpdateEmpID", "UpdateName", "UpdateTime" };
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");
                for (int i = 0; i < summaryList.Rows.Count; i++)
                {
                    //Material_Summary
                    sql.Append(" update Material_Summary set ");
                    for (int j = 0; j < summaryColumns.Length; j++)
                    {
                        if (summaryColumns[j].Equals("StartUseDate"))
                        {
                            sql.Append(summaryColumns[j]).Append(" = (case when StartUseDate = '' then '").Append(summaryList.Rows[i][summaryColumns[j]]).Append("' else StartUseDate end),");
                        }
                        else
                        {
                            sql.Append(summaryColumns[j]).Append(" = '").Append(summaryList.Rows[i][summaryColumns[j]]).Append("',");
                        }
                    }
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append(" where ID = '").Append(summaryList.Rows[i]["ID"]).Append("'; ");

                    //Material_History
                    sql.Append(" insert into Material_History (");
                    for (int j = 0; j < historyColumns.Length; j++)
                    {
                        sql.Append(historyColumns[j]).Append(",");
                    }
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append(") values (");

                    for (int j = 0; j < historyList.Columns.Count; j++)
                    {
                        sql.Append("'").Append(historyList.Rows[i][historyList.Columns[j]]).Append("',");
                    }
                    sql.Remove(sql.Length - 1, 1);
                    sql.Append(");");
                }
                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 返廠更新
        /// </summary>
        /// <param name="state"></param>
        /// <param name="summaryEntity"></param>
        /// <param name="historyEntity"></param>
        /// <returns></returns>
        public Boolean updateSummaryInFactory(string state, MaterialSummaryEntity summaryEntity, MaterialHistoryEntity historyEntity)
        {
            string[] historyColumns = { "GUID", "Identifier", "DoAction", "MaterialType", "ID",
                             "MaterialName", "UseCount", "EditCount", "Factory", "MaterialState",
                             "RefNumber", "Remark", "CreateEmpID", "CreateName", "CreateTime",
                             "UpdateEmpID", "UpdateName", "UpdateTime" };
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");

                //Material_Summary
                sql.Append(" update Material_Summary set ");
                
                switch (state)
                {
                    case "I":
                        sql.Append(" Factory = '").Append(summaryEntity.Factory).Append("', ");
                        sql.Append(" StartUseDate = (case when StartUseDate = '' then '").Append(summaryEntity.StartUseDate).Append("' else StartUseDate end), ");
                        break;
                    case "Y":
                        sql.Append(" MaterialState = '").Append(summaryEntity.MaterialState).Append("', ");
                        sql.Append(" ScrapDate = '").Append(summaryEntity.ScrapDate).Append("', ");
                        break;
                    case "N":
                        sql.Append(" MaterialState = '").Append(summaryEntity.MaterialState).Append("', ");
                        sql.Append(" ScrapDate = '").Append(summaryEntity.ScrapDate).Append("', ");
                        break;
                    default:
                        break;
                }
                sql.Append(" Remark = '").Append(summaryEntity.Remark).Append("', ");
                sql.Append(" UpdateEmpID = '").Append(summaryEntity.UpdateEmpID).Append("', ");
                sql.Append(" UpdateName = '").Append(summaryEntity.UpdateName).Append("', ");
                sql.Append(" UpdateTime = '").Append(summaryEntity.UpdateTime).Append("' ");
                sql.Append(" where ID = '").Append(summaryEntity.ID).Append("'; ");

                //Material_History
                sql.Append(" insert into Material_History (");
                for (int j = 0; j < historyColumns.Length; j++)
                {
                    sql.Append(historyColumns[j]).Append(",");
                }
                sql.Remove(sql.Length - 1, 1);
                sql.Append(") ");

                sql.Append(" select '").Append(historyEntity.GUID).Append("', ");
                sql.Append(" '").Append(historyEntity.Identifier).Append("', ");
                sql.Append(" '").Append(historyEntity.DoAction).Append("', ");
                sql.Append(" MaterialType, ID, MaterialName, UseCount, UseCount, ");
                
                switch (state)
                {
                    case "I":
                        sql.Append(" '").Append(historyEntity.Factory).Append("', ");
                        sql.Append(" MaterialState, ");
                        break;
                    default:
                        sql.Append(" Factory, ");
                        sql.Append(" '").Append(historyEntity.MaterialState).Append("', ");
                        break;
                }
                sql.Append(" RefNumber, ");
                sql.Append(" '").Append(historyEntity.Remark).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateEmpID).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateName).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateTime).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateEmpID).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateName).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateTime).Append("' ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where ID = '").Append(historyEntity.ID).Append("'; ");
                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 出廠更新
        /// </summary>
        /// <param name="state"></param>
        /// <param name="summaryEntity"></param>
        /// <param name="historyEntity"></param>
        /// <returns></returns>
        public Boolean updateSummaryOutFactory(string state, MaterialSummaryEntity summaryEntity, MaterialHistoryEntity historyEntity)
        {
            string[] historyColumns = { "GUID", "Identifier", "DoAction", "MaterialType", "ID",
                             "MaterialName", "UseCount", "EditCount", "Factory", "MaterialState",
                             "RefNumber", "Remark", "CreateEmpID", "CreateName", "CreateTime",
                             "UpdateEmpID", "UpdateName", "UpdateTime" };
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");

                //Material_History
                sql.Append(" insert into Material_History (");
                for (int j = 0; j < historyColumns.Length; j++)
                {
                    sql.Append(historyColumns[j]).Append(",");
                }
                sql.Remove(sql.Length - 1, 1);
                sql.Append(") ");

                sql.Append(" select '").Append(historyEntity.GUID).Append("', ");
                sql.Append(" '").Append(historyEntity.Identifier).Append("', ");
                sql.Append(" '").Append(historyEntity.DoAction).Append("', ");
                sql.Append(" MaterialType, ID, MaterialName, UseCount, ");

                switch (state)
                {
                    case "R":
                        sql.Append(" '").Append(historyEntity.EditCount).Append("', ");
                        sql.Append(" Factory, MaterialState, RefNumber, ");
                        break;
                    case "O":
                        sql.Append(" UseCount - 1, ");
                        sql.Append(" '").Append(historyEntity.Factory).Append("', ");
                        sql.Append(" MaterialState, ");
                        sql.Append(" '").Append(historyEntity.RefNumber).Append("', ");
                        break;
                    case "C":
                        sql.Append(" UseCount + 1, ");
                        sql.Append(" '").Append(historyEntity.Factory).Append("', ");
                        sql.Append(" MaterialState, RefNumber, ");
                        break;
                    default:
                        sql.Append(" UseCount, Factory, ");
                        sql.Append(" '").Append(historyEntity.MaterialState).Append("', ");
                        sql.Append(" RefNumber, ");
                        break;
                }
                //sql.Append(" '").Append(historyEntity.RefNumber).Append("', ");
                sql.Append(" '").Append(historyEntity.Remark).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateEmpID).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateName).Append("', ");
                sql.Append(" '").Append(historyEntity.CreateTime).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateEmpID).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateName).Append("', ");
                sql.Append(" '").Append(historyEntity.UpdateTime).Append("' ");
                sql.Append(" from Material_Summary ");
                sql.Append(" where ID = '").Append(historyEntity.ID).Append("'; ");

                //Material_Summary
                sql.Append(" update Material_Summary set ");
                switch (state)
                {
                    case "O":
                        sql.Append(" Factory = '").Append(summaryEntity.Factory).Append("', ");
                        sql.Append(" UseCount = UseCount - 1, ");
                        sql.Append(" StartUseDate = (case when StartUseDate = '' then '").Append(summaryEntity.StartUseDate).Append("' else StartUseDate end), ");
                        sql.Append(" RefNumber = '").Append(summaryEntity.RefNumber).Append("', ");
                        break;
                    case "Y":
                        sql.Append(" MaterialState = '").Append(summaryEntity.MaterialState).Append("', ");
                        sql.Append(" ScrapDate = '', ");
                        break;
                    case "N":
                        sql.Append(" MaterialState = '").Append(summaryEntity.MaterialState).Append("', ");
                        sql.Append(" ScrapDate = '").Append(summaryEntity.ScrapDate).Append("', ");
                        break;
                    case "R":
                        sql.Append(" UseCount = '").Append(summaryEntity.UseCount).Append("', ");
                        sql.Append(" StartUseDate = (case when StartUseDate = '' then '").Append(summaryEntity.StartUseDate).Append("' else StartUseDate end), ");

                        break;
                    case "C":
                        sql.Append(" Factory = '").Append(summaryEntity.Factory).Append("', ");
                        sql.Append(" UseCount = UseCount + 1, ");
                        sql.Append(" CancelFactory = CancelFactory + 1, ");
                        break;
                }
                sql.Append(" Remark = '").Append(summaryEntity.Remark).Append("', ");
                sql.Append(" UpdateEmpID = '").Append(summaryEntity.UpdateEmpID).Append("', ");
                sql.Append(" UpdateName = '").Append(summaryEntity.UpdateName).Append("', ");
                sql.Append(" UpdateTime = '").Append(summaryEntity.UpdateTime).Append("' ");
                sql.Append(" where ID = '").Append(summaryEntity.ID).Append("'; ");

                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 可使用次數更新
        /// </summary>
        /// <param name="summaryList"></param>
        /// <param name="historyList"></param>
        /// <returns></returns>
        public Boolean updateSummaryRecycling(DataTable summaryList, DataTable historyList)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");
                for (int i = 0; i < summaryList.Rows.Count; i++)
                {
                    //Material_History
                    sql.Append(" insert into Material_History(GUID, Identifier,  DoAction, MaterialType, ID, ");
                    sql.Append(" MaterialName, UseCount, EditCount, Factory, MaterialState, ");
                    sql.Append(" RefNumber, Remark, CreateEmpID, CreateName, CreateTime, ");
                    sql.Append(" UpdateEmpID, UpdateName, UpdateTime) ");
                    sql.Append(" select '").Append(historyList.Rows[i]["GUID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["Identifier"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["DoAction"]).Append("', ");
                    sql.Append(" MaterialType, ID, MaterialName, UseCount, ");
                    sql.Append(" '").Append(historyList.Rows[i]["EditCount"]).Append("', ");
                    sql.Append(" Factory, MaterialState, RefNumber, ");
                    sql.Append(" '").Append(historyList.Rows[i]["Remark"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateEmpID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateName"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateTime"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateEmpID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateName"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateTime"]).Append("' ");
                    sql.Append(" from Material_Summary ");
                    sql.Append(" where ID = '").Append(historyList.Rows[i]["ID"]).Append("'; ");

                    //Material_Summary
                    sql.Append(" update Material_Summary ");
                    sql.Append(" set UseCount = '").Append(summaryList.Rows[i]["UseCount"]).Append("', ");
                    sql.Append(" StartUseDate = (case when StartUseDate = '' then '").Append(summaryList.Rows[i]["StartUseDate"]).Append("' else StartUseDate end), ");
                    sql.Append(" Remark = '").Append(summaryList.Rows[i]["Remark"]).Append("', ");
                    sql.Append(" UpdateEmpID = '").Append(summaryList.Rows[i]["UpdateEmpID"]).Append("', ");
                    sql.Append(" UpdateName = '").Append(summaryList.Rows[i]["UpdateName"]).Append("', ");
                    sql.Append(" UpdateTime = '").Append(summaryList.Rows[i]["UpdateTime"]).Append("' ");
                    sql.Append(" where ID = '").Append(summaryList.Rows[i]["ID"]).Append("'; ");
                }
                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 報廢更新
        /// </summary>
        /// <param name="summaryList"></param>
        /// <param name="historyList"></param>
        /// <returns></returns>
        public Boolean updateSummaryScrap(DataTable summaryList, DataTable historyList)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("BEGIN TRY BEGIN TRAN; ");
                for (int i = 0; i < summaryList.Rows.Count; i++)
                {
                    //Material_Summary
                    sql.Append(" update Material_Summary ");
                    sql.Append(" set MaterialState = '").Append(summaryList.Rows[i]["MaterialState"]).Append("', ");
                    sql.Append(" ScrapDate = '").Append(summaryList.Rows[i]["ScrapDate"]).Append("', ");
                    sql.Append(" Remark = '").Append(summaryList.Rows[i]["Remark"]).Append("', ");
                    sql.Append(" UpdateEmpID = '").Append(summaryList.Rows[i]["UpdateEmpID"]).Append("', ");
                    sql.Append(" UpdateName = '").Append(summaryList.Rows[i]["UpdateName"]).Append("', ");
                    sql.Append(" UpdateTime = '").Append(summaryList.Rows[i]["UpdateTime"]).Append("' ");
                    sql.Append(" where ID = '").Append(summaryList.Rows[i]["ID"]).Append("'; ");

                    //Material_History
                    sql.Append(" insert into Material_History(GUID, Identifier,  DoAction, MaterialType, ID, ");
                    sql.Append(" MaterialName, UseCount, EditCount, Factory, MaterialState, ");
                    sql.Append(" RefNumber, Remark, CreateEmpID, CreateName, CreateTime, ");
                    sql.Append(" UpdateEmpID, UpdateName, UpdateTime) ");
                    sql.Append(" select '").Append(historyList.Rows[i]["GUID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["Identifier"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["DoAction"]).Append("', ");
                    sql.Append(" MaterialType, ID, MaterialName, UseCount, UseCount, ");
                    sql.Append(" Factory, ");
                    sql.Append(" '").Append(historyList.Rows[i]["MaterialState"]).Append("', ");
                    sql.Append(" RefNumber, ");
                    sql.Append(" '").Append(historyList.Rows[i]["Remark"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateEmpID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateName"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["CreateTime"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateEmpID"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateName"]).Append("', ");
                    sql.Append(" '").Append(historyList.Rows[i]["UpdateTime"]).Append("' ");
                    sql.Append(" from Material_Summary ");
                    sql.Append(" where ID = '").Append(historyList.Rows[i]["ID"]).Append("'; ");
                }
                sql.Append("COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增包裝類別
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Boolean insertMaterialType(MaterialTypeEntity entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" BEGIN TRY BEGIN TRAN; ");
                sql.Append(" insert into Material_Type (ID, TypeName, Remark, CreateEmpID, CreateName, ");
                sql.Append(" CreateTime, UpdateEmpID, UpdateName, UpdateTime) ");
                sql.Append(" values(@ID, @TypeName, @Remark, @CreateEmpID, @CreateName, ");
                sql.Append(" @CreateTime, @UpdateEmpID, @UpdateName, @UpdateTime); ");
                sql.Append(" COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@ID", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ID.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@TypeName", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.TypeName.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@Remark", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.Remark.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@CreateEmpID", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.CreateEmpID.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@CreateName", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.CreateName.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@CreateTime", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.CreateTime.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateEmpID", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateEmpID.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateName", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateName.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateTime", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateTime.Trim();
                SqlParameters.Add(parameter);

                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 更新包裝類別
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Boolean updateMaterialType(MaterialTypeEntity entity)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" BEGIN TRY BEGIN TRAN; ");
                sql.Append(" update Material_Type ");
                sql.Append(" set TypeName = @TypeName, ");
                sql.Append(" Remark = @Remark, ");
                sql.Append(" UpdateEmpID = @UpdateEmpID, ");
                sql.Append(" UpdateName = @UpdateName, ");
                sql.Append(" UpdateTime = @UpdateTime ");
                sql.Append(" where ID = @ID;");
                sql.Append(" COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@TypeName", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.TypeName.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@Remark", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.Remark.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateEmpID", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateEmpID.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateName", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateName.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@UpdateTime", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateTime.Trim();
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@ID", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ID.Trim();
                SqlParameters.Add(parameter);

                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 刪除包裝類別
        /// </summary>
        /// <param name="ID"></param>
        /// <returns></returns>
        public Boolean deleteMaterialType(string ID)
        {
            try
            {
                StringBuilder sql = new StringBuilder();
                sql.Append(" BEGIN TRY BEGIN TRAN; ");
                sql.Append(" delete from Material_Type ");
                sql.Append(" where ID = @ID;");
                sql.Append(" COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");

                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@ID", System.Data.SqlDbType.NVarChar);
                parameter.Value = ID.Trim();
                SqlParameters.Add(parameter);

                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 新增LOG
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Boolean insertToLog(WasLogEntity entity)
        {
            try
            {
                return true;

                StringBuilder sql = new StringBuilder();
                sql.Append(" BEGIN TRY BEGIN TRAN; ");
                sql.Append(" insert into wasLog (exeMethod, exeScreen, exeButton, exeMsg, memo, ");
                sql.Append(" updateEmp, updateDate, updateTime, status) ");
                sql.Append(" values (@exeMethod, @exeScreen, @exeButton, @exeMsg, @memo, ");
                sql.Append(" @updateEmp, @updateDate, @updateTime, @status) ");
                sql.Append(" COMMIT TRAN; END TRY BEGIN CATCH ROLLBACK TRAN; THROW; END CATCH; ");
                List<SqlParameter> SqlParameters = new List<SqlParameter>();
                SqlParameter parameter = new SqlParameter("@exeMethod", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ExeMethod;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@exeScreen", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ExeScreen;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@exeButton", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ExeButton;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@exeMsg", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.ExeMsg;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@memo", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.Memo;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@updateEmp", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateEmp;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@updateDate", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateDate;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@updateTime", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.UpdateTime;
                SqlParameters.Add(parameter);
                parameter = new SqlParameter("@status", System.Data.SqlDbType.NVarChar);
                parameter.Value = entity.Status;
                SqlParameters.Add(parameter);

                DataSet ds = ExecuteSql("MSSQL", sql.ToString(), SqlParameters, getLocal());
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 執行MSSQL
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="sqlText"></param>
        /// <param name="parameters"></param>
        /// <param name="cs"></param>
        /// <returns></returns>
        private DataSet ExecuteSql(string dbType, string sqlText, List<SqlParameter> parameters, string cs)
        {
            try
            {
                //if (IsValidUser())
                Boolean test = true;
                if (test)
                {
                    DataSet ds = new DataSet();

                    ds = GetDataSourceMSSQL(sqlText, parameters, cs);

                    return ds;
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
        /// <param name="sSqlCmd">要執行的語法</param>
        /// <param name="connectionStringCfg">設定在WebConfig的連線字串"名稱"</param>
        /// <returns>DataSet</returns>
        private DataSet GetDataSourceMSSQL(string sSqlCmd, List<SqlParameter> parameters, string connectionStringCfg)
        {
            DataSet dataSource = new DataSet();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionStringCfg))
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
                    conn.Close();
                }
                return dataSource;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
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