using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.Actors.DancerBehaviors
{
    public class FollowingDancerBehavior : IDancerBehavior
    {
        private Dancer _following;
        private Dancer _me;

        private Vector3 _destination;
        private float _currentSpeed;

        public const float WaypointDuration = 0.3f;

        public FollowingDancerBehavior(Dancer me, Dancer following)
        {
            _me = me;
            _following = following;
            SetWaypoint();
            _me.SetAnimation("Conga");
        }

        private void SetWaypoint()
        {
            _destination = _following.Position;
            _me.Forward = Vector3.Normalize(_destination - _me.Position);
            _currentSpeed = Vector3.Distance(_me.Position, _destination) / WaypointDuration;
        }

        public void Update(GameTime gameTime)
        {
            Vector3 displacement = _destination - _me.Position;

            float stepLength = (float) gameTime.ElapsedGameTime.TotalSeconds * _currentSpeed;

            // Square roots are expensive.
            // If we would pass, or arrive at our destination this step, grab a new waypoint.
            if (stepLength * stepLength >= displacement.LengthSquared())
            {
                _me.Position = _destination;
                SetWaypoint();
                return;
            }

            _me.Position += _me.Forward * stepLength;
        }

        public bool IsHostile()
        {
            return _following.IsHostile();
        }
    }
}
