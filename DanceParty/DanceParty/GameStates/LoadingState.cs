using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

using DanceParty.Utilities.Threading;

namespace DanceParty.GameStates
{
    public delegate void LoadContentFunction();

    public static class LoadingStateFactory
    {
        public static LoadingState GetLoadingState(LoadContentFunction loadFunction)
        {
            return new LoadingState(loadFunction, DancePartyGame.Instance.GraphicsDevice, DancePartyGame.Instance.SpriteBatch);
        }
    }

    public class LoadingState : IGameState
    {
        private LoadContentFunction _loadFunction;
        private bool _loadingComplete;

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        private Texture2D _loadingTexture;

        public LoadingState(LoadContentFunction loadFunction, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            _loadingComplete = false;
            _loadFunction = loadFunction;

            _loadingTexture = DancePartyGame.Instance.Content.Load<Texture2D>("Textures\\LoadingMessage");
            ThreadHelper.RunAsync(LoadContentMain);
        }

        private void LoadContentMain()
        {
            _loadFunction();
            _loadingComplete = true;
        }

        public void LoadContent()
        {
        }

        public void Update(GameTime gameTime)
        {
            if (!_loadingComplete)
            {

            }
            else
            {
                //Get off the gamestack.
                GameStateManager.Instance.PopGameState();
            }
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkSlateBlue);
            _spriteBatch.Begin();
            _spriteBatch.Draw(_loadingTexture, Vector2.Zero, Color.Wheat);
            _spriteBatch.End();
        }

        public void Dispose()
        {
        }
    }
}
