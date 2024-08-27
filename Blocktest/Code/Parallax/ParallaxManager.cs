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
        ParallaxLayer layer = new("duskwood_trees", new Vector2Int(0, -200), 3 * Vector2.One, 1, _camera);
        ParallaxLayers.Add("trees", layer);

        layer = new("duskwood_trees2", new Vector2Int(0, -120), 2 * Vector2.One, 2, _camera);
        ParallaxLayers.Add("trees2", layer);

        Debug.WriteLine("Default ParallaxManager Initialized");

        LayerManager();
    }

    private void LayerManager() // clean up code so this horseshit doesn't have to exist
    {
        float maxZLevel = 0;

        if (ParallaxLayers.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, ParallaxLayer> parallax in ParallaxLayers)
        {
            Debug.WriteLine(parallax.Value.ZLevel);
            if (parallax.Value.ZLevel > maxZLevel)
            {
                maxZLevel = parallax.Value.ZLevel;
            }
        }

        foreach (KeyValuePair<string, ParallaxLayer> parallax in ParallaxLayers)
        {
            Debug.WriteLine(parallax.Value.ZLevel);
            parallax.Value.Initialize((float)Layer.Parallax + parallax.Value.ZLevel / maxZLevel);
        }
    }

    public void Draw()
    {
        if (ParallaxLayers.Count == 0)
        {
            return;
        }

        foreach (KeyValuePair<string, ParallaxLayer> parallax in ParallaxLayers)
        {
            //Debug.WriteLine(parallax.Value);
            parallax.Value.Draw();
        }
    }
}