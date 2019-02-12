using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NeteaseCloudMusic.Wpf.ViewModel;

namespace NeteaseCloudMusic.Wpf.View
{
    /// <summary>
    /// FindMusicView.xaml 的交互逻辑
    /// </summary>
    public partial class FindMusicView  
    {
        public FindMusicView(FindMusicViewModel viewModel)
        {
            InitializeComponent();
            this.DataContext = viewModel;
        }
    }
}
