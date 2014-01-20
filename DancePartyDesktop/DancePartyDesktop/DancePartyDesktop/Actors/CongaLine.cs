using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DanceParty.Actors.DancerBehaviors;

using Microsoft.Xna.Framework;
using DanceParty.Cameras;

namespace DanceParty.Actors
{
    public class CongaLine
    {
        public int DrunkFactor
        {
            get
            {
                return _drunkController.GetDrunkFactor();
            }
        }

        private DrunkController _drunkController;
        private List<Dancer> _dancers;
        private bool _stopped;
        private double _timeSinceStopped;
        private int _dancersFallen;

        public Dancer LeadDancer
        {
            get
            {
                return _dancers[0];
            }
        }

        public Dancer TailDancer
        {
            get
            {
                return _dancers[_dancers.Count - 1];
            }
        }

        public Dancer LastFallenDancer
        {
            get
            {
                if (_dancersFallen < _dancers.Count)
                {
                    return _dancers[_dancersFallen];
                }
                return _dancers[_dancers.Count - 1];
            }
        }

        public void AddDrink(int drinkValue)
        {
            _drunkController.AddDrink(drinkValue);
        }
        
        public CongaLine(Dancer leadDancer, PerspectiveCamera camera)
        {
            _dancers = new List<Dancer>();
            _dancers.Add(leadDancer);
            _drunkController = new DrunkController(leadDancer, camera);
        }

        /// <summary>
        /// Adds a dancer to this conga line and forces it to follow the person at the end.
        /// </summary>
        public void AppendDancer(Dancer newDancer)
        {
            newDancer.SetDancerBehavior(new JoinLineDancerBehavior(newDancer, TailDancer));
            _dancers.Add(newDancer);
        }

        /// <summary>
        /// Updates all the dancers in this congaline.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            if (_stopped)
            {
                _timeSinceStopped += gameTime.ElapsedGameTime.TotalSeconds;
                if (_dancersFallen < _dancers.Count && _timeSinceStopped >= 0.05f)
                {
                    _timeSinceStopped = 0;
                    Dancer fallingDancer = _dancers[_dancersFallen++];
                    fallingDancer.SetDancerBehavior(new FallingDancerBehavior(fallingDancer));
                }
            }

            _drunkController.Update(gameTime);

            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);
        }

        public bool CollidesWithSelf()
        {
            // Can't collide with the first 10 people in the line.
            for (int i = 10; i < _dancers.Count; i++)
            {
                if (LeadDancer.CollidesWith(_dancers[i]) && _dancers[i].IsHostile())
                    return true;
            }

            return false;
        }

        public void Stop()
        {
		    _stopped = true;
        }

        public int Count
        {
            get
            {
                return _dancers.Count;
            }
        }

        public bool HasStopped
        {
            get
            {
                return _stopped;
            }
        }
    }
}
