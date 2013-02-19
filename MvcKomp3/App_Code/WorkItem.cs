using System.Collections.Generic;
using System.Threading;
using System;
using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
namespace MvcKomp3.App_Code
{
    public enum ActionType
    {
        None = 0,
        Add = 1
    }

    //public class WorkItem
    //{
    //    public const string ConnString = "Data Source=.;Integrated Security=True";
    //    private static Queue<WorkItem> queue = new Queue<WorkItem>();
    //    private static Semaphore maxQueueSemaphore = new Semaphore(MaxQueueLength, MaxQueueLength);
    //    private static Object workItemLockObject = new Object();
    //    private static WorkItem currentWorkItem;
    //    private static Thread worker;
    //    public delegate void Worker();
    //    public ActionType Action { get; set; }
    //    public ICollection<RequestInfo> RequestInfoList { get; private set; }

    //    public static int MaxQueueLength
    //    {
    //        get { return 100; }
    //    }

    //    public int Count
    //    {
    //        get { return this.RequestInfoList.Count; }
    //    }

    //    public static int QueueCount
    //    {
    //        get { return queue.Count; }
    //    }

    //    public WorkItem(ActionType action)
    //    {
    //        this.Action = action;
    //        this.RequestInfoList = new List<RequestInfo>();
    //    }

    //    private void Add(RequestInfo info)
    //    {
    //        this.RequestInfoList.Add(info);
    //    }

    //    private void Enqueue()
    //    {
    //        if (maxQueueSemaphore.WaitOne(1000))
    //        {
    //            lock (queue)
    //            {
    //                queue.Enqueue(this);
    //                Monitor.Pulse(queue);
    //            }
    //        }
    //        else
    //        {
    //            EventLog.WriteEntry("Application", "Timed-out enqueueing a WorkItem.  Queue size = " + QueueCount + ", Action = " + this.Action,
    //                EventLogEntryType.Error, 101);
    //        }
    //    }

    //    public static void QueuePageView(RequestInfo info, int batchSize)
    //    {
    //        lock (workItemLockObject)
    //        {
    //            if (currentWorkItem == null)
    //            {
    //                currentWorkItem = new WorkItem(ActionType.Add);
    //            }
    //            currentWorkItem.Add(info);
    //            if (currentWorkItem.Count >= batchSize)
    //            {
    //                currentWorkItem.Enqueue();
    //                currentWorkItem = null;
    //            }
    //        }
    //    }

    //    public static WorkItem Dequeue()
    //    {
    //        lock (queue)
    //        {
    //            for (; ; )
    //            {
    //                if (queue.Count > 0)
    //                {
    //                    WorkItem workItem = queue.Dequeue();
    //                    maxQueueSemaphore.Release();
    //                    return workItem;
    //                }
    //                Monitor.Wait(queue);
    //            }
    //        }
    //    }

    //    public static void Init(Worker work)
    //    {
    //        lock (workItemLockObject)
    //        {
    //            if (worker == null)
    //                worker = new Thread(new ThreadStart(work));
    //            if (!worker.IsAlive)
    //                worker.Start();
    //        }
    //    }

    //    public static void Work()
    //    {
    //        try
    //        {
    //            for (; ; )
    //            {
    //                WorkItem workItem = WorkItem.Dequeue();
    //                switch (workItem.Action)
    //                {
    //                    case ActionType.Add:
    //                        string sql = "[Traffic].[AddPageView]";
    //                        using (SqlConnection conn = new SqlConnection(ConnString))
    //                        {
    //                            foreach (RequestInfo info in workItem.RequestInfoList)
    //                            {
    //                                using (SqlCommand cmd = new SqlCommand(sql, conn))
    //                                {
    //                                    cmd.CommandType = CommandType.StoredProcedure;
    //                                    SqlParameterCollection p = cmd.Parameters;
    //                                    p.Add("pageurl", SqlDbType.VarChar, 256).Value = (object)info.Page ?? DBNull.Value;
    //                                    try
    //                                    {
    //                                        conn.Open();
    //                                        cmd.ExecuteNonQuery();
    //                                    }
    //                                    catch (SqlException e)
    //                                    {
    //                                        EventLog.WriteEntry("Application", "Error in WritePageView: " + e.Message + "\n",
    //                                            EventLogEntryType.Error, 102);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        break;
    //                }
    //            }
    //        }
    //        catch (ThreadAbortException)
    //        {
    //            return;
    //        }
    //        catch (Exception e)
    //        {
    //            EventLog.WriteEntry("Application", "Error in MarketModule worker thread: " + e.Message, EventLogEntryType.Error, 103);
    //            throw;
    //        }
    //    }
    //}
}