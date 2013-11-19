using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.Accelerometer
{
#if WINDOWS_PHONE
using Microsoft.Devices.Sensors;
using Accelerometer = Microsoft.Devices.Sensors.Accelerometer;

    public class AccelerometerWrapperWindowsPhone : IAccelerometerWrapper
    {
        private Accelerometer _accelerometer;

        public AccelerometerWrapperWindowsPhone()
        {
            _accelerometer = new Accelerometer();
        }

        public void Start()
        {
            _accelerometer.Start();
        }

        public void Stop()
        {
            _accelerometer.Stop();
        }

        public bool IsSupported
        {
            get { return Accelerometer.IsSupported; }
        }

        public Vector3 CurrentReading
        {
            get { return _accelerometer.CurrentValue.Acceleration; }
        }
    }
#endif
}
