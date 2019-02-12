using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    public interface  IDataAccess:IDisposable
    {
        /// <summary>
        /// 表示连接正在关闭的事件。
        /// </summary>
        event EventHandler Closing;
        /// <summary>
        ///  表示连接已打开的事件。
        /// </summary>
        event EventHandler Opened;
        /// <summary>
        ///  获取或设置执行SQL语句的超时时间。
        /// </summary>
        int CommandTimeOut { get; set; }
        /// <summary>
        ///  获取当前连接的连接字符串。
        /// </summary>
        string ConnectionString { get; set; }
        /// <summary>
        ///  获取连接的超时时间。
        /// </summary>
        int ConnectionTimeout { get; }
        /// <summary>
        /// 获取当前连接的数据库。
        /// </summary>
        string Database { get; }
        /// <summary>
        ///  获取当前对象是否已经释放资源了。
        /// </summary>
        bool Disposed { get; }
        /// <summary>
        ///  获取当前是否处于事务中。
        /// </summary>
        bool IsTransaction { get; }
        /// <summary>
        ///  获取在SQL语句中所带参数的前缀。
        /// </summary>
        string ParameterPrefixInSql { get; }
        /// <summary>
        /// 获取或设置执行填充页面数据方法时，返回总数据行数的方法，该值表示返回的数据总行数为截止到从前页面开始到第N个页面的末尾的数据的总行数，这样可以避免计算总数据行数而提高性能。
        /// 如果设置为小于或等于0的值，则始终返回总数据行数。
        /// 如设置本属性值为10，每页显示数据行数为20，当前需要获取第2页，则返回值至多不超过 220 【(2+10-1)*20】 行。
        /// </summary>
        int ReturnRowCountMaxPages { get; set; }

        /// <summary>
        ///  获取当前数据库连接的状态。
        /// </summary>
        ConnectionState State { get; }
        /// <summary>
        /// 每次以50行数据批量插入数据。 将任意类型对象的数据插入到数据库。
        /// 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList,IDataReader
        /// 以及其他的包含基本数据类型（如 int,string, ）作为公共属性的 Class 。 如果提供的数据源是 IDataReader
        /// 对象,则请确保打开 System.Data.IDataReader 的数据库连接对象不能是当前连接对象。
        /// </summary>
        /// <param name="dataModelObj"> 需要插入数据的存储对象。</param>
        /// <param name="tableName">数据库中对应的表名称。</param>
        /// <returns>返回总共影响的行数.</returns>
        int BatchInsert(object dataModelObj, string tableName);
        /// <summary>
        ///将指定的数据以批次提交的形式插入到数据库中。 支持表示数据对象的类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList,IDataReader
        ///以及其他的包含基本数据类型（如 int,string ）作为公共属性的普通类型对象 。 如果提供的数据源是 IDataReader
        ///对象,则请确保打开 System.Data.IDataReader 的数据库连接对象不能是当前连接对象。
        /// </summary>
        /// <param name="dataModelObj"> 存储需要插入的数据的对象或对象集合。</param>
        /// <param name="tableName">需要插入数据的数据库中对应的表名称。</param>
        /// <param name="fields"> 字段选项。指定需要保存到数据库中的字段或者不要保存到数据库的字段。</param>
        /// <param name="buffer"> 缓冲值，即每多少行数据提交到数据库服务器执行一次。</param>
        /// <returns>  返回总共插入到数据库中的数据行数。</returns>
        int BatchInsert(object dataModelObj, string tableName, UpdateFields fields, int buffer);
        /// <summary>
        ///以50行数据批量更新数据。 将任意类型对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj"> 需要更新数据的存储对象。</param>
        /// <param name="tableName">需要更新数据库中对应的表的名称。</param>
        /// <param name="primaryKeyFields">  指定主键字段</param>
        /// <returns> 返回总共影响的行数。</returns>
        int BatchUpdate(object dataModelObj, string tableName, params string[] primaryKeyFields);
        /// <summary>
        ///批量更新数据。 将任意类型对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj">需要更新数据的存储对象。</param>
        /// <param name="tableName"> 需要更新数据库中对应的表的名称。</param>
        /// <param name="fields">指定需要更新的字段或排除更新的字段。</param>
        /// <param name="buffer"> 缓冲值，即每多少行提交执行一次SQL语句。</param>
        /// <param name="primaryKeyFields">指定主键字段</param>
        /// <returns> 返回总共影响的行数。</returns>
        int BatchUpdate(object dataModelObj, string tableName, UpdateFields fields, int buffer, params string[] primaryKeyFields);
        /// <summary>
        ///  启动事务。
        /// </summary>
        void BeginTransaction();
        /// <summary>
        ///  启动指定级别的事务。
        /// </summary>
        /// <param name="isolationlevel">  指定事务级别。</param>
        void BeginTransaction(IsolationLevel isolationlevel);
        /// <summary>
        ///  关闭数据库连接。
        /// </summary>
        void Close();
        /// <summary>
        ///  组装“字段=值”的SQL表达式语句。
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">对应的值</param>
        /// <returns>结果</returns>
        string CombineFieldValueEqualExpress(string fieldName, object value);
        /// <summary>
        ///  组装“字段=值”的SQL表达式语句。
        /// </summary>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">对应的值</param>
        /// <param name="datetimeFormat">格式</param>
        /// <returns>结果</returns>
        string CombineFieldValueEqualExpress(string fieldName, object value, string datetimeFormat);
        /// <summary>
        ///  提交事务。
        /// </summary>
        void CommitTransaction();
        /// <summary>
        /// 将符合类型要求的对象的数据从数据库中删除。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj"> 需要删除数据的存储对象。</param>
        /// <param name="tableName">需要删除的数据库表名称。</param>
        /// <param name="deleteConditionFields"> 指定作为删除数据的主键信息，
        /// 如果不指定，则使用 DataTable 中定义的主键或 DataModel 中定义的 ModelUpdatableAttribute
        ///指定的主键。
        ///</param>
        /// <returns></returns>
        int Delete(object dataModelObj, string tableName, params string[] deleteConditionFields);
        /// <summary>
        /// 根据提供的不带 WHERE 关键字的条件 SQL 语句删除数据库中指定表中的数据。
        /// </summary>
        /// <param name="tableName">需要删除数据的表名称。</param>
        /// <param name="sqlCondition"> 指定删除数据的依据，不带 WHERE 关键字的条件 SQL 语句。</param>
        /// <returns>返回总共影响的行数。</returns>
        int DeleteByCondition(string tableName, string sqlCondition);
        /// <summary>
        /// 根据提供的 System.Data.DataRow 对象，从数据库中删除指定的数据行。
        /// </summary>
        /// <param name="row">  需要删除的数据行。</param>
        /// <param name="tableName">  删除数据库中数据的表名称。</param>
        /// <param name="deleteConditionFields"> 删除数据的主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为删除条件的值。 如果不指定这个参数， 则删除的主键依据是
        ///  System.Data.DataTable 对象中对应的主键。
        ///  </param>
        /// <returns></returns>
        int DeleteDataRow(DataRow row, string tableName, params string[] deleteConditionFields);
        /// <summary>
        ///  从数据库中删除指定 System.Data.DataSet 对象表示的数据。
        /// </summary>
        /// <param name="dataSet"> 存储需要删除的数据行的 System.Data.DataSet 对象。</param>
        /// <returns> 返回总共影响的行数。</returns>
        int DeleteDataSet(DataSet dataSet);
        /// <summary>
        ///   根据 对象删除数据库中对应的数据行。
        /// </summary>
        /// <param name="dataTable"> 存储需要删除数据行的 对象。</param>
        /// <param name="tableName"> 需要删除数据的数据库表名称。</param>
        /// <param name="deleteConditionFields">
        /// 删除数据的依据，即主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为删除条件的值。 如果不指定这个参数，
        ///则删除的主键依据是 System.Data.DataTable 对象中对应的主键。</param>
        /// <returns> </returns>
        int DeleteDataTable(DataTable dataTable, string tableName, params string[] deleteConditionFields);
        /// <summary>
        /// 加密字段名称
        /// </summary>
        /// <param name="fieldName">需要加密的字段名称</param>
        /// <returns>加密结果</returns>
        string EncodeFieldName(string fieldName);
        /// <summary>
        /// 加密表名
        /// </summary>
        /// <param name="tableName">需要加密的表名</param>
        /// <returns></returns>
        string EncodeTableName(string tableName);
        /// <summary>
        ///  执行查询，并返回 System.Data.IDataReader 对象。
        /// </summary>
        /// <param name="selectSql"> 需要执行的 SQL 语句。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 查询到的 System.Data.IDataReader 对象。</returns>
        IDataReader ExecuteDataReader(string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行查询，并返回 System.Data.IDataReader 对象。
        /// </summary>
        /// <param name="selectSql">  需要执行的 SQL 查询语句。</param>
        /// <param name="commandParameters">   SQL 语句中的参数信息。</param>
        /// <returns> 查询到的 System.Data.IDataReader 对象。</returns>
        IDataReader ExecuteDataReader(string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        ///   执行指定的存储过程，并返回 System.Data.IDataReader 对象。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters"> 存储过程的参数信息。</param>
        /// <returns> 查询到的 System.Data.IDataReader 对象。</returns>
        IDataReader ExecuteDataReaderBySP(string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行指定的存储过程，并返回 System.Data.IDataReader 对象。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 查询到的 System.Data.IDataReader 对象。</returns>
        IDataReader ExecuteDataReaderBySP(string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的查询语句。
        /// </summary>
        /// <param name="selectSql"> 需要执行的SQL语句。</param>
        /// <param name="commandParameters"> IDataParameter 数组。</param>
        /// <returns>询到的 DataSet。</returns>
        DataSet ExecuteDataSet(string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        ///   执行指定的查询语句。
        /// </summary>
        /// <param name="selectSql"> 需要执行的SQL语句。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 查询到的 DataSet。</returns>
        DataSet ExecuteDataSet(string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///   通过执行存储过程填充 DataSet 对象。
        /// </summary>
        /// <param name="spName">   需要执行的存储过程。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns>  返回查询到的 DataSet 对象。</returns>
        DataSet ExecuteDataSetBySP(string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  通过执行存储过程填充 DataSet 对象。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程。</param>
        /// <param name="commandParameters">IDataParameter 数组。</param>
        /// <returns>返回查询到的 DataSet 对象。</returns>
        DataSet ExecuteDataSetBySP(string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///   执行指定的查询语句。
        /// </summary>
        /// <param name="selectSql"> 需要执行的SQL语句。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 返回查询到的DataTable。</returns>
        DataTable ExecuteDataTable(string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行指定的查询语句。
        /// </summary>
        /// <param name="selectSql">需要执行的SQL语句。</param>
        /// <param name="commandParameters"> IDataParameter 数组。</param>
        /// <returns>  返回查询到的DataTable。</returns>
        DataTable ExecuteDataTable(string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        ///  通过执行存储过程填充 DataTable 对象。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns>返回查询到的DataTable。</returns>
        DataTable ExecuteDataTableBySP(string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  通过执行存储过程填充 DataTable 对象。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters">  IDataParameter 数组。</param>
        /// <returns>返回查询到的DataTable。</returns>
        DataTable ExecuteDataTableBySP(string spName, IDataParameter[] commandParameters);
        /// <summary>
        /// 执行非查询的SQL语句。
        /// </summary>
        /// <param name="nonQuerySql">需要执行的 SQL 语句。</param>
        /// <param name="parameterNameAndValues">  SQL 语句中的参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 返回执行后影响的行数。</returns>
        int ExecuteNonQuery(string nonQuerySql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行非查询的SQL语句。
        /// </summary>
        /// <param name="nonQuerySql">需要执行的 SQL 语句。</param>
        /// <param name="commandParameters"> SQL 语句中的参数信息。</param>
        /// <returns>返回执行后影响的行数。</returns>
        int ExecuteNonQuery(string nonQuerySql, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行非查询的 SQL 存储过程。
        /// </summary>
        /// <param name="spName">  需要执行的存储过程名称。</param>
        /// <param name="commandParameters"> 存储过程中的参数信息。</param>
        /// <returns>返回执行后影响的行数。</returns>
        int ExecuteNonQueryBySP(string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行非查询的 SQL 存储过程。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 返回执行后影响的行数。</returns>
        int ExecuteNonQueryBySP(string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的查询语句，并将结果填充到指定类型的元素的 ArrayList 集合中。
        /// </summary>
        /// <param name="t"> 存储每行数据的对象的类型。</param>
        /// <param name="selectSql"> 需要执行的查询语句。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组。格式如下：参数名1，值1，参数名2，值2...</param>
        /// <returns> 返回 ArrayList 集合。</returns>
        ArrayList ExecuteQuery(Type t, string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行指定的查询语句，并将结果填充到指定类型的元素的 ArrayList 集合中。
        /// </summary>
        /// <param name="t"> 存储每行数据的对象的类型。</param>
        /// <param name="selectSql"> 需要执行的查询语句。</param>
        /// <param name="commandParameters">IDataParameter 数组，提供查询语句中的参数信息。</param>
        /// <returns> 返回 ArrayList 集合。</returns>
        ArrayList ExecuteQuery(Type t, string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行指定的查询语句，并将结果填充到指定类型的元素的 List  泛类型集合中。
        /// </summary>
        /// <typeparam name="T"> 存储每行数据的对象的类型。 </typeparam>
        /// <param name="selectSql">需要执行的查询语句。</param>
        /// <param name="commandParameters">IDataParameter 数组，提供查询语句中的参数信息。</param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string selectSql, IDataParameter[] commandParameters) where T : class, new();
        /// <summary>
        ///  执行指定的查询语句，并将结果填充到指定类型的元素的 List 泛类型集合中。
        /// </summary>
        /// <typeparam name="T">存储每行数据的对象的类型。 </typeparam>
        /// <param name="selectSql"> 需要执行的SQL语句。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。 </param>
        /// <returns></returns>
        List<T> ExecuteQuery<T>(string selectSql, params KeyValuePair<string ,object>[] parameterNameAndValues) where T : class, new();
        /// <summary>
        ///  执行指定的存储过程，并将结果填充到指定类型的元素的 ArrayList 集合中。
        /// </summary>
        /// <param name="t"> 存储每行数据的对象的类型。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters"> IDataParameter 数组，提供查询语句中的参数信息。</param>
        /// <returns></returns>
        ArrayList ExecuteQueryBySP(Type t, string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行指定的存储过程，并将结果填充到指定类型的元素的 ArrayList 集合中。
        /// </summary>
        /// <param name="t"> 存储每行数据的对象的类型。</param>
        /// <param name="spName">  需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues"> IDataParameter 数组，提供查询语句中的参数信息。</param>
        /// <returns>返回 ArrayList 集合。</returns>
        ArrayList ExecuteQueryBySP(Type t, string spName, params KeyValuePair<string,object>[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的存储过程，并将结果填充到指定类型的元素的 List  泛类型集合中。
        /// </summary>
        /// <typeparam name="T"> 存储每行数据的对象的类型。</typeparam>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组</param>
        /// <returns></returns>
        List<T> ExecuteQueryBySP<T>(string spName, params KeyValuePair<string, object>[] parameterNameAndValues) where T : class, new();
        /// <summary>
        ///  执行指定的存储过程，并将结果填充到指定类型的元素的 List  泛类型集合中。
        /// </summary>
        /// <typeparam name="T"> 存储每行数据的对象的类型。</typeparam>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters">IDataParameter 数组，提供查询语句中的参数信息。</param>
        /// <returns></returns>
        List<T> ExecuteQueryBySP<T>(string spName, IDataParameter[] commandParameters) where T : class, new();
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="selectSql">需要执行的 SQL 查询语句。</param>
        /// <param name="commandParameters">  SQL 语句中的参数信息。</param>
        /// <returns> 返回查询到的结果集中第一行的第一列的值，如果未查询到数据则返回 null。</returns>
        object ExecuteScalar(string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        /// 执行查询，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="selectSql"> 需要执行的SQL语句。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组</param>
        /// <returns> 返回查询到的结果集中第一行的第一列的值，如果未查询到数据则返回 null。</returns>
        object ExecuteScalar(string selectSql, params KeyValuePair<string, object>[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的存储过程，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters"> 存储过程中的参数信息。</param>
        /// <returns>返回查询到的结果集中第一行的第一列的值，如果未查询到数据则返回 null。</returns>
        object ExecuteScalarBySP(string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///   执行指定的存储过程，并返回查询所返回的结果集中第一行的第一列。忽略额外的列或行。
        /// </summary>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组。</param>
        /// <returns> 返回查询到的结果集中第一行的第一列的值，如果未查询到数据则返回 null。</returns>
        object ExecuteScalarBySP(string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行指定的 SQL 查询语句，并将结果填充到指定的 System.Data.DataTable 对象中。
        /// </summary>
        /// <param name="dataTable"> 用于填充执行结果的 System.Data.DataTable 对象。</param>
        /// <param name="selectSql"> 需要执行的 SQL 查询语句。</param>
        /// <param name="commandParameters"> SQL 查询语句中的参数信息。</param>
        void Fill(DataTable dataTable, string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        /// 执行指定的 SQL 查询语句，并将结果填充到指定的 System.Data.DataTable 对象中。
        /// </summary>
        /// <param name="dataTable"> 用于填充执行结果的 System.Data.DataTable 对象。</param>
        /// <param name="selectSql"> 需要执行的 SQL 查询语句。</param>
        /// <param name="parameterNameAndValues">参数名称和值数组。</param>
        void Fill(DataTable dataTable, string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的存储过程，并将结果填充到指定的 System.Data.DataSet 对象中。
        /// </summary>
        /// <param name="dataSet"> 用于填充执行结果的 System.Data.DataSet 对象。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters">存储过程的参数信息。</param>
        void FillBySP(DataSet dataSet, string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行指定的存储过程，并将结果填充到指定的 System.Data.DataSet 对象中。
        /// </summary>
        /// <param name="dataSet"> 用于填充执行结果的 System.Data.DataSet 对象。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组</param>
        void FillBySP(DataSet dataSet, string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        /// 执行指定的存储过程，并将结果填充到指定的 System.Data.DataTable 对象中。
        /// </summary>
        /// <param name="dataTable"> 用于填充执行结果的 System.Data.DataTable 对象。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters"> 存储过程的参数信息。</param>
        void FillBySP(DataTable dataTable, string spName, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行指定的存储过程，并将结果填充到指定的 System.Data.DataTable 对象中。
        /// </summary>
        /// <param name="dataTable"> 用于填充执行结果的 System.Data.DataTable 对象。</param>
        /// <param name="spName">需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。</param>
        void FillBySP(DataTable dataTable, string spName, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  检索指定的数据填充到指定的 DataTable 对象中。 适用于指定字段条件检索多条数据。
        /// </summary>
        /// <param name="table"> 要填充检索结果数据的 DataTable 对象，如果存在列信息，则只填充已存在的列，否则，填充数据库中指定名称表的所有字段。</param>
        /// <param name="tableName">检索数据的表名称。</param>
        /// <param name="fieldsAndValues"> 指示检索数据的字段名称和值。</param>
        void FillDataTable(DataTable table, string tableName, params KeyValuePair<string ,object>[] fieldsAndValues);
        /// <summary>
        ///   执行SQL查询语句，向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <param name="dataTable"> 要填充数据的 DataTable 对象。</param>
        /// <param name="selectSql"> 需要执行的 SQL 查询语句。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum">  当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="commandParameters"> SQL 语句中的参数信息。</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行SQL查询语句，向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <param name="dataTable">  要填充数据的 DataTable 对象。</param>
        /// <param name="selectSql">要检索数据的查询 SQL 语句。</param>
        /// <param name="pageSize"> 分页时每页的数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。 </param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <param name="dataTable">  要填充数据的 DataTable 对象。</param>
        /// <param name="selectSql">要检索数据的查询 SQL 语句。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum">当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="sumFields"> 需要存储进行合计的字段集合，对应的 Value 即为合计后的值。</param>
        /// <param name="commandParameters"> 参数名称和值数组。</param>
        /// <returns> 提供的查询语句中结果集总行数。</returns>
        int FillPaginalData(DataTable dataTable, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters);
        /// <summary>
        ///  向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"> 要填充数据的 IList 对象。</param>
        /// <param name="selectSql"> 要检索数据的查询 SQL 语句。</param>
        /// <param name="pageSize">分页时每页数据行数。</param>
        /// <param name="curPageNum">当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组。</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        /// 执行 SQL 查询语句，向指定的 IList<T> 对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"> 要填充数据的 IList  对象。</param>
        /// <param name="selectSql">  需要执行的 SQL 查询语句。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum">当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="commandParameters">  SQL 语句中包含的参数信息。</param>
        /// <returns> 提供的查询语句中结果集总行数。</returns>
        int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, IDataParameter[] commandParameters);
        /// <summary>
        /// 向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"> 要填充数据的 IList 对象。</param>
        /// <param name="selectSql">需要执行的 SQL 查询语句。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="sumFields">需要存储进行合计的字段集合，对应的 Value 即为合计后的值。</param>
        /// <param name="commandParameters">SQL 语句中包含的参数信息</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters);

        /// <summary>
        ///  向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要填充数据的 IList 对象。</param>
        /// <param name="selectSql"> 要检索数据的查询 SQL 语句。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum">当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="sumFields">需要存储进行合计的字段集合，对应的 Value 即为合计后的值。</param>
        /// <param name="parameterNameAndValues">SQL 语句中包含的参数信息</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalData<T>(IList<T> list, string selectSql, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行存储过程，向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <param name="dataTable">要填充数据的 DataTable 对象。</param>
        /// <param name="spName"> 存储过程名称。</param>
        /// <param name="pageSize">分页时每页数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="commandParameters">存储过程中的参数信息。</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalDataBySP(DataTable dataTable, string spName, int pageSize, int curPageNum, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行存储过程，向指定的 DataTable 对象填充指定页的数据。
        /// </summary>
        /// <param name="dataTable">要填充数据的 DataTable 对象。</param>
        /// <param name="spName"> 存储过程名称。</param>
        /// <param name="pageSize">分页时每页数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalDataBySP(DataTable dataTable, string spName, int pageSize, int curPageNum, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行存储过程，向指定的 IList  对象填充指定页的数据。      
        ///   </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"> 要填充数据的 IList  对象。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="pageSize">分页时每页数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组。</param>
        /// <returns>提供的查询语句中结果集总行数。</returns>
        int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行存储过程，向指定的 IList  对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"> </typeparam>
        /// <param name="list">要填充数据的 IList 对象。</param>
        /// <param name="spName">存储过程名称。</param>
        /// <param name="pageSize"> 分页时每页数据行数。</param>
        /// <param name="curPageNum"> 当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="commandParameters"> 存储过程中包含的参数信息。</param>
        /// <returns> 提供的查询语句中结果集总行数。</returns>
        int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, IDataParameter[] commandParameters);
        /// <summary>
        ///  执行存储过程，向指定的 IList  对象填充指定页的数据。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">要填充数据的 IList 对象。</param>
        /// <param name="spName">存储过程名称。</param>
        /// <param name="pageSize">分页时每页数据行数。</param>
        /// <param name="curPageNum">当前需要取得数据的页码（起始页从 1 开始计数）。</param>
        /// <param name="sumFields"> 需要存储进行合计的字段集合，对应的 Value 即为合计后的值。</param>
        /// <param name="commandParameters">存储过程中包含的参数信息</param>
        /// <returns> 提供的查询语句中结果集总行数。</returns>
        int FillPaginalDataBySP<T>(IList<T> list, string spName, int pageSize, int curPageNum, Dictionary<string, decimal> sumFields, IDataParameter[] commandParameters);
        /// <summary>
        /// 执行指定的 SQL 查询语句，并将结果填充到指定的 IList 集合中。
        /// </summary>
        /// <param name="list"> 填充结果的 IList 集合。</param>
        /// <param name="t">存储每行数据的对象的类型</param>
        /// <param name="selectSql">需要执行的 SQL 查询语句。</param>
        /// <param name="commandParameters">SQL 语句中的参数信息</param>
        void FillQuery(IList list, Type t, string selectSql, IDataParameter[] commandParameters);
        /// <summary>
        /// 执行指定的 SQL 查询语句，并将结果填充到指定的 IList 集合中。
        /// </summary>
        /// <param name="list"> 填充结果的 IList 集合。</param>
        /// <param name="t">存储每行数据的对象的类型</param>
        /// <param name="selectSql">需要执行的 SQL 查询语句。</param>
        /// <param name="parameterNameAndValues">SQL 语句中的参数信息</param>
        void FillQuery(IList list, Type t, string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues);
        /// <summary>
        ///  执行指定的 SQL 查询语句，并将结果填充到指定的 IList 集合中。
        /// </summary>
        /// <typeparam name="T">存储每行数据的对象的类型。</typeparam>
        /// <param name="list"> 填充结果的 IList  集合。</param>
        /// <param name="selectSql">需要执行的 SQL 查询语句</param>
        /// <param name="commandParameters">存储过程的参数信息。</param>
        void FillQuery<T>(IList<T> list, string selectSql, IDataParameter[] commandParameters) where T : class, new();
        /// <summary>
        /// 执行指定的 SQL 查询语句，并将结果填充到指定的 IList 集合中。
        /// </summary>
        /// <typeparam name="T">存储每行数据的对象的类型。</typeparam>
        /// <param name="list">填充结果的 IList  集合</param>
        /// <param name="selectSql">需要执行的 SQL 查询语句</param>
        /// <param name="parameterNameAndValues">  参数名称和值数组。</param>
        void FillQuery<T>(IList<T> list, string selectSql, params KeyValuePair<string ,object >[] parameterNameAndValues) where T : class, new();
        /// <summary>
        ///  通过执行存储过程将结果填充到指定的 IList 集合。
        /// </summary>
        /// <param name="list"> 填充结果的 IList 集合。</param>
        /// <param name="t">存储每行数据的对象的类型。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组</param>
        void FillQueryBySP(IList list, Type t, string spName, params KeyValuePair<string, object>[] parameterNameAndValues);
        /// <summary>
        ///  通过执行存储过程将结果填充到指定的 IList 集合。
        /// </summary>
        /// <param name="list"> 填充结果的 IList 集合。</param>
        /// <param name="t">存储每行数据的对象的类型。</param>
        /// <param name="spName"> 需要执行的存储过程名称。</param>
        /// <param name="commandParameters">  存储过程中的参数信息。</param>
        void FillQueryBySP(IList list, Type t, string spName, IDataParameter[] commandParameters);
        /// <summary>
        /// 通过执行存储过程将结果填充到指定的 IList 集合。
        /// </summary>
        /// <typeparam name="T">存储每行数据的对象的类型</typeparam>
        /// <param name="list">填充结果的 IList  集合。</param>
        /// <param name="spName">需要执行的存储过程名称</param>
        /// <param name="parameterNameAndValues">参数名称和值数组</param>
        void FillQueryBySP<T>(IList<T> list, string spName, params KeyValuePair<string ,object >[] parameterNameAndValues) where T : class, new();
        /// <summary>
        /// 通过执行存储过程将结果填充到指定的 IList 集合。
        /// </summary>
        /// <typeparam name="T">存储每行数据的对象的类型</typeparam>
        /// <param name="list">填充结果的 IList  集合。</param>
        /// <param name="spName">需要执行的存储过程名称</param>
        /// <param name="commandParameters"> 存储过程中的参数信息。</param>
        void FillQueryBySP<T>(IList<T> list, string spName, IDataParameter[] commandParameters) where T : class, new();
        /// <summary>
        ///  根据提供的 DataTable 对象的主键字段和值作为条件检索数据填充该 DataTable 对象中的非主键字段。
        /// </summary>
        /// <param name="dataTable">填充数据的 DataTable 对象。</param>
        /// <returns>实际填充的行数。</returns>
        int FillRowsByPrimaryKeysValue(DataTable dataTable);
        /// <summary>
        ///  将名为“Table”的 System.Data.DataTable 添加到指定的 System.Data.DataSet 中， 并根据指定的 System.Data.SchemaType
        ///配置架构以匹配数据源中的架构。
        /// </summary>
        /// <param name="dataSet">要用数据源中的架构填充的 System.Data.DataSet。</param>
        /// <param name="schemaType">System.Data.SchemaType 值之一。</param>
        /// <param name="tableName"> 要取得架构信息的表名称。</param>
        /// <returns>System.Data.DataTable 对象的数组，这些对象包含从数据源返回的架构信息。</returns>
        DataTable[] FillTableSchema(DataSet dataSet, SchemaType schemaType, string tableName);
        /// <summary>
        ///  获取批处理SQL语句语句块的开始字符，如 “Begin\r\n” 等。
        /// </summary>
        /// <returns></returns>
        string GetBatchExecuteSqlBeginString();
        /// <summary>
        ///  获取批处理SQL语句块的结束字符，如 “END;” 等。
        /// </summary>
        /// <returns></returns>
        string GetBatchExecuteSqlEndString();
        /// <summary>
        /// 获取批处理语句块中多个SQL语句之间的分隔符号，如“;\r\n”。
        /// </summary>
        /// <returns></returns>
        string GetBatchExecuteSqlSeparator();
        /// <summary>
        ///  创建一个 IDataParameter 实例。
        /// </summary>
        /// <returns>返回创建好的 IDataParameter 对象。</returns>
        IDataParameter GetDataParameterInstance();
        /// <summary>
        ///  通过指定参数名称，创建 IDataParameter 实例。
        /// </summary>
        /// <param name="paramName"> 参数名称。</param>
        /// <returns> 返回创建好的 IDataParameter 对象。</returns>
        IDataParameter GetDataParameterInstance(string paramName);
        /// <summary>
        ///  根据指定的参数名称，参数类型字符串创建 IDataParameter 对象。
        /// </summary>
        /// <param name="paramName"> 参数名称。</param>
        /// <param name="dbTypeString"> 参数类型字符串。 该字符串为符合指定数据库类型的 DbType 的枚举名称，依赖于当前 IDataAccess 的对象实例。 例如： 如果当前对象是 OracleDataAccess
        ///类型实例，则应该为 OracleType 枚举类型下的值表示的字符串，如：Clob 如果是 SqlDataAccess 类型实例，则应该为 SqlDbType
        ///枚举类型下的值表示的字符串，如：Bit</param>
        /// <returns>返回创建好的 IDataParameter 对象。</returns>
        IDataParameter GetDataParameterInstance(string paramName, string dbTypeString);
        /// <summary>
        ///  通过指定参数名称，参数值，参数传递方向创建 IDataParameter 实例。
        /// </summary>
        /// <param name="paramName">参数名称。</param>
        /// <param name="paramValue">参数值</param>
        /// <param name="direction">参数传递方向</param>
        /// <returns>返回创建好的 IDataParameter 对象</returns>
        IDataParameter GetDataParameterInstance(string paramName, object paramValue, ParameterDirection direction);
        /// <summary>
        /// 创建指定名称的参数，并且指定大小。 如果底层的数据库类型不支持将忽略 size 。
        /// </summary>
        /// <param name="paramName">参数名称</param>
        /// <param name="dbTypeString">对应的数据库类型的参数类型名称(不区分大小写)，
        /// 根据不同的数据库类型对应的 DBType 枚举值名称（如：OleDbType、SqlDbType 的枚举名称）。如：Varchar，如果为空则使用默认值。
        /// </param>
        /// <param name="size">指定参数大小，如果底层的数据库类型不支持将忽略 size </param>
        /// <returns></returns>
        IDataParameter GetDataParameterInstance(string paramName, string dbTypeString, int size);
       
        /// <summary>
        ///  获取符合 LIKE 语法的 SQL 语句常量值字符串，如果是字符类型则已经包含了前置单引号和后置单引号。
        /// </summary>
        /// <param name="value">表示 SQL 语句常量值字符串的真实 .net 类型的数据。</param>
        /// <param name="isLeftLike"> 是否需要加上左模糊。</param>
        /// <param name="isRightLike">是否需要加上右模糊。</param>
        /// <param name="dateFormat"> 如果是表示日期或时间类型的值时，指示格式字符串。</param>
        /// <returns> 返回符合 LIKE 语法的 SQL 语句常量值字符串，如果是字符类型则已经包含了前置单引号和后置单引号。 例如：'%abc%'</returns>
        string GetLikeSqlValueString(object value, bool isLeftLike=true , bool isRightLike=false , string dateFormat=null );
        /// <summary>
        ///  返回此当前连接的数据源的架构信息。
        /// </summary>
        /// <returns> 返回包含架构信息的 System.Data.DataTable。</returns>
        DataTable GetSchema();
        /// <summary>
        /// 使用指定的架构名称字符串返回当前连接的数据源的架构信息
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称</param>
        /// <returns> 包含架构信息的 System.Data.DataTable</returns>
        DataTable GetSchema(string collectionName);
        /// <summary>
        ///  使用指定的架构名称字符串和指定的限制值字符串数组，返回当前连接的数据源的架构信息。
        /// </summary>
        /// <param name="collectionName">指定要返回的架构的名称。</param>
        /// <param name="restrictionValues">为请求的架构指定一组限制值</param>
        /// <returns>包含架构信息的 System.Data.DataTable。</returns>
        DataTable GetSchema(string collectionName, string[] restrictionValues);
        
        /// <summary>
        ///  获取符合 SQL 语句常量值字符串，如果是字符类型则已经包含了前置单引号和后置单引号。
        /// </summary>
        /// <param name="value"> 表示 SQL 语句常量值字符串的真实 .net 类型的数据。</param>
        /// <param name="dateFormat"> 如果是表示日期或时间类型的值时，指示格式字符串。</param>
        /// <returns>返回能用于直接拼接 SQL 语句的常量值字符串，如果是字符类型则已经包含了前置单引号和后置单引号。 例如：'abc' 或 234</returns>
        string GetSqlValueString(object value, string dateFormat=null );
        /// <summary>
        ///  将符合类型的对象表示的数据插入到数据库中。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList,IDataReader
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。 如果提供的数据源是 IDataReader
        ///对象,则请确保打开 System.Data.IDataReader 的数据库连接对象不能是当前连接对象。
        /// </summary>
        /// <param name="dataModelObj">需要插入数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <returns>返回总共插入到数据库中的数据行数</returns>
        int Insert(object dataModelObj, string tableName);
        /// <summary>
        ///  将符合类型的对象表示的数据插入到数据库中。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList,IDataReader
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。 如果提供的数据源是 IDataReader
        ///对象,则请确保打开 System.Data.IDataReader 的数据库连接对象不能是当前连接对象。
        /// </summary>
        /// <param name="dataModelObj">需要插入数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="fields">字段选项</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int Insert(object dataModelObj, string tableName, UpdateFields fields);
        /// <summary>
        ///  将 System.Data.IDataReader 对象插入数据库指定表中。 请确保打开 System.Data.IDataReader 的数据库连接对象不是当前连接对象。
        /// </summary>
        /// <param name="reader">需要插入数据库的数据源</param>
        /// <param name="tableName">插入数据库指定表名称</param>
        /// <returns>返回总共插入数据库中的数据行数</returns>
        int InsertDataReader(IDataReader reader, string tableName);
        /// <summary>
        /// 将 System.Data.IDataReader 对象插入数据库指定表中。 请确保打开 System.Data.IDataReader 的数据库连接对象不能是当前连接对象。
        /// </summary>
        /// <param name="reader">需要插入数据库的数据源</param>
        /// <param name="tableName">插入数据库指定表名称</param>
        /// <param name="fields">指字字段选项，可以为 null。</param>
        /// <returns>返回总共插入数据库中的数据行数</returns>
        int InsertDataReader(IDataReader reader, string tableName, UpdateFields fields);
        /// <summary>
        ///  将指定的 System.Data.DataRow 对象表示的数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataRow 对象所属
        ///System.Data.DataTable 对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="insertRow"> 需要插入到数据库的 System.Data.DataRow 对象。</param>
        /// <returns>返回总共插入到数据库中的行数。</returns>
        int InsertDataRow(DataRow insertRow);
        /// <summary>
        /// 将指定的 System.Data.DataRow 对象表示的数据插入到数据库指定的表中。
        /// </summary>
        /// <param name="insertRow">需要插入到数据库的 System.Data.DataRow 对象。</param>
        /// <param name="tableName">指定需要插入数据的目标数据表名称</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataRow(DataRow insertRow, string tableName);
        /// <summary>
        /// 将指定的 System.Data.DataRow 对象表示的数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataRow 对象所属
        ///System.Data.DataTable 对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="insertRow">需要插入到数据库的 System.Data.DataRow 对象。</param>
        /// <param name="fields">指定需要保存到数据库中的字段或者不要保存到数据库的字段</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataRow(DataRow insertRow, UpdateFields fields);
        /// <summary>
        ///  将指定的 System.Data.DataRow 对象表示的数据插入到数据库指定的表中。
        /// </summary>
        /// <param name="insertRow">需要插入到数据库的 System.Data.DataRow 对象。</param>
        /// <param name="tableName">指定需要插入数据的目标数据表名称。</param>
        /// <param name="fields">指定需要保存到数据库中的字段或者不要保存到数据库的字段。</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataRow(DataRow insertRow, string tableName, UpdateFields fields);
        /// <summary>
        /// 将指定的 System.Data.DataSet 对象中的所有数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataSet 对象下相应的
        ///System.Data.DataTable 对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="dataSet"> 需要插入到数据库的 System.Data.DataSet 对象。</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataSet(DataSet dataSet);
        /// <summary>
        ///  将指定的 System.Data.DataSet 对象中的所有数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataSet 对象下相应的
        ///System.Data.DataTable 对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="dataSet">需要插入到数据库的 System.Data.DataSet 对象。</param>
        /// <param name="fields">指定需要保存到数据库中的字段或者不要保存到数据库的字段</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataSet(DataSet dataSet, UpdateFields fields);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象中的所有数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataTable
        ///对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="dataTable"> 需要插入数据库的 System.Data.DataTable 对象。</param>
        /// <returns></returns>
        int InsertDataTable(DataTable dataTable);
        /// <summary>
        ///  将指定的 System.Data.DataTable 对象中的所有数据插入到数据库中。 插入数据的表对象为提供的 System.Data.DataTable
        ///对象的表名称对应的数据库表。
        /// </summary>
        /// <param name="dataTable"> 需要插入数据库的 System.Data.DataTable 对象。</param>
        /// <param name="fields">指定需要保存到数据库中的字段或者不要保存到数据库的字段</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataTable(DataTable dataTable, UpdateFields fields);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象中的所有数据插入到数据库指定的表中。
        /// </summary>
        /// <param name="dataTable">需要插入到数据库的 System.Data.DataTable 对象。</param>
        /// <param name="tableName">指定需要插入数据的目标数据表名称</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataTable(DataTable dataTable, string tableName);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象中的所有数据插入到数据库指定的表中。
        /// </summary>
        /// <param name="dataTable">需要插入到数据库的 System.Data.DataTable 对象。</param>
        /// <param name="tableName">指定需要插入数据的目标数据表名称</param>
        /// <param name="fields">指定需要保存到数据库中的字段或者不要保存到数据库的字段</param>
        /// <returns>返回总共插入到数据库中的行数</returns>
        int InsertDataTable(DataTable dataTable, string tableName, UpdateFields fields);
        /// <summary>
        ///  根据提供的参数 dataTable ，从参数中提取查询的表名及字段名，并从数据库中检索数据填充到该 dataTable 。
        /// </summary>
        /// <param name="dataTable"> 要填充数据的 DataTable 对象。</param>
        void Load(DataTable dataTable);
        /// <summary>
        /// 根据提供的表示一行数据的类型对象提取查询的字段名称，从数据库中检索数据填充到 ArrayList 集合中。
        /// </summary>
        /// <param name="t">表示一行数据的对象类型</param>
        /// <param name="tableName">需要查询数据的表名称</param>
        /// <returns>返回表示查询结果的 ArrayList 集合，其中每一个元素为提供的参数 t 所创建的对象。</returns>
        ArrayList Load(Type t, string tableName);
        /// <summary>
        /// 根据提供的参数 dataTable ，从参数中提取查询的表名及字段名，并从数据库中检索数据填充到该 dataTable 。
        /// </summary>
        /// <param name="dataTable">要填充数据的 DataTable 对象。</param>
        /// <param name="conditionSql"> 检索数据时的限制条件 SQL 子句(不带 WHERE 关键字)。</param>
        void Load(DataTable dataTable, string conditionSql);
        /// <summary>
        ///  根据提供的表示一行数据的类型对象提取查询的字段名称，从数据库中检索数据填充到 ArrayList 集合中。
        /// </summary>
        /// <param name="t">表示一行数据的对象类型</param>
        /// <param name="tableName">需要查询数据的表名称</param>
        /// <param name="conditionSql">检索数据时的限制条件 SQL 子句(不带 WHERE 关键字)，如果没有可以为空</param>
        /// <returns>返回表示查询结果的 ArrayList 集合，其中每一个元素为提供的参数 t 所创建的对象。</returns>
        ArrayList Load(Type t, string tableName, string conditionSql);
        /// <summary>
        ///  根据提供的参数 dataTable ，从参数中提取查询的表名及字段名，并从数据库中检索数据填充到该 dataTable 。
        /// </summary>
        /// <param name="dataTable"> 要填充数据的 DataTable 对象。</param>
        /// <param name="conditionSql">检索数据时的限制条件 SQL 子句(不带 WHERE 关键字)，如果没有可以为空。</param>
        /// <param name="sort"> 查询时排序的规则(不带 Order By 关键字)，如果没有可以为空。</param>
        void Load(DataTable dataTable, string conditionSql, string sort);
        /// <summary>
        /// 根据提供的表示一行数据的类型对象提取查询的字段名称，从数据库中检索数据填充到 ArrayList 集合中。
        /// </summary>
        /// <param name="t">表示一行数据的对象类型</param>
        /// <param name="tableName">需要查询数据的表名称</param>
        /// <param name="conditionSql">检索数据时的限制条件 SQL 子句(不带 WHERE 关键字)，如果没有可以为空。</param>
        /// <param name="sort">查询时排序的规则(不带 Order By 关键字)，如果没有可以为空。</param>
        /// <returns>返回表示查询结果的 ArrayList 集合，其中每一个元素为提供的参数 t 所创建的对象。</returns>
        ArrayList Load(Type t, string tableName, string conditionSql, string sort);
        /// <summary>
        ///  根据提供的表示一行数据的类型提取查询的字段名称，从数据库中检索数据填充到 List 集合中。
        /// </summary>
        /// <typeparam name="T">行数据所属类型</typeparam>
        /// <param name="tableName"> 需要查询数据的表名称。</param>
        /// <param name="conditionSql"> 检索数据时的限制条件 SQL 子句(不带 WHERE 关键字)，如果没有可以为空。</param>
        /// <param name="sort"> 查询时排序的规则(不带 Order By 关键字)，如果没有可以为空。</param>
        /// <returns></returns>
        List<T> Load<T>(string tableName, string conditionSql, string sort) where T : class, new();
        /// <summary>
        ///  执行指定的SQL语句，返回第一行数据。
        /// </summary>
        /// <typeparam name="T">行数据所属类型</typeparam>
        /// <param name="selectSql">需要执行的SQL语句</param>
        /// <param name="parameterNameAndValues"> 参数名称和值数组</param>
        /// <returns>返回执行查询的第一行数据</returns>
        T LoadBySql<T>(string selectSql, params KeyValuePair<string ,object>[] parameterNameAndValues) where T : class, new();
        /// <summary>
        /// 检索指定的数据返回数据模型对象。 适用于指定主键条件检索单条数据。
        /// </summary>
        /// <typeparam name="T">表示一行数据的对象类型</typeparam>
        /// <param name="tableName">检索数据的表名称</param>
        /// <param name="fieldsAndValues">指示检索数据的字段名称和值</param>
        /// <returns> 返回数据模型对象。 如果检索的结果有多条记录，则返回第一条记录组装的数据对象。 如果没有检索到结果则返回 null 。</returns>
        T LoadDataModel<T>(string tableName, params KeyValuePair<string ,object>[] fieldsAndValues) where T : class, new();
        /// <summary>
        ///  检索指定的数据并填充到 List 集合中。 适用于指定字段条件检索多条数据。
        /// </summary>
        /// <typeparam name="T">表示一行数据的对象类型。</typeparam>
        /// <param name="tableName">检索数据的表名称</param>
        /// <param name="fieldsAndValues">指示检索数据的字段名称和值</param>
        /// <returns>返回数据模型 IList 对象。 如果没有检索到结果则返回零长度的 List对象。</returns>
        List<T> LoadDataModelList<T>(string tableName, params KeyValuePair<string ,object >[] fieldsAndValues) where T : class, new();
        /// <summary>
        /// 打开数据库连接
        /// </summary>
        void Open();
        /// <summary>
        /// 回滚当前的数据库事务
        /// </summary>
        void RollbackTransaction();
        /// <summary>
        /// 将符合类型要求的对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj">需要更新数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="primaryKeyFields">指定作为更新数据的主键信息，如果不指定，则使用 DataTable 中定义的主键或 DataModel 中定义的 ModelUpdatableAttribute
        ///指定的主键。</param>
        /// <returns>返回总共影响的行数</returns>
        int Update(object dataModelObj, string tableName, params string[] primaryKeyFields);
        /// <summary>
        /// 将符合类型要求的对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj">需要更新数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="primaryKeyFields">指定作为更新数据的主键信息，如果不指定，则使用 DataTable 中定义的主键或 DataModel 中定义的 ModelUpdatableAttribute
        ///指定的主键。</param>
        ///<param name="fields"> 字段选项。指定需要更新数据库中的字段或者不要更新数据库的字段。</param>
        /// <returns>返回总共影响的行数</returns>
        int Update(object dataModelObj, string tableName, UpdateFields fields, params string[] primaryKeyFields);
        /// <summary>
        ///  将符合类型要求的对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj">需要更新数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="conditionSql"> 更新数据时的条件 SQL 语句（不带 WHERE 关键字）</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateByCondition(object dataModelObj, string tableName, string conditionSql);
        /// <summary>
        ///  将符合类型要求的对象的数据更新到数据库。 支持类型有：DataTable,DataRow,DataSet,DataView,DataRowView,IList
        ///以及其他的包含基本数据类型（如 int,string, ）作为公共属性的普通类型对象 。
        /// </summary>
        /// <param name="dataModelObj">需要更新数据的存储对象</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="fields">指定需要更新的字段或排除更新的字段</param>
        /// <param name="conditionSql"> 更新数据时的条件 SQL 语句（不带 WHERE 关键字）</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateByCondition(object dataModelObj, string tableName, UpdateFields fields, string conditionSql);
        /// <summary>
        /// 将指定的 System.Data.DataRow 对象更新到数据库的指定表中。
        /// </summary>
        /// <param name="updateRow"> 将该 System.Data.DataRow 对象表示的数据更新到数据库中。</param>
        /// <param name="tableName">要更新数据库中的表名称</param>
        /// <param name="primaryKeyFields"> 指定更新的主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为更新条件的值。 如果不指定这个参数， 则更新的主键依据是
        ///System.Data.DataTable 对象中对应的主键。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataRow(DataRow updateRow, string tableName, params string[] primaryKeyFields);
        /// <summary>
        /// 将指定的 System.Data.DataRow 对象更新到数据库的指定表中。
        /// </summary>
        /// <param name="updateRow"> 将该 System.Data.DataRow 对象表示的数据更新到数据库中。</param>
        /// <param name="tableName">要更新数据库中的表名称</param>
        /// <param name="fields"> 字段选项。指定需要更新数据库中的字段或者不要更新数据库的字段。</param>
        /// <param name="primaryKeyFields"> 指定更新的主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为更新条件的值。 如果不指定这个参数， 则更新的主键依据是
        ///System.Data.DataTable 对象中对应的主键。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataRow(DataRow updateRow, string tableName, UpdateFields fields, params string[] primaryKeyFields);
        /// <summary>
        ///  将指定的 System.Data.DataRow 对象更新到数据库的指定表中。
        /// </summary>
        /// <param name="updateRow"> 将该 System.Data.DataRow 对象表示的数据更新到数据库中。</param>
        /// <param name="tableName"> 要更新数据库中的表名称。</param>
        /// <param name="conditionSql"> 指定更新的依据，不带 WHERE 关键字的条件 SQL 语句。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataRowByCondition(DataRow updateRow, string tableName, string conditionSql);
        /// <summary>
        /// 使用不带 WHERE 关键字的条件 SQL 语句作为条件，将 System.Data.DataRow 对象表示的数据更新到数据库中。
        /// </summary>
        /// <param name="updateRow"> 需要更新数据的 System.Data.DataRow 对象。</param>
        /// <param name="tableName">数据库中对应的表名称</param>
        /// <param name="fields">字段选项。指定需要更新数据库中的字段或者不要更新数据库的字段。</param>
        /// <param name="conditionSql">指定更新的依据，不带 WHERE 关键字的条件 SQL 语句。</param>
        /// <returns>返回总共更新到数据库的总行数</returns>
        int UpdateDataRowByCondition(DataRow updateRow, string tableName, UpdateFields fields, string conditionSql);
        /// <summary>
        ///  将 System.Data.DataSet 对象更新到数据库中，更新的数据库中的表为 System.Data.DataSet 对象中对应的 System.Data.DataTable
        ///名称。
        /// </summary>
        /// <param name="dataSet">将该 System.Data.DataSet 对象表示的数据更新到数据库中。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataSet(DataSet dataSet);
        /// <summary>
        ///  将指定的 System.Data.DataSet 对象更新到数据库中。 更新的数据库目标表是 System.Data.DataSet 对象中的 System.Data.DataTable
        ///对象的表名称表示的数据库表。 更新的主键依据是 System.Data.DataTable 对象中对应的主键。
        /// </summary>
        /// <param name="dataSet">需要更新数据的 System.Data.DataSet 对象。</param>
        /// <param name="fields">指定需要更新的字段或排除更新的字段。</param>
        /// <returns>返回总共更新到数据库的总行数</returns>
        int UpdateDataSet(DataSet dataSet, UpdateFields fields);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象更新到数据库指定的表中。
        /// </summary>
        /// <param name="dataTable">将该 System.Data.DataTable 对象表示的数据更新到数据为中。</param>
        /// <param name="tableName"> 要更新数据库的表名称。</param>
        /// <param name="primaryKeyFields">指定更新的主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为更新条件的值。 如果不指定这个参数， 则更新的主键依据是
        ///System.Data.DataTable 对象中对应的主键。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataTable(DataTable dataTable, string tableName, params string[] primaryKeyFields);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象更新到数据库指定的表中。
        /// </summary>
        /// <param name="dataTable">将该 System.Data.DataTable 对象表示的数据更新到数据为中。</param>
        /// <param name="tableName"> 要更新数据库的表名称。</param>
        /// <param name="primaryKeyFields">指定更新的主键字段。 如果 System.Data.DataRow 对象存在原始数据版本，则以原始数据版本作为更新条件的值。 如果不指定这个参数， 则更新的主键依据是
        ///System.Data.DataTable 对象中对应的主键。</param>
        ///<param name="fields">指定需要更新的字段或排除更新的字段</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataTable(DataTable dataTable, string tableName, UpdateFields fields, params string[] primaryKeyFields);
        /// <summary>
        /// 将指定的 System.Data.DataTable 对象更新到数据库指定的表中。
        /// </summary>
        /// <param name="dataTable">将该 System.Data.DataTable 对象表示的数据更新到数据为中。</param>
        /// <param name="tableName">要更新数据库的表名称</param>
        /// <param name="conditionSql">指定更新的依据，不带 WHERE 关键字的条件 SQL 语句。</param>
        /// <returns>返回总共影响的行数</returns>
        int UpdateDataTableByCondition(DataTable dataTable, string tableName, string conditionSql);
        /// <summary>
        ///  使用不带 WHERE 关键字的条件 SQL 语句作为条件，将 System.Data.DataTable 对象表示的数据更新到数据库中。
        /// </summary>
        /// <param name="dataTable">需要更新数据的 System.Data.DataTable 对象。</param>
        /// <param name="tableName"> 指定需要更新数据的目标数据表名称。</param>
        /// <param name="fields"> 指定需要更新的字段或排除更新的字段。</param>
        /// <param name="conditionSql">指定更新的依据，不带 WHERE 关键字的条件 SQL 语句。</param>
        /// <returns> 返回总共更新到数据库的总行数。</returns>
        int UpdateDataTableByCondition(DataTable dataTable, string tableName, UpdateFields fields, string conditionSql);
    }
}
