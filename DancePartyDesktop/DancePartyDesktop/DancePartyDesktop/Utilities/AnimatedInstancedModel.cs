using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using DanceParty.Actors;
using DanceParty.Cameras;

namespace DanceParty.Utilities
{
    public class AnimatedModelInstance
    {
        public Matrix Transform;
        public Matrix[] SkinTransforms;
        public Model OriginalModel;

        public AnimatedModelInstance(Model originalModel)
        {
            OriginalModel = originalModel;
        }
    }

    /// <summary>
    /// A class which does a lot of crazy stuff to shove a few copies of the model into 
    /// The GPU so we can draw a few at a time rather than one draw call each.
    /// 
    /// To use this function you need:
    ///   A model who has a SkinnedEffect on the model.
    ///   A model with only 1 mesh.
    ///   All vertices dependant on 3 bones at most.  (where bone4 is never used).
    ///   We'll draw floor(72 / (n+1)) models per draw call.
    ///   In DanceParty (Models have 20 bones, so this gets us 3 models / draw)
    /// </summary>
    public class BatchRenderedAnimatedModel
    {
        const int maxShaderMatrices = SkinnedEffect.MaxBones;

        /// <summary>
        /// There are 4 bone weights per vertex.
        /// We want to know the index into the vertex byte array from the start of the bone indices
        /// for bone 4.
        /// </summary>
        private const int BoneWeight4Offset = (4 - 1) * sizeof(float);

        private VertexBuffer _instancedVertexBuffer;
        private IndexBuffer _instancedIndexBuffer;

        private int _originalVertexCount = 0;
        private int _originalIndexCount = 0;

        private int _vertexStride = 0;
        private int _maxInstances = 0;
        
        // Maybe we can share this amongst ALL the instanced
        Matrix[] tempMatrices = new Matrix[maxShaderMatrices];

        private Model _originalModel;
        private int _originalBoneCount;

        /// <summary>
        /// The device to render to.
        /// </summary>
        private GraphicsDevice _graphicsDevice;

        /// <summary>
        /// The effect extracted from the model which we'll use to render the instances.
        /// </summary>
        private SkinnedEffect _skinnedEffect;

        public BatchRenderedAnimatedModel(GraphicsDevice graphics, Model model)
        {
            _graphicsDevice = graphics;
            _originalModel = model;
            _originalBoneCount = model.Bones.Count;

            _skinnedEffect = new SkinnedEffect(_graphicsDevice);
            _skinnedEffect.EnableDefaultLighting();
            _skinnedEffect.AmbientLightColor = Color.Gray.ToVector3();
            _skinnedEffect.SpecularColor = Color.Gray.ToVector3();
            _skinnedEffect.PreferPerPixelLighting = true;

            SetupInstancedVertexData();
        }

        public AnimatedModelInstance GetInstance()
        {
            AnimatedModelInstance newInstance = new AnimatedModelInstance(_originalModel);
            return newInstance;
        }

