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
using Microsoft.Xna.Framework.Media;

using DanceParty.Utilities.Threading;

namespace DanceParty.GameStates
{
    public delegate void LoadContentFunction();

    public static class LoadingStateFactory
    {
        public static LoadingState GetLoadingState(LoadContentFunction loadFunction)
        {
            return new LoadingState(loadFunction,
                DancePartyGame.Instance.GraphicsDevice, 
                DancePartyGame.Instance.SpriteBatch,
                FontManager.Instance.BangersLarge);
        }
    }

    public class LoadingState : IGameState
    {
        private LoadContentFunction _loadFunction;
        private bool _loadingComplete;

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;

        private string[] _loadingTips = new string[] {
            "Make sure you don't bump into the conga line!", 
            "Add people to your line by bumping into them!"
        };

        private const string _loadingTextBase = "Loading";
        private const string _dot = ".";

        private string _animatedMessage;
        private int _numberOfDots = 0;
        private int _maxDots = 3;

        private Vector2 _loadingMessageDimensions;
        private Vector2 _loadingMessagePosition;

        //Time between animating dots in seconds.
        private float _loadingTickLength = 0.5f;
        private float _timeSinceTick = 0;

        public LoadingState(LoadContentFunction loadFunction, 
            GraphicsDevice graphicsDevice, 
            SpriteBatch spriteBatch, 
            SpriteFont UIFont)
        {
            _graphicsDevice = graphicsDevice;
            _spriteBatch = spriteBatch;

            _loadingComplete = false;
            _loadFunction = loadFunction;
            _animatedMessage = _loadingTextBase;
            
            _loadingMessageDimensions = new Vector2();
            _loadingMessagePosition = new Vector2();

            _loadingMessageDimensions.X = FontManager.Instance.BangersLarge.MeasureString(_animatedMessage).X;
            _loadingMessageDimensions.Y = FontManager.Instance.BangersLarge.MeasureString(_animatedMessage).Y;
            _loadingMessagePosition.X = (_graphicsDevice.Viewport.Width - _loadingMessageDimensions.X) / 2;
            _loadingMessagePosition.Y = (_graphicsDevice.Viewport.Height - _loadingMessageDimensions.Y) / 2;

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
                _timeSinceTick += (float) gameTime.ElapsedGameTime.TotalSeconds;

                if (_timeSinceTick >= _loadingTickLength)
                {
                    _timeSinceTick -= _loadingTickLength;
                    _numberOfDots++;

                    if (_numberOfDots > _maxDots)
                    {
                        _animatedMessage = _loadingTextBase;
                        _numberOfDots = 0;
                    }
                    else
                    {
                        _animatedMessage = _dot + _animatedMessage + _dot;
                    }
                }
                
                _loadingMessageDimensions.X = FontManager.Instance.BangersLarge.MeasureString(_animatedMessage).X;
                _loadingMessageDimensions.Y = FontManager.Instance.BangersLarge.MeasureString(_animatedMessage).Y;
                _loadingMessagePosition.X = (_graphicsDevice.Viewport.Width - _loadingMessageDimensions.X) / 2;
                _loadingMessagePosition.Y = (_graphicsDevice.Viewport.Height - _loadingMessageDimensions.Y) / 2;
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
            _spriteBatch.DrawString(FontManager.Instance.BangersLarge, _animatedMessage, _loadingMessagePosition, Color.Yellow);
            _spriteBatch.End();
        }

        public void Dispose()
        {
        }
    }
}
