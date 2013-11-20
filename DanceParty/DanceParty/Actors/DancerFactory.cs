using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        private Model _maleModel;
        private Model _femaleModel;

        private List<Model> _maleHairTypes;
        private List<Model> _femaleHairTypes;

        private List<Texture2D> _hairSkins;

        private List<Texture2D> _femaleSkins;
        private List<Texture2D> _maleSkins;

        private DancerFactory()
        {
            _femaleSkins = new List<Texture2D>();
            _maleSkins = new List<Texture2D>();

            _maleHairTypes = new List<Model>();
            _femaleHairTypes = new List<Model>();
            _hairSkins = new List<Texture2D>();
        }

        public void LoadContent(ContentManager contentManager)
        {
            _maleModel = contentManager.Load<Model>("Models\\male_low");
            _femaleModel = contentManager.Load<Model>("Models\\human_low_female");

            // Load hair types
            _maleHairTypes.Add(null);
            _maleHairTypes.Add(contentManager.Load<Model>("Models\\mohawk"));

            _femaleHairTypes.Add(contentManager.Load<Model>("Models\\longhair1"));

            // Load hair skins
            _hairSkins.Add(contentManager.Load<Texture2D>("Textures\\black"));
            _hairSkins.Add(contentManager.Load<Texture2D>("Textures\\brown"));
            _hairSkins.Add(contentManager.Load<Texture2D>("Textures\\blonde"));

            // Load all male skins
            _maleSkins.Add(contentManager.Load<Texture2D>("Textures\\male0"));
            
            // Load all female skins.
            _femaleSkins.Add(contentManager.Load<Texture2D>("Textures\\female1"));
        }

        private Random _random = new Random();
        public Dancer GetRandomDancer()
        {
            Model model;
            Texture2D skin;
            Accessory hair;

            if (_random.Next(2) == 1)
            {
                model = _maleModel;
                skin = _maleSkins[_random.Next(_maleSkins.Count)];
                hair = new Accessory() { 
                    AttachedToBone = "Head", 
                    Model = _maleHairTypes[_random.Next(_maleHairTypes.Count)],
                    Skin = _hairSkins[_random.Next(_hairSkins.Count)]
                };
            }
            else
            {
                model = _femaleModel;
                skin = _femaleSkins[_random.Next(_femaleSkins.Count)];
                hair = new Accessory() { 
                    AttachedToBone = "Head", 
                    Model = _femaleHairTypes[_random.Next(_femaleHairTypes.Count)], 
                    Skin = _hairSkins[_random.Next(_hairSkins.Count)] 
                };
            }

            // Add a random hair bound to the head.
            Dancer d =  new Dancer(model, skin);
            d.AddAccessory(hair);
            return d;
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
    }
}
