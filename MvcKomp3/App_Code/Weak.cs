using System;
using System.Data;

public static class Weak
{
    public static WeakReference MyItem { get; set; }
    public static readonly Object lockObject = new Object();

    public static DataSet WeakData()
    {
        DataSet ds = null;
        lock (lockObject)
        {
            if (MyItem != null)
                ds = MyItem.Target as DataSet;
            if (ds == null)
            {
                ds = new DataSet();
                MyItem = new WeakReference(ds);
            }
        }
        return ds;
    }
}
