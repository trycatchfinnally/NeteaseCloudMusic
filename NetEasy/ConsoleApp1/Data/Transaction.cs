using CommonServiceLocator;
 
using Prism.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
    /// <summary>
    /// Transaction 类，提供DAO连接和事务管理。 
    /// 请使用 using() 语句块处理。 如果所有工作都已全部成功处理完毕，请必须调用一次（并且仅仅调用一次） Complete() 方法。
    /// </summary>
    public class Transaction:IDisposable
    {
        [ThreadStatic]
        private static Transaction _current;
        [ThreadStatic]
        private static IDataAccess _dataAccessInstance;
        [ThreadStatic]
        private static Type _dataAccessType;
        [ThreadStatic]
        public static List<Transaction> _TransactionList;
        private bool _isComplete;
        private bool _isDisposed;
        [ThreadStatic]
        private static bool _isTransacting;
        [ThreadStatic]
        private static Transaction _transactionOwner;
        [ThreadStatic]
        private static bool _isUnComplete;
        public static Transaction Current
        {
            get
            {
                return _current;
            }

           
        }
        internal   static IDataAccess DataAccessInstance
        {
            get
            {
                return _dataAccessInstance;
            }
            set
            {
                _dataAccessInstance = value;
                _dataAccessType = value.GetType();
            }
        }
        static Transaction()
        {
            DataAccessInstance = ServiceLocator.Current.GetInstance<IDataAccess>();
        }
        public bool  Disposed
        {
            get
            {
                return _isDisposed;
            }

            
        }
        /// <summary>
        /// 获取当前 Transaction 对象是否已经执行了 Complete 方法。 
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return _isComplete;
            }

            
        }
        /// <summary>
        /// 或取当前 Transaction 对象是否已经执行了 Complete 方法。 
        /// </summary>
        public     bool IsTransacting
        {
            get
            {
                return _isTransacting;
            }

           
        }

        internal static List<Transaction> TransactionList
        {
            get
            {
                if (_TransactionList == null)
                {
                    _TransactionList = new List<Transaction>();
                }
                return _TransactionList;
            }
        }




        private static void DisposeDataAccess()
        {
            if ((_dataAccessInstance != null) && !_dataAccessInstance.Disposed)
            {
                if ((_dataAccessInstance.State != ConnectionState.Closed) && _dataAccessInstance.IsTransaction)
                {
                    try
                    {
                        _dataAccessInstance.RollbackTransaction();
                    }
                    catch (Exception e )
                    {
                        
                        //DevDebug.WriteTraceInfo("Transaction -> 忽略了的 Dispose() 方法异常，发生在 _dataAccessInstance.RollbackTransaction()：" + exception.Message);
                    }
                }
                try
                {
                    _dataAccessInstance.Dispose();
                }
                catch
                {
                }
                _dataAccessInstance = null;
            }
            //if ((_dataAccessTemplateInstance != null) && !_dataAccessTemplateInstance.Disposed)
            //{
            //    if ((_dataAccessTemplateInstance.State != ConnectionState.Closed) && _dataAccessTemplateInstance.IsTransaction)
            //    {
            //        try
            //        {
            //            _dataAccessTemplateInstance.RollbackTransaction();
            //        }
            //        catch (Exception exception2)
            //        {
            //           // DevDebug.WriteTraceInfo("Transaction -> 忽略了的 Dispose() 方法异常，发生在 _dataAccessTemplateInstance.RollbackTransaction()：" + exception2.Message);
            //        }
            //    }
            //    try
            //    {
            //        _dataAccessTemplateInstance.Dispose();
            //    }
            //    catch
            //    {
            //    }
            //    _dataAccessTemplateInstance = null;
            //}
        }
        /// <summary>
        /// 将当前线程数据进行清理。 
        /// </summary>
        public static void DisposeThreadData()
        {
            if (_TransactionList != null)
            {
                _TransactionList.Clear();
            }
            _transactionOwner = null;
            DisposeDataAccess();
        }



        private void RollbackDataAccess()
        {
            if (((_dataAccessType != null) && (_dataAccessInstance != null)) && _dataAccessInstance.IsTransaction)
            {
                try
                {
                    _dataAccessInstance.RollbackTransaction();
                }
                catch (Exception e)
                {
                    ServiceLocator.Current.GetInstance< ILoggerFacade>().Exception ("忽略了的 Dispose 方法异常，发生在 _dataAccessInstance.RollbackTransaction()：" + e.Message, e);
                   // DevDebug.WriteTraceInfo(this, ");
                }
            }
            //if (((_dataAccessTemplateType != null) && (_dataAccessTemplateInstance != null)) && _dataAccessTemplateInstance.IsTransaction)
            //{
            //    try
            //    {
            //        _dataAccessTemplateInstance.RollbackTransaction();
            //    }
            //    catch (Exception exception2)
            //    {
            //        DevDebug.WriteTraceInfo(this, "忽略了的 Dispose() 方法异常，发生在 _dataAccessTemplateInstance.RollbackTransaction()：" + exception2.Message);
            //    }
            //}
        }





        /// <summary>
        /// 创建 Transaction 对象实例并且根据参数决定是否启用事务。 
        /// 如果当前代码所在外层 Transaction 对象实例已经启用事务， 则会忽略当前传递的参数而自动会启用当前对象实例的事务。 
        /// 事务提交发生在第一个启用事务的 Transaction 对象的 Complete 方法时。 事务回滚发生在第一个启用事务的 Transaction 对象的 Dispose 方法。 
        /// </summary>
        /// <param name="isRequiredTransaction">是否启用事务</param>
        public Transaction(bool isRequiredTransaction=false )
        {
            if ((_TransactionList == null) || (_TransactionList.Count == 0))
            {
                if (_TransactionList == null)
                {
                    _TransactionList = new List<Transaction>();
                }
               _isTransacting = isRequiredTransaction;
                _TransactionList.Add(this);
                _current = this;
                if (isRequiredTransaction)
                {
                    _transactionOwner = this;
                }
                else
                {
                    _transactionOwner = null;
                }
                _dataAccessInstance = null;
               // _dataAccessTemplateInstance = null;
            }
            else
            {
                if (Current == null)
                {
                    throw new NullReferenceException("当前的 Transaction 对象（Transaction.Current 属性值）已不存在。可能是 Transaction 嵌套调用不正确，请在当前代码后面调用上一层 Transaction 对象的 Complete() 或 Dispose() 方法。");
                }
                if (isRequiredTransaction && !IsTransacting)
                {
                    _isTransacting = isRequiredTransaction;
                    if (_dataAccessInstance != null)
                    {
                        _dataAccessInstance.BeginTransaction();
                    }
                   
                }
                _TransactionList.Add(this);
                _current = this;
                if ((_transactionOwner == null) & isRequiredTransaction)
                {
                    _transactionOwner = this;
                }
            }
        }
        /// <summary>
        /// 当前事务中所有工作已经处理完毕，通知 Transaction 已经成功完成操作。
        /// 如果当前对象处于事务环境中，并且是第一个启用事务的 Transaction 对象，则立即提交更改。 
        /// </summary>
        public void Complete()
        {
            if (this.Disposed)
            {
                throw new ObjectDisposedException("Transaction");
            }
            if (Current == null)
            {
                throw new NullReferenceException("当前的 Transaction 对象（Transaction.Current 属性值）已不存在。请不要重复调用 Complete() 方法。");
            }
            if (Current != this)
            {
                throw new NullReferenceException("Transaction 嵌套不正确。你只能调用当前层次的 Transaction 对象（Transaction.Current 属性值） Complete() 方法。");
            }
            if (_transactionOwner == this)
            {
                if ((_dataAccessInstance != null) && _dataAccessInstance.IsTransaction)
                {
                    _dataAccessInstance.CommitTransaction();
                }
                //if ((_dataAccessTemplateInstance != null) && _dataAccessTemplateInstance.IsTransaction)
                //{
                //    _dataAccessTemplateInstance.CommitTransaction();
                //}
                _transactionOwner = null;
                _isTransacting = false;
            }
           this._isComplete = true;
        }
        public void Dispose()
        {
            if (!this._isDisposed)
            {
                if (_TransactionList[_TransactionList.Count - 1] != this)
                {
                    try
                    {
                        DisposeDataAccess();
                    }
                    catch
                    {
                    }
                    throw new NullReferenceException("Transaction 嵌套使用不正确。你只能调用当前层次的 Transaction 对象 Dispose() 方法。");
                }
                if (!this._isComplete)
                {
                    _isUnComplete = true;
                }
                if (_isUnComplete && ((this == _transactionOwner) || (_transactionOwner == null)))
                {
                    this.RollbackDataAccess();
                    _isTransacting = false;
                    _isUnComplete = false;
                }
                if (TransactionList[0] == this)
                {
                    DisposeDataAccess();
                    _isTransacting = false;
                    _transactionOwner = null;
                }
                else if (this == _transactionOwner)
                {
                    _transactionOwner = null;
                }
                _TransactionList.Remove(this);
                if (_TransactionList.Count > 0)
                {
                    _current = _TransactionList[_TransactionList.Count - 1];
                }
                else
                {
                    _current = null;
                }
                this._isDisposed = true;
            }
        }



    }
}
