using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities
{
    public class FPSTracker
    {
        private double _accumulatedTime;
        private int _currentFPS;
        private int _frames;

        public int CurrentFPS
        {
            get
            {
                return _currentFPS;
            }
        }

        public FPSTracker()
        {
            TrackFPS();
        }

        private void TrackFPS()
        {
            _currentFPS = _frames;
            _accumulatedTime = 0;
            _frames = 0;
        }

        public void Update(GameTime gameTime)
        {
            _frames++;
            _accumulatedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (_accumulatedTime > 1)
                TrackFPS();
        }
    }
}
