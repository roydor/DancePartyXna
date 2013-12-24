using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FunctionCallback = DanceParty.Utilities.Threading.ThreadHelper.FunctionCallback;

namespace DanceParty.Utilities.Threading
{
    #if WINDOWS_PHONE
    using System.Threading;
    public class WindowsPhoneThreadHelper
    {
        public static void RunAsync(FunctionCallback function)
        {
            Thread loadingThread = new Thread(new ThreadStart(function));
            loadingThread.Start();
        }
    }
    #endif
}
