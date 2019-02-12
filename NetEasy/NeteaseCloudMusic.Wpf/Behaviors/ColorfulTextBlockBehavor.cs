using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interactivity;
using System.Windows.Media;

namespace NeteaseCloudMusic.Wpf.Behaviors
{
    /// <summary>
    /// 能够被搜索文本颜色
    /// </summary>
    public class ColorfulTextBlockBehavor : Behavior<TextBlock>
    {



        /// <summary>
        /// 未被搜索到的颜色
        /// </summary>
        public Color DefaultColor
        {
            get { return (Color)GetValue(DefaultColorProperty); }
            set { SetValue(DefaultColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DefaultColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DefaultColorProperty =
            DependencyProperty.RegisterAttached("DefaultColor", typeof(Color), typeof(ColorfulTextBlockBehavor), new PropertyMetadata(Colors.Black));


        /// <summary>
        /// 被搜索到后的颜色
        /// </summary>
        public Color HighlineColor
        {
            get { return (Color)GetValue(HighlineColorProperty); }
            set { SetValue(HighlineColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HighlineColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HighlineColorProperty =
            DependencyProperty.RegisterAttached("HighlineColor", typeof(Color), typeof(ColorfulTextBlockBehavor), new PropertyMetadata(Colors.Blue));


        public string InlineText
        {
            get { return (string)GetValue(InlineTextProperty); }
            set { SetValue(InlineTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for InlineText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty InlineTextProperty =
            DependencyProperty.RegisterAttached("InlineText", typeof(string), typeof(ColorfulTextBlockBehavor), new PropertyMetadata(String.Empty));



        public string SearchKeyWord
        {
            get { return (string)GetValue(SearchKeyWordProperty); }
            set { SetValue(SearchKeyWordProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchKeyWord.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchKeyWordProperty =
            DependencyProperty.RegisterAttached("SearchKeyWord", typeof(string), typeof(ColorfulTextBlockBehavor), new PropertyMetadata(string.Empty));


        protected override void OnAttached()
        {
            var txtBlock = this.AssociatedObject;
            var text = this.InlineText;
            txtBlock.Inlines.Clear();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }
            if (string.IsNullOrEmpty(SearchKeyWord))
            {
                txtBlock.Inlines.Add(new Run(text) { Foreground = new SolidColorBrush(DefaultColor) });
            }
            else
            {
                var temp = text.IndexOf(SearchKeyWord,StringComparison.OrdinalIgnoreCase);

                if (temp!=-1)
                {
                    var lpart =temp==0?"": text.Substring(0, temp);
                    var rpart =temp==text.Length-SearchKeyWord.Length?"": text.Substring(temp+SearchKeyWord.Length);
                    txtBlock.Inlines.Add(new Run(lpart) { Foreground = new SolidColorBrush(DefaultColor) });
                    txtBlock.Inlines.Add(new Run(text.Substring(temp, SearchKeyWord.Length)) { Foreground = new SolidColorBrush(HighlineColor) });
                    txtBlock.Inlines.Add(new Run(rpart) { Foreground = new SolidColorBrush(DefaultColor) });
                }
                else
                    txtBlock.Inlines.Add(new Run(text) { Foreground = new SolidColorBrush(DefaultColor) });

            }
        }

    }
}
