using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Utilities.TouchWrapper
{
    public class TouchWrapperFactory
    {
        public static ITouchWrapper GetTouchWrapper()
        {
#if WINDOWS_DESKTOP
            return new TouchWrapperUnsupported();
#elif WINDOWS_PHONE || WINDOWS8
            return new TouchWrapperSupported();
#endif
        }
    }
}
