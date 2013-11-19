using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.Accelerometer
{
    public interface IAccelerometerWrapper
    {
        bool IsSupported { get; }
        Vector3 CurrentReading { get; }
        void Start();
        void Stop();
    }
}
