 
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Core.Data
{
   public  class DLBase:IDisposable
    {
        
        private readonly  IDataAccess _dataAccessClient;
       
       
        private bool isDisposed;
        
        public IDataAccess DataAccessClient
        {
            get
            {
                return _dataAccessClient;
            }
        }

        public bool Disposed
        {
            get
            {
                return isDisposed;
            }

            
        }

        public  DLBase()
        {
            
            if (Transaction.TransactionList.Count > 0)
            {
                if (Transaction.Current == null)
                {
                    throw new NullReferenceException("无法在未创建过 Transaction 对象或当前的 Transaction 对象已经 Dispose 的情况下实例化 DL 对象。");
                }
                if (Transaction.Current.Disposed || Transaction.Current.IsComplete)
                {
                    throw new NullReferenceException("不能在当前 Transaction 对象已经 Dispose 或 Complete 之后创建 DL 对象。");
                }
                if (Transaction.DataAccessInstance != null)
                {
                    this._dataAccessClient = Transaction.DataAccessInstance;
                }
                else
                {
                    this._dataAccessClient =  CommonServiceLocator.ServiceLocator.Current.GetInstance<IDataAccess>();
                    Transaction.DataAccessInstance = this._dataAccessClient;
                }
                if (Transaction.Current.IsTransacting && !this.DataAccessClient.IsTransaction)
                {
                    this.DataAccessClient.BeginTransaction();
                }
            }
            else
            {
                throw new InvalidOperationException("未建立事务环境。");


            }
        }
    
        private void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new InvalidOperationException("IDataAccess已经被释放掉，无法使用释放掉的IDataAccess.");
            }
        }
        

        private void CheckTransaction()
        {
            if (Transaction.TransactionList.Count > 0)
            {
                throw new InvalidOperationException("环境事务管理模式下不允许调用本方法。");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                try
                {
                    
                        if ((this._dataAccessClient != null) && !this._dataAccessClient.Disposed)
                        {
                            if (this._dataAccessClient.IsTransaction)
                            {
                                try
                                {
                                    this._dataAccessClient.RollbackTransaction();
                                }
                                catch
                                {
                                }
                            }
                            try
                            {
                                this._dataAccessClient.Close();
                            }
                            catch
                            {
                            }
                            try
                            {
                                this._dataAccessClient.Dispose();
                            }
                            catch
                            {
                            }
                        }
                         
                }
                catch
                {
                }
                this.isDisposed = true;
            }
        }


        public void BeginTrans()
        {
            this.CheckTransaction();
            this.DataAccessClient.BeginTransaction();
        }
        public void CommitTrans()
        {
            this.CheckTransaction();
            this.DataAccessClient.CommitTransaction();
        }
       
         
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void RollbackTrans()
        {
            this.CheckTransaction();
            this.DataAccessClient.RollbackTransaction();
        }








    }

}
