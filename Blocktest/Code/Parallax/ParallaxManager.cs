using Blocktest.Rendering;
using Shared.Code;
using Shared.Code.Components;
using System.Diagnostics;
namespace Blocktest.Parallax;

public sealed class ParallaxManager
{
    public readonly Dictionary<string,ParallaxLayer> ParallaxLayers;
    private readonly Camera _camera;

    public ParallaxManager(Camera camera)
    {
        _camera = camera;
        ParallaxLayers = new Dictionary<string, ParallaxLayer>();

        Default();
    }

    public void AddParallaxLayer(ParallaxLayer parallaxLayer)
    {

    }

    public void RemoveParallaxLayer(ParallaxLayer parallaxLayer)
    {

    }

    public void GetParallaxLayer(string parallaxLayerName)
    {

    }

    public void Default()
    {
        ParallaxLayer layer = new("duskwood_trees", new Vector2Int(0, -250), new Vector2(0.3f, 0.3f), _camera);
        ParallaxLayers.Add("trees", layer);

        layer = new("duskwood_trees2", new Vector2Int(0, -170), new Vector2(0.7f, 0.7f), _camera);
        ParallaxLayers.Add("trees2", layer);

        layer = new("prometheus_towers", new Vector2Int(-20, 200), new Vector2(1.0f, 1.0f), _camera);
        ParallaxLayers.Add("towers", layer);

        layer = new("prometheus_city", new Vector2Int(0, 100), new Vector2(1.2f, 1.1f), _camera);
        ParallaxLayers.Add("city", layer);

        Debug.WriteLine("Default ParallaxManager Initialized");
    }

    public void Draw()
    {
        if (ParallaxLayers.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, ParallaxLayer> parallax in ParallaxLayers)
        {
            parallax.Value.Draw();
        }
    }
}