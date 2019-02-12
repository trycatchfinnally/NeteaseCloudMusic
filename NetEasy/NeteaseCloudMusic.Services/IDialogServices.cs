using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services
{
    public interface  IDialogServices
    {
        void ShowDialog(string text);
        Task ShowDialogAsync(string text);
    }
}
