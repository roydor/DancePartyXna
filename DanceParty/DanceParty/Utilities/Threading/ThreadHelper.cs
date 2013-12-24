using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Utilities.Threading
{
    public static class ThreadHelper
    {
        public delegate void FunctionCallback();
        public static void RunAsync(FunctionCallback functionToCall)
        {
#if WINDOWS8
            Windows8ThreadHelper.RunAsync(functionToCall);
            return;
#endif

#if WINDOWS_PHONE
            WindowsPhoneThreadHelper.RunAsync(functionToCall);
            return;
#endif
        }
    }
}
