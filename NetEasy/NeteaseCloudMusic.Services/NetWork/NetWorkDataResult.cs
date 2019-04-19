namespace NeteaseCloudMusic.Services.NetWork
{
    /// <summary>
    /// 代表网络请求返回的结果
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NetWorkDataResult<T> : NetWorkDataResultBase
    {
        private string _errorMessage;

        //  public bool Successed { get; internal set; } = false;
        public string ErrorMessage
        {
            get
            {
                if (Successed)
                    return string.Empty;
                return this._errorMessage;
            }
            set
            {
                this._errorMessage = value;
            }
        }
        public T Data { get; internal set; }
    }
}
