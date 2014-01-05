using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ServiceWebApi.Dal
{
    public class VideoComparer:IEqualityComparer<Video>
    {
        public bool Equals(Video x, Video y)
        {
            if (x != null && y != null)
                return string.Compare(x.Path.ToUpper(), y.Path.ToUpper(), true) == 0;
            return false;
        }

        public int GetHashCode(Video obj)
        {
            return obj.Path.GetHashCode() ^ obj.Size.Value.GetHashCode();
        }
    }
}
