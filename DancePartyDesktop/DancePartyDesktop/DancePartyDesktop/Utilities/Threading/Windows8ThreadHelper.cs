using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FunctionCallback = DanceParty.Utilities.Threading.ThreadHelper.FunctionCallback;

namespace DanceParty.Utilities.Threading
{
#if WINDOWS8
    using Windows.System.Threading;
    public class Windows8ThreadHelper
    {
        public static void RunAsync(FunctionCallback function)
        {
            ThreadPool.RunAsync( (workitem) => function());
        }
    }
#endif
}
