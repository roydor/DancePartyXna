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
    public class ScreenString
    {
        public Vector2 Position;
        public string Label;
    }

    public class CreditsSection
    {
        public ScreenString Title;
        public List<ScreenString> Contributors;

        public CreditsSection(string label, List<string> contributors)
        {
            Title = new ScreenString() { Label = label, Position = Vector2.Zero };
            Contributors = new List<ScreenString>();
            foreach (string contributor in contributors)
                Contributors.Add(new ScreenString() { Position = Vector2.Zero, Label = contributor });
        }

        public float UpdatePosition(float verticalOffset, float screenWidth)
        {
            float totalHeight = verticalOffset;

            Vector2 titleBounds = FontManager.Instance.BangersMedium.MeasureString(Title.Label);
            Title.Position.Y = totalHeight;
            Title.Position.X = (screenWidth - titleBounds.X) / 2;
            totalHeight += titleBounds.Y;

            for (int i = 0; i < Contributors.Count; i++)
            {
                ScreenString contributor = Contributors[i];
                Vector2 contributorBounds = FontManager.Instance.BangersSmall.MeasureString(contributor.Label);
                contributor.Position.Y = totalHeight;
                contributor.Position.X = (screenWidth - contributorBounds.X) / 2;
                totalHeight += contributorBounds.Y;
            }

            return totalHeight - verticalOffset;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(FontManager.Instance.BangersMedium, Title.Label, Title.Position, Color.Yellow);

            foreach (ScreenString contributor in Contributors)
            {
                spriteBatch.DrawString(FontManager.Instance.BangersSmall, contributor.Label, contributor.Position, Color.Yellow);
            }
        }

    }

    public class CreditsState : IGameState
    {
        private const float _spacerHeight = 20.0f;
        private const float _scrollSpeed = 50.0f;

        private List<CreditsSection> _credits = new List<CreditsSection>
        {
            new CreditsSection("Animation", new List<string> { "Roy Dorombozi"} ),
            new CreditsSection("Development", new List<string> { "Roy Dorombozi"} ),
            new CreditsSection("Models", new List<string> { "Clint Bellanger", "Roy Dorombozi"} ),
            new CreditsSection("Music", new List<string> { "Roden Santos"}),
            new CreditsSection("Skinning", new List<string> { "Roy Dorombozi"} ),
            new CreditsSection("Special Thanks", new List<string> { "Nancy Do", "James Leung"}),
        };

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;

        private float _verticalOffset;

        public CreditsState(GraphicsDevice graphicsDevice, SpriteBatch spriteBatch)
        {
            _spriteBatch = spriteBatch;
            _graphicsDevice = graphicsDevice;

            _verticalOffset = graphicsDevice.Viewport.Height;
        }

        private static CreditsState _instance;
        public static IGameState Instance
        {
            get { return _instance ?? (_instance =
                new CreditsState(
                    DancePartyGame.Instance.GraphicsDevice, 
                    DancePartyGame.Instance.SpriteBatch)); }
        }

        public void Update(GameTime gameTime)
        {
            Vector2? clickedPosition = PointerInputManager.Instance.GetClickedPosition();

            if (clickedPosition.HasValue)
            {
                GameStateManager.Instance.PopGameState();
                return;
            }

            float totalHeight = _verticalOffset;
            foreach (CreditsSection creditSection in _credits)
            {
                totalHeight += creditSection.UpdatePosition(totalHeight, _graphicsDevice.Viewport.Width);
                totalHeight += _spacerHeight;
            }

            if (totalHeight < 0)
                _verticalOffset = _graphicsDevice.Viewport.Height;

            _verticalOffset -= _scrollSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        }

        public void Draw(GameTime gameTime)
        {
            _graphicsDevice.Clear(Color.DarkSlateBlue);
            _spriteBatch.Begin();
            foreach (CreditsSection creditSection in _credits)
                creditSection.Draw(_spriteBatch);

            _spriteBatch.End();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
