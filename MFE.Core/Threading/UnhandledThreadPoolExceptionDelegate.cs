using System;

namespace MFE.Core.Threading
{
    public delegate void UnhandledThreadPoolExceptionDelegate(object state, Exception ex);
}
