// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.
using Stride.Core.Mathematics;
using Stride.Graphics;
using Stride.Rendering;

namespace CustomRootRenderFeature
{
    public class MyRenderObject : RenderObject
    {
        // Shader properties
        public Color4 Color = Color4.White;
        public Texture Texture;
        public float TextureScale = 1;
        public Matrix WorldMatrix = Matrix.Identity;

        // Vertex buffer setup
        public const int VertexCount = 4;
        public static VertexDeclaration VertexDeclaration = VertexPositionNormalTexture.Layout;
        public static PrimitiveType PrimitiveType = PrimitiveType.TriangleStrip;
        public Buffer VertexBuffer;

        public void Prepare(GraphicsDevice graphicsDevice)
        {
            if (VertexBuffer != null)
                return;

            var normal = new Vector3(0, 0, 1);
            VertexBuffer = Buffer.New(graphicsDevice, new[]
                {
                    new VertexPositionNormalTexture(new Vector3(-0.5f,  0.5f, 0), normal, new Vector2(0, 0)),
                    new VertexPositionNormalTexture(new Vector3( 0.5f,  0.5f, 0), normal, new Vector2(1, 0)),
                    new VertexPositionNormalTexture(new Vector3(-0.5f, -0.5f, 0), normal, new Vector2(0, 1)),
                    new VertexPositionNormalTexture(new Vector3( 0.5f, -0.5f, 0), normal, new Vector2(1, 1)),
                }, BufferFlags.VertexBuffer, GraphicsResourceUsage.Immutable);
        }

        private float phase;
        internal void Update()
        {
            phase += 0.1f;
            phase %= 5;
            TextureScale *= phase;
        }
    }
}
