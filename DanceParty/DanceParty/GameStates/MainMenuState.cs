using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace DanceParty.GameStates
{
    public class UIButton
    {
        public delegate void OnClick();
        
        private string _label;
        public Vector2 Size { get; private set;}
        public Vector2 Position;

        public OnClick OnActivate;

        public UIButton(string label, OnClick onClick)
        {
            OnActivate = onClick;
            _label = label;
            Size = FontManager.Instance.BangersLarge.MeasureString(_label);
        }

        public bool ContainsPoint(Vector2 point)
        {
            return Position.X <= point.X && point.X <= Position.X + Size.X
                && Position.Y <= point.Y && point.Y <= Position.Y + Size.Y;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontManager.Instance.BangersLarge, _label, Position, Color.Yellow);
        }
    }

    public class MainMenuState : IGameState
    {
        private const float _spacerHeight = 10.0f;
        private List<UIButton> _buttons = new List<UIButton> 
        {
            new UIButton("Play", OnPlayClick),
            new UIButton("Options", OnQuitClick),
            new UIButton("Quit", OnQuitClick),
        };

        private static void OnPlayClick()
        {
            GameStateManager.Instance.EnqueueGameState(
                new MainGameState(
                    DancePartyGame.Instance.GraphicsDevice,
                    DancePartyGame.Instance.SpriteBatch,
                    DancePartyGame.Instance.Content));
        }

        private static void OnQuitClick()
        {
            DancePartyGame.Instance.Exit();
        }

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        private MainMenuState(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;
        }

        private static MainMenuState _instance;
        public static IGameState Instance
        {
            get { return _instance ?? (_instance = 
                new MainMenuState(
                    DancePartyGame.Instance.GraphicsDevice, 
                    DancePartyGame.Instance.SpriteBatch)); }
        }

        public void Update(GameTime gameTime)
        {
            Vector2? clickedPosition = PointerInputManager.Instance.GetClickedPosition();
            if (clickedPosition.HasValue)
            {
                foreach (UIButton button in _buttons)
                {
                    if (button.ContainsPoint(clickedPosition.Value))
                    {
                        button.OnActivate();
                        return;
                    }
                }
            }

            float totalHeight = 0;
            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Position.X = (_graphicsDevice.Viewport.Width - _buttons[i].Size.X) / 2;
                totalHeight += _buttons[i].Size.Y + _spacerHeight;
            }

            float buttonsTop = (_graphicsDevice.Viewport.Height - totalHeight) / 2;

            for (int i = 0; i < _buttons.Count; i++)
            {
                _buttons[i].Position.Y = buttonsTop;
                buttonsTop += _buttons[i].Size.Y + _spacerHeight;
            }
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkSlateBlue);
            _spriteBatch.Begin();
            foreach (UIButton button in _buttons)
                button.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
