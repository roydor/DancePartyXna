using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using DanceParty.Cameras;
using DanceParty.Utilities;

namespace DanceParty
{
    public enum DancerType
    {
        Male,
        Female,
    }

    public class BatchedAnimatedModelContainer
    {
        public BatchRenderedAnimatedModel BatchedModel;
        public List<AnimatedModelInstance> ActiveInstances;
        public Texture2D Texture;
        public DancerType DancerType;

        public BatchedAnimatedModelContainer(BatchRenderedAnimatedModel batchedModel, Texture2D texture, DancerType dancerType)
        {
            ActiveInstances = new List<AnimatedModelInstance>();
            BatchedModel = batchedModel;
            Texture = texture;
            DancerType = dancerType;
        }

        public void DrawInstances(PerspectiveCamera camera)
        {
            BatchedModel.DrawInstances(camera, ActiveInstances, Texture);
        }
    }

    public class BatchedModelContainer
    {
        public InstancedModel BatchedModel;
        public List<AnimatedModelInstance> ActiveInstances;
        public Texture2D Texture;
    }

    public class BatchedModelManager
    {
        private static BatchedModelManager _instance;
        public static BatchedModelManager Instance
        {
            get
            {
                return _instance ?? (_instance = new BatchedModelManager());
            }
        }

        List<BatchedAnimatedModelContainer> _batchedAnimatedModelContainers;
        List<BatchedModelContainer> _batchedModelContainers;

        private BatchedModelManager()
        {
            _batchedAnimatedModelContainers = new List<BatchedAnimatedModelContainer>();
            _batchedModelContainers = new List<BatchedModelContainer>();
        }

        public void LoadContent(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {   
            //// Create the animated models and process them.
            BatchRenderedAnimatedModel animatedMaleModel =
                new BatchRenderedAnimatedModel(
                    graphicsDevice,
                    contentManager.Load<Model>("Models\\male_low"));

            //BatchRenderedAnimatedModel animatedFemaleMOdel =
            //    new BatchRenderedAnimatedModel(
            //        graphicsDevice,
            //        contentManager.Load<Model>("Models\\female_low"));

            BatchRenderedAnimatedModel animatedBobDress =
                new BatchRenderedAnimatedModel(
                    graphicsDevice,
                    contentManager.Load<Model>("Models\\female1"));

            BatchRenderedAnimatedModel animatedFemale2 =
                new BatchRenderedAnimatedModel(
                    graphicsDevice,
                    contentManager.Load<Model>("Models\\female2"));

            // Create instance containers to group skins with the models.
            BatchedAnimatedModelContainer maleContainer =
                new BatchedAnimatedModelContainer(
                    animatedMaleModel,
                    contentManager.Load<Texture2D>("Textures\\male_0"),
                    DancerType.Male);

            //BatchedAnimatedModelContainer femaleContainer =
            //    new BatchedAnimatedModelContainer(
            //        animatedFemaleMOdel,
            //        contentManager.Load<Texture2D>("Textures\\female_0"),
            //        DancerType.Female);

            BatchedAnimatedModelContainer bobContainer =
             new BatchedAnimatedModelContainer(
                 animatedBobDress,
                 contentManager.Load<Texture2D>("Textures\\female1_0"),
                 DancerType.Female);

            BatchedAnimatedModelContainer animatedFemale2Container =
                new BatchedAnimatedModelContainer(
                    animatedFemale2,
                    contentManager.Load<Texture2D>("Textures\\female2_0"),
                    DancerType.Female);

            _batchedAnimatedModelContainers.Add(maleContainer);
            //_batchedAnimatedModelContainers.Add(femaleContainer);
            _batchedAnimatedModelContainers.Add(bobContainer);
            _batchedAnimatedModelContainers.Add(animatedFemale2Container);


        }

        public AnimatedModelInstance GetAnimatedModelInstance()
        {
            BatchedAnimatedModelContainer container;

            container = _batchedAnimatedModelContainers[RandomHelper.GetRandomInt(0, _batchedAnimatedModelContainers.Count)];

            AnimatedModelInstance instance = container.BatchedModel.GetInstance();
            container.ActiveInstances.Add(instance);
            return instance;
        }

        public void DrawInstances(PerspectiveCamera camera)
        {
            foreach (BatchedAnimatedModelContainer modelContainer in _batchedAnimatedModelContainers)
                modelContainer.DrawInstances(camera);
        }

        public void ClearInstances()
        {
            foreach (BatchedAnimatedModelContainer modelContainer in _batchedAnimatedModelContainers)
                modelContainer.ActiveInstances.Clear();
        }

    }
}
