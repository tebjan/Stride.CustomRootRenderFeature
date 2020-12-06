using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Stride.Core.Mathematics;
using Stride.Input;
using Stride.Engine;
using Stride.Graphics;
using Stride.Rendering;

namespace CustomRootRenderFeature
{
    public class MyRenderObjectManagerScript : SyncScript
    {
        public int CountX { get; set; } = 10;
        public int CountY { get; set; } = 10;

        private List<MyRenderObject> myRenderObjects;
        private VisibilityGroup VisibilityGroup;

        private float CurrentTime => (float)Game.UpdateTime.Total.TotalSeconds;

        public override void Start()
        {
            // Initialization of the script.
        }

        private int lastCountX;
        private int lastCountY;

        public override void Update()
        {
            // Wait until visibility group is available
            if(VisibilityGroup == null)
                VisibilityGroup = SceneSystem.SceneInstance.VisibilityGroups.FirstOrDefault();

            if (VisibilityGroup == null)
                return;

            // Check whether counts have changed
            if (lastCountX != CountX || lastCountY != CountY)
            {
                RebuildRenderObjects();
            }

            // Update render objects
            UpdateRenderObjects();

            lastCountX = CountX;
            lastCountY = CountY;
        }

        private void UpdateRenderObjects()
        {
            var k = 0;
            for (int i = 0; i < CountX; i++)
            {
                for (int j = 0; j < CountY; j++)
                {
                    var myRenderObject = myRenderObjects[k++];
                    myRenderObject.WorldMatrix.TranslationVector = CalcPosition(i, j);
                    myRenderObject.TextureScale = myRenderObject.WorldMatrix.TranslationVector.Z * 5;
                }
            }
        }

        private void RebuildRenderObjects()
        {
            // Remove all old render objects
            if (myRenderObjects != null)
            {
                foreach (var myRenderObject in myRenderObjects)
                {
                    VisibilityGroup.RenderObjects.Remove(myRenderObject);
                    myRenderObject.VertexBuffer?.Dispose();
                }
            }

            // Create new render objects
            var newRenderObjects = new List<MyRenderObject>();
            for (int i = 0; i < CountX; i++)
            {
                for (int j = 0; j < CountY; j++)
                {
                    var myRenderObject = new MyRenderObject();
                    myRenderObject.Prepare(GraphicsDevice);
                    myRenderObject.Color = Color.Red;
                    myRenderObject.Texture = Content.Load<Texture>("Stride_Logo");
                    myRenderObject.WorldMatrix = Matrix.Scaling(1.5f / CountX, 1.5f / CountY, 1);
                    myRenderObject.WorldMatrix.TranslationVector = CalcPosition(i, j);
                    newRenderObjects.Add(myRenderObject);
                    VisibilityGroup.RenderObjects.Add(myRenderObject);
                }
            }

            myRenderObjects = newRenderObjects;
        }

        private Vector3 CalcPosition(int i, int j)
        {
            var x = (i * 2.0f / CountX) + 2.5f;
            var y = (j * 2.0f / CountY) + 0.1f;
            var z = (float)Math.Cos(CurrentTime + x + y) * 0.5f;
            return new Vector3(x, y, z);
        }
    }
}
