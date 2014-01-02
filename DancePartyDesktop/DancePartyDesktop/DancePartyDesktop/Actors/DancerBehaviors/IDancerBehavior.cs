using System;
using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public interface IDancerBehavior
    {
        void Update(GameTime gameTime);
        bool IsHostile();
    }
}
