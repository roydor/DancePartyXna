using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using DanceParty.Utilities;
using DanceParty.Cameras;

namespace DanceParty.Actors
{
    public class DancerFactory
    {
        private static DancerFactory _instance;
        public static DancerFactory Instance
        {
            get 
            {
                return _instance ?? (_instance = new DancerFactory());
            }
        }

        private DancerFactory()
        {

        }

        private Random _random = new Random();
        public Dancer GetRandomDancer()
        {
            // Rewrite this using the batchedmodel manager.
            return new Dancer(BatchedModelManager.Instance.GetAnimatedModelInstance());
        }
    }

    /// <summary>
    /// A class describing a wearable accessory for a dancer.
    /// </summary>
    public class Accessory
    {
        /// <summary>
        /// The name of the bone to bind to
        /// </summary>
        public string AttachedToBone;

        /// <summary>
        /// The model to draw.
        /// </summary>
        public Model Model;

        /// <summary>
        /// The skin of that model
        /// </summary>
        public Texture2D Skin;

        /// <summary>
        /// The transform for this Matrix.
        /// </summary>
        public Matrix Transform;
    }

    public enum AccessoryType
    {
        LongBlackHair,
        LongBrownHair,
    }
}
