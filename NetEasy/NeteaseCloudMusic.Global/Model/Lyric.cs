using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeteaseCloudMusic.Global.Model
{
  public   class Lyric
    {
        public Lyric()
        {

        }
        public Lyric(TimeSpan time,string value)
        {
            Time = time;Value = value;
        }
        public TimeSpan  Time { get; set; }
        public string  Value { get; set; }
        public override string ToString()
        {
            return Value;
        }
    }
}
