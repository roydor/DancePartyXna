using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DanceParty.Actors
{
    public class DrinkEmitter
    {
        private Random _random = new Random();
        private Model _drinkModel;

        public void LoadContent()
        {
            _drinkModel = DancePartyGame.Instance.Content.Load<Model>("Models\\Drink2");
        }

        public Drink EmitDrink()
        {
            float angle = ((float)_random.NextDouble()) * MathHelper.TwoPi;
            float radius = (float)  (_random.NextDouble() * 1000);
            Drink newDrink = new Drink(_drinkModel);

            newDrink.Position.X = radius * (float)Math.Sin(angle);
            newDrink.Position.Z = radius * (float)Math.Cos(angle);

            return newDrink;
        }
    }
}
