using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DanceParty.GameStates
{
    public class GameOverMessage
    {
        private string _message;
        private Vector2 _messageDimensions;
        private Vector2 _messagePosition;
        private Vector2 _messageDestination;

        private string _description;
        private Vector2 _descriptionDimensions;
        private Vector2 _descriptionPosition;
        private Vector2 _descriptionDestination;

        private string _tapMessage;
        private Vector2 _tapMessageDimensions;
        private Vector2 _tapMessagePosition;
        private Vector2 _tapMessageDestination;

        private float _velocity;
        private float _totalTime;

        public bool ShowTapMessage;

        private GraphicsDevice _graphicsDevice;

        public GameOverMessage(GraphicsDevice graphicsDevice, string message, string description)
        {
            _graphicsDevice = graphicsDevice;
            _message = message;
            _description = description;

            _messageDimensions = FontManager.Instance.BangersLarge.MeasureString(_message);
            _descriptionDimensions = FontManager.Instance.BangersSmall.MeasureString(_description);

            _messagePosition.Y = -_messageDimensions.Y;
            _descriptionPosition.Y = -_descriptionDestination.Y;

            //Set the message to finish 3/5 down the screen.
            _messagePosition.X = (_graphicsDevice.Viewport.Width - _messageDimensions.X) / 2;
            _messageDestination.X = (_graphicsDevice.Viewport.Width - _messageDimensions.X) / 2;
            _messageDestination.Y = 3 * (_graphicsDevice.Viewport.Height / 5) - (_messageDimensions.Y / 2);

            // Set the description to fall all the way to the bottom.
            _descriptionPosition.X = (_graphicsDevice.Viewport.Width - _descriptionDimensions.X) / 2;
            _descriptionDestination.X = (_graphicsDevice.Viewport.Width - _descriptionDimensions.X) / 2;
            _descriptionDestination.Y = _messageDestination.Y + _messageDimensions.Y;

            _tapMessage = "(... Tap to continue ...)";
            _tapMessageDimensions = FontManager.Instance.BangersSmall.MeasureString(_tapMessage);
            _tapMessagePosition.X = (_graphicsDevice.Viewport.Width - _tapMessageDimensions.X) / 2;
            _tapMessageDestination.X = _tapMessagePosition.X;
            _tapMessageDestination.Y = _descriptionDestination.Y + _descriptionDimensions.Y;
        }

        public void Update(GameTime gameTime)
        {
            float dt = (float)(gameTime.ElapsedGameTime.TotalSeconds);
            _velocity += 20;
            _totalTime += dt;

            float dist = _velocity * dt;

            _messagePosition.Y += dist;
            _descriptionPosition.Y += dist;
            _tapMessagePosition.Y += dist;

            if (_messagePosition.Y > _messageDestination.Y)
            {
                _messagePosition.Y = _messageDestination.Y;
                _descriptionPosition.Y = _descriptionDestination.Y;
                _tapMessagePosition.Y = _tapMessageDestination.Y;

                _velocity *= -0.5f;
            }

            if (_totalTime > 5)
                ShowTapMessage = true;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontManager.Instance.BangersLarge, _message, _messagePosition, Color.Yellow);
            spriteBatch.DrawString(FontManager.Instance.BangersSmall, _description, _descriptionPosition, Color.Yellow);

            if (ShowTapMessage)
                spriteBatch.DrawString(FontManager.Instance.BangersSmall, _tapMessage, _tapMessagePosition, Color.White);
        }
    }
}
