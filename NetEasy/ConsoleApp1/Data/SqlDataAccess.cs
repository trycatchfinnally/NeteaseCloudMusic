using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
 

namespace NeteaseCloudMusic.Core.Data
{
    public class SqlDataAccess : DataAccessBase
    {
        private bool? _isSql2012AndLater;
        private bool _isUnicodeSqlString = true;
        private SqlConnection mSqlConnection
        {
            get
            {
                return (SqlConnection)base.ConnectionInstance;
            }
        }
        public string Database
        {
            get
            {
                return this.mSqlConnection.Database;
            }
        }
        public string DataSource
        {
            get
            {
                return this.mSqlConnection.DataSource;
            }
        }
        protected override int FalseValue => 0;
        protected override int TrueValue => 1;

        private bool IsSql2012AndLater
        {
            get
            {
                if (!this._isSql2012AndLater.HasValue)
                {
                    this._isSql2012AndLater = false;
                    try
                    {
                        string str = this.ServerVersion.Substring(0, this.ServerVersion.IndexOf("."));
                        this._isSql2012AndLater = new bool?(Convert.ToInt32(str) >= 11);
                    }
                    catch (Exception exception)
                    {
                        throw new Exception("判断 SQL Server 版本是否 SQL SERVER 2012 及以后版本时发生错误：" + exception.Message);
                    }
                }
                return this._isSql2012AndLater.Value;
            }
        }
        /// <summary>
        /// 是否对SQL语句中的字符串常量进行Unicode化，如在前面加上“N”。 
        /// </summary>
        public bool IsUnicodeSqlString
        {
            get
            {
                return this._isUnicodeSqlString;
            }
            set
            {
                this._isUnicodeSqlString = value;
            }
        }
        protected override string LikeString => "%";

        protected override string ParameterPrefixStringInSql => "@";
        public string ServerVersion
        {
            get
            {
                return this.mSqlConnection.ServerVersion;
            }
        }
        [Unity.Attributes.InjectionConstructor]
        public SqlDataAccess(ILoggerFacade logger) : this(DataSettingManager.LoadSetting().DataConnectionString, logger)
        {
           
        }
        
        public SqlDataAccess(SqlConnection connection, ILoggerFacade logger) : base(connection, true,logger)
        {
            this._isUnicodeSqlString = true;
        }


        public SqlDataAccess(string connectionString, ILoggerFacade logger) : base(connectionString,logger)
        {

            this._isUnicodeSqlString = true;

        }



        public SqlDataAccess(SqlConnection connection, bool canClose, ILoggerFacade logger) : base(connection, canClose,logger )
        {
            this._isUnicodeSqlString = true;
        }

        protected override IDbCommand CreateCommandInstance()
        {
            SqlCommand command = new SqlCommand
            {
                Connection = this.mSqlConnection
            };
            if (base.TransactionInstance != null)
            {
                command.Transaction = (SqlTransaction)base.TransactionInstance;
            }
            return command;
        }


        protected override IDbConnection CreateConnectionInstance()
        {
            return new SqlConnection();

        }
        protected override IDbDataAdapter CreateDataAdapterInstance()
        {
            return new SqlDataAdapter((SqlCommand)this.CreateCommandInstance());

        }
        protected override string EncodeFieldEntityName(string fieldName)
        {
            if (fieldName == null)
            {
                throw new ArgumentNullException("fieldName");
            }

            StringBuilder sb = new StringBuilder();
            var separator = new[] { '.' };
            string[] strArray = fieldName.Split(separator);
            for (int i = 0; i < strArray.Length; i++)
            {
                strArray[i] = strArray[i].Trim();
                if (!string.IsNullOrEmpty(strArray[i]) && !string.IsNullOrEmpty(strArray[i].Trim()))
                {
                    if (strArray[i].StartsWith("[") && strArray[i].EndsWith("]"))
                    {
                        sb.Append(strArray[i]);

                    }
                    else
                    {
                        sb.Append($"[{strArray[i].Replace("[", "[[").Replace("]", "]]")}]");

                    }
                }
                if (i < (strArray.Length - 1))
                {
                    sb.Append(".");
                }
            }
            return sb.ToString();

        }
        protected override string EncodeTableEntityName(string tableName)
        {
            return this.EncodeFieldEntityName(tableName);
        }
        protected override string EndProcessStringSqlValueString(string stringSqlValue)
        {
            if (this.IsUnicodeSqlString)
            {
                return ("N" + base.EndProcessStringSqlValueString(stringSqlValue));
            }
            return base.EndProcessStringSqlValueString(stringSqlValue);
        }

