using System;
using System.Threading.Tasks;
using NeteaseCloudMusic.Global.Model;

namespace NeteaseCloudMusic.Services.Identity
{
    public interface IdentityService
    {
      //  bool IsLoginIn { get; }
        Task<String> LoginInAsync(string cellPhoneOrEmail, string passWord, bool remember);
        Task<bool> LoginOutAsync();
        Task<bool> RefreshLoginAsync();
        User CurrentUser { get; }
        event EventHandler<bool> LoginStateChanged;
    }
}
