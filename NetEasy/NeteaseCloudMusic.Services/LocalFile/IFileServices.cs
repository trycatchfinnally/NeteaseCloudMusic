using System;
using System.Collections.Generic;

namespace NeteaseCloudMusic.Services.LocalFile
{
    public interface IFileServices
    {
        /// <summary>
        /// 获取指定文件夹下的所有文件
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="includChildDirectories">是否包含子文件夹</param>
        /// <returns></returns>
        IEnumerable<string> GetFiles(string dirPath, bool includChildDirectories);
        /// <summary>
        /// 根据指定的通配符获取指定文件夹下的所有文件
        /// </summary>
        /// <param name="dirPath">文件夹路径</param>
        /// <param name="includChildDirectories">是否包含子文件夹</param>
        /// <param name="searchPattern">通配符</param>
        /// <returns></returns>
        IEnumerable<string> GetFiles(string dirPath, bool includChildDirectories,string searchPattern);
        /// <summary>
        /// 选择文件夹
        /// </summary>
        /// <returns></returns>
        string SelectDirectory();
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <returns></returns>
        string SelectFile();
        /// <summary>
        /// 选择文件
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        string SelectFile(string filter);

        (string title,string albumName, IEnumerable<string>artistName, TimeSpan duration,long fileSize, string picPath) GetFileId3(string filePath);
    }
}
