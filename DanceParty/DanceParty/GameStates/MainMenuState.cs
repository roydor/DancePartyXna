using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace DanceParty.GameStates
{
    public class MainMenuState : IGameState
    {
        private MainMenuState()
        {
            // Load buttons
        }

        private static MainMenuState _instance;
        public static IGameState Instance
        {
            get { return _instance ?? (_instance = new MainMenuState()); }
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void LoadContent()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
