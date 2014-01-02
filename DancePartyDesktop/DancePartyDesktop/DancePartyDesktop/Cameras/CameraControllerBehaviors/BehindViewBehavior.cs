using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using DanceParty.Actors;

namespace DanceParty.Cameras.CameraControllerBehaviors
{
    public class BehindViewBehavior : ICameraControllerBehavior
    {
        public const int UnitsBehind = 500;
        public const int UnitsUp = 500;
        public const int UnitsForward = 500;

        private Dancer _leadDancer;
        private PerspectiveCamera _camera;

        public BehindViewBehavior(PerspectiveCamera camera, Dancer leadDancer)
        {
            _camera = camera;
            _leadDancer = leadDancer;
        }

        public void Update(GameTime gameTime)
        {
            _camera.Position = _leadDancer.Position - _leadDancer.Forward * UnitsBehind + _leadDancer.Up * UnitsUp;
            _camera.LookAt = _leadDancer.Position + _leadDancer.Forward * UnitsForward;
        }
    }
}
