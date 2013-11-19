using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Cameras.CameraControllerBehaviors
{
    public interface ICameraControllerBehavior
    {
        void Update(GameTime gameTime);
    }
}
