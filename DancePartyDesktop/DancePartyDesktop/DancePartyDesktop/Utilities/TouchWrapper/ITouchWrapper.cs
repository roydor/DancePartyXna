using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Utilities.TouchWrapper
{
    public interface ITouchWrapper
    {
        bool IsSupported();
        Vector2? GetTouchPoint();
    }
}
