namespace NeteaseCloudMusic.Global.Model
{
   public  class User
    {
        /// <summary>
        /// 获取或设置用户头像
        /// </summary>
        public string  UserImage
        {
            set;get;
        }
        /// <summary>
        /// 获取或设置用户名
        /// </summary>
        public string Province { get; set; }
        public string City  { get; set; }
        public string  UserName
        {
            get;set;
        }
        /// <summary>
        /// 用户的id
        /// </summary>
        public long  UserId { get; set; }
        /// <summary>
        /// 关注数量
        /// </summary>
        public int Follows { get; set; }
        /// <summary>
        /// 粉丝数量
        /// </summary>
        public int Followeds { set; get; }
        /// <summary>
        /// 个人介绍
        /// </summary>
        public string DetailDescription { get; set; }
        /// <summary>
        /// 动态
        /// </summary>
        public int EventCount { get; set; }
        /// <summary>
        /// vip点击
        /// </summary>
        public string  VipLevel { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
    }
}
