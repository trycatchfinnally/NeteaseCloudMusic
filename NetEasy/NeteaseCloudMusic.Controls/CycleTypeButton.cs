using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace NeteaseCloudMusic.Controls
{
    [TemplatePart(Name = "PART_Root", Type = typeof(Grid))]
    public class CycleTypeButton : ItemsControl
    {

        static CycleTypeButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(CycleTypeButton),
                new FrameworkPropertyMetadata(typeof(CycleTypeButton)));
        }




        public int SelectIndex
        {
            get { return (int)GetValue(SelectIndexProperty); }
            set { SetValue(SelectIndexProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SelectIndex.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SelectIndexProperty =
            DependencyProperty.Register("SelectIndex", typeof(int), typeof(CycleTypeButton),
                new PropertyMetadata(0, new PropertyChangedCallback(OnSelectIndexChanged)));

        private static void OnSelectIndexChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            var sender = (CycleTypeButton)d;
            var rootGrid = sender.GetTemplateChild("PART_Root") as Grid;
            if (sender.Items.Count == 0 && rootGrid != null && rootGrid.Children.Count == 0) return;
            if (rootGrid != null)
            {
                var uiElements = rootGrid.Children.Cast<UIElement>().ToArray();
                var newIndex = Convert.ToInt32(e.NewValue);
                var oldIndex = Convert.ToInt32(e.OldValue);
                //if (oldIndex == newIndex) return;
                uiElements[oldIndex].Visibility = Visibility.Hidden;
                newIndex = newIndex >= uiElements.Length ? 0 : newIndex;
                uiElements[newIndex].Visibility = Visibility.Visible;
                sender.SelectIndex = newIndex;
                 
            }

            
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var rootGrid = GetTemplateChild("PART_Root") as Grid;
            if (rootGrid != null && rootGrid.Children.Count > 0)
            {

                var uiElements = rootGrid.Children.Cast<UIElement>().ToArray();
                foreach (var item in uiElements)
                {
                    item.Visibility = Visibility.Hidden;
                }

                var visibilityIndex = SelectIndex >= uiElements.Length ? uiElements.Length-1 : SelectIndex;
                uiElements[visibilityIndex].Visibility = Visibility.Visible;
                rootGrid.MouseLeftButtonDown += (sender1, e1) =>
                {
                    //if (e.ButtonState == System.Windows.Input.MouseButtonState.Released)
                    {
                        /*uiElements[visibilityIndex].Visibility = Visibility.Hidden;
                        visibilityIndex++;
                        if (visibilityIndex == uiElements.Length)
                            visibilityIndex = 0;
                        uiElements[visibilityIndex].Visibility = Visibility.Visible;*/
                        if (SelectIndex < uiElements.Length - 1)
                            SelectIndex++;
                        else SelectIndex = 0;

                    }
                };
            }
        }
    }
}
