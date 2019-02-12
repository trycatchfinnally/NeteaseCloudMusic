using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Services.UniqueSymbol
{
    public class UniqueServices : IUniqueServices
    {
        public int UniqueNum(int maxValue)
        {
            try
            {
                int result = 0;
                foreach (var item in UniqueString)
                {
                    result += item;
                }
                return new Random(1024).Next(result);
            }
            catch (OverflowException)
            {

                return 0;
            }
        }


        public string UniqueString
        {
            get
            {
                try
                {
                    ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
                    ManagementObjectCollection mbsList = mbs.Get();
                    string id = "";
                    foreach (ManagementObject mo in mbsList)
                    {
                        id = mo["ProcessorID"].ToString();
                    }
                    ManagementObjectSearcher mos = new ManagementObjectSearcher("Select * From Win32_BaseBoard");
                    ManagementObjectCollection moc = mbs.Get();
                    string motherBoard = "";
                    foreach (ManagementObject mo in moc)
                    {
                        motherBoard = (string)mo["SerialNumber"];
                    }
                    return id + motherBoard;
                }
                catch (Exception ex)
                {
                    return "102454458";

                }

            }
        }
    }
}
