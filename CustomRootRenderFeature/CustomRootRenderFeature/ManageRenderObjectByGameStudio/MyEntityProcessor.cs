// Copyright (c) Xenko contributors (https://xenko.com) and Silicon Studio Corp. (https://www.siliconstudio.co.jp)
// Distributed under the MIT license. See the LICENSE.md file in the project root for more information.

using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Rendering;

namespace CustomRootRenderFeature
{
    /// <summary>
    /// A default entity processor for <see cref="MyEntityComponent"/>.
    /// This class assigns parameters from the entity component (in th Game Studio UI) to the render object.
    /// It takes care of adding and removing the appropriate render objects to the visibility group.
    /// </summary>
    public class MyEntityProcessor : EntityProcessor<MyEntityComponent, MyRenderObject>, IEntityComponentRenderProcessor
    {
        public VisibilityGroup VisibilityGroup { get; set; }

        /// <summary>
        /// Gets the active render object.
        /// </summary>
        /// <value>The active render object.</value>
        public MyRenderObject MyRenderObject { get; private set; }

        /// <inheritdoc />
        protected override void OnSystemRemove()
        {
            if (MyRenderObject != null)
            {
                VisibilityGroup.RenderObjects.Remove(MyRenderObject);
                MyRenderObject.VertexBuffer?.Dispose();
                MyRenderObject = null;
            }

            base.OnSystemRemove();
        }

        /// <inheritdoc />
        protected override MyRenderObject GenerateComponentData(Entity entity, MyEntityComponent component)
        {
            return new MyRenderObject { RenderGroup = component.RenderGroup };
        }

        /// <inheritdoc />
        protected override bool IsAssociatedDataValid(Entity entity, MyEntityComponent component, MyRenderObject associatedData)
        {
            return component.RenderGroup == associatedData.RenderGroup;
        }

        /// <inheritdoc />
        public override void Draw(RenderContext context)
        {
            var previousRenderObject = MyRenderObject;
            MyRenderObject = null;

            // Go thru components of this entity
            foreach (var entityKeyPair in ComponentDatas)
            {
                var myEntityComponent = entityKeyPair.Key;
                var myRenderObject = entityKeyPair.Value;
                if (myEntityComponent.Enabled)
                {
                    // Select the first enabled component and assign data from UI
                    myRenderObject.Color = myEntityComponent.Color;
                    myRenderObject.Texture = myEntityComponent.Texture;
                    myRenderObject.TextureScale = myEntityComponent.TextureScale;
                    myRenderObject.RenderGroup = myEntityComponent.RenderGroup;
                    myRenderObject.WorldMatrix = myEntityComponent.Entity.Transform.WorldMatrix;
                    myRenderObject.Update();
                    MyRenderObject = myRenderObject;
                    break;
                }
            }

            if (MyRenderObject != previousRenderObject)
            {
                if (previousRenderObject != null)
                    VisibilityGroup.RenderObjects.Remove(previousRenderObject);
                if (MyRenderObject != null)
                    VisibilityGroup.RenderObjects.Add(MyRenderObject);
            }
        }
    }
}
