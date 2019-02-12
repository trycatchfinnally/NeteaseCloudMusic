using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.Model
{
    public abstract  class NotifyPropertyChangedModelBase<TInnerModel> : Prism.Mvvm.BindableBase where TInnerModel : new()
    {
        protected readonly TInnerModel innerModel;
        public NotifyPropertyChangedModelBase() : this(new TInnerModel()) { }
        public  NotifyPropertyChangedModelBase(TInnerModel innermodel) { this.innerModel = innermodel; }
    }
}
