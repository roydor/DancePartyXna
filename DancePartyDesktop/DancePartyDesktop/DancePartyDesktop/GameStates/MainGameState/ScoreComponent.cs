using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace DanceParty.GameStates
{
    public class ScoreComponent
    {
        private float _currentScore;
        private float _totalScore;

        public int Score
        {
            get
            {
                return (int)Math.Round(_currentScore);
            }
        }

        public void Update(GameTime gameTime)
        {
            _currentScore = MathHelper.Lerp(_currentScore, _totalScore, 5 * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public void FinalizeScore()
        {
            _currentScore = _totalScore;
        }

        public void AddPoints(int points)
        {
            _totalScore += points;
        }

    }
}
