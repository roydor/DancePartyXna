using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DanceParty.Cameras.CameraControllerBehaviors;
using Microsoft.Xna.Framework;

namespace DanceParty.Cameras
{
    public class CameraController
    {
        private PerspectiveCamera _camera;
        public PerspectiveCamera Camera
        {
            get { return _camera; }
        }

        private ICameraControllerBehavior _behavior;

        public CameraController(PerspectiveCamera camera)
        {
            _camera = camera;
        }

        public void SetCameraBehavior(ICameraControllerBehavior behavior)
        {
            _behavior = behavior;
        }

        public void Update(GameTime gameTime)
        {
            _behavior.Update(gameTime);
            _camera.Update();
        }
    }
}