        /// <summary>
        /// Do lots of crazy stuff to create the uber model.
        /// </summary>
        private void SetupInstancedVertexData()
        {
            // Read the existing vertex data, then destroy the existing vertex buffer.
            ModelMesh mesh = _originalModel.Meshes[0];
            ModelMeshPart part = mesh.MeshParts[0];

            _originalVertexCount = part.VertexBuffer.VertexCount;
            VertexDeclaration originalVertexDeclaration = part.VertexBuffer.VertexDeclaration;

            // Gather the information from the original vertices and mark the offsets for
            // where bone weights and indices are defined.
            VertexElement[] originalVertexElements = originalVertexDeclaration.GetVertexElements();

            int blendIndicesOffset = -1;
            int blendWeightOffset = -1;
            for (int vertexElementIndex = 0; vertexElementIndex < originalVertexElements.Length; vertexElementIndex++)
            {
                if ((originalVertexElements[vertexElementIndex].VertexElementUsage == VertexElementUsage.BlendIndices))
                    blendIndicesOffset = originalVertexElements[vertexElementIndex].Offset;

                if ((originalVertexElements[vertexElementIndex].VertexElementUsage == VertexElementUsage.BlendWeight))
                    blendWeightOffset = originalVertexElements[vertexElementIndex].Offset;
            }

            if (blendIndicesOffset < 0 || blendWeightOffset < 0)
                throw new InvalidOperationException("Vertices of the model don't suggest its animated.");

            int indexOverflowLimit = ushort.MaxValue / _originalVertexCount;
            
            // Each model has n bones, so we can only fit max bones / bones per model instances per draw
            _maxInstances = Math.Min(indexOverflowLimit, maxShaderMatrices / _originalBoneCount);

            // Get the number of bytes per vertex.
            _vertexStride = part.VertexBuffer.VertexDeclaration.VertexStride;

            int newVertexCount = _originalVertexCount * _maxInstances;
            int newVertexBufferSize = newVertexCount * _vertexStride;

            // Vertex and Index buffers are cap'ed at 64MB
            if (newVertexBufferSize > 0x3FFFFFF)
                throw new InvalidOperationException(
                    String.Format(
                       "Attempted to create a vertex buffer of size {0} bytes.\r\n" +
                       "Maximum vertex or index buffer size is 67108863 bytes (64MB)",
                           newVertexBufferSize / 0x1000000));

            byte[] oldVertexData = new byte[_originalVertexCount * _vertexStride];
            part.VertexBuffer.GetData(oldVertexData);

            // Allocate a temporary array to hold the replicated vertex data.
            byte[] newVertexData = new byte[_originalVertexCount * _vertexStride * _maxInstances];

            // Keep track of the byte we're at in our output buffer.
            int outputPosition = 0;

            // Replicate one copy of the original vertex buffer for each instance.
            for (int instanceIndex = 0; instanceIndex < _maxInstances; instanceIndex++)
            {
                // Reset at the beginning of our source vertices
                int sourcePosition = 0;
                byte boneOffset = (byte) (instanceIndex * _originalModel.Bones.Count);

                for (int i = 0; i < _originalVertexCount; i++)
                {
                    // Copy over the existing data for this vertex.
                    Array.Copy(oldVertexData, sourcePosition,
                               newVertexData, outputPosition, _vertexStride);

                    // Copy over the bone indices from the original vertex to the new vertex
                    newVertexData[outputPosition + blendIndicesOffset + 0] = (byte)(oldVertexData[sourcePosition + blendIndicesOffset + 0] + boneOffset);
                    newVertexData[outputPosition + blendIndicesOffset + 1] = (byte)(oldVertexData[sourcePosition + blendIndicesOffset + 1] + boneOffset);
                    newVertexData[outputPosition + blendIndicesOffset + 2] = (byte)(oldVertexData[sourcePosition + blendIndicesOffset + 2] + boneOffset);

                    // Set bone 4 to be special for the transformation matrix
                    newVertexData[outputPosition + blendIndicesOffset + 3] = (byte)(boneOffset + _originalModel.Bones.Count - 1);

                    // Set the weight on bone4 to be 100%
                    int bone4offset = BoneWeight4Offset + blendWeightOffset;
                    byte[] bone4weight = BitConverter.GetBytes(1.0f);

                    float weight0 = BitConverter.ToSingle(oldVertexData, blendWeightOffset+0);
                    float weight1 = BitConverter.ToSingle(oldVertexData, blendWeightOffset+4);
                    float weight2 = BitConverter.ToSingle(oldVertexData, blendWeightOffset+8);
                    float weight3 = BitConverter.ToSingle(oldVertexData, blendWeightOffset+12);

                    // 16-20-24-28
                    newVertexData[outputPosition + bone4offset + 0] = bone4weight[0];
                    newVertexData[outputPosition + bone4offset + 1] = bone4weight[1];
                    newVertexData[outputPosition + bone4offset + 2] = bone4weight[2];
                    newVertexData[outputPosition + bone4offset + 3] = bone4weight[3];

                    // Move the positions ahead by the number of bytes per vertex.
                    outputPosition += _vertexStride;
                    sourcePosition += _vertexStride;
                }
            }

            // Create a new vertex buffer, and set the replicated data into it.
            // The format of our vertices hasn't changed, so we can use the original vertex declaration.
            _instancedVertexBuffer = new VertexBuffer(_graphicsDevice, 
                originalVertexDeclaration, 
                newVertexCount, 
                BufferUsage.None);

            _instancedVertexBuffer.SetData(newVertexData);

            //handle vertex indices
            _originalIndexCount = part.IndexBuffer.IndexCount;

            ushort[] oldIndices = new ushort[_originalIndexCount];
            part.IndexBuffer.GetData(oldIndices);

            // Allocate a temporary array to hold the replicated index data.
            ushort[] newIndices = new ushort[_originalIndexCount * _maxInstances];
            outputPosition = 0;

            // Replicate one copy of the original index buffer for each instance.
            for (int instanceIndex = 0; instanceIndex < _maxInstances; instanceIndex++)
            {
                int instanceOffset = instanceIndex * _originalVertexCount;

                for (int i = 0; i < part.IndexBuffer.IndexCount; i++)
                {
                    newIndices[outputPosition] = (ushort)(oldIndices[i] + instanceOffset);
                    outputPosition++;
                }
            }

            // Create a new index buffer, and set the replicated data into it.
            _instancedIndexBuffer = 
                new IndexBuffer(
                    _graphicsDevice, 
                    IndexElementSize.SixteenBits, 
                    newIndices.Length, 
                    BufferUsage.None);

            _instancedIndexBuffer.SetData(newIndices);
        }

