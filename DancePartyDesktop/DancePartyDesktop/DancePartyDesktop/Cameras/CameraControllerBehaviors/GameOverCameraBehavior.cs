using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DanceParty.Actors;
using Microsoft.Xna.Framework;

namespace DanceParty.Cameras.CameraControllerBehaviors
{
    public class GameOverCameraBehavior : ICameraControllerBehavior
    {
        public const int UnitsBehind = 500;
        public const int UnitsUp = 500;
        public const int UnitsForward = 500;

        private CongaLine _congaLine;
        private PerspectiveCamera _camera;

        public GameOverCameraBehavior(PerspectiveCamera camera, CongaLine congaLine)
        {
            _camera = camera;
            _congaLine = congaLine;
        }
        
        public void Update(GameTime gameTime)
        {
            _camera.LookAt = Vector3.Lerp(_camera.LookAt, (_congaLine.LastFallenDancer.Position + new Vector3(0, 50f, 0)), 0.02f);
        }
    }
}