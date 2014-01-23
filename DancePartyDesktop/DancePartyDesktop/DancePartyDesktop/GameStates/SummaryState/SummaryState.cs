using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DanceParty.Actors;
using Microsoft.Xna.Framework;

namespace DanceParty.GameStates
{
    public class SummaryState : IGameState
    {
        private CongaLine _line;
        private ScoreComponent _score;

        public SummaryState(CongaLine line, ScoreComponent score)
        {
            _line = line;
            _score = score;
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
