using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using DanceParty.Actors;
using DanceParty.Cameras;
using DanceParty.Utilities;

namespace DanceParty
{
    public class BatchedAnimatedModelContainer
    {
        public BatchRenderedAnimatedModel BatchedModel;
        public List<AnimatedModelInstance> ActiveInstances;
        public Texture2D Texture;

        public BatchedAnimatedModelContainer(BatchRenderedAnimatedModel batchedModel, Texture2D texture)
        {
            ActiveInstances = new List<AnimatedModelInstance>();
            BatchedModel = batchedModel;
            Texture = texture;
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
            // This isn't so good, I want to reuse the same batchrenderedanimatedmodel class, with different textures

            Dictionary<string, BatchRenderedAnimatedModel> modelMap = new Dictionary<string, BatchRenderedAnimatedModel>();
            foreach (DancerTypeData dancerData in DancerTypes.Dancers)
            {

                BatchRenderedAnimatedModel animatedModel;

                if (!modelMap.TryGetValue(dancerData.ModelAssetPath, out animatedModel))
                {
                    animatedModel = new BatchRenderedAnimatedModel(
                        graphicsDevice,
                        contentManager.Load<Model>(dancerData.ModelAssetPath));

                    modelMap[dancerData.ModelAssetPath] = animatedModel;
                }
                
                BatchedAnimatedModelContainer animatedContainer =
                    new BatchedAnimatedModelContainer(
                        animatedModel,
                        contentManager.Load<Texture2D>(dancerData.TextureAssetPath));

                _batchedAnimatedModelContainers.Add(animatedContainer);
            }
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
