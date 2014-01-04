using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SkinnedModel;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DanceParty.Utilities
{
    public class AnimationManager
    {
        private SkinningData _animationData;

        private static AnimationManager _instance;
        public static AnimationManager Instance
        {
            get
            {
                return _instance ?? (_instance = new AnimationManager(DancePartyGame.Instance.Content));
            }
        }

        private ContentManager _contentManager;

        private AnimationManager(ContentManager contentManager)
        {
            _contentManager = contentManager;
        }

        public void LoadContent()
        {
            Model animatedModel = _contentManager.Load<Model>("Animation\\Base");
            _animationData = (SkinningData) animatedModel.Tag;
        }

        public SkinningData GetAnimationData()
        {
            return _animationData;
        }
    }
}