        protected override void FillImpl(DataSet dataSet, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            base.CheckConnection();
            base.DebugSql(commandText, commandParameters);
            SqlCommand selectCommand = null;
            try
            {
                selectCommand = (SqlCommand)GetCommand(commandType, commandText, commandParameters);
                new SqlDataAdapter(selectCommand).Fill(dataSet);
            }
            catch (Exception e)
            {
                base.DebugSql(commandText, commandParameters, e);
                throw;
            }
            finally
            {
                try
                {
                    if (selectCommand != null)
                    {
                        selectCommand.Parameters.Clear();
                    }
                }
                catch
                {
                }
            }
        }
        protected override void FillImpl(DataTable dataTable, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            base.CheckConnection();
            base.DebugSql(commandText, commandParameters);
            SqlCommand selectCommand = null;
            try
            {
                selectCommand = (SqlCommand)base.GetCommand(commandType, commandText, commandParameters);
                new SqlDataAdapter(selectCommand).Fill(dataTable);
            }
            catch(Exception e )
            {
                base.DebugSql(commandText, commandParameters,e );

                throw;
            }
            finally
            {
                try
                {
                    if (selectCommand != null)
                    {
                        selectCommand.Parameters.Clear();
                    }
                }
                catch
                {
                }
            }
        }




        protected override string GetDateTimeSqlString(DateTime date, string format)
        {
            return ("'" + date.ToString(string.IsNullOrEmpty(format) ? @"yyyyMMdd HH\:mm\:ss" : format) + "'");
        }



        public override IDataParameter GetDataParameterInstance()
        {
            return new SqlParameter();
        }

        public override IDataParameter GetDataParameterInstance(string paramName, string dbTypeString)
        {
            SqlParameter parameter = new SqlParameter
            {
                ParameterName = paramName
            };
            if (!string.IsNullOrWhiteSpace(dbTypeString))
            {
                parameter.SqlDbType = (SqlDbType)Enum.Parse(typeof(SqlDbType), dbTypeString, true);
            }
            return parameter;
        }


        public override IDataParameter GetDataParameterInstance(string paramName, string dbTypeString, int size)
        {
            SqlParameter dataParameterInstance = this.GetDataParameterInstance(paramName, dbTypeString) as SqlParameter;
            dataParameterInstance.Size = size;
            return dataParameterInstance;
        }






        public void ChangeDatabase(string database)
        {
            this.mSqlConnection.ChangeDatabase(database);
        }


        public override int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters)
        {
            if (((base.ReturnRowCountMaxPages > 0) && ((sumFields == null) || (sumFields.Count == 0))) && this.IsSql2012AndLater)
            {
                if (pageSize <= 0)
                {
                    throw new ArgumentException("参数值必须大于0。", "pageSize");
                }
                if (curPageNum <= 0)
                {
                    throw new ArgumentException("参数值必须大于0。", "curPageNum");
                }
                var textArray1 = new[] { selectSql, " OFFSET ", pageSize.ToString(), " * (", curPageNum.ToString(), "-1) ROW FETCH NEXT ", (base.ReturnRowCountMaxPages * pageSize).ToString(), " ROWS ONLY " };
                selectSql = string.Concat(textArray1);
                int num = 1;
                return (base.FillPaginalData<T>(list, selectSql, pageSize, num, sumFields, commandParameters) + ((curPageNum - 1) * pageSize));
            }
            return base.FillPaginalData<T>(list, selectSql, pageSize, curPageNum, sumFields, commandParameters);
        }

        public override int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters)
        {
            if (((base.ReturnRowCountMaxPages > 0) && ((sumFields == null) || (sumFields.Count == 0))) && this.IsSql2012AndLater)
            {
                if (pageSize <= 0)
                {
                    throw new ArgumentException("参数值必须大于0。", "pageSize");
                }
                if (curPageNum <= 0)
                {
                    throw new ArgumentException("参数值必须大于0。", "curPageNum");
                }
                var textArray1 = new[] { selectSql, " OFFSET ", pageSize.ToString(), " * (", curPageNum.ToString(), "-1) ROW FETCH NEXT ", (base.ReturnRowCountMaxPages * pageSize).ToString(), " ROWS ONLY " };
                selectSql = string.Concat(textArray1);
                int num = 1;
                return (base.FillPaginalData(dataTable, selectSql, pageSize, num, sumFields, commandParameters) + ((curPageNum - 1) * pageSize));
            }
            return base.FillPaginalData(dataTable, selectSql, pageSize, curPageNum, sumFields, commandParameters);
        }
        public override DataTable GetSchema()
        {
            return this.mSqlConnection.GetSchema();
        }



        public override DataTable GetSchema(string collectionName)
        {
            return this.mSqlConnection.GetSchema(collectionName);
        }



        public override DataTable GetSchema(string collectionName, string[] restrictionValues)
        {
            return this.mSqlConnection.GetSchema(collectionName, restrictionValues);
        }
        public void RollbackTransaction(string savePintName)
        {
            if (!base.IsTransaction)
            {
                throw new InvalidOperationException("还未开始事务");
            }
    ((SqlTransaction)base.TransactionInstance).Rollback(savePintName);
        }




        public void TransactionSave(string savePintName)
        {
            if (!base.IsTransaction)
            {
                throw new InvalidOperationException("还未开始事务");
            }
          ((SqlTransaction)base.TransactionInstance).Save(savePintName);
        }











    }
}
