using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DanceParty.Utilities.TouchWrapper
{

#if WINDOWS_PHONE || WINDOWS8
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input.Touch;

    public class TouchWrapperSupported : ITouchWrapper
    {
        public bool IsSupported()
        {
            return TouchPanel.GetCapabilities().IsConnected;
        }

        public Microsoft.Xna.Framework.Vector2? GetTouchPoint()
        {
            Vector2? clickedLocation = null;

                foreach (TouchLocation touchLocation in TouchPanel.GetState())
                {
                    if (touchLocation.State == TouchLocationState.Pressed)
                    {
                        clickedLocation = touchLocation.Position;
                    }
                }
           return clickedLocation;
        }
    }
#endif
}
