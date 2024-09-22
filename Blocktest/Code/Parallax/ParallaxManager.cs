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
        ParallaxLayers = [];

        //Test(); // Initialize parallax here
        Prometheus();
    }

    public void AddParallaxLayer(ParallaxLayer parallaxLayer, string? parallaxLayerName = null)
    {
        if (parallaxLayerName != null)
        {
            ParallaxLayers.Add(parallaxLayerName, parallaxLayer);
        }
        else
        {
            parallaxLayerName = parallaxLayer.Image.Texture.Name.Split(@"\")[^1];

            int i = 1;
            while ((i - 1) < ParallaxLayers.Count+1)
            {
                if (ParallaxLayers.TryAdd(parallaxLayerName, parallaxLayer))
                {
                    break;
                }

                if (parallaxLayerName.EndsWith(@"_" + (i - 1).ToString())) // )(@"_")[^1] == (i-1).ToString())
                {
                    parallaxLayerName = parallaxLayerName.Substring(0, parallaxLayerName.LastIndexOf(@"_" + (i - 1).ToString())) + @"_" + i.ToString();
                }
                else
                {
                    parallaxLayerName = parallaxLayerName + @"_" + i.ToString();
                }
                i += 1;
            }
        }

        Debug.WriteLine("Successfully added " + parallaxLayerName + " to ParallaxLayers");
    }

    public void AddParallaxLayers(List<ParallaxLayer> parallaxLayers, List<string>? parallaxLayerNames = null)
    {
        for (int i = 0; i < parallaxLayers.Count; i++)
        {
            if (parallaxLayerNames != null && parallaxLayerNames[i] != string.Empty)
            {
                AddParallaxLayer(parallaxLayers[i], parallaxLayerNames[i]);
            }
            else
            {
                AddParallaxLayer(parallaxLayers[i]);
            }
        }
    }

    public void RemoveParallaxLayer(string parallaxLayerName)
    {
        if (ParallaxLayers.Remove(parallaxLayerName))
        {
            Debug.WriteLine("Successfully removed " + parallaxLayerName + " from ParallaxLayers");
        }
        else
        {
            Debug.WriteLine("Failed to remove " + parallaxLayerName + " from ParallaxLayers - doesn't exist");
        }
    }

    public ParallaxLayer? GetParallaxLayer(string parallaxLayerName)
    {
        ParallaxLayer? layerToGet = null;

        if (ParallaxLayers.TryGetValue(parallaxLayerName, out layerToGet))
        {
            Debug.WriteLine("Successfully got parallax layer " + parallaxLayerName + " as " + layerToGet);
            return layerToGet;
        }
        else
        {
            Debug.WriteLine("Failed to get " + parallaxLayerName);
            return null;
        }
    }

    public void Test()
    {
        ParallaxLayer layer;

        layer = new("road_front", new Vector2Int(0, -180), new Vector2(0.4f, 0.2f), _camera);
        AddParallaxLayer(layer);

        Debug.WriteLine("Default ParallaxManager Initialized");
    }

    public void Prometheus()
    {
        ParallaxLayer layer;

        layer = new("prometheus_back", new Vector2Int(0, 80), new Vector2(1.3f, 1.0f), _camera);
        AddParallaxLayer(layer);

        layer = new("prometheus_city", new Vector2Int(0, 70), new Vector2(1.2f, 0.9f), _camera);
        AddParallaxLayer(layer);

        layer = new("prometheus_towers", new Vector2Int(-210, 180), new Vector2(1.0f, 0.78f), 1.0f, new Vector2(1.5f), Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        layer = new("streets_back", new Vector2Int(0, 120), new Vector2(0.8f, 0.75f), 0.7f, new Vector2(1.5f), Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        layer = new("streets_middle", new Vector2Int(0, 100), new Vector2(0.6f, 0.7f), 0.6f, new Vector2(1.2f), Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        layer = new("road_front", new Vector2Int(0, 0), new Vector2(0.55f, 0.6f), 0.55f, new Vector2(0.5f), Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        layer = new("streets_front", new Vector2Int(0, 85), new Vector2(0.5f, 0.25f), _camera);
        AddParallaxLayer(layer);

        layer = new("road_front", new Vector2Int(0, -200), new Vector2(0.4f, 0.2f), _camera);
        AddParallaxLayer(layer);

        layer = new("bridge_rear", new Vector2Int(0, 0), new Vector2(0.0f), 0.1f, Vector2.One, Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        layer = new("bridge_front", new Vector2Int(0, 0), new Vector2(0.0f), -0.1f, Vector2.One, Vector2.Zero, _camera);
        AddParallaxLayer(layer);

        //layer = new("bridge_front", new Vector2Int(0, -30), new Vector2(-0.5f, 0.8f), -0.5f, 2.0f*Vector2.One, Vector2.Zero, _camera);
        //AddParallaxLayer(layer);

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