using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;

namespace NeteaseCloudMusic.Controls
{
    /// <summary>
    /// 为了在焦点离开窗体的时候popup不在最前并且随着窗体的大小位置改变而改变
    /// <see cref="http://www.cnblogs.com/Leaco/p/3164394.html"/>
    /// </summary>
    public class TopmostPopup: Popup
    {
        
        protected override void OnOpened(EventArgs e)
        {
            UpdateWindow();
        }
       

        private void UpdateWindow()
        {
            Window window = Window.GetWindow(this);
            if (window != null)
            {
                var hwnd = ((HwndSource)PresentationSource.FromVisual(this))?.Handle;
                if (!hwnd.HasValue) return;
                Rect rect;
                if (GetWindowRect(hwnd.Value, out rect))
                {
                    SetWindowPos(hwnd.Value, -2, rect.Left, rect.Top, (int)window.Width, (int)window.Height, 0);
                }
                //防止多次绑定
                window.LocationChanged -= WindowLocationChanged;
                window.LocationChanged += WindowLocationChanged;
                window.SizeChanged -= WindowLocationChanged;
                window.SizeChanged += WindowLocationChanged;
            }

        }

        private void WindowLocationChanged(object sender, EventArgs e)
        {
            if (this.IsOpen)
            {
                this.HorizontalOffset++;
                this.HorizontalOffset--;
            }
        }
        #region imports definitions
        [StructLayout(LayoutKind.Sequential)]
       private   struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowRect(IntPtr hWnd, out Rect lpRect);
        [DllImport("user32", EntryPoint = "SetWindowPos")]
        private static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
        #endregion
    }
}
