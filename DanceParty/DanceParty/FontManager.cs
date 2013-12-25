using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace DanceParty
{
    public class FontManager
    {
        public SpriteFont BangersSmall { get; private set; }
        public SpriteFont BangersMedium { get; private set; }
        public SpriteFont BangersLarge { get; private set; }

        private ContentManager _contentManager;

        private static FontManager _instance;
        public static FontManager Instance
        {
            get
            {
                return _instance ?? (_instance = new FontManager(DancePartyGame.Instance.Content));
            }
        }

        private FontManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void LoadContent()
        {
            BangersSmall = _contentManager.Load<SpriteFont>("Fonts\\BangersSmall");
            BangersSmall.Spacing = -4.0f;
            
            BangersMedium = _contentManager.Load<SpriteFont>("Fonts\\BangersMedium");
            BangersMedium.Spacing = -6.0f;

            BangersLarge = _contentManager.Load<SpriteFont>("Fonts\\BangersLarge");
            BangersLarge.Spacing = -8.0f;
        }
    }
}
