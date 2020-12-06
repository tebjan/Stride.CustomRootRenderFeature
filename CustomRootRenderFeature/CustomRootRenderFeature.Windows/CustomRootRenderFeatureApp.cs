using Stride.Engine;

namespace CustomRootRenderFeature
{
    internal static class CustomRootRenderFeatureApp
    {
        private static void Main()
        {
            using (var game = new Game())
            {
                game.Run();
            }
        }
    }
}
