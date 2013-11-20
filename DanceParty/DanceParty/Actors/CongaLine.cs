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
        private List<Dancer> _dancers;

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
        
        public CongaLine(Dancer leadDancer)
        {
            _dancers = new List<Dancer>();
            _dancers.Add(leadDancer);
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
            foreach (Dancer dancer in _dancers)
                dancer.Update(gameTime);
        }

        /// <summary>
        /// Draws this congaline and 
        /// </summary>
        public void Draw(PerspectiveCamera camera)
        {
            foreach (Dancer dancer in _dancers)
                dancer.Draw(camera);
        }
    }
}
