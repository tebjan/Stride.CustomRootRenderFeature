// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using System;
using Stride.Core.Annotations;
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;
using Stride.Rendering.Skyboxes;
using Stride.Streaming;

namespace CustomRootRenderFeature
{
    /// <summary>
    /// This class will show up in the graphics compositor in the render feature list.
    /// </summary>
    /// <seealso cref="Stride.Rendering.RootRenderFeature" />
    public class MyRootRenderFeature : RootRenderFeature
    {
        private MutablePipelineState pipelineState;
        private DynamicEffectInstance myCustomShader;

        public override Type SupportedRenderObjectType => typeof(MyRenderObject);

        public MyRootRenderFeature()
        {
            //pre adjust render priority, low numer is early, high number is late
            SortKey = 0;
        }

        protected override void InitializeCore()
        {
            // Initalize the shader
            myCustomShader = new DynamicEffectInstance("MyCustomShader");
            myCustomShader.Initialize(Context.Services);

            // Create the pipeline state and set properties that won't change
            pipelineState = new MutablePipelineState(Context.GraphicsDevice);
            pipelineState.State.SetDefaults();
            pipelineState.State.InputElements = MyRenderObject.VertexDeclaration.CreateInputElements();
            pipelineState.State.PrimitiveType = MyRenderObject.PrimitiveType;
            pipelineState.State.BlendState = BlendStates.Default;
            pipelineState.State.RasterizerState.CullMode = CullMode.None;
        }

        public override void Prepare(RenderDrawContext context)
        {
            base.Prepare(context);

            // Register resources usage
            foreach (var renderObject in RenderObjects)
            {
                var myRenderObject = (MyRenderObject)renderObject;
                Context.StreamingManager?.StreamResources(myRenderObject.Texture, StreamingOptions.LoadAtOnce);
                myRenderObject.Prepare(context.GraphicsDevice);
            }
        }

        public override void Draw(RenderDrawContext context, RenderView renderView, RenderViewStage renderViewStage, int startIndex, int endIndex)
        {
            // First do everything that doesn't change per individual render object
            var graphicsDevice = context.GraphicsDevice;
            var graphicsContext = context.GraphicsContext;
            var commandList = context.GraphicsContext.CommandList;

            // Refresh shader, might have changed during runtime
            myCustomShader.UpdateEffect(graphicsDevice);

            // Set common shader parameters if needed
            //myCustomShader.Parameters.Set(TransformationKeys.ViewProjection, renderView.ViewProjection);

            for (int index = startIndex; index < endIndex; index++)
            {
                var renderNodeReference = renderViewStage.SortedRenderNodes[index].RenderNode;
                var renderNode = GetRenderNode(renderNodeReference);
                var myRenderObject = (MyRenderObject)renderNode.RenderObject;

                if (myRenderObject.VertexBuffer == null)
                    continue; //next render object

                // Assign shader parameters
                myCustomShader.Parameters.Set(TransformationKeys.WorldViewProjection, myRenderObject.WorldMatrix * renderView.ViewProjection);
                myCustomShader.Parameters.Set(TexturingKeys.Texture0, myRenderObject.Texture);
                myCustomShader.Parameters.Set(MyCustomShaderKeys.TextureScale, myRenderObject.TextureScale);
                myCustomShader.Parameters.Set(MyCustomShaderKeys.Color, myRenderObject.Color);

                // Prepare pipeline state
                pipelineState.State.RootSignature = myCustomShader.RootSignature;
                pipelineState.State.EffectBytecode = myCustomShader.Effect.Bytecode;
                pipelineState.State.Output.CaptureState(commandList);
                pipelineState.Update();
                commandList.SetPipelineState(pipelineState.CurrentState);

                // Apply the effect
                myCustomShader.Apply(graphicsContext);

                // Set vertex buffer and draw
                commandList.SetVertexBuffer(0, myRenderObject.VertexBuffer, 0, MyRenderObject.VertexDeclaration.VertexStride);
                commandList.Draw(MyRenderObject.VertexCount, 0);
            }
        }
    }
}
