using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Prism.Regions;

namespace NeteaseCloudMusic.Wpf.ViewModel
{
    /// <summary>
    /// 可以导航和属性通知的viewmodel继承该类
    /// </summary>
    public abstract class NavigationViewModelBase : BindableBase, Prism.Regions.INavigationAware
    {
        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual  void OnNavigatedFrom(NavigationContext navigationContext)
        {
             
        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            
        }
    }
}
