using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.ViewModel.IndirectView
{
    /// <summary>
    /// 代表包含id导航参数的类型
    /// </summary>
    public abstract class IndirectViewModelBase : NavigationViewModelBase
    {
        /// <summary>
        /// 导航过程中的id名
        /// </summary>
        public  const string NavigationIdParmmeterName = "Id";
        /// <summary>
        /// 表示每个页面对应的id，如果一致，则直接用缓存的项目以避免刷新
        /// </summary>
       protected  long? Id { get; set; } 
         
        public  override bool IsNavigationTarget(NavigationContext navigationContext)
        {
            if (!navigationContext.Parameters.ContainsKey(NavigationIdParmmeterName))
                return false ;
            return true;
            //var id =long.Parse( navigationContext.Parameters[NavigationIdParmmeterName].ToString());
            //if (Id.GetValueOrDefault() == 0) return true;
            //return id == Id;
        }
        public  override void OnNavigatedTo(NavigationContext navigationContext)
        {
            var id = Convert.ToInt64(navigationContext.Parameters[NavigationIdParmmeterName]);
            if (id == Id) return;
            Id = id;
            SetById(id);
            
        }
        protected abstract  void SetById(long id);
         
        protected void RaiseAllPropertyChanged()
        {
            Array.ForEach(this.GetType().GetProperties(), x => RaisePropertyChanged(x.Name));
        }
    }
}
