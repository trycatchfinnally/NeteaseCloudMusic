using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Wpf.Model
{
    public class LocalMusicModel : MusicModel
    {
        private string _filePath;

        /// <summary>
        /// 本地文件的地址
        /// </summary>
        public string FilePath
        {
            get
            {
                return _filePath;
            }

            set
            {
                SetProperty(ref _filePath, value);
            }
        }
        /// <summary>
        /// 本地文件的大小
        /// </summary>
        public FileSize FileSize
        {
            get
            {
                if (string.IsNullOrWhiteSpace(FilePath) || !System.IO.File.Exists(FilePath))
                    return 0;
                return new System.IO.FileInfo(FilePath).Length;
            }
        }
        /// <summary>
        /// 本地文件的图片
        /// </summary>
        public string ArtistImage
        {
            /*  get { return innerModel.Artist?.ArtistImage; }
              set
              {
                  if (innerModel.Artist == null)
                      innerModel.Artist = new Global.Model.Artist();
                  innerModel.Artist.ArtistImage = value;
                  RaisePropertyChanged();
              }*/
            set;get;
        }

    }
    /// <summary>
    /// 代表本地文件大小的结构
    /// </summary>
    public struct FileSize: IFormattable
    {
        public static readonly FileSize EmptyFileSize = new FileSize(0);
        private const long BytesPerKiloByte = 1024;
        private const long KiloBytesPerMegaByte = 1024;
        private const long MegaBytesPerGigabyte = 1024;
        private long _size;
        /// <summary>
        /// 用字节数表示的文件大小
        /// </summary>
        public long Size
        {
            get { return _size; }
            set
            {
                if (value < 0)
                    throw new ArgumentException("文件大小不能小于0");
                _size = value;
            }
        }
        /// <summary>
        /// 用kb表示的文件大小
        /// </summary>
        public double KiloByteSize
        {
            get
            {
                return Size * 1.0 / BytesPerKiloByte;
            }
        }
        /// <summary>
        /// 以mb表示的文件大小
        /// </summary>
        public double MegaByteSize
        {
            get
            {
                return KiloByteSize / KiloBytesPerMegaByte;
            }
        }
        /// <summary>
        /// 以GB表示的文件大小
        /// </summary>
        public double GigabyteSize
        {
            get
            {
                return MegaByteSize / MegaBytesPerGigabyte;
            }
        }
        /// <summary>
        /// 只有这里能录入负数
        /// </summary>
        /// <param name="size"></param>
        public FileSize(long size)
        {
            _size = size;
        }
        public override bool Equals(object obj)
        {

            if (!(obj is FileSize)) return false;
            return this.Size == ((FileSize)obj).Size;
        }
        public override int GetHashCode()
        {
            return Size.GetHashCode();
        }
        public override string ToString()
        {

            if (GigabyteSize > 1)
                return ToString("0.00G");
            if (MegaByteSize > 1)
                return ToString("0.00M");
            if (KiloByteSize > 1)
                return ToString("0.00K");
            return ToString("Bytes");

        }
        /// <summary>
        /// 按特定的格式格式化该结构 如0.00m
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToString(format, null);
        }
        /// <summary>
        /// 在XAML中会默认调用此方法
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                return ToString();
            string part1 = string.Empty;
            string part2 = string.Empty;
            var subString = format;
            while (subString.Length > 0)
            {
                double temp;
                if (double.TryParse(subString, out temp))
                {
                    part1 = subString;
                    part2 = new string(format.Skip(subString.Length).ToArray());
                    break;
                }
                subString = subString.Substring(0, subString.Length - 1);
                part2 = new string(format.Skip(subString.Length).ToArray());
            }

            switch (part2.ToUpper().FirstOrDefault())
            {
                case 'K':
                    if (string.IsNullOrEmpty(part1))
                        return KiloByteSize.ToString(formatProvider) + part2;
                    return KiloByteSize.ToString(part1, formatProvider) + part2;
                case 'M':
                    if (string.IsNullOrEmpty(part1))
                        return MegaByteSize.ToString(formatProvider) + part2;
                    return MegaByteSize.ToString(part1, formatProvider) + part2;
                case 'G':
                    if (string.IsNullOrEmpty(part1))
                        return GigabyteSize.ToString() + part2;
                    return GigabyteSize.ToString(part1, formatProvider) + part2;
                default:
                    if (string.IsNullOrEmpty(part1))
                        return Size.ToString(formatProvider) + part2;
                    return Size.ToString(part1, formatProvider) + part2;
            }
        }

        public static bool operator ==(FileSize left, FileSize right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(FileSize left, FileSize right)
        {
            return !left.Equals(right);
        }
        /// <summary>
        /// 将用字节表示的文件大小隐式转换为该类型对象
        /// </summary>
        /// <param name="fileLenthInBytes"></param>
        public static implicit operator FileSize(long fileLenthInBytes) => new FileSize(fileLenthInBytes);

    }

}
