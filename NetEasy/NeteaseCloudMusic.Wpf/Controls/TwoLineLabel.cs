using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NeteaseCloudMusic.Wpf.Controls
{
   public  class TwoLineLabel:ContentControl
    {

        static TwoLineLabel()
        {
            //DefaultStyleKeyProperty.OverrideMetadata(typeof(TwoLineLabel),
            //    new FrameworkPropertyMetadata(typeof(TwoLineLabel)));
        }
        public string  FirstLineText    
        {
            get { return (string)GetValue(FirstLineTextProperty); }
            set { SetValue(FirstLineTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FirstLineTextProperty =
            DependencyProperty.Register("FirstLineText", typeof(string ), typeof(TwoLineLabel), new PropertyMetadata(null));

        public string SecondLineText
        {
            get { return (string)GetValue(SecondLineTextProperty); }
            set { SetValue(SecondLineTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SecondLineTextProperty =
            DependencyProperty.Register("SecondLineText", typeof(string), typeof(TwoLineLabel), new PropertyMetadata(null));


        public ICommand ButtonCommand   
        {
            get { return (ICommand)GetValue(ButtonCommandProperty); }
            set { SetValue(ButtonCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ButtonCommandProperty =
            DependencyProperty.Register("ButtonCommand", typeof(ICommand), typeof(TwoLineLabel), new PropertyMetadata(null));


    }
}
