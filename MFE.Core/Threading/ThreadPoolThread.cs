using System;
using System.Threading;

namespace MFE.Core.Threading
{
    class ThreadPoolThread : IDisposable
    {
        private ThreadPoolItem item;
        private Thread thread;
        private readonly ManualResetEvent waitEvent = new ManualResetEvent(false);

        public ThreadPoolThread()
        {
            thread = new Thread(ThreadProc);
            thread.Start();
        }
        public void Dispose()
        {
            IsBusy = true;
            thread = null;
            waitEvent.Set();
        }

        public bool IsBusy
        {
            get;
            set;
        }
        public ThreadPoolItem Item
        {
            get { return item; }
            set
            {
                item = value;
                if (item != null)
                {
                    IsBusy = true;
                    waitEvent.Set();
                }
            }
        }

        private void ThreadProc()
        {
            while (thread != null)
            {
                try
                {
                    waitEvent.WaitOne();

                    if (thread != null && item != null)
                    {
                        item.Callback(item.State);
                    }
                }
                // ReSharper disable once EmptyGeneralCatchClause
                catch (Exception ex)
                {
                    ThreadPool.OnUnhandledThreadPoolException(Item, ex);
                }

                if (thread != null)
                {
                    waitEvent.Reset();
                    item = null;
                    IsBusy = ThreadPool.NotifyThreadIdle(this);
                }
            }
        }
    }
}
