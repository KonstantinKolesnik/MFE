using Microsoft.SPOT;
using System;
using System.Collections;

namespace MFE.Core.Threading
{
    /*
    see https://www.ghielectronics.com/community/codeshare/entry/806 
    
    Sometimes it is necesary to start a short running operation in background.
    Starting a new thread each time requires a couple of lines of code and is not very performant. Starting a new timer with dueTime = 0 and an Intervall of -1 (infinite) is also not very sexy. You also have to make sure you keep a reference to the thread or timer to prevent it's finilazation by the GC.

    In the 'big' .NET Framework you can use the ThreadPool to start short running background threads. Unfortuenately this class is not available in NETMF.

    This is a lightweigth and minimum feature implementation of a thread pool for NETMF.
    The interface is similar to the big .NET implementation.

    Features:
    QueueUserWorkItem, Get/SetMinThreads, GetSetMaxThreads, callback for unhandled exceptions

    Missing features:
    Automatically release threads which have not been used for a while.
    */

   /// <summary>
   /// Provides a pool of threads that can be used to execute tasks, post work items, 
   /// process asynchronous I/O, wait on behalf of other threads, and process timers.
   /// </summary>
   /// <remarks>
   /// Because the maximum number of threads used by the ThreadPool only short running
   /// operations should be executed by using it.
   /// Multiple long running operations would block the available threads.
   /// New operations will not be called at all if all threads are blocked.
   /// </remarks>
   public static class ThreadPool
   {
      static ThreadPool()
      {
         // create the initial number of threads
         SetMinThreads(3);
      }

      /// <summary>
      /// Queues a method for execution. The method executes when a thread pool thread becomes available.
      /// </summary>
      /// <param name="callback">A WaitCallback that represents the method to be executed.</param>
      /// <returns>true if the method is successfully queued.</returns>
      public static bool QueueUserWorkItem(WaitCallback callback)
      {
         return QueueUserWorkItem(callback, null);
      }

      /// <summary>
      /// Queues a method for execution, and specifies an object containing data to be used by the method. 
      /// The method executes when a thread pool thread becomes available.
      /// </summary>
      /// <param name="callback">A WaitCallback representing the method to execute.</param>
      /// <param name="state">An object containing data to be used by the method.</param>
      /// <returns>true if the method is successfully queued.</returns>
      public static bool QueueUserWorkItem(WaitCallback callback, object state)
      {
         lock (_ItemsQueue.SyncRoot)
         {
            var thread = GetThread();
            if (thread != null)
            {
               thread.Item = new ThreadPoolItem(callback, state);
            }
            else
            {
               _ItemsQueue.Enqueue(new ThreadPoolItem(callback, state));
            }
            return true;
         }
      }

      private static ThreadPoolThread GetThread()
      {
         lock (_Threads)
         {
            foreach (ThreadPoolThread thread in _Threads)
            {
               if (!thread.IsBusy)
               {
                  thread.IsBusy = true;
                  return thread;
               }
            }
            if (_Threads.Count < _maxThreadCount)
            {
               var thread = new ThreadPoolThread {IsBusy = true};
               _Threads.Add(thread);
               return thread;
            }
            return null;
         }
      }

      private static int _minThreadCount;
      private static int _maxThreadCount = 10;
      private static readonly ArrayList _Threads = new ArrayList();
      private static readonly Queue _ItemsQueue = new Queue();

      /// <summary>
      /// Retrieves the minimum number of threads the thread pool creates on demand.
      /// </summary>
      /// <returns>Returns the minimum number of worker threads that the thread pool creates on demand.</returns>
      public static int GetMinThreads()
      {
         return _minThreadCount;
      }

      /// <summary>
      /// Sets the minimum number of threads the thread pool creates on demand.
      /// </summary>
      /// <param name="count">The minimum number of worker threads that the thread pool creates on demand.</param>
      /// <returns>true if the change is successful; otherwise, false.</returns>
      public static bool SetMinThreads(int count)
      {
         _minThreadCount = count;

         while (_Threads.Count < _minThreadCount)
         {
            CreateNewThread();
         }
         return true;
      }

      /// <summary>
      /// Retrieves the number of requests to the thread pool that can be active concurrently. 
      /// All requests above that number remain queued until thread pool threads become available.
      /// </summary>
      /// <returns>The maximum number of asynchronous I/O threads in the thread pool.</returns>
      public static int GetMaxThreads()
      {
         return _maxThreadCount;
      }

      /// <summary>
      /// Sets the number of requests to the thread pool that can be active concurrently. 
      /// All requests above that number remain queued until thread pool threads become available.
      /// </summary>
      /// <param name="count">The maximum number of worker threads in the thread pool.</param>
      /// <returns>true if the change is successful; otherwise, false.</returns>
      public static bool SetMaxThreads(int count)
      {
         _maxThreadCount = count;
         return true;
      }

      private static void CreateNewThread()
      {
         lock (_Threads)
         {
            _Threads.Add(new ThreadPoolThread());
            Debug.Print("ThreadPool thread created: #" + _Threads.Count);
         }
      }

      /// <summary>
      /// Shuts down all threads after they have finished theire work.
      /// </summary>
      public static void Shutdown()
      {
         lock (_Threads)
         {
            foreach (ThreadPoolThread thread in _Threads)
            {
               thread.Dispose();
            }
            _Threads.Clear();
         }
      }

      internal static bool NotifyThreadIdle(ThreadPoolThread thread)
      {
         lock (_Threads)
         {
            if (_Threads.Count > _maxThreadCount)
            {
               thread.Dispose();
               _Threads.Remove(thread);
               Debug.Print("ThreadPool thread stoped: #" + _Threads.Count);
               return false;
            }
         }
         // start next enqueued item
         lock (_ItemsQueue.SyncRoot)
         {
            if (_ItemsQueue.Count > 0)
            {
               thread.Item = _ItemsQueue.Dequeue() as ThreadPoolItem;
               return true;
            }
         }
         return false;
      }

      internal static void OnUnhandledThreadPoolException(ThreadPoolItem item, Exception exception)
      {
         var tmp = UnhandledThreadPoolException;
         if (tmp != null)
         {
            tmp(item.State, exception);
         }
      }

      /// <summary>
      /// Is fired when a excption in one of the worker threads in unhandeld.
      /// </summary>
      public static event UnhandledThreadPoolExceptionDelegate UnhandledThreadPoolException;
   }
}


// usage example:
/*
public class Program
   {
      private static void ThreadPoolTestProc(object status)
      {
         int n = (int) status;
         for (int i = 0; i < 20; ++i)
         {
            Debug.Print("Thread pool proc " + n);
            Thread.Sleep(100);
         }
      }

      public static void Main()
      {
         // overclock memory clock
         var emcClkSel = new Register(0x400FC100);
         emcClkSel.ClearBits(1 << 0);  // OVERDRIVE
         //emcClkSel.SetBits(1 << 0);    // NORMAL


         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 1);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 2);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 3);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 4);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 5);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 6);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 7);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 8);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 9);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 10);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 11);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 12);
         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 13);

         ThreadPool.SetMaxThreads(5);

         Thread.Sleep(3000);

         ThreadPool.QueueUserWorkItem(ThreadPoolTestProc, 14);

         Thread.Sleep(-1);
      }
   }*/