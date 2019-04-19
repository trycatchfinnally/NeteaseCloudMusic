using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NeteaseCloudMusic.Wpf.Services
{
    internal class WindowsFileServices : NeteaseCloudMusic.Services.LocalFile.IFileServices
    {
        public string SelectFile()
        {
            return SelectFile(string.Empty);
        }

        public string SelectFile(string filter)
        {
            var ofd = new System.Windows.Forms.OpenFileDialog();
            if (!string.IsNullOrEmpty(filter)) ofd.Filter = filter;
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                return ofd.FileName;
            }

            return null;
        }
        public string SelectDirectory()
        {
            var fd = new System.Windows.Forms.FolderBrowserDialog();
            string selectPath = string.Empty;
            if (fd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                selectPath = fd.SelectedPath;
            }

            return selectPath;
        }
        public IEnumerable<string> GetFiles(string dirPath, bool includChildDirectories) =>
          GetFiles(dirPath, includChildDirectories, "*");


        public IEnumerable<string> GetFiles(string dirPath, bool includChildDirectories, string searchPattern)
        {
            var result = new List<string>();
            if (Directory.Exists(dirPath))
            {
                result.AddRange(Directory.GetFiles(dirPath, searchPattern, includChildDirectories ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly));
            }
            return result;
        }

        public (string title,string albumName, IEnumerable<string> artistName, TimeSpan duration,long fileSize,string picPath) GetFileId3(string filePath)
        {
            if (File.Exists(filePath))
            {
               // var id3s = new Id3.Mp3(filePath).GetAllTags().First();
                var file = TagLib.File.Create(filePath);
                // id3s.AudioFileUrl = filePath;
                string tempPath = null;
                if (file.Tag.Pictures.Length>0)
                {
                    tempPath = Path.GetTempFileName();
                    using (var fs = new FileStream(tempPath, FileMode.OpenOrCreate))
                    {
                        var buffer = file.Tag.Pictures[0].Data.ToArray();
                        fs.Write(buffer,0,buffer.Length);
                    }
                }
               
                return (file.Tag.Title??Path.GetFileNameWithoutExtension(filePath),file.Tag.Album, file.Tag.Performers, file.Properties.Duration,new FileInfo(filePath).Length,tempPath);
            }

            throw new FileNotFoundException(nameof(filePath));
        }
    }
}
