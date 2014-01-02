using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.TouchWrapper
{
    public class TouchWrapperUnsupported : ITouchWrapper
    {
        public bool IsSupported()
        {
            return false;
        }

        public Vector2? GetTouchPoint()
        {
            return null;
        }
    }
}
