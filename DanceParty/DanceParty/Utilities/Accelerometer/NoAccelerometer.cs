using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.Accelerometer
{
    public class NoAccelerometer : IAccelerometerWrapper
    {
        public bool IsSupported
        {
            get 
            {
                return false;
            }
        }

        public Vector3 CurrentReading
        {
            get
            {
                return Vector3.Zero;
            }
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }
    }
}
