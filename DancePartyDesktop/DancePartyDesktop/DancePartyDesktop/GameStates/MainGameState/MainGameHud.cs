using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DanceParty.GameStates
{
    public class MainGameHud
    {
        private Vector2 _scorePosition;
        private Vector2 _drunkFactorPosition;

        private GraphicsDevice _graphicsDevice;
        private SpriteBatch _spriteBatch;
        private SpriteFont _font;

        public MainGameHud(GraphicsDevice device, SpriteBatch spriteBatch, SpriteFont spriteFont)
        {
            _graphicsDevice = device;
            _spriteBatch = spriteBatch;
            _font = spriteFont;
        }

        private string GetHumanReadableTime(TimeSpan timeSpan)
        {
            int minutes = timeSpan.Minutes;
            int seconds = timeSpan.Seconds;

            if (seconds < 10)
                return minutes + ":0" + seconds;
            else
                return minutes + ":" + seconds;
        }

        public void Update(TimeSpan timeRemaining, int dancersCollected, int drunkFactor)
        {
            Vector2 drunkFactorMeasurement = _font.MeasureString(drunkFactor.ToString());
            Vector2 scoreMeasurement = _font.MeasureString(dancersCollected.ToString());

            _scorePosition.X = _graphicsDevice.Viewport.Width - scoreMeasurement.X - 50;
            _drunkFactorPosition.X = (_graphicsDevice.Viewport.Width - drunkFactorMeasurement.X) / 2;
        }

        public void Draw(TimeSpan timeRemaining, int score, int drunkFactor)
        {
            _spriteBatch.DrawString(_font, score.ToString(), _scorePosition, Color.Yellow);
            _spriteBatch.DrawString(_font, drunkFactor.ToString(), _drunkFactorPosition, Color.Yellow);
            _spriteBatch.DrawString(_font, GetHumanReadableTime(timeRemaining), new Vector2(50, 0), Color.Yellow);
        }
    }
}
