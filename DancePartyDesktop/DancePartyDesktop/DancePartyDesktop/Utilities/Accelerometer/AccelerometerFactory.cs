using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Utilities.Accelerometer
{
    public static class AccelerometerFactory
    {
        public static IAccelerometerWrapper GetAccelerometer()
        {
#if WINDOWS8
            return new AccelerometerWrapperWindows8();
#elif WINDOWS_PHONE
            return new AccelerometerWrapperWindowsPhone();
#endif
#if WINDOWS_DESKTOP
            return new NoAccelerometer();
#endif
        }
    }
}