        /// <summary>
        /// Draws all the instances of this model.
        /// </summary>
        /// <param name="camera"></param>
        public void DrawInstances(PerspectiveCamera camera, List<AnimatedModelInstance> activeInstances, Texture2D texture)
        {
            int numberOfInstances = activeInstances.Count;
            Matrix[] transforms = new Matrix[numberOfInstances];
            Matrix[][] boneTransforms = new Matrix[numberOfInstances][];

            for (int i = 0; i < activeInstances.Count; i++)
            {
                AnimatedModelInstance modelInstance = activeInstances[i];
                Utilities.CopyMatrix(ref modelInstance.Transform, ref transforms[i]);
                boneTransforms[i] = new Matrix[modelInstance.SkinTransforms.Length];

                for(int bone = 0; bone < modelInstance.SkinTransforms.Length; bone++)
                    Utilities.CopyMatrix(ref modelInstance.SkinTransforms[bone], ref boneTransforms[i][bone]);
            }
            Draw(transforms, boneTransforms, numberOfInstances, camera, texture);
        }

        /// <summary>
        /// Draws all the instances specified
        /// </summary>
        private void Draw(
            Matrix[] transformMatrices, 
            Matrix[][] boneMatrices, 
            int totalInstances, 
            PerspectiveCamera camera,
            Texture2D skin)
        {
            _skinnedEffect.Texture = skin;
            _skinnedEffect.View = camera.ViewMatrix;
            _skinnedEffect.Projection = camera.ProjectionMatrix;

            _graphicsDevice.SetVertexBuffer(_instancedVertexBuffer);
            _graphicsDevice.Indices = _instancedIndexBuffer;

            for (int i = 0; i < totalInstances; i += _maxInstances)
            {
                // How many instances can we fit into this batch?
                int instanceCount = totalInstances - i;

                if (instanceCount > _maxInstances)
                    instanceCount = _maxInstances;

                for (int batchedInstanceIndex = 0; batchedInstanceIndex < instanceCount; batchedInstanceIndex++)
                {
                    int currentInstance = batchedInstanceIndex + i;

                    for (int boneIndex = 0; boneIndex < boneMatrices[currentInstance].Length; boneIndex++)
                        Utilities.CopyMatrix(ref boneMatrices[currentInstance][boneIndex], ref tempMatrices[batchedInstanceIndex * _originalModel.Bones.Count + boneIndex]);

                    Utilities.CopyMatrix(
                        ref transformMatrices[currentInstance] /*source*/,
                        ref tempMatrices[batchedInstanceIndex * _originalModel.Bones.Count + _originalModel.Bones.Count - 1] /*dest*/);
                }

                // Set the especially bones into the effect
                _skinnedEffect.SetBoneTransforms(tempMatrices);

                foreach (EffectPass pass in _skinnedEffect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    _graphicsDevice.DrawIndexedPrimitives(
                        PrimitiveType.TriangleList,
                        0,
                        0,
                        instanceCount * _originalVertexCount,
                        0,
                        instanceCount * _originalIndexCount / 3);

                    DancePartyGame.DrawsPerFrame++;
                }
            }
        }
    }
}

