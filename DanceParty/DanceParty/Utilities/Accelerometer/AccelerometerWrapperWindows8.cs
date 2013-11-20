using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.Accelerometer
{
#if WINDOWS8
using Windows.Devices.Sensors;

    public class AccelerometerWrapperWindows8 : IAccelerometerWrapper
    {
        private Accelerometer _accelerometer;
        private Vector3 _currentReading;
        /// <summary>
        /// Sets up the Accelerometer for Windows8
        /// </summary>
        public AccelerometerWrapperWindows8()
        {
            _accelerometer = Accelerometer.GetDefault();
            _currentReading = Vector3.Zero;
        }

        /// <summary>
        /// Reads in the Accelerometer data for windows8 devices.
        /// </summary>
        private void Windows8AccelerometerChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs args)
        {
            // For whatever reason, this is insconsistent with windows phone...
            // so lets swap the reading here.
            _currentReading.X = -(float)args.Reading.AccelerationY;
            _currentReading.Y = -(float)args.Reading.AccelerationX;
            _currentReading.Z = -(float)args.Reading.AccelerationZ;
        }

        public bool IsSupported
        {
            get { return _accelerometer != null; }
        }

        public Vector3 CurrentReading
        {
            get { return _currentReading; }
        }

        public void Start()
        {
            if (IsSupported)
                _accelerometer.ReadingChanged += Windows8AccelerometerChanged;
        }

        public void Stop()
        {
            if (IsSupported)
                _accelerometer.ReadingChanged -= Windows8AccelerometerChanged;
        }
    }
#endif
}
