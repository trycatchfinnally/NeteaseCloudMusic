using CommonServiceLocator;
 
using Prism.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
 
using System.Runtime.ConstrainedExecution;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    public abstract class DataAccessBase : IDataAccess
    {
        private string _batchExecuteSqlBeginString;
        private string _BatchExecuteSqlEndString;
        private int _commandTimeOut;
        private IDbConnection _connectionInstance;
        private IDbTransaction _dbTransaction;
        private bool _disposed;
        private static bool _enableTraceSqlOnError;
        private static bool _isDebugWriteSql;
        private bool _isNeedCloseConnection;
        private bool _isTransaction;
        private string _parameterPrefixStringInSql;
        private int _returnRowCountMaxPages;
        private string _sqlStatementSeparator;
        private readonly int CON_COMMANDTIMEOUT_DEFAULT = 30;
        private readonly int CON_RETURNROWCOUNTMAXPAGES_DEFAULT = 0;
       
        private readonly ILoggerFacade _logger;
        protected virtual string BatchExecuteSqlBeginString
        {
            get
            {
                return _batchExecuteSqlBeginString;
            }

            set
            {
                _batchExecuteSqlBeginString = value;
            }
        }

        protected virtual string BatchExecuteSqlEndString
        {
            get
            {
                return _BatchExecuteSqlEndString;
            }

            set
            {
                _BatchExecuteSqlEndString = value;
            }
        }

        protected virtual string BatchExecuteSqlSeparator
        {
            get
            {
                return _sqlStatementSeparator;
            }

            set
            {
                _sqlStatementSeparator = value;
            }
        }
        string IDataAccess.Database => this.ConnectionInstance.Database;
        public int CommandTimeOut
        {
            get
            {
                return this._commandTimeOut;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentException("连接超时不能小于0");
                }
                this._commandTimeOut = value;
            }
        }
        protected IDbConnection ConnectionInstance => this._connectionInstance;
        public string ConnectionString
        {
            get
            {
                return this.ConnectionInstance.ConnectionString;
            }
            set
            {
                this.ConnectionInstance.ConnectionString = value;
            }
        }
        public int ConnectionTimeout => ConnectionInstance.ConnectionTimeout;
        public bool Disposed => this._disposed;
        protected abstract int TrueValue { get; }

        protected abstract int FalseValue { get; }
        public bool IsTransaction => this._isTransaction;
        protected abstract string LikeString { get; }
        public virtual string ParameterPrefixInSql
        {
            get
            {
                return this.ParameterPrefixStringInSql;
            }
        }
        protected virtual string ParameterPrefixStringInSql
        {
            get
            {
                return this._parameterPrefixStringInSql;
            }
            set
            {
                this._parameterPrefixStringInSql = value;
            }
        }

        protected string ParamPrefixFullString
        {
            get
            {
                return (this.ParameterPrefixInSql + "p");
            }
        }
        public int ReturnRowCountMaxPages
        {
            get
            {
                return this._returnRowCountMaxPages;
            }
            set
            {
                this._returnRowCountMaxPages = value;
            }
        }

        public ConnectionState State => this.ConnectionInstance.State;

        protected IDbTransaction TransactionInstance => this._dbTransaction;

        public event EventHandler Closing;
        public event EventHandler Opened;
        
        public DataAccessBase(ILoggerFacade logger)
        {
            this._isNeedCloseConnection = true;

            this._parameterPrefixStringInSql = "";
            // this.SqlStatementSeparator = ";\r\n";
            this.BatchExecuteSqlBeginString = "BEGIN\r\n";
            this.BatchExecuteSqlEndString = "END;";
            this._returnRowCountMaxPages = (CON_RETURNROWCOUNTMAXPAGES_DEFAULT < 0) ? 0 : CON_RETURNROWCOUNTMAXPAGES_DEFAULT;
            this._commandTimeOut = (CON_COMMANDTIMEOUT_DEFAULT < 0) ? 30 : CON_COMMANDTIMEOUT_DEFAULT;

            //this._sqlTextCacheList = new List<string>();
            this._connectionInstance = this.CreateConnectionInstance();
            this._logger = logger;

        }
        public DataAccessBase(string connectionString, ILoggerFacade logger) : this(logger)
        {
            this.ConnectionString = connectionString;
            this.Open();
        }
        protected DataAccessBase(IDbConnection connection, bool canClose,ILoggerFacade logger)
        {
            this._isNeedCloseConnection = true;

            this._parameterPrefixStringInSql = "";
            // this.SqlStatementSeparator = ";\r\n";
            this.BatchExecuteSqlBeginString = "BEGIN\r\n";
            this.BatchExecuteSqlEndString = "END;";
            this._returnRowCountMaxPages = (CON_RETURNROWCOUNTMAXPAGES_DEFAULT < 0) ? 0 : CON_RETURNROWCOUNTMAXPAGES_DEFAULT;
            this._commandTimeOut = (CON_COMMANDTIMEOUT_DEFAULT < 0) ? 30 : CON_COMMANDTIMEOUT_DEFAULT;


            // this._sqlTextCacheList = new List<string>();
            this._connectionInstance = connection;
            this._isNeedCloseConnection = canClose;

        }
        private int BatchInsertDataReader(IDataReader reader, string tableName, UpdateFields fields, int buffer)
        {
            //Argument.CheckParameterNull(reader, "reader");
            //Argument.CheckStringParameter(tableName, "tableName");
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));
            if (string.IsNullOrWhiteSpace(tableName))
                throw new ArgumentNullException(nameof(tableName));
            if (reader.IsClosed)
            {
                throw new ArgumentException("提供的 System.Data.IDataReader 对象处于关闭状态。");
            }
            if (buffer <= 0)
            {
                buffer = 1;
            }
            bool appendSeparator = buffer > 1;
            int num = 0;
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();
            int num2 = 0;
            int num3;
            try
            {
                while (reader.Read())
                {
                    if (this.GenerateInsertDataReaderSql(sqlBuffer, parameterList, parameterList.Count, reader, tableName, fields, appendSeparator))
                    {
                        num2++;
                        if (num2 == buffer)
                        {
                            if (appendSeparator)
                            {
                                sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                sqlBuffer.Append(this.BatchExecuteSqlEndString);
                            }
                            if (parameterList.Count > 0)
                            {
                                num += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                            }
                            else
                            {
                                num += this.ExecuteNonQuery(sqlBuffer.ToString());
                            }
                            num2 = 0;
                            parameterList.Clear();
                            sqlBuffer.Remove(0, sqlBuffer.Length);
                        }
                    }
                }
                if (num2 > 0)
                {
                    if (appendSeparator)
                    {
                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                    }
                    if (parameterList.Count > 0)
                    {
                        num += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                    }
                    else
                    {
                        num += this.ExecuteNonQuery(sqlBuffer.ToString());
                    }
                    num2 = 0;
                    parameterList.Clear();
                    sqlBuffer.Remove(0, sqlBuffer.Length);
                }
                num3 = num;
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (parameterList != null)
                    {
                        parameterList.Clear();
                        parameterList = null;
                    }
                    if (sqlBuffer != null)
                    {
                        sqlBuffer.Remove(0, sqlBuffer.Length);
                        sqlBuffer = null;
                    }
                }
                catch
                {
                }
            }
            return num3;
        }
        private IDataParameter CreateParameter(string name, object value)
        {
            IDataParameter dataParameterInstance = this.GetDataParameterInstance();
            dataParameterInstance.Value = (value == null) ? DBNull.Value : value;
            dataParameterInstance.ParameterName = name;
            return dataParameterInstance;
        }

        private int DeleteNonDataTable(object obj, Type objType, PropertyInfo[] properties, string tableName, params string[] primaryKeyProperties)
        {
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (objType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException("Value cannot be null or empty.", "tableName");
            }
            if ((primaryKeyProperties == null) || (primaryKeyProperties.Length == 0))
            {
                throw new ArgumentException("The array length of parameter value cannot be 0.", "primaryKeyProperties");
            }
            if (properties.Length == 0)
            {
                return 0;
            }
            string strA = "";
            ArrayList list = null;
            int temp1 = 0;
            IDataParameter dataParameterInstance = null;
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            int temp2 = 0;
            sb1.Append($"DELETE  {tableName} WHERE ");
            PropertyInfo info = null;
            foreach (string str2 in primaryKeyProperties)
            {
                if (string.IsNullOrEmpty(str2))
                {
                    throw new ArgumentException("primaryKeyProperties cannot include null or empty.", "primaryKeyProperties");
                }
                if (sb2.Length > 0)
                {
                    sb2.Append(" AND ");
                }
                info = this.FindProperty(properties, str2);
                if (info == null)
                {
                    throw new ArgumentException("Property(" + str2 + ") is not exist in provied type.", "primaryKeyProperties");
                }
                strA = this.GetSqlValueString(info.GetValue(obj, null));
                if (strA != null)
                {
                    if (string.Compare(strA, "null", true) == 0)
                    {
                        sb2.Append(this.EncodeFieldEntityName(str2) + " IS " + strA);
                    }
                    else
                    {
                        sb2.Append(this.GetWhereConditionFieldExpress(tableName, str2) + " = " + this.GetWhereConditionValueExpress(tableName, str2, strA));
                    }
                }
                else
                {
                    temp1++;
                    sb2.Append(this.EncodeFieldEntityName(str2) + " = " + this.ParamPrefixFullString + temp1.ToString());
                    if (list == null)
                    {
                        list = new ArrayList();
                    }
                    dataParameterInstance = this.GetDataParameterInstance();
                    dataParameterInstance.ParameterName = this.ParamPrefixFullString + temp1.ToString();
                    dataParameterInstance.Value = info.GetValue(obj, null);
                    list.Add(dataParameterInstance);
                }
            }
            sb1.Append(sb2);
            sb1.Append(" ");
            if (sb1.Length <= 0)
            {
                return temp2;
            }
            if ((list == null) || (list.Count == 0))
            {
                return (temp2 + this.ExecuteNonQuery(sb1.ToString()));
            }
            return (temp2 + this.ExecuteNonQuery(sb1.ToString(), (IDataParameter[])list.ToArray(typeof(IDataParameter))));
        }
        protected void DebugSql(string commandText, IDataParameter[] commandParameters, Exception e = null)
        {
            StringBuilder sb = new StringBuilder(commandText);

            sb.AppendLine();
            if (commandParameters != null && commandParameters.Length > 0)
            {
                commandParameters.Each(x => sb.Append($"{x.ParameterName}={x.Value } "));
            }
            if (e == null)
            {
                sb.Insert(0, "开始执行sql->");
                this._logger.Info(sb.ToString());

            }
            else
            {
                sb.Insert(0, "出现错误，最后执行的sql为：");
                this._logger.Exception (sb.ToString(), e);
            }
        }


        private int ExecuteNonQueryImpl(CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            int result;
            this.DebugSql(commandText, commandParameters);

            IDbCommand command = null;
            try
            {
                command = this.GetCommand(commandType, commandText, commandParameters);
                result = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                this.DebugSql(commandText, commandParameters, e);
                throw;
            }
            finally
            {
                try
                {
                    if (command != null)
                    {
                        command.Parameters.Clear();
                        command.Dispose();
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        private void ExecuteQueryImpl(IList resultList, Type t, CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            if (resultList == null)
            {
                throw new ArgumentNullException("resultList");
            }
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            this.CheckConnection();
            DebugSql(commandText, commandParameters);
            IDbCommand command = null;
            IDataReader reader = null;
            try
            {
                command = this.GetCommand(commandType, commandText, commandParameters);
                reader = command.ExecuteReader();
                this.DataReaderToList(resultList, t, reader, 0, null, null);
            }
            catch (Exception e)
            {
                DebugSql(commandText, commandParameters, e);

                throw;
            }
            finally
            {
                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                    catch
                    {
                    }
                }
                if (command != null)
                {
                    try
                    {
                        command.Parameters.Clear();
                        command.Dispose();
                    }
                    catch
                    {
                    }
                }
            }
        }

        private IDataParameter[] NameValueArrayToParamValueArray(params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            if (parameterNameAndValues == null || parameterNameAndValues.Length == 0)
            {
                return null;
            }

            IDataParameter[] parameterArray = new IDataParameter[parameterNameAndValues.Length];
            for (int i = 0; i < parameterNameAndValues.Length; i++)
            {

                parameterArray[i] = this.CreateParameter(parameterNameAndValues[i].Key, parameterNameAndValues[i].Value);
            }
            return parameterArray;
        }
        private int FindInArray(string[] array, string findStr, bool igoreCase)
        {
            if ((array != null) && (array.Length != 0))
            {
                for (int i = 0; i < array.Length; i++)
                {
                    if (string.Compare(array[i], findStr, igoreCase) == 0)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        private PropertyInfo FindProperty(PropertyInfo[] properties, string findName)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (findName == null)
            {
                throw new ArgumentNullException("findName");
            }
            /*
            foreach (PropertyInfo info in properties)
            {
                if (string.Compare(info.Name, findName, true, CultureInfo.InvariantCulture) == 0)
                {
                    return info;
                }
            }*/
            return properties.FirstOrDefault(x => string.Compare(x.Name, findName, true, CultureInfo.InvariantCulture) == 0);

        }
        private bool GenerateInsertDataReaderSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, IDataReader insertReader, string tableName, UpdateFields fields)
        {
            return this.GenerateInsertDataReaderSql(sqlBuffer, parameterList, startParamIndex, insertReader, tableName, fields, false);
        }


        private bool GenerateInsertDataReaderSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, IDataReader insertReader, string tableName, UpdateFields fields, bool appendSeparator)
        {
            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }

            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (insertReader == null)
            {
                throw new ArgumentNullException("insertRow");
            }
            if (insertReader.FieldCount == 0)
            {
                return false;
            }
            bool result;
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            string sqlValueString = "";
            int num = startParamIndex;
            IDataParameter dataParameterInstance = null;
            try
            {
                sb1.AppendLine($"INSERT INTO {tableName}");
                string[] textArray1 = new string[insertReader.FieldCount];
                string field = "";
                for (int i = 0; i < insertReader.FieldCount; i++)
                {
                    field = insertReader.GetName(i);
                    if (fields != null)
                    {
                        if (fields.ContainsField(field) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                            continue;

                        /*
                        if (fields.ContainsField(field))
                        {
                            if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                            {
                                goto Label_00CB;
                            }
                            continue;
                        }
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }*/
                    }
                    //    Label_00CB:
                    if (sb2.Length == 0)
                    {
                        sb2.Append("(");
                    }
                    else
                    {
                        sb2.Append(",");
                    }
                    sb2.Append(this.EncodeFieldEntityName(field));
                    if (sb3.Length == 0)
                    {
                        sb3.Append("(");
                    }
                    else
                    {
                        sb3.Append(",");
                    }
                    sqlValueString = this.GetSqlValueString(insertReader.GetValue(i));
                    if (sqlValueString != null)
                    {
                        sb3.Append(this.GetUpdateValueExpress(tableName, field, sqlValueString));
                    }
                    else
                    {
                        num++;
                        sb3.Append(this.ParamPrefixFullString + num.ToString());
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = insertReader.GetValue(i);
                        parameterList.Add(dataParameterInstance);
                    }
                }
                if (sb2.Length == 0)
                {
                    return false;
                }
                sb2.Append(") ");
                sb3.Append(")");
                sb1.Append(" " + sb2.ToString() + " VALUES " + sb3.ToString());
                sqlBuffer.Append(sb1.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                sb1.Remove(0, sb1.Length);
                sb2.Remove(0, sb2.Length);
                sb3.Remove(0, sb3.Length);
            }
            return result;
        }
        private bool GenerateInsertDataRowSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, DataRow insertRow, string tableName, UpdateFields fields)
        {
            return this.GenerateInsertDataRowSql(sqlBuffer, parameterList, startParamIndex, insertRow, tableName, fields, false);
        }

        private bool GenerateInsertDataRowSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, DataRow insertRow, string tableName, UpdateFields fields, bool appendSeparator)
        {
            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (insertRow == null)
            {
                throw new ArgumentNullException("insertRow");
            }
            if (insertRow.Table.Columns.Count == 0)
            {
                return false;
            }
            bool result;
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            string sqlValueString = "";
            int num = startParamIndex;
            IDataParameter dataParameterInstance = null;
            try
            {
                DataTable table = insertRow.Table;
                string str2 = string.IsNullOrEmpty(tableName) ? table.TableName : tableName;
                DataRow row = insertRow;
                sb1.Append("INSERT INTO " + (string.IsNullOrEmpty(tableName) ? this.EncodeTableEntityName(table.TableName) : tableName));
                foreach (DataColumn column in table.Columns)
                {
                    if (fields != null)
                    {
                        if (fields.ContainsField(column.ColumnName) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                            continue;
                        /*
                        if (fields.ContainsField(column.ColumnName))
                        {
                            if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                            {
                                goto Label_0103;
                            }
                            continue;
                        }
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                        */
                    }
                    // Label_0103:
                    if (sb2.Length == 0)
                    {
                        sb2.Append("(");
                    }
                    else
                    {
                        sb2.Append(",");
                    }
                    sb2.Append(this.EncodeFieldEntityName(column.ColumnName));
                    if (sb3.Length == 0)
                    {
                        sb3.Append("(");
                    }
                    else
                    {
                        sb3.Append(",");
                    }
                    sqlValueString = this.GetSqlValueString(row[column.ColumnName]);
                    if (sqlValueString != null)
                    {
                        sb3.Append(this.GetUpdateValueExpress(str2, column.ColumnName, sqlValueString));
                    }
                    else
                    {
                        num++;
                        sb3.Append(this.ParamPrefixFullString + num.ToString());
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = row[column.ColumnName];
                        parameterList.Add(dataParameterInstance);
                    }
                }
                if (sb2.Length == 0)
                {
                    return false;
                }
                sb2.Append(") ");
                sb3.Append(")");
                sb1.Append(" " + sb2.ToString() + " VALUES " + sb3.ToString());
                sqlBuffer.Append(sb1.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                sb1.Remove(0, sb1.Length);
                sb2.Remove(0, sb2.Length);
                sb3.Remove(0, sb3.Length);
            }
            return result;
        }

        private bool GenerateInsertNonDataTableSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields, bool appendSeparator = false)
        {
            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (objType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException(nameof(tableName));
            }
            if (properties.Length == 0)
            {
                return false;
            }
            bool result;
            StringBuilder sb1 = new StringBuilder();
            StringBuilder sb2 = new StringBuilder();
            StringBuilder sb3 = new StringBuilder();
            try
            {
                var temp = obj as UpdatableDataModelBase;
                string sqlValueString = "";
                int num = startParamIndex;
                IDataParameter dataParameterInstance = null;
                sb1.Append("INSERT INTO " + tableName);
                foreach (PropertyInfo info in properties)
                {
                    if (!info.CanRead || (temp != null && !temp.CheckIsSetValue(info)))
                    {
                        continue;
                    }
                    var customAttribute = Attribute.GetCustomAttribute(info, typeof(ModelUpdatableAttribute)) as ModelUpdatableAttribute;
                    if ((customAttribute != null) && customAttribute.IsIdentity)
                    {
                        continue;
                    }
                    if (fields != null)
                    {
                        /*
                        if (fields.ContainsField(info.Name))
                        {
                            if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                            {
                                goto Label_013F;
                            }
                            continue;
                        }
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                        */
                        if (fields.ContainsField(info.Name) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                            continue;
                    }
                    // Label_013F:
                    if (sb2.Length == 0)
                    {
                        sb2.Append("(");
                    }
                    else
                    {
                        sb2.Append(",");
                    }
                    sb2.Append(this.EncodeFieldEntityName(info.Name));
                    if (sb3.Length == 0)
                    {
                        sb3.Append("(");
                    }
                    else
                    {
                        sb3.Append(",");
                    }
                    sqlValueString = this.GetSqlValueString(info.GetValue(obj, null));
                    if (sqlValueString != null)
                    {
                        sb3.Append(this.GetUpdateValueExpress(tableName, info.Name, sqlValueString));
                    }
                    else
                    {
                        num++;
                        sb3.Append(this.ParamPrefixFullString + num.ToString());
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = info.GetValue(obj, null);
                        parameterList.Add(dataParameterInstance);
                    }
                }
                if (sb2.Length == 0)
                {
                    sb2.Remove(0, sb2.Length);
                    sb3.Remove(0, sb3.Length);
                    sb1.Remove(0, sb1.Length);
                    return false;
                }
                sb2.Append(") ");
                sb3.Append(")");
                sb1.Append(" " + sb2.ToString() + " VALUES " + sb3.ToString());
                sqlBuffer.Append(sb1.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                sb2.Remove(0, sb2.Length);
                sb3.Remove(0, sb3.Length);
                sb1.Remove(0, sb1.Length);
            }
            return result;

        }



        private bool GenerateUpdateDataRowSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, DataRow updateRow, string tableName, UpdateFields fields, bool appendSeparator = false, params string[] updateConditionFields)
        {

            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (updateRow == null)
            {
                throw new ArgumentNullException("updateRow");
            }
            bool result;
            string str = tableName;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = this.EncodeTableEntityName(updateRow.Table.TableName);
                str = updateRow.Table.TableName;
            }
            string[] strArray = null;
            if ((updateConditionFields == null) || (updateConditionFields.Length == 0))
            {
                if ((updateRow.Table.PrimaryKey == null) || (updateRow.Table.PrimaryKey.Length == 0))
                {

                    throw new InvalidOperationException("没有对应的主键");
                }
                strArray = new string[updateRow.Table.PrimaryKey.Length];
                for (int i = 0; i < updateRow.Table.PrimaryKey.Length; i++)
                {
                    strArray[i] = updateRow.Table.PrimaryKey[i].ColumnName;
                }
            }
            else
            {
                strArray = updateConditionFields;
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            try
            {
                string sqlValueString = "";
                int num2 = startParamIndex;
                DataRow row = updateRow;
                builder.Append($"UPDATE {tableName } SET ");
                foreach (DataColumn column in updateRow.Table.Columns)
                {
                    if (fields != null)
                    {
                        /*
                        if (fields.ContainsField(column.ColumnName))
                        {
                            if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                            {
                                goto Label_017B;
                            }
                            continue;
                        }
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                        */
                        if (fields.ContainsField(column.ColumnName) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                    }
                    // Label_017B:
                    builder2.Append((builder2.Length == 0) ? "" : ",");
                    sqlValueString = this.GetSqlValueString(row[column.ColumnName]);
                    if (sqlValueString != null)
                    {
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.GetUpdateValueExpress(str, column.ColumnName, sqlValueString));
                    }
                    else
                    {
                        num2++;
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.ParamPrefixFullString + num2.ToString());
                        IDataParameter dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num2.ToString();
                        dataParameterInstance.Value = row[column.ColumnName];
                        parameterList.Add(parameterList);
                    }
                }
                if (builder2.Length == 0)
                {
                    return false;
                }
                builder.Append(builder2);
                builder.Append(" WHERE ");
                builder2.Clear();
                if (row.HasVersion(DataRowVersion.Original))
                {
                    foreach (string str3 in strArray)
                    {
                        if (builder2.Length > 0)
                        {
                            builder2.Append(" AND ");
                        }
                        sqlValueString = this.GetSqlValueString(row[str3, DataRowVersion.Original]);
                        if (sqlValueString != null)
                        {
                            builder2.Append(this.GetWhereConditionFieldExpress(str, str3) + " =" + this.GetWhereConditionValueExpress(str, str3, sqlValueString));
                        }
                        else
                        {
                            num2++;
                            builder2.Append(this.EncodeFieldEntityName(str3) + " = " + this.ParamPrefixFullString + num2.ToString());
                            IDataParameter parameter2 = this.GetDataParameterInstance();
                            parameter2.ParameterName = this.ParamPrefixFullString + num2.ToString();
                            parameter2.Value = row[str3, DataRowVersion.Original];
                            parameterList.Add(parameterList);
                        }
                    }
                }
                else
                {
                    foreach (string str4 in strArray)
                    {
                        if (builder2.Length > 0)
                        {
                            builder2.Append(" AND ");
                        }
                        sqlValueString = this.GetSqlValueString(row[str4]);
                        if (sqlValueString != null)
                        {
                            builder2.Append(this.GetWhereConditionFieldExpress(str, str4) + " = " + this.GetWhereConditionValueExpress(str, str4, sqlValueString));
                        }
                        else
                        {
                            num2++;
                            builder2.Append(this.EncodeFieldEntityName(str4) + " = " + this.ParamPrefixFullString + num2.ToString());
                            IDataParameter parameter3 = this.GetDataParameterInstance();
                            parameter3.ParameterName = this.ParamPrefixFullString + num2.ToString();
                            parameter3.Value = row[str4];
                            parameterList.Add(parameterList);
                        }
                    }
                }
                builder.Append(builder2);
                sqlBuffer.Append(builder.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                builder.Clear();
                builder2.Clear();
            }
            return result;
        }
        private bool GenerateUpdateNonDataTableSql(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields, bool appendSeparator = false, params string[] primaryKeyProperties)
        {
            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (objType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            if ((primaryKeyProperties == null) || (primaryKeyProperties.Length == 0))
            {
                throw new ArgumentException("未定义主键");
            }
            if (properties.Length == 0)
            {
                return false;
            }
            bool result;

            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            StringBuilder builder3 = new StringBuilder();
            
            try
            {
                string sqlValueString = "";
                int num = 0;
                IDataParameter dataParameterInstance = null;
                var temp = obj as UpdatableDataModelBase;
                builder.Append("UPDATE " + tableName + " SET ");
                foreach (PropertyInfo info2 in properties)
                {
                    if (!info2.CanRead || (temp != null && !temp.CheckIsSetValue(info2)))
                    {
                        continue;
                    }
                 var    customAttribute = Attribute.GetCustomAttribute(info2, typeof(ModelUpdatableAttribute)) as ModelUpdatableAttribute;
                    if (fields != null)
                    {
                        
                        if (fields.ContainsField(info2.Name) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                            continue;
                    }
                    // Label_015C:
                    sqlValueString = this.GetSqlValueString(info2.GetValue(obj));
                    if (customAttribute==null ||( !customAttribute.IsIdentity&&!customAttribute.IsIgnoreField))
                    {
                        if (sqlValueString != null)
                        {
                            if (this.FindInArray(primaryKeyProperties, info2.Name, true) >= 0)
                            {
                                builder3.Append((builder3.Length == 0) ? "" : ",");
                                builder3.Append(this.EncodeFieldEntityName(info2.Name) + " = " + this.GetUpdateValueExpress(tableName, info2.Name, sqlValueString));
                            }
                            else
                            {
                                builder2.Append((builder2.Length == 0) ? "" : ",");
                                builder2.Append(this.EncodeFieldEntityName(info2.Name) + " = " + this.GetUpdateValueExpress(tableName, info2.Name, sqlValueString));
                            }
                        }
                        else
                        {
                            num++;
                            if (this.FindInArray(primaryKeyProperties, info2.Name, true) >= 0)
                            {
                                builder3.Append((builder3.Length == 0) ? "" : ",");
                                builder3.Append(this.EncodeFieldEntityName(info2.Name) + " = " + this.ParamPrefixFullString + num.ToString());
                            }
                            else
                            {
                                builder2.Append((builder2.Length == 0) ? "" : ",");
                                builder2.Append(this.EncodeFieldEntityName(info2.Name) + " = " + this.ParamPrefixFullString + num.ToString());
                            }
                            dataParameterInstance = this.GetDataParameterInstance();
                            dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                            dataParameterInstance.Value = info2.GetValue(obj, null);
                            parameterList.Add(dataParameterInstance);
                        }
                    }
                }
                if ((builder2.Length == 0) && (builder3.Length == 0))
                {
                    return false;
                }
                if (builder2.Length > 0)
                {
                    builder.Append(builder2);
                }
                else
                {
                    builder.Append(builder3);
                }
                builder.Append(" WHERE ");
                builder2.Clear();
                builder3.Clear();
                
                foreach (string primaryKey in primaryKeyProperties)
                {
                    if (string.IsNullOrEmpty(primaryKey))
                    {
                        throw new ArgumentException("主键不能为空");
                    }
                    if (builder2.Length > 0)
                    {
                        builder2.Append(" AND ");
                    }
                    var info = this.FindProperty(properties, primaryKey);
                    if (info == null)
                    {

                        throw new ArgumentException("未找到对应的属性");
                    }
                    sqlValueString = this.GetSqlValueString(info.GetValue(obj));
                    if (sqlValueString != null)
                    {
                        if (string.Compare(sqlValueString, "null", true) == 0)
                        {
                            builder2.Append(this.EncodeFieldEntityName(primaryKey) + " IS " + sqlValueString);
                        }
                        else
                        {
                            builder2.Append(this.GetWhereConditionFieldExpress(tableName, primaryKey) + " = " + this.GetWhereConditionValueExpress(tableName, primaryKey, sqlValueString));
                        }
                    }
                    else
                    {
                        num++;
                        builder2.Append(this.EncodeFieldEntityName(primaryKey) + " = " + this.ParamPrefixFullString + num.ToString());
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = info.GetValue(obj, null);
                        parameterList.Add(dataParameterInstance);
                    }
                }
                builder.Append(builder2);
                sqlBuffer.Append(builder.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                builder.Clear();
                builder2.Clear();
                builder3.Clear();
            }
            return result;
        }
        private bool GenerateUpdateNonDataTableSqlByCondition(StringBuilder sqlBuffer, ArrayList parameterList, int startParamIndex, object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields, bool appendSeparator, string conditionSql)
        {
            if (sqlBuffer == null)
            {
                throw new ArgumentNullException("sqlBuffer");
            }
            if (parameterList == null)
            {
                throw new ArgumentNullException("parameterList");
            }
            if (startParamIndex < 0)
            {
                startParamIndex = 0;
            }
            if (obj == null)
            {
                throw new ArgumentNullException("obj");
            }
            if (objType == null)
            {
                throw new ArgumentNullException("type");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            if (properties.Length == 0)
            {
                return false;
            }
            bool result;
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            try
            {
                string sqlValueString = "";
                int num = 0;
                IDataParameter dataParameterInstance = null;
                var temp = obj as UpdatableDataModelBase;
                builder.Append($"UPDATE {tableName } SET ");
                foreach (PropertyInfo info in properties)
                {
                    if (!info.CanRead || (temp != null && !temp.CheckIsSetValue(info)))
                    {
                        continue;
                    }
                    if (fields != null)
                    {
                        /*
                        if (fields.ContainsField(info.Name))
                        {
                            if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                            {
                                goto Label_0116;
                            }
                            continue;
                        }
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                        */
                        if (fields.ContainsField(info.Name) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                            continue;
                        if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        {
                            continue;
                        }
                    }
                    // Label_0116:
                    sqlValueString = this.GetSqlValueString(info.GetValue(obj, null));
                    if (sqlValueString != null)
                    {
                        builder2.Append((builder2.Length == 0) ? "" : ",");
                        builder2.Append(this.EncodeFieldEntityName(info.Name) + " = " + this.GetUpdateValueExpress(tableName, info.Name, sqlValueString));
                    }
                    else
                    {
                        num++;
                        builder2.Append((builder2.Length == 0) ? "" : ",");
                        builder2.Append(this.EncodeFieldEntityName(info.Name) + " = " + this.ParamPrefixFullString + num.ToString());
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = info.GetValue(obj);
                        parameterList.Add(dataParameterInstance);
                    }
                }
                if (builder2.Length == 0)
                {
                    return false;
                }
                builder.Append(builder2);
                if (!string.IsNullOrWhiteSpace(conditionSql))
                {
                    builder.Append($" WHERE {conditionSql}");
                }
                sqlBuffer.Append(builder.ToString());
                if (appendSeparator)
                {
                    sqlBuffer.Append(this.BatchExecuteSqlSeparator);
                }
                result = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                builder.Clear();
                builder2.Clear();
            }
            return result;
        }
        private FieldInfo GetFieldInfo(string typeName, string fieldName)
        {
            Type baseType = this.GetType();
            while (null != baseType)
            {
                if (baseType.FullName.Equals(typeName))
                {
                    break;
                }
                baseType = baseType.BaseType;
            }
            if (null == baseType)
            {
                throw new RemotingException($"未找到type名为{typeName}的type");
            }
            FieldInfo field = baseType.GetField(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (null == field)
            {
                throw new RemotingException($"未在type名为{typeName }的类中找到{fieldName}的字段");
            }
            return field;
        }

        private string[] GetPrimaryKeyInfoByAttribute(PropertyInfo[] properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            if (properties.Length == 0)
            {
                return new string[0];
            }
            List<string> list = new List<string>();
            foreach (PropertyInfo info in properties)
            {
                ModelUpdatableAttribute customAttribute = Attribute.GetCustomAttribute(info, typeof(ModelUpdatableAttribute)) as ModelUpdatableAttribute;
                if ((customAttribute != null) && customAttribute.IsPrimaryKey)
                {
                    list.Add(info.Name);
                }
            }

            return list.ToArray();
        }
        private Type GetRealDataType(Type t)
        {

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return t.GetGenericArguments()[0];
            }
            return t;

        }

        private string GetSelectFieldExpress(string tableName, string fieldName, bool isAddAlias)
        {
            /*
            if (((setting != null) && !string.IsNullOrEmpty(setting.SelectFieldPattern)) && !string.IsNullOrEmpty(setting.SelectFieldPattern.Trim()))
            {
                return (string.Format(setting.SelectFieldPattern.Trim(), tableName, this.EncodeFieldName(fieldName)) + (isAddAlias ? (" AS " + this.EncodeFieldName(fieldName)) : ""));
            }*/
            return this.EncodeFieldName(fieldName);
        }

        private string GetSelectSql(DataTable dt, string tableName = null)
        {
            string str = tableName;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = this.EncodeTableEntityName(dt.TableName);
                str = dt.TableName;
            }
            if (dt == null)
            {
                throw new ArgumentNullException("dt");
            }
            if (dt.Columns.Count == 0)
            {
                throw new ArgumentException("dt的列数不能为0");
            }
            StringBuilder sb = new StringBuilder("SELECT ");
            foreach (DataColumn column in dt.Columns)
            {
                sb.Append(((sb.Length > 0) ? "," : "") + this.GetSelectFieldExpress(str, column.ColumnName, true));
            }
            sb.Append($" FROM {tableName}");
            return sb.ToString();
        }

        private string GetSelectSql(PropertyInfo[] properties, string tableName)
        {
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            StringBuilder sb = new StringBuilder("SELECT ");
            foreach (PropertyInfo info in properties)
            {
                if (info.CanWrite)
                {
                    sb.Append(((sb.Length > 0) ? "," : "") + this.GetSelectFieldExpress(tableName, info.Name, true));
                }
            }
            sb.Append($" FROM {tableName }");
            return sb.ToString();
        }
        private string GetUpdateValueExpress(string tableName, string fieldName, string sqlValueString)
        {
            //SqlGeneratorSetting setting = this.SqlGeneratorSettingFind(tableName, fieldName);
            //if (((setting != null) && !string.IsNullOrEmpty(setting.UpdateValuePattern)) && !string.IsNullOrEmpty(setting.UpdateValuePattern.Trim()))
            //{
            //    return string.Format(setting.UpdateValuePattern.Trim(), tableName, fieldName, sqlValueString);
            //}
            return sqlValueString;
        }

        private string GetWhereConditionFieldExpress(string tableName, string fieldName)
        {
            //SqlGeneratorSetting setting = this.SqlGeneratorSettingFind(tableName, fieldName);
            //if (((setting != null) && !string.IsNullOrEmpty(setting.WhereConditionFieldPattern)) && !string.IsNullOrEmpty(setting.WhereConditionFieldPattern.Trim()))
            //{
            //    return string.Format(setting.WhereConditionFieldPattern.Trim(), tableName, fieldName);
            //}
            return this.EncodeFieldName(fieldName);
        }

        private string GetWhereConditionValueExpress(string tableName, string fieldName, string sqlValueString)
        {
            //SqlGeneratorSetting setting = this.SqlGeneratorSettingFind(tableName, fieldName);
            //if (((setting != null) && !string.IsNullOrEmpty(setting.WhereConditionValuePattern)) && !string.IsNullOrEmpty(setting.WhereConditionValuePattern.Trim()))
            //{
            //    return string.Format(setting.WhereConditionValuePattern.Trim(), tableName, fieldName, sqlValueString);
            //}
            return sqlValueString;
        }

        private int InsertNonDataTable(object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields)
        {
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();

            if (!this.GenerateInsertNonDataTableSql(sqlBuffer, parameterList, 0, obj, objType, properties, tableName, fields))
            {
                return 0;
            }
            if ((parameterList == null) || (parameterList.Count == 0))
            {
                return this.ExecuteNonQuery(sqlBuffer.ToString());
            }
            return this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
        }
        private void Load(IList list, Type t, string tableName, string conditionSql, string sort)
        {
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            PropertyInfo[] properties = t.GetProperties();
            string commandText = this.GetSelectSql(properties, tableName) + (string.IsNullOrEmpty(conditionSql) ? "" : (" WHERE " + conditionSql)) + (string.IsNullOrEmpty(sort) ? "" : (" ORDER BY " + sort));
            this.ExecuteQueryImpl(list, t, CommandType.Text, commandText, new IDataParameter[0]);
        }


        private List<T> LoadDataModelListImp<T>(string tableName, bool isGetFirstData, params KeyValuePair<string, object>[] fieldsAndValues) where T : class, new()
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            string str = "";
            if ((fieldsAndValues != null) && (fieldsAndValues.Length != 0))
            {
                string str4 = "";
                object obj2 = null;
                for (int i = 0; i < fieldsAndValues.Length; i++)
                {
                    str4 = fieldsAndValues[i].Key;
                    if (string.IsNullOrWhiteSpace(str4))
                    {

                        throw new ArgumentException("字段名不能为空");
                    }

                    obj2 = fieldsAndValues[i].Value;
                    if (!string.IsNullOrEmpty(str))
                    {
                        str = str + " AND ";
                    }
                    if ((obj2 == null) || (obj2 == DBNull.Value))
                    {
                        str = str + this.EncodeFieldName(str4) + " IS " + this.GetSqlValueString(obj2);
                    }
                    else
                    {
                        str = str + this.GetWhereConditionFieldExpress(tableName, str4) + "=" + this.GetWhereConditionValueExpress(tableName, str4, this.GetSqlValueString(obj2));
                    }
                }
            }
            string str2 = "";

            List<T> resultList = new List<T>();
            PropertyInfo[] properties = typeof(T).GetProperties();
            string selectSql = this.GetSelectSql(properties, tableName) + (string.IsNullOrEmpty(str) ? "" : (" WHERE " + str)) + str2;
            if (isGetFirstData)
            {
                using (IDataReader reader = this.ExecuteDataReader(selectSql, (IDataParameter[])null))
                {
                    this.DataReaderToList(resultList, typeof(T), reader, 0, 0, null, 0);
                    return resultList;
                }
            }
            this.FillQuery<T>(resultList, selectSql);
            return resultList;
        }


        private int UpdateNonDataTable(object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields, params string[] primaryKeyProperties)
        {

            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();
            if (!this.GenerateUpdateNonDataTableSql(sqlBuffer, parameterList, 0, obj, objType, properties, tableName, fields, false, primaryKeyProperties))
            {
                return 0;
            }
            if (sqlBuffer.Length <= 0)
            {
                return 0;
            }
            if ((parameterList == null) || (parameterList.Count == 0))
            {
                return this.ExecuteNonQuery(sqlBuffer.ToString());
            }
            return this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
        }
        private int UpdateNonDataTableByCondition(object obj, Type objType, PropertyInfo[] properties, string tableName, UpdateFields fields, string conditionSql)
        {

            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();
            if (!this.GenerateUpdateNonDataTableSqlByCondition(sqlBuffer, parameterList, 0, obj, objType, properties, tableName, fields, false, conditionSql))
            {
                return 0;
            }
            if (sqlBuffer.Length <= 0)
            {
                return 0;
            }
            if ((parameterList == null) || (parameterList.Count == 0))
            {
                return this.ExecuteNonQuery(sqlBuffer.ToString());
            }
            return this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
        }



        protected void CheckConnection()
        {
            this.CheckConnection(this.ConnectionInstance);
        }

        protected void CheckConnection(IDbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            this.CheckConnection(command.Connection);
        }

        protected void CheckConnection(IDbConnection connection)
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException("对象已经释放");
            }
            if (connection == null)
            {
                throw new InvalidOperationException("连接尚未初始化");
            }
            if (connection.State != ConnectionState.Open)
            {
                throw new InvalidOperationException("连接尚未打开");
            }
        }
        protected abstract IDbCommand CreateCommandInstance();
        protected abstract IDbConnection CreateConnectionInstance();
        protected abstract IDbDataAdapter CreateDataAdapterInstance();

        protected int DataReaderToList(IList resultList, Type t, IDataReader reader, int startIndex, int? endIndex, Dictionary<string, decimal> sumFields, int returnRowCountMaxValue = 0)
        {

            if (resultList == null)
            {
                throw new ArgumentNullException("resultList");
            }
            if (t == null)
            {
                throw new ArgumentNullException("t");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (startIndex < 0)
            {
                throw new ArgumentOutOfRangeException("startIndex");
            }
            if (!endIndex.HasValue || endIndex < 0)
            {
                if (returnRowCountMaxValue <  0)
                    throw new ArgumentOutOfRangeException(nameof(endIndex));
                endIndex = startIndex + returnRowCountMaxValue;
            }


            if (returnRowCountMaxValue != 0)
            {

                if (endIndex >= returnRowCountMaxValue)
                    throw new ArgumentOutOfRangeException(nameof(returnRowCountMaxValue), "参数值要么为0，否则必须大于 endIndex 参数的值。");

            }
            string[] fieldNames = new string[reader.FieldCount];
            for (int i = 0; i < fieldNames.Length; i++)
            {
                fieldNames[i] = reader.GetName(i);
            }
            var tempArray = new ValueTuple<PropertyInfo, bool, Type>[fieldNames.Length];
            PropertyInfo[] properties = t.GetProperties();
            //合计字段名称和索引
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            if ((sumFields != null) && (sumFields.Count > 0))
            {
                var tempflag = false;
                foreach (KeyValuePair<string, decimal> pair in sumFields)
                {
                    tempflag = false;
                    if (string.IsNullOrWhiteSpace(pair.Key))
                    {
                        throw new ArgumentException("指定的需要合计的列没有提供字段名称。", "sumFields");
                    }
                    for (int i = 0; i < fieldNames.Length; i++)
                    {
                        if (string.Compare(pair.Key, fieldNames[i], true, CultureInfo.InvariantCulture) == 0)
                        {
                            dictionary.Add(pair.Key, i);
                            tempflag = true;
                            break;
                        }
                    }
                    if (!tempflag)
                    {
                        throw new ArgumentException("需要合计的列“" + pair.Key + "”在查询的数据结果中不存在。", "sumFields");
                    }
                }
                foreach (KeyValuePair<string, int> pair2 in dictionary)
                {
                    sumFields[pair2.Key] = decimal.Zero;
                }
            }

            foreach (var info in properties)
            {

                for (int i = 0; i < fieldNames.Length; i++)
                {
                    if (string.Compare(info.Name, fieldNames[i], true, CultureInfo.InvariantCulture) == 0)
                    {

                        var tempType = this.GetRealDataType(info.PropertyType);
                        tempArray[i] = new ValueTuple<PropertyInfo, bool, Type>(info, !tempType.Equals(reader.GetFieldType(i)), tempType);
                        break;
                    }
                }
            }


            int num = -1;
            object obj2 = null;
            this._logger.Info($"{nameof(DataReaderToList)}() 数据扫描开始。。。");
            while (reader.Read())
            {
                num++;
                if ((sumFields != null) && (sumFields.Count > 0))
                {
                    foreach (KeyValuePair<string, int> pair3 in dictionary)
                    {
                        obj2 = reader.GetValue(pair3.Value);
                        if (obj2 != DBNull.Value)
                        {

                            sumFields[pair3.Key] += Convert.ToDecimal(obj2);
                        }
                    }
                }
                else if ((returnRowCountMaxValue > 0) && (num >= returnRowCountMaxValue))
                {
                    break;
                }
                if (startIndex <= num)
                {
                    if (startIndex == num)
                    {
                        this._logger.Info($"{nameof(DataReaderToList)}() 要获取的页面数据读取开始。。。");

                    }
                    if (endIndex >= num)
                    {
                        object objTemp = Activator.CreateInstance(t);
                        for (int i = 0; i < tempArray.Length; i++)
                        {
                            var temp = tempArray[i];
                            if (temp.Item1 != null && temp.Item1.CanWrite)
                            {
                                try
                                {
                                    if (reader.IsDBNull(i))
                                    {
                                        temp.Item1.SetValue(objTemp, null);
                                    }
                                    else if (temp.Item2)
                                    {
                                        temp.Item1.SetValue(objTemp, Convert.ChangeType(reader.GetValue(i), temp.Item3));
                                    }
                                    else
                                    {
                                        temp.Item1.SetValue(objTemp, reader.GetValue(i));
                                    }
                                }
                                catch (Exception e)
                                {

                                    throw new InvalidCastException("属性设置失败", e);
                                }
                            }
                        }
                        resultList.Add(objTemp);


                        if (endIndex == num)
                        {
                            this._logger.Info($"{nameof(DataReaderToList)}() 要获取的页面数据读取结束。。。");

                        }
                    }
                }
            }
            this._logger.Info($"{nameof(DataReaderToList)}()数据扫描结束.");
            return (num + 1);
        }

        protected virtual void Dispose(bool disposing)
        {

            if (!this._disposed)
            {
                try
                {
                    if (this.IsTransaction && (this._dbTransaction != null))
                    {
                        this.RollbackTransaction();
                    }
                }
                catch
                {
                }
                try
                {
                    if ((this.ConnectionInstance != null) && (this.State != ConnectionState.Closed))
                    {
                        this.Close();
                    }
                }
                catch
                {
                }
                try
                {
                    if (this._isNeedCloseConnection && (this.ConnectionInstance != null))
                    {
                        this.ConnectionInstance.Dispose();
                    }
                }
                catch
                {
                }
            }
            this._disposed = true;
            this._logger.Info("关闭连接->");
        }

        protected abstract string EncodeFieldEntityName(string fieldName);
        protected abstract string EncodeTableEntityName(string tableName);
        protected virtual string EndProcessStringSqlValueString(string stringSqlValue)
        {
            return stringSqlValue;
        }
        protected virtual IDataReader ExecuteDataReaderImpl(CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            IDataReader reader2;
            IDbCommand command = this.GetCommand(commandType, commandText, commandParameters);
            IDataReader reader = null;

            try
            {
                reader = command.ExecuteReader();
                command.Parameters.Clear();
                command.Dispose();
                reader2 = reader;
            }
            catch
            {

                if (reader != null)
                {
                    try
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                    catch
                    {
                    }
                }
                try
                {
                    command.Parameters.Clear();
                    command.Dispose();
                }
                catch
                {
                }
                throw;
            }
            return reader2;
        }

        protected virtual object ExecuteScalarImpl(CommandType commandType, string commandText, params IDataParameter[] commandParameters)
        {
            object result;
            IDbCommand command = this.GetCommand(commandType, commandText, commandParameters);
            try
            {
                this.DebugSql(commandText, commandParameters);
                result = command.ExecuteScalar();
            }
            catch (Exception e)
            {
                this.DebugSql(commandText, commandParameters, e);

                throw;
            }
            finally
            {
                try
                {
                    command.Parameters.Clear();
                    command.Dispose();
                }
                catch
                {
                }
            }
            return result;
        }

        protected int FillDataTable(DataTable dataTable, IDataReader reader, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields)
        {
            return this.FillDataTable(dataTable, reader, pageSize, curPageNum, sumFields, 0);
        }

        /// <summary>
        /// 根据 DataReader 填充表。 如果 DataTable 对象没有表结构，则会根据 IDataReader 对象创建表结构（仅仅创建 ColumnName,DataType）。
        /// 如果 DataTable 对象存在表结构，则仅仅会填充在 IDataReader 对象中存在的列。 
        /// </summary>
        /// <param name="dataTable">需要填充的表</param>
        /// <param name="reader">填充 DataTable 对象的数据源。</param>
        /// <param name="pageSize">每页数据的行数。</param>
        /// <param name="curPageNum">当前需要填充的页码（从1开始）。</param>
        /// <param name="sumFields">需要存储进行合计的字段集合，对应的 Value 即为合计后的值。</param>
        /// <param name="returnRowCountMaxValue">返回数据总行数的最大值。用于提高性能不返回总数据行数。sumFields 参数有值时本参数无效。</param>
        /// <returns>IDataReader 对象中总共的数据行数和returnRowCountMaxValue参数的最小值。</returns>
        protected int FillDataTable(DataTable dataTable, IDataReader reader, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, int returnRowCountMaxValue)
        {

            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }
            if (pageSize <= 0)
            {
                throw new ArgumentException("pageSize不能小于等于0");
            }
            if (curPageNum <= 0)
            {
                throw new ArgumentException("curPageNum不能小于等于0");
            }
            if (reader.FieldCount == 0)
            {
                throw new ArgumentException("reader的列数不能小于0！");
            }
            if (dataTable.Columns.Count == 0)
            {
                DataColumn column = null;
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    column = new DataColumn(reader.GetName(i), reader.GetFieldType(i));
                    dataTable.Columns.Add(column);
                }
            }
            string[] sumFiledNames = null;
            if ((sumFields != null) && (sumFields.Count > 0))
            {
                sumFiledNames = sumFields.Keys.ToArray();
                string[] fieldsName = new string[reader.FieldCount];
                for (int i = 0; i < fieldsName.Length; i++)
                {
                    fieldsName[i] = reader.GetName(i);
                }
                foreach (KeyValuePair<string, decimal> field in sumFields)
                {

                    if (string.IsNullOrWhiteSpace(field.Key))
                    {
                        throw new ArgumentException("指定的需要合计的列没有提供字段名称。", "sumFields");
                    }
                    if (!fieldsName.HasValue(x => string.Compare(x, field.Key, true, CultureInfo.InvariantCulture) == 0))
                    {

                        throw new ArgumentException("需要合计的列“" + field.Key + "”在查询的数据结果中不存在。", "sumFields");

                    }
                }
                foreach (var item in sumFiledNames)
                {
                    sumFields[item] = 0;
                }

            }
            if ((returnRowCountMaxValue > 0) && (pageSize * curPageNum >= returnRowCountMaxValue))
            {
                throw new ArgumentOutOfRangeException("returnRowCountMaxValue", "参数值要么为0，否则必须大于参数 pageSize * curPageNum 的值。");
            }
            dataTable.BeginLoadData();
            int result = 0;
            {

                while (reader.Read())
                {
                    result++;
                    var row = dataTable.NewRow();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        if (dataTable.Columns.Contains(reader.GetName(i)))
                        {
                            row[reader.GetName(i)] = reader.GetValue(i);
                        }
                    }
                    if ((sumFields != null) && (sumFields.Count > 0))
                    {
                        foreach (var fieldname in sumFiledNames)
                        {
                            sumFields[fieldname] += Convert.ToDecimal(row[fieldname]);
                        }

                    }
                    else if ((returnRowCountMaxValue > 0) && (result > returnRowCountMaxValue))
                    {
                        break;
                    }
                    if ((result >= (pageSize * (curPageNum - 1)) + 1) && (result <= pageSize * curPageNum))
                    {

                        dataTable.Rows.Add(row);
                    }
                }
            }
            dataTable.EndLoadData();
            return result;
        }
        protected abstract void FillImpl(DataSet dataSet, CommandType commandType, string commandText, params IDataParameter[] commandParameters);
        protected abstract void FillImpl(DataTable dataTable, CommandType commandType, string commandText, params IDataParameter[] commandParameters);
        protected IDbCommand GetCommand(CommandType commandType, string commandText, params IDataParameter[] parameters)
        {
            IDbCommand result = this.CreateCommandInstance();
            try
            {
                this.CheckConnection(result);
                result.CommandTimeout = this.CommandTimeOut;
                result.CommandType = commandType;
                result.CommandText = commandText;
                if ((parameters != null) && (parameters.Length != 0))
                {
                    foreach (IDataParameter parameter in parameters)
                    {
                        result.Parameters.Add(parameter);
                    }
                }

            }
            catch
            {
                if (result != null)
                {
                    result.Parameters.Clear();
                    result.Dispose();
                }
                throw;
            }
            return result;
        }

        protected abstract string GetDateTimeSqlString(DateTime date, string format);

        protected virtual void OnClosing(object sender, EventArgs arg)
        {
            this.Closing?.Invoke(sender, arg);
        }
        protected virtual void OnOpened(object sender, EventArgs arg)
        {
            this.Opened?.Invoke(sender, arg);
        }





        internal IDbConnection GetConnectionInstance()
        {
            return this._connectionInstance;
        }





        public int BatchInsert(object dataModelObj, string tableName)
        {
            return this.BatchInsert(dataModelObj, tableName, null, 50);
        }

        public int BatchInsert(object dataModelObj, string tableName, UpdateFields fields, int buffer)
        {
            if (buffer <= 0)
            {
                buffer = 1;
            }
            bool appendSeparator = buffer > 1;
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            int result=0;
            int num = 0;
           // int num2 = 0;
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();
           
            try
            {
                if (dataModelObj is DataRow)
                {
                    result  = this.InsertDataRow((DataRow)dataModelObj, tableName, fields);
                }
                else if (dataModelObj is DataTable)
                {
                  var   table = (DataTable)dataModelObj;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if (this.GenerateInsertDataRowSql(sqlBuffer, parameterList, parameterList.Count, table.Rows[i], tableName, fields, appendSeparator))
                        {
                            num++;
                            if (num >= buffer)
                            {
                                if (appendSeparator)
                                {
                                    sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                    sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                }
                                if ((parameterList == null) || (parameterList.Count == 0))
                                {
                                    result  += this.ExecuteNonQuery(sqlBuffer.ToString());
                                }
                                else
                                {
                                    result  += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                }
                                num = 0;
                                sqlBuffer.Clear();
                                parameterList.Clear();
                            }
                        }
                    }
                }
                else if (dataModelObj is DataSet)
                {
                    DataSet set = (DataSet)dataModelObj;
                    for (int j = 0; j < set.Tables.Count; j++)
                    {
                       var  table = set.Tables[j];
                        for (int k = 0; k < table.Rows.Count; k++)
                        {
                            if (this.GenerateInsertDataRowSql(sqlBuffer, parameterList, parameterList.Count, table.Rows[k], tableName, fields, appendSeparator))
                            {
                                num++;
                                if (num >= buffer)
                                {
                                    if (appendSeparator)
                                    {
                                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                    }
                                    if ((parameterList == null) || (parameterList.Count == 0))
                                    {
                                        result  += this.ExecuteNonQuery(sqlBuffer.ToString());
                                    }
                                    else
                                    {
                                        result += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                    }
                                    num = 0;
                                    sqlBuffer.Clear ();
                                    parameterList.Clear();
                                }
                            }
                        }
                    }
                }
                else if (dataModelObj is DataView)
                {
                    result  = this.BatchInsert(((DataView)dataModelObj).Table, tableName, fields, buffer);
                }
                else if (dataModelObj is DataRowView)
                {
                    result  = this.BatchInsert(((DataRowView)dataModelObj).Row, tableName, fields, buffer);
                }
                else if (dataModelObj is IDataReader)
                {
                    result  = this.BatchInsertDataReader((IDataReader)dataModelObj, tableName, fields, buffer);
                }
                else if (dataModelObj is IList)
                {
                    IList list2 = dataModelObj as IList;
                    if (list2.Count == 0)
                    {
                        return 0;
                    }
                    Type o = null;
                    PropertyInfo[] properties = null;
                    for (int m = 0; m < list2.Count; m++)
                    {
                        if (list2[m] != null)
                        {
                            if (((list2[m] is IList) || (list2[m] is DataTable)) || ((list2[m] is DataSet) || (list2[m] is IDataReader)))
                            {
                                result  += this.BatchInsert(list2[m], tableName, fields, buffer);
                            }
                            else
                            {
                                if (list2[m] is DataRow)
                                {
                                    if (!this.GenerateInsertDataRowSql(sqlBuffer, parameterList, parameterList.Count, list2[m] as DataRow, tableName, fields, appendSeparator))
                                    {
                                        continue;
                                    }
                                    num++;
                                }
                                else
                                {
                                    if (o == null)
                                    {
                                        o = list2[m].GetType();
                                        properties = o.GetProperties();
                                        string fullName = o.FullName;
                                    }
                                    else if (list2[m].GetType().Equals(o))
                                    {
                                        o = list2[m].GetType();
                                        properties = o.GetProperties();
                                        string text2 = o.FullName;
                                    }
                                    if (!this.GenerateInsertNonDataTableSql(sqlBuffer, parameterList, parameterList.Count, list2[m], o, properties, tableName, fields, appendSeparator))
                                    {
                                        continue;
                                    }
                                    num++;
                                }
                                if (num >= buffer)
                                {
                                    if (appendSeparator)
                                    {
                                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                    }
                                    if ((parameterList == null) || (parameterList.Count == 0))
                                    {
                                        result  += this.ExecuteNonQuery(sqlBuffer.ToString());
                                    }
                                    else
                                    {
                                        result  += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                    }
                                    num = 0;
                                    sqlBuffer.Clear ();
                                    parameterList.Clear();
                                }
                            }
                        }
                    }
                }
                else
                {
                    Type type = dataModelObj.GetType();
                    PropertyInfo[] infoArray2 = dataModelObj.GetType().GetProperties();
                    result  = this.InsertNonDataTable(dataModelObj, type, infoArray2, tableName, fields);
                }
                if (sqlBuffer.Length > 0)
                {
                    if (appendSeparator)
                    {
                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                    }
                    if ((parameterList == null) || (parameterList.Count == 0))
                    {
                        result  += this.ExecuteNonQuery(sqlBuffer.ToString());
                    }
                    else
                    {
                        result  += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                    }
                    num = 0;
                    sqlBuffer.Clear();
                    parameterList.Clear();
                }
                 
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sqlBuffer != null)
                {
                    sqlBuffer.Clear();
                }
                sqlBuffer = null;
                if (parameterList != null)
                {
                    parameterList.Clear();
                }
                parameterList = null;
            }
            return result;
        }
        public int BatchUpdate(object dataModelObj, string tableName, params string[] primaryKeyFields)
        {
            return this.BatchUpdate(dataModelObj, tableName, null, 50, primaryKeyFields);
        }
        public int BatchUpdate(object dataModelObj, string tableName, UpdateFields fields, int buffer, params string[] primaryKeyFields)
        {
            int result;
            if (buffer <= 0)
            {
                buffer = 1;
            }
            bool appendSeparator = buffer > 1;
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            int num = 0;
            int num2 = 0;
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();
            DataTable table = null;
            try
            {
                if (dataModelObj is DataRow)
                {
                    num2 = this.UpdateDataRow((DataRow)dataModelObj, tableName, fields, primaryKeyFields);
                }
                else if (dataModelObj is DataTable)
                {
                    table = (DataTable)dataModelObj;
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        if (this.GenerateUpdateDataRowSql(sqlBuffer, parameterList, parameterList.Count, table.Rows[i], tableName, fields, appendSeparator, primaryKeyFields))
                        {
                            num++;
                            if (num >= buffer)
                            {
                                if (appendSeparator)
                                {
                                    sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                    sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                }
                                if ((parameterList == null) || (parameterList.Count == 0))
                                {
                                    num2 += this.ExecuteNonQuery(sqlBuffer.ToString());
                                }
                                else
                                {
                                    num2 += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                }
                                num = 0;
                                sqlBuffer.Remove(0, sqlBuffer.Length);
                                parameterList.Clear();
                            }
                        }
                    }
                }
                else if (dataModelObj is DataSet)
                {
                    DataSet set = (DataSet)dataModelObj;
                    for (int j = 0; j < set.Tables.Count; j++)
                    {
                        table = set.Tables[j];
                        for (int k = 0; k < table.Rows.Count; k++)
                        {
                            if (this.GenerateUpdateDataRowSql(sqlBuffer, parameterList, parameterList.Count, table.Rows[k], tableName, fields, appendSeparator, primaryKeyFields))
                            {
                                num++;
                                if (num >= buffer)
                                {
                                    if (appendSeparator)
                                    {
                                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                    }
                                    if ((parameterList == null) || (parameterList.Count == 0))
                                    {
                                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString());
                                    }
                                    else
                                    {
                                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                    }
                                    num = 0;
                                    sqlBuffer.Remove(0, sqlBuffer.Length);
                                    parameterList.Clear();
                                }
                            }
                        }
                    }
                }
                else if (dataModelObj is DataView)
                {
                    num2 = this.BatchUpdate(((DataView)dataModelObj).Table, tableName, fields, buffer, primaryKeyFields);
                }
                else if (dataModelObj is DataRowView)
                {
                    num2 = this.BatchUpdate(((DataRowView)dataModelObj).Row, tableName, fields, buffer, primaryKeyFields);
                }
                else if (dataModelObj is IList)
                {
                    IList list2 = dataModelObj as IList;
                    if (list2.Count == 0)
                    {
                        return 0;
                    }
                    Type o = null;
                    PropertyInfo[] properties = null;
                    for (int m = 0; m < list2.Count; m++)
                    {
                        if (list2[m] != null)
                        {
                            if (((list2[m] is IList) || (list2[m] is DataTable)) || (list2[m] is DataSet))
                            {
                                num2 += this.BatchUpdate(list2[m], tableName, fields, buffer, primaryKeyFields);
                            }
                            else
                            {
                                if (list2[m] is DataRow)
                                {
                                    if (!this.GenerateUpdateDataRowSql(sqlBuffer, parameterList, parameterList.Count, list2[m] as DataRow, tableName, fields, appendSeparator, primaryKeyFields))
                                    {
                                        continue;
                                    }
                                    num++;
                                }
                                else
                                {
                                    if (o == null)
                                    {
                                        o = list2[m].GetType();
                                        properties = o.GetProperties();
                                        string fullName = o.FullName;
                                    }
                                    else if (list2[m].GetType().Equals(o))
                                    {
                                        o = list2[m].GetType();
                                        properties = o.GetProperties();
                                        string text2 = o.FullName;
                                    }
                                    if (!this.GenerateUpdateNonDataTableSql(sqlBuffer, parameterList, parameterList.Count, list2[m], o, properties, tableName, fields, appendSeparator, primaryKeyFields))
                                    {
                                        continue;
                                    }
                                    num++;
                                }
                                if (num >= buffer)
                                {
                                    if (appendSeparator)
                                    {
                                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                                    }
                                    if ((parameterList == null) || (parameterList.Count == 0))
                                    {
                                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString());
                                    }
                                    else
                                    {
                                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                                    }
                                    num = 0;
                                    sqlBuffer.Remove(0, sqlBuffer.Length);
                                    parameterList.Clear();
                                }
                            }
                        }
                    }
                }
                else
                {
                    Type type = dataModelObj.GetType();
                    PropertyInfo[] infoArray2 = dataModelObj.GetType().GetProperties();
                    num2 = this.UpdateNonDataTable(dataModelObj, type, infoArray2, tableName, fields, primaryKeyFields);
                }
                if (sqlBuffer.Length > 0)
                {
                    if (appendSeparator)
                    {
                        sqlBuffer.Insert(0, this.BatchExecuteSqlBeginString);
                        sqlBuffer.Append(this.BatchExecuteSqlEndString);
                    }
                    if ((parameterList == null) || (parameterList.Count == 0))
                    {
                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString());
                    }
                    else
                    {
                        num2 += this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
                    }
                    num = 0;
                    sqlBuffer.Remove(0, sqlBuffer.Length);
                    parameterList.Clear();
                }
                result = num2;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (sqlBuffer != null)
                {
                    sqlBuffer.Remove(0, sqlBuffer.Length);
                }
                sqlBuffer = null;
                if (parameterList != null)
                {
                    parameterList.Clear();
                }
                parameterList = null;
            }
            return result;
        }
        public void BeginTransaction()
        {
            this._dbTransaction = this.ConnectionInstance.BeginTransaction();
            this._isTransaction = true;
            this._logger.Info("开始事务->");
        }
        public void BeginTransaction(IsolationLevel isolationlevel)
        {
            this._dbTransaction = this.ConnectionInstance.BeginTransaction(isolationlevel);
            this._isTransaction = true;
            this._logger.Info($"开始事务->事务级别为{isolationlevel}");
        }


        public virtual void Close()
        {
            try
            {
                if ((this.IsTransaction && (this._dbTransaction != null)) && (this.ConnectionInstance.State != ConnectionState.Closed))
                {
                    this.RollbackTransaction();
                }
            }
            catch
            {
            }
            if (this._isNeedCloseConnection && (this.ConnectionInstance.State != ConnectionState.Closed))
            {
                try
                {
                    this.OnClosing(this, new EventArgs());
                }
                catch
                {
                    throw;
                }
                finally
                {
                    this.ConnectionInstance.Close();
                    this._logger.Info("关闭连接->");
                }
            }
        }
        public virtual string CombineFieldValueEqualExpress(string fieldName, object value)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                return (this.EncodeFieldEntityName(fieldName) + " = " + this.GetSqlValueString(value));
            }
            return (this.EncodeFieldEntityName(fieldName) + " IS " + this.GetSqlValueString(value));
        }

        public virtual string CombineFieldValueEqualExpress(string fieldName, object value, string datetimeFormat)
        {
            if ((value != null) && (value != DBNull.Value))
            {
                return (this.EncodeFieldEntityName(fieldName) + " = " + this.GetSqlValueString(value, datetimeFormat));
            }
            return (this.EncodeFieldEntityName(fieldName) + " IS " + this.GetSqlValueString(value));
        }
        public void CommitTransaction()
        {
            if (!this.IsTransaction)
            {
                throw new InvalidOperationException("未开始事务！");
            }
            this._dbTransaction.Commit();
            this._logger.Info("提交事务->");
            this._dbTransaction.Dispose();
            this._dbTransaction = null;
            this._isTransaction = false;
        }
        public int Delete(object dataModelObj, string tableName)
        {
            return this.Delete(dataModelObj, tableName, null);
        }
        public int Delete(object dataModelObj, string tableName, params string[] primaryKeyFields)
        {
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            string[] strArray = primaryKeyFields;
            if (dataModelObj is DataRow)
            {
                return this.DeleteDataRow((DataRow)dataModelObj, tableName, primaryKeyFields);
            }
            if (dataModelObj is DataTable)
            {
                return this.DeleteDataTable((DataTable)dataModelObj, tableName, primaryKeyFields);
            }
            if (dataModelObj is DataSet)
            {
                return this.DeleteDataSet((DataSet)dataModelObj);
            }
            if (dataModelObj is DataView)
            {
                return this.DeleteDataTable(((DataView)dataModelObj).Table, tableName, primaryKeyFields);
            }
            if (dataModelObj is DataRowView)
            {
                return this.DeleteDataRow(((DataRowView)dataModelObj).Row, tableName, primaryKeyFields);
            }
            if (dataModelObj is IList)
            {
                int num = 0;
                IList list = dataModelObj as IList;
                if (list.Count == 0)
                {
                    return 0;
                }
                Type type = null;
                PropertyInfo[] infoArray = null;
                string fullName = null;
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            if (((list[i] is IList) || (list[i] is DataTable)) || ((list[i] is DataRow) || (list[i] is DataSet)))
                            {
                                num += this.Delete(list[i], tableName, primaryKeyFields);
                            }
                            else
                            {
                                if (type == null)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                    if ((strArray == null) || (strArray.Length == 0))
                                    {
                                        primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(infoArray);
                                    }
                                }
                                else if (list[i].GetType().FullName != fullName)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                    if ((strArray == null) || (strArray.Length == 0))
                                    {
                                        primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(infoArray);
                                    }
                                }
                                num += this.DeleteNonDataTable(list[i], type, infoArray, tableName, primaryKeyFields);
                            }
                        }
                    }
                    return num;
                }
                catch
                {
                    throw;
                }
            }
            Type objType = dataModelObj.GetType();
            PropertyInfo[] properties = dataModelObj.GetType().GetProperties();
            if ((strArray == null) || (strArray.Length == 0))
            {
                primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(properties);
            }
            return this.DeleteNonDataTable(dataModelObj, objType, properties, tableName, primaryKeyFields);
        }
        public int DeleteByCondition(string tableName, string sqlCondition)
        {


            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            StringBuilder sb = new StringBuilder($"DELETE {tableName} ");
            if (string.IsNullOrEmpty(sqlCondition))
            {

            }
            else
            {
                sb.Append($" WHERE {sqlCondition}");
            }

            return this.ExecuteNonQuery(sb.ToString());
        }

        public int DeleteDataRow(DataRow row, string tableName, params string[] deleteConditionFields)
        {
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            string str = tableName;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = this.EncodeTableEntityName(row.Table.TableName);
                str = row.Table.TableName;
            }
            string sqlValueString = "";
            ArrayList list = null;
            int num = 0;
            IDataParameter dataParameterInstance = null;
            int num2 = 0;
            DataTable table = row.Table;
            if ((table.PrimaryKey.Length == 0) && ((deleteConditionFields == null) || (deleteConditionFields.Length == 0)))
            {
                throw new InvalidOperationException(string.Format("Table {0} is not defined primary key(s)。", table.TableName));
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            DataRow row2 = row;
            builder.Append(" DELETE FROM " + tableName);
            DataColumn[] primaryKey = null;
            if ((deleteConditionFields != null) && (deleteConditionFields.Length != 0))
            {
                primaryKey = new DataColumn[deleteConditionFields.Length];
                for (int i = 0; i < deleteConditionFields.Length; i++)
                {
                    primaryKey[i] = table.Columns[deleteConditionFields[i]];
                }
            }
            else
            {
                primaryKey = table.PrimaryKey;
            }
            foreach (DataColumn column in primaryKey)
            {
                if (builder2.Length > 0)
                {
                    builder2.Append(" AND ");
                }
                if (row2.HasVersion(DataRowVersion.Original))
                {
                    sqlValueString = this.GetSqlValueString(row2[column.ColumnName, DataRowVersion.Original]);
                    if (row2[column.ColumnName, DataRowVersion.Original] == DBNull.Value)
                    {
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " IS " + sqlValueString + " ");
                    }
                    else if (sqlValueString != null)
                    {
                        builder2.Append(this.GetWhereConditionFieldExpress(str, column.ColumnName) + " = " + this.GetWhereConditionValueExpress(str, column.ColumnName, sqlValueString));
                    }
                    else
                    {
                        num++;
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.ParamPrefixFullString + num.ToString());
                        if (list == null)
                        {
                            list = new ArrayList();
                        }
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = row2[column.ColumnName, DataRowVersion.Original];
                        list.Add(dataParameterInstance);
                    }
                }
                else
                {
                    sqlValueString = this.GetSqlValueString(row2[column.ColumnName]);
                    if (row2[column.ColumnName] == DBNull.Value)
                    {
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " IS " + sqlValueString + " ");
                    }
                    else if (sqlValueString != null)
                    {
                        builder2.Append(this.GetWhereConditionFieldExpress(str, column.ColumnName) + " = " + this.GetWhereConditionValueExpress(str, column.ColumnName, sqlValueString));
                    }
                    else
                    {
                        num++;
                        builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.ParamPrefixFullString + num.ToString());
                        if (list == null)
                        {
                            list = new ArrayList();
                        }
                        dataParameterInstance = this.GetDataParameterInstance();
                        dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                        dataParameterInstance.Value = row2[column.ColumnName];
                        list.Add(dataParameterInstance);
                    }
                }
            }
            if (builder2.Length > 0)
            {
                builder.Append(" WHERE ");
                builder.Append(builder2.ToString());
            }
            if ((list == null) || (list.Count == 0))
            {
                return (num2 + this.ExecuteNonQuery(builder.ToString()));
            }
            return (num2 + this.ExecuteNonQuery(builder.ToString(), (IDataParameter[])list.ToArray(dataParameterInstance.GetType())));
        }

        public int DeleteDataSet(DataSet dataSet)
        {
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if (dataSet.Tables.Count == 0)
            {
                return 0;
            }
            int result;
            int temp = 0;
            try
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    temp += this.DeleteDataTable(table, table.TableName, new string[0]);
                }
                result = temp;
            }
            catch
            {
                throw;
            }
            return result;
        }
        public int DeleteDataTable(DataTable dataTable, string tableName, params string[] deleteConditionFields)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            int temp = 0;
            int result;
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    temp += this.DeleteDataRow(row, tableName, deleteConditionFields);
                }
                result = temp;
            }
            catch
            {
                throw;
            }
            return result;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }
        public string EncodeFieldName(string fieldName)
        {
            return this.EncodeFieldEntityName(fieldName);
        }
        public virtual string EncodeLikeSqlValueString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return "";
            }
            value = value.Replace("'", "''");
            string[] strArray = new string[] { "[", "%", "_", "^" };
            for (int i = 0; i < strArray.Length; i++)
            {
                value = value.Replace(strArray[i], "[" + strArray[i] + "]");
            }
            return value;
        }
        public string EncodeTableName(string tableName)
        {
            return this.EncodeTableEntityName(tableName);
        }

        public IDataReader ExecuteDataReader(string selectSql, IDataParameter[] commandParameters)
        {
            return this.ExecuteDataReaderImpl(CommandType.Text, selectSql, commandParameters);
        }
        public IDataReader ExecuteDataReader(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteDataReader(selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public IDataReader ExecuteDataReaderBySP(string spName, IDataParameter[] commandParameters)
        {
            return this.ExecuteDataReaderImpl(CommandType.StoredProcedure, spName, commandParameters);
        }

        public IDataReader ExecuteDataReaderBySP(string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteDataReaderBySP(spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }



        public DataSet ExecuteDataSet(string selectSql, IDataParameter[] commandParameters)
        {
            DataSet dataSet = new DataSet
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillImpl(dataSet, CommandType.Text, selectSql, commandParameters);
            return dataSet;
        }
        public DataSet ExecuteDataSet(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            DataSet dataSet = new DataSet
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillImpl(dataSet, CommandType.Text, selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
            return dataSet;
        }
        public DataSet ExecuteDataSetBySP(string spName, IDataParameter[] commandParameters)
        {
            DataSet dataSet = new DataSet
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillBySP(dataSet, spName, commandParameters);
            return dataSet;
        }
        public DataSet ExecuteDataSetBySP(string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            DataSet dataSet = new DataSet
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillBySP(dataSet, spName, parameterNameAndValues);
            return dataSet;
        }
        public DataTable ExecuteDataTable(string selectSql, IDataParameter[] commandParameters)
        {
            DataTable dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.Fill(dataTable, selectSql, commandParameters);
            return dataTable;
        }


        public DataTable ExecuteDataTable(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            DataTable dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.Fill(dataTable, selectSql, parameterNameAndValues);
            return dataTable;
        }

        public DataTable ExecuteDataTableBySP(string spName, IDataParameter[] commandParameters)
        {
            DataTable dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillBySP(dataTable, spName, commandParameters);
            return dataTable;
        }
        public DataTable ExecuteDataTableBySP(string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            DataTable dataTable = new DataTable
            {
                Locale = CultureInfo.InvariantCulture
            };
            this.FillBySP(dataTable, spName, parameterNameAndValues);
            return dataTable;
        }

        public int ExecuteNonQuery(string nonQuerySql, IDataParameter[] commandParameters)
        {
            return this.ExecuteNonQueryImpl(CommandType.Text, nonQuerySql, commandParameters);
        }


        public int ExecuteNonQuery(string nonQuerySql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteNonQuery(nonQuerySql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public int ExecuteNonQueryBySP(string spName, IDataParameter[] commandParameters)
        {
            return this.ExecuteNonQueryImpl(CommandType.StoredProcedure, spName, commandParameters);
        }



        public int ExecuteNonQueryBySP(string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteNonQueryBySP(spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public List<T> ExecuteQuery<T>(string selectSql, IDataParameter[] commandParameters) where T : class, new()
        {
            List<T> resultList = new List<T>();
            this.ExecuteQueryImpl(resultList, typeof(T), CommandType.Text, selectSql, commandParameters);
            return resultList;
        }



        public List<T> ExecuteQuery<T>(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new()
        {
            return this.ExecuteQuery<T>(selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }


        public ArrayList ExecuteQuery(Type t, string selectSql, IDataParameter[] commandParameters)
        {
            ArrayList resultList = new ArrayList();
            this.ExecuteQueryImpl(resultList, t, CommandType.Text, selectSql, commandParameters);
            return resultList;
        }



        public ArrayList ExecuteQuery(Type t, string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteQuery(t, selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public List<T> ExecuteQueryBySP<T>(string spName, IDataParameter[] commandParameters) where T : class, new()
        {
            List<T> resultList = new List<T>();
            this.ExecuteQueryImpl(resultList, typeof(T), CommandType.StoredProcedure, spName, commandParameters);
            return resultList;
        }


        public List<T> ExecuteQueryBySP<T>(string spName, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new()
        {
            return this.ExecuteQueryBySP<T>(spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public ArrayList ExecuteQueryBySP(Type t, string spName, IDataParameter[] commandParameters)
        {
            ArrayList resultList = new ArrayList();
            this.ExecuteQueryImpl(resultList, t, CommandType.StoredProcedure, spName, commandParameters);
            return resultList;
        }


        public ArrayList ExecuteQueryBySP(Type t, string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteQueryBySP(t, spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }




        public object ExecuteScalar(string selectSql, IDataParameter[] commandParameters)
        {
            return this.ExecuteScalarImpl(CommandType.Text, selectSql, commandParameters);
        }



        public object ExecuteScalar(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteScalar(selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public object ExecuteScalarBySP(string spName, IDataParameter[] commandParameters)
        {
            return this.ExecuteScalarImpl(CommandType.StoredProcedure, spName, commandParameters);
        }


        public object ExecuteScalarBySP(string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.ExecuteDataReaderBySP(spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }



        public void Fill(DataTable dataTable, string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            this.Fill(dataTable, selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }



        public void Fill(DataTable dataTable, string selectSql, IDataParameter[] commandParameters)
        {
            this.FillImpl(dataTable, CommandType.Text, selectSql, commandParameters);
        }


        public void FillBySP(DataSet dataSet, string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            this.FillBySP(dataSet, spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public void FillBySP(DataSet dataSet, string spName, IDataParameter[] commandParameters)
        {
            this.FillImpl(dataSet, CommandType.StoredProcedure, spName, commandParameters);
        }

        public void FillBySP(DataTable dataTable, string spName, IDataParameter[] commandParameters)
        {
            this.FillImpl(dataTable, CommandType.StoredProcedure, spName, commandParameters);
        }
        public void FillBySP(DataTable dataTable, string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            this.FillBySP(dataTable, spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public void FillDataTable(DataTable table, string tableName, params KeyValuePair<string, object>[] fieldsAndValues)
        {
            StringBuilder sb = new StringBuilder(" 1=1 ");
            string str2 = tableName;
            if (string.IsNullOrWhiteSpace(tableName))
            {
                tableName = this.EncodeTableName(table.TableName);
                str2 = table.TableName;
            }
            if ((fieldsAndValues != null) && (fieldsAndValues.Length != 0))
            {
              
                
                for (int i = 0; i < fieldsAndValues.Length; i++)
                {
                  var   key = fieldsAndValues[i].Key;
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        
                        throw new ArgumentException($"fieldsAndValues的Key{i}为空");
                    }

                 var    objValue = fieldsAndValues[i].Value;

                    if ((objValue == null) || (objValue == DBNull.Value))
                    {

                        sb.AppendLine($"{ this.EncodeFieldEntityName(key)} IS {this.GetSqlValueString(objValue)}");
                    }
                    else
                    {
                        sb.AppendLine($"{this.GetWhereConditionFieldExpress(str2, key)} = {this.GetWhereConditionValueExpress(str2, key, this.GetSqlValueString(objValue))}");
                    }
                }
            }
            if ((table.Columns.Count == 0))
            {
                this.Fill(table, $"SELECT * FROM {tableName} WHERE 1=0");
            }
            string  sql = null;
            if (table.Columns.Count == 0)
            {
                sql = $"SELECT * FROM {tableName} "  + (sb.Length == 0 ? "" : (" WHERE " + sb.ToString()));
            }
            else
            {
                sql = this.GetSelectSql(table, tableName) + (sb.Length == 0 ? "" : (" WHERE " + sb.ToString()));
            }

            this.Fill(table, sql);

        }

        public virtual int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters)
        {
            IDataReader reader = null;
            int result;
            if (pageSize <= 0)
            {
                throw new ArgumentException("pageSize不能小于等于0");
            }
            if (curPageNum <= 0)
            {
                throw new ArgumentException("curPageNum不能小于等于0");
            }
            try
            {
                int returnRowCountMaxValue = 0;
                if (this.ReturnRowCountMaxPages > 0)
                {
                    returnRowCountMaxValue = ((curPageNum + this.ReturnRowCountMaxPages) - 1) * pageSize;
                }
                reader = this.ExecuteDataReader(selectSql, commandParameters);
                result = this.DataReaderToList(list as IList, typeof(T), reader, pageSize * (curPageNum - 1), (pageSize * curPageNum) - 1, sumFields, returnRowCountMaxValue);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
            }
            return result;
        }

        public int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.FillPaginalData<T>(list, selectSql, pageSize, curPageNum, sumFields, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }


        public virtual int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, IDataParameter[] commandParameters)
        {
            return this.FillPaginalData<T>(list, selectSql, pageSize, curPageNum, null, commandParameters);
        }
        public virtual int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, IDataParameter[] commandParameters)
        {
            return this.FillPaginalData(dataTable, selectSql, pageSize, curPageNum, null, commandParameters);
        }



        public int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.FillPaginalData(dataTable, selectSql, pageSize, curPageNum, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public virtual int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters)
        {
            IDataReader reader = null;
            int result;
            try
            {
                int returnRowCountMaxValue = 0;
                if (this.ReturnRowCountMaxPages > 0)
                {
                    returnRowCountMaxValue = ((curPageNum + this.ReturnRowCountMaxPages) - 1) * pageSize;
                }
                reader = this.ExecuteDataReader(selectSql, commandParameters);
                result = this.FillDataTable(dataTable, reader, pageSize, curPageNum, sumFields, returnRowCountMaxValue);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
            }
            return result;
        }









        public int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.FillPaginalData<T>(list, selectSql, pageSize, curPageNum, parameterNameAndValues);
        }

        public virtual int FillPaginalDataBySP(DataTable dataTable, string spName, int pageSize, int curPageNum, IDataParameter[] commandParameters)
        {
            IDataReader reader = null;
            int result;
            try
            {
                reader = this.ExecuteDataReaderBySP(spName, commandParameters);
                result = this.FillDataTable(dataTable, reader, pageSize, curPageNum, null);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
            }
            return result;
        }



        public int FillPaginalDataBySP(DataTable dataTable, string spName, int pageSize, int curPageNum, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.FillPaginalDataBySP(dataTable, spName, pageSize, curPageNum, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public virtual int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters)
        {
            IDataReader reader = null;
            int result;
            try
            {
                reader = this.ExecuteDataReaderBySP(spName, commandParameters);
                result = this.DataReaderToList(list as IList, typeof(T), reader, pageSize * (curPageNum - 1), (pageSize * curPageNum) - 1, sumFields);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (reader != null)
                    {
                        reader.Close();
                        reader.Dispose();
                    }
                }
                catch
                {
                }
            }
            return result;
        }
        public virtual int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, IDataParameter[] commandParameters)
        {
            return this.FillPaginalDataBySP<T>(list, spName, pageSize, curPageNum, null, commandParameters);
        }



        public int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            return this.FillPaginalDataBySP<T>(list, spName, pageSize, curPageNum, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public void FillQuery(IList list, Type t, string selectSql, IDataParameter[] commandParameters)
        {
            this.ExecuteQueryImpl(list, t, CommandType.Text, selectSql, commandParameters);
        }
        public void FillQuery(IList list, Type t, string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            this.FillQuery(list, t, selectSql, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }
        public void FillQuery<T>(IList<T> list, string selectSql, IDataParameter[] commandParameters) where T : class, new()
        {
            this.FillQuery(list as IList, typeof(T), selectSql, commandParameters);
        }
        public void FillQuery<T>(IList<T> list, string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new()
        {
            this.FillQuery(list as IList, typeof(T), selectSql, parameterNameAndValues);
        }



        public void FillQueryBySP(IList list, Type t, string spName, IDataParameter[] commandParameters)
        {
            this.ExecuteQueryImpl(list, t, CommandType.StoredProcedure, spName, commandParameters);
        }


        public void FillQueryBySP(IList list, Type t, string spName, params KeyValuePair<string, object>[] parameterNameAndValues)
        {
            this.FillQueryBySP(list, t, spName, this.NameValueArrayToParamValueArray(parameterNameAndValues));
        }

        public void FillQueryBySP<T>(IList<T> list, string spName, IDataParameter[] commandParameters) where T : class, new()
        {
            this.FillQueryBySP(list as IList, typeof(T), spName, commandParameters);
        }


        public void FillQueryBySP<T>(IList<T> list, string spName, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new()
        {
            this.FillQueryBySP(list as IList, typeof(T), spName, parameterNameAndValues);
        }


        public int FillRowsByPrimaryKeysValue(DataTable dataTable)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (dataTable.Rows.Count == 0)
            {
                return 0;
            }
            if ((dataTable.PrimaryKey == null) || (dataTable.PrimaryKey.Length == 0))
            {
                throw new ArgumentException("未定义主键！~");
            }
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();
            foreach (DataRow row in dataTable.Rows)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" OR ");
                }
                if (builder2.Length > 0)
                {
                    builder2.Remove(0, builder2.Length);
                }
                foreach (DataColumn column in dataTable.PrimaryKey)
                {
                    if (builder2.Length > 0)
                    {
                        builder2.Append(" AND ");
                    }
                    builder2.Append(column.ColumnName + "=" + this.GetSqlValueString(row[column]));
                }
                builder.Append("(" + builder2 + ")");
            }
            DataTable table = dataTable.Clone();
            this.Load(table, builder.ToString(), "");
            foreach (DataRow row2 in table.Rows)
            {
                object[] keys = new object[table.PrimaryKey.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = row2[table.PrimaryKey[i]];
                }
                DataRow row3 = dataTable.Rows.Find(keys);
                if (row3 != null)
                {
                    foreach (DataColumn column2 in dataTable.Columns)
                    {
                        row3[column2] = row2[column2.ColumnName];
                    }
                }
            }
            return table.Rows.Count;
        }
        public virtual DataTable[] FillTableSchema(DataSet dataSet, SchemaType schemaType, string tableName)
        {
            DataTable[] tableArray;

            IDbDataAdapter adapter = this.CreateDataAdapterInstance();
            try
            {
                adapter.SelectCommand.CommandText = $"SELECT * FROM {tableName}";
                tableArray = adapter.FillSchema(dataSet, schemaType);
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    adapter.SelectCommand.Dispose();
                }
                catch
                {
                }
            }
            return tableArray;
        }
        public virtual string GetBatchExecuteSqlBeginString()
        {
            return this.BatchExecuteSqlBeginString;
        }
        public string GetBatchExecuteSqlEndString()
        {
            return this.BatchExecuteSqlEndString;
        }





        public virtual string GetBatchExecuteSqlSeparator()
        {
            return this.BatchExecuteSqlSeparator;
        }

        public abstract IDataParameter GetDataParameterInstance();


        public virtual IDataParameter GetDataParameterInstance(string paramName)
        {
            IDataParameter dataParameterInstance = this.GetDataParameterInstance();
            dataParameterInstance.ParameterName = paramName;
            return dataParameterInstance;
        }
        public virtual IDataParameter GetDataParameterInstance(string paramName, object paramValue, ParameterDirection direction)
        {
            IDataParameter dataParameterInstance = this.GetDataParameterInstance(paramName);
            dataParameterInstance.Value = (paramValue == null) ? DBNull.Value : paramValue;
            dataParameterInstance.Direction = direction;
            return dataParameterInstance;
        }


        public abstract IDataParameter GetDataParameterInstance(string paramName, string dbTypeString);
        public abstract IDataParameter GetDataParameterInstance(string paramName, string dbTypeString, int size);


        public string GetLikeSqlValueString(object value, bool isLeftLike = true, bool isRightLike = true, string dateFormat = null)
        {
            string stringSqlValue = "";
            if ((value == null) || (value == DBNull.Value))
            {
                return "NULL";
            }

            if (value is DateTime)
            {
                stringSqlValue = this.GetDateTimeSqlString(Convert.ToDateTime(value), dateFormat);
            }
            else if (value is bool)
            {

                if (Convert.ToBoolean(value))
                    stringSqlValue = this.TrueValue.ToString();

                else
                    stringSqlValue = this.FalseValue.ToString();

            }
            else if ((((value is decimal) || (value is short)) || ((value is int) || (value is long))) || ((((value is double) || (value is byte)) || ((value is sbyte) || (value is float))) || (((value is ushort) || (value is uint)) || (value is ulong))))
            {
                stringSqlValue = value.ToString();
            }
            else if (((value is string) || (value is char)) || (value is Guid))
            {
                stringSqlValue = this.EncodeLikeSqlValueString(value.ToString());
            }
            else
            {
                stringSqlValue = null;
            }
            if (stringSqlValue == null)
            {
                return stringSqlValue;
            }
            if (isLeftLike & isRightLike)
            {

                stringSqlValue = string.Concat(new[] { "'", this.LikeString, stringSqlValue, this.LikeString, "'" });
            }
            else if (isLeftLike)
            {
                stringSqlValue = "'" + this.LikeString + stringSqlValue + "'";
            }
            else
            {
                stringSqlValue = "'" + stringSqlValue + this.LikeString + "'";
            }
            return this.EndProcessStringSqlValueString(stringSqlValue);
        }
        public abstract DataTable GetSchema();

        public abstract DataTable GetSchema(string collectionName);
        public abstract DataTable GetSchema(string collectionName, string[] restrictionValues);

        public virtual string GetSqlValueString(object value, string dateFormat = null)
        {
            if ((value == null) || (value == DBNull.Value))
            {
                return "NULL";
            }
            if (value is DateTime)
            {
                return this.GetDateTimeSqlString((DateTime)value, dateFormat);
            }
            if (value is bool)
            {
                if ((bool)value)
                {
                    return this.TrueValue.ToString();
                }
                return this.FalseValue.ToString();
            }
            if ((((value is decimal) || (value is short)) || ((value is int) || (value is long))) || ((((value is double) || (value is byte)) || ((value is sbyte) || (value is float))) || (((value is ushort) || (value is uint)) || (value is ulong))))
            {
                return value.ToString();
            }
            if (((value is string) || (value is char)) || (value is Guid))
            {
                return this.EndProcessStringSqlValueString("'" + value.ToString().Replace("'", "''") + "'");
            }
            return null;
        }
        public int Insert(object dataModelObj, string tableName)
        {
            return this.Insert(dataModelObj, tableName, null);
        }




        public int Insert(object dataModelObj, string tableName, UpdateFields fields)
        {
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            if (dataModelObj is DataRow)
            {
                return this.InsertDataRow((DataRow)dataModelObj, tableName, fields);
            }
            if (dataModelObj is DataTable)
            {
                return this.InsertDataTable((DataTable)dataModelObj, tableName, fields);
            }
            if (dataModelObj is DataSet)
            {
                return this.InsertDataSet((DataSet)dataModelObj, fields);
            }
            if (dataModelObj is DataView)
            {
                return this.InsertDataTable(((DataView)dataModelObj).Table, tableName, fields);
            }
            if (dataModelObj is DataRowView)
            {
                return this.InsertDataRow(((DataRowView)dataModelObj).Row, tableName, fields);
            }
            if (dataModelObj is IDataReader)
            {
                return this.BatchInsertDataReader((IDataReader)dataModelObj, tableName, fields, 1);
            }
            if (dataModelObj is IList)
            {
                int num = 0;
                IList list = dataModelObj as IList;
                if (list.Count == 0)
                {
                    return 0;
                }
                Type type = null;
                PropertyInfo[] infoArray = null;
                string fullName = null;
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            if (((list[i] is IList) || (list[i] is DataTable)) || ((list[i] is DataRow) || (list[i] is DataSet)))
                            {
                                num += this.Insert(list[i], tableName, fields);
                            }
                            else
                            {
                                if (type == null)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                }
                                else if (list[i].GetType().FullName != fullName)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                }
                                num += this.InsertNonDataTable(list[i], type, infoArray, tableName, fields);
                            }
                        }
                    }
                    return num;
                }
                catch
                {
                    throw;
                }
            }
            Type objType = dataModelObj.GetType();
            PropertyInfo[] properties = dataModelObj.GetType().GetProperties();
            return this.InsertNonDataTable(dataModelObj, objType, properties, tableName, fields);
        }

        public int InsertDataReader(IDataReader reader, string tableName)
        {
            return this.BatchInsertDataReader(reader, tableName, null, 1);
        }
        public int InsertDataReader(IDataReader reader, string tableName, UpdateFields fields)
        {
            return this.BatchInsertDataReader(reader, tableName, fields, 1);
        }

        public int InsertDataRow(DataRow insertRow)
        {
            return this.InsertDataRow(insertRow, null, null);
        }

        public int InsertDataRow(DataRow insertRow, string tableName)
        {
            return this.InsertDataRow(insertRow, tableName, null);
        }

        public int InsertDataRow(DataRow insertRow, string tableName, UpdateFields fields)
        {
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();

            if (!this.GenerateInsertDataRowSql(sqlBuffer, parameterList, parameterList.Count, insertRow, tableName, fields))
            {
                return 0;
            }
            if ((parameterList == null) || (parameterList.Count == 0))
            {
                return this.ExecuteNonQuery(sqlBuffer.ToString());
            }
            return this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
        }

        public int InsertDataRow(DataRow insertRow, UpdateFields fields)
        {
            return this.InsertDataRow(insertRow, null, fields);
        }




        public int InsertDataSet(DataSet dataSet)
        {
            return this.InsertDataSet(dataSet, null);
        }

        public int InsertDataSet(DataSet dataSet, UpdateFields fields)
        {
            int num2;
            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if ((dataSet.Tables.Count == 0) || (dataSet.Tables.Count == 0))
            {
                return 0;
            }
            int num = 0;
            try
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    num += this.InsertDataTable(table, this.EncodeTableEntityName(table.TableName), fields);
                }
                num2 = num;
            }
            catch
            {
                throw;
            }
            return num2;
        }
        public int InsertDataTable(DataTable dataTable)
        {
            return this.InsertDataTable(dataTable, null, null);
        }


        public int InsertDataTable(DataTable dataTable, string tableName)
        {
            return this.InsertDataTable(dataTable, tableName, null);
        }

        public int InsertDataTable(DataTable dataTable, string tableName, UpdateFields fields)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if ((dataTable.Rows.Count == 0) || (dataTable.Columns.Count == 0))
            {
                return 0;
            }
            int result = 0;
            try
            {

                foreach (DataRow row in dataTable.Rows)
                {
                    result += this.InsertDataRow(row, tableName, fields);
                }

            }
            catch
            {
                throw;
            }
            return result;
        }


        public int InsertDataTable(DataTable dataTable, UpdateFields fields)
        {
            return this.InsertDataTable(dataTable, null, fields);
        }

        public void Load(DataTable dataTable)
        {
            this.Load(dataTable, "", "");
        }



        public void Load(DataTable dataTable, string conditionSql)
        {
            this.Load(dataTable, conditionSql, "");
        }



        public void Load(DataTable dataTable, string conditionSql, string sort)
        {
            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if (dataTable.Columns.Count != 0)
            {
                StringBuilder builder = new StringBuilder();
                builder.Append(this.GetSelectSql(dataTable));
                if (!string.IsNullOrEmpty(conditionSql))
                {
                    builder.Append(" WHERE " + conditionSql);
                }
                if (!string.IsNullOrEmpty(sort))
                {
                    builder.Append(" ORDER BY " + sort);
                }
                this.Fill(dataTable, builder.ToString());
            }
        }

        public ArrayList Load(Type t, string tableName)
        {
            return this.Load(t, tableName, null);
        }




        public ArrayList Load(Type t, string tableName, string conditionSql)
        {
            return this.Load(t, tableName, conditionSql, null);
        }



        public ArrayList Load(Type t, string tableName, string conditionSql, string sort)
        {
            ArrayList list = new ArrayList();
            this.Load(list, t, tableName, conditionSql, sort);
            return list;
        }
        public List<T> Load<T>(string tableName, string conditionSql, string sort) where T : class, new()
        {
            List<T> list = new List<T>();
            this.Load(list, typeof(T), tableName, conditionSql, sort);
            return list;
        }
        public T LoadBySql<T>(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new()
        {
            T result = null;

            List<T> list = null;
            try
            {
                list = this.ExecuteQuery<T>(selectSql, parameterNameAndValues);
                if ((list != null) && (list.Count > 0))
                {
                    result = list[0];
                }

            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    if (list != null)
                    {
                        list.Clear();
                        list.Capacity = 0;
                    }
                }
                catch
                {
                }
            }
            return result;

        }

        public T LoadDataModel<T>(string tableName, params KeyValuePair<string, object>[] fieldsAndValues) where T : class, new()
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            List<T> list = this.LoadDataModelListImp<T>(tableName, true, fieldsAndValues);
            return list.FirstOrDefault();
        }



        public List<T> LoadDataModelList<T>(string tableName, params KeyValuePair<string, object>[] fieldsAndValues) where T : class, new()
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }
            return this.LoadDataModelListImp<T>(tableName, false, fieldsAndValues);
        }
        public virtual void Open()
        {
            if (this.ConnectionInstance.State == ConnectionState.Closed)
            {
                this.ConnectionInstance.Open();
                this._logger.Info(  "打开连接->");
                this.OnOpened(this, new EventArgs());
            }
        }
        public void RollbackTransaction()
        {
            if (!this.IsTransaction)
            {
                throw new InvalidOperationException("当前未处于事务环境");
            }
            this._dbTransaction.Rollback();
            this._logger.Info ("事务回滚->");
            this._dbTransaction.Dispose();
            this._dbTransaction = null;
            this._isTransaction = false;
        }
        public override string ToString()
        {
            return (base.GetType().ToString() + ", ConnectionString:" + this.ConnectionInstance.ConnectionString);

        }


        public int Update(object dataModelObj, string tableName)
        {
            return this.Update(dataModelObj, tableName, new string[0]);
        }




        public int Update(object dataModelObj, string tableName, params string[] primaryKeyFields)
        {
            return this.Update(dataModelObj, tableName, null, primaryKeyFields);
        }
        public int Update(object dataModelObj, string tableName, UpdateFields fields, params string[] primaryKeyFields)
        {
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            string[] strArray = primaryKeyFields;
            if (dataModelObj is DataRow)
            {
                return this.UpdateDataRow((DataRow)dataModelObj, tableName, fields, primaryKeyFields);
            }
            if (dataModelObj is DataTable)
            {
                return this.UpdateDataTable((DataTable)dataModelObj, tableName, fields, primaryKeyFields);
            }
            if (dataModelObj is DataSet)
            {
                return this.UpdateDataSet((DataSet)dataModelObj, fields);
            }
            if (dataModelObj is DataView)
            {
                return this.UpdateDataTable(((DataView)dataModelObj).Table, tableName, fields, primaryKeyFields);
            }
            if (dataModelObj is DataRowView)
            {
                return this.UpdateDataRow(((DataRowView)dataModelObj).Row, tableName, fields, primaryKeyFields);
            }
            if (dataModelObj is IList)
            {
                int num = 0;
                IList list = dataModelObj as IList;
                if (list.Count == 0)
                {
                    return 0;
                }
                Type type = null;
                PropertyInfo[] infoArray = null;
                string fullName = null;
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            if (((list[i] is IList) || (list[i] is DataTable)) || ((list[i] is DataRow) || (list[i] is DataSet)))
                            {
                                num += this.Update(list[i], tableName, fields, primaryKeyFields);
                            }
                            else
                            {
                                if (type == null)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                    if ((strArray == null) || (strArray.Length == 0))
                                    {
                                        primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(infoArray);
                                    }
                                }
                                else if (list[i].GetType().FullName != fullName)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                    if ((strArray == null) || (strArray.Length == 0))
                                    {
                                        primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(infoArray);
                                    }
                                }
                                num += this.UpdateNonDataTable(list[i], type, infoArray, tableName, fields, primaryKeyFields);
                            }
                        }
                    }
                    return num;
                }
                catch
                {
                    throw;
                }
            }
            Type objType = dataModelObj.GetType();
            PropertyInfo[] properties = dataModelObj.GetType().GetProperties();
            if ((strArray == null) || (strArray.Length == 0))
            {
                primaryKeyFields = this.GetPrimaryKeyInfoByAttribute(properties);
            }
            return this.UpdateNonDataTable(dataModelObj, objType, properties, tableName, fields, primaryKeyFields);
        }
        public int UpdateByCondition(object dataModelObj, string tableName, string conditionSql)
        {
            return this.UpdateByCondition(dataModelObj, tableName, null, conditionSql);
        }
        public int UpdateByCondition(object dataModelObj, string tableName, UpdateFields fields, string conditionSql)
        {
            if (dataModelObj == null)
            {
                throw new ArgumentNullException("dataModelObj");
            }
            if (dataModelObj is DataRow)
            {
                return this.UpdateDataRowByCondition((DataRow)dataModelObj, tableName, fields, conditionSql);
            }
            if (dataModelObj is DataTable)
            {
                return this.UpdateDataTableByCondition((DataTable)dataModelObj, tableName, fields, conditionSql);
            }
            if (dataModelObj is DataSet)
            {
                DataSet set = (DataSet)dataModelObj;
                if (set.Tables.Count == 0)
                {
                    return 0;
                }
                int num = 0;
                foreach (DataTable table in set.Tables)
                {
                    num += this.UpdateDataTableByCondition(table, this.EncodeTableEntityName(table.TableName), conditionSql);
                }
                return num;
            }
            if (dataModelObj is DataView)
            {
                return this.UpdateDataTableByCondition(((DataView)dataModelObj).Table, tableName, fields, conditionSql);
            }
            if (dataModelObj is DataRowView)
            {
                return this.UpdateDataRowByCondition(((DataRowView)dataModelObj).Row, tableName, fields, conditionSql);
            }
            if (dataModelObj is IList)
            {
                int num2 = 0;
                IList list = dataModelObj as IList;
                if (list.Count == 0)
                {
                    return 0;
                }
                Type type = null;
                PropertyInfo[] infoArray = null;
                string fullName = null;
                try
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list[i] != null)
                        {
                            if (((list[i] is IList) || (list[i] is DataTable)) || ((list[i] is DataRow) || (list[i] is DataSet)))
                            {
                                num2 += this.UpdateByCondition(list[i], tableName, fields, conditionSql);
                            }
                            else
                            {
                                if (type == null)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                }
                                else if (list[i].GetType().FullName != fullName)
                                {
                                    type = list[i].GetType();
                                    infoArray = type.GetProperties();
                                    fullName = type.FullName;
                                }
                                num2 += this.UpdateNonDataTableByCondition(list[i], type, infoArray, tableName, fields, conditionSql);
                            }
                        }
                    }
                    return num2;
                }
                catch
                {
                    throw;
                }
            }
            Type objType = dataModelObj.GetType();
            PropertyInfo[] properties = dataModelObj.GetType().GetProperties();
            return this.UpdateNonDataTableByCondition(dataModelObj, objType, properties, tableName, fields, conditionSql);
        }
        public int UpdateDataRow(DataRow updateRow, string tableName, params string[] updateConditionFields)
        {
            return this.UpdateDataRow(updateRow, tableName, null, updateConditionFields);
        }

        public int UpdateDataRow(DataRow updateRow, string tableName, UpdateFields fields, params string[] updateConditionFields)
        {
            StringBuilder sqlBuffer = new StringBuilder();
            ArrayList parameterList = new ArrayList();

            if (!this.GenerateUpdateDataRowSql(sqlBuffer, parameterList, 0, updateRow, tableName, fields, false, updateConditionFields))
            {
                return 0;
            }
            if (sqlBuffer.Length <= 0)
            {
                return 0;
            }
            if ((parameterList == null) || (parameterList.Count == 0))
            {
                return this.ExecuteNonQuery(sqlBuffer.ToString());
            }
            return this.ExecuteNonQuery(sqlBuffer.ToString(), (IDataParameter[])parameterList.ToArray(typeof(IDataParameter)));
        }




        public int UpdateDataRowByCondition(DataRow updateRow, string tableName, string conditionSql)
        {
            return this.UpdateDataRowByCondition(updateRow, tableName, null, conditionSql);
        }


        public int UpdateDataRowByCondition(DataRow updateRow, string tableName, UpdateFields fields, string conditionSql)
        {
            if (updateRow == null)
            {
                throw new ArgumentNullException("updateRow");
            }
            string str = tableName;
            if (string.IsNullOrEmpty(tableName))
            {
                tableName = this.EncodeTableEntityName(updateRow.Table.TableName);
                str = updateRow.Table.TableName;
            }
            string sqlValueString = "";
            ArrayList list = null;
            int num = 0;
            IDataParameter dataParameterInstance = null;
            string str3 = "";
            StringBuilder builder = new StringBuilder();
            StringBuilder builder2 = new StringBuilder();

            DataRow row = updateRow;
            builder.Append("UPDATE " + tableName + " SET ");
            foreach (DataColumn column in updateRow.Table.Columns)
            {
                if (fields != null)
                {
                    /*
                    if (fields.ContainsField(column.ColumnName))
                    {
                        if (fields.Option != UpdateFieldsOptions.ExcludeFields)
                        {
                            goto Label_00C7;
                        }
                        continue;
                    }
                    if (fields.Option == UpdateFieldsOptions.IncludeFields)
                    {
                        continue;
                    }*/
                    if (fields.ContainsField(column.ColumnName) && fields.Option == UpdateFieldsOptions.ExcludeFields)
                        continue;
                    if (fields.Option == UpdateFieldsOptions.IncludeFields)
                        continue;
                }
                // Label_00C7:
                builder2.Append((builder2.Length == 0) ? "" : ",");
                sqlValueString = this.GetSqlValueString(row[column.ColumnName]);
                if (sqlValueString != null)
                {
                    builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.GetUpdateValueExpress(str, column.ColumnName, sqlValueString));
                }
                else
                {
                    num++;
                    builder2.Append(this.EncodeFieldEntityName(column.ColumnName) + " = " + this.ParamPrefixFullString + num.ToString());
                    if (list == null)
                    {
                        list = new ArrayList();
                    }
                    dataParameterInstance = this.GetDataParameterInstance();
                    dataParameterInstance.ParameterName = this.ParamPrefixFullString + num.ToString();
                    dataParameterInstance.Value = row[column.ColumnName];
                    list.Add(dataParameterInstance);
                }
            }
            if (builder2.Length == 0)
            {
                return 0;
            }
            builder.Append(builder2.ToString());
            builder2.Remove(0, builder2.Length);
            if (!string.IsNullOrEmpty(conditionSql))
            {
                builder2.Append(" WHERE " + conditionSql);
            }
            builder.Append(builder2.ToString());
            builder.Append(" ");
            if (str3.Length <= 0)
            {
                return 0;
            }
            if ((list == null) || (list.Count > 0))
            {
                return this.ExecuteNonQuery(str3.ToString());
            }
            return this.ExecuteNonQuery(str3.ToString(), (IDataParameter[])list.ToArray(dataParameterInstance.GetType()));
        }




        public int UpdateDataSet(DataSet dataSet)
        {
            return this.UpdateDataSet(dataSet, null);
        }

        public int UpdateDataSet(DataSet dataSet, UpdateFields fields)
        {
            int result = 0;

            if (dataSet == null)
            {
                throw new ArgumentNullException("dataSet");
            }
            if (dataSet.Tables.Count == 0)
            {
                return 0;
            }
            try
            {
                foreach (DataTable table in dataSet.Tables)
                {
                    result += this.UpdateDataTable(table, this.EncodeTableEntityName(table.TableName), fields);
                }

            }
            catch
            {
                throw;
            }
            return result;
        }





        public int UpdateDataTable(DataTable dataTable, string tableName, params string[] updateConditionFields)
        {
            return this.UpdateDataTable(dataTable, tableName, null, updateConditionFields);
        }
        public int UpdateDataTable(DataTable dataTable, string tableName, UpdateFields fields, params string[] updateConditionFields)
        {
            int result = 0;

            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if ((dataTable.Rows.Count == 0) || (dataTable.Columns.Count == 0))
            {
                return 0;
            }
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result += this.UpdateDataRow(row, tableName, fields, updateConditionFields);
                }

            }
            catch
            {
                throw;
            }
            return result;
        }
        public int UpdateDataTableByCondition(DataTable dataTable, string tableName, string conditionSql)
        {
            return this.UpdateDataTableByCondition(dataTable, tableName, null, conditionSql);
        }


        public int UpdateDataTableByCondition(DataTable dataTable, string tableName, UpdateFields fields, string conditionSql)
        {

            if (dataTable == null)
            {
                throw new ArgumentNullException("dataTable");
            }
            if ((dataTable.Rows.Count == 0) || (dataTable.Columns.Count == 0))
            {
                return 0;
            }
            int result = 0;
            try
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    result += this.UpdateDataRowByCondition(row, tableName, fields, conditionSql);
                }

            }
            catch
            {
                throw;
            }
            return result;
        }





    }
}
