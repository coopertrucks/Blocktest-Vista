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

        Test();
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
        //List<ParallaxLayer> layers = new List<ParallaxLayer>();
        //layers.Add(new("duskwood_trees", new Vector2Int(0, -250), new Vector2(0.3f), _camera));
        //layers.Add(new("duskwood_trees", new Vector2Int(0, -225), new Vector2(0.5f), _camera));
        //layers.Add(new("duskwood_trees", new Vector2Int(0, -200), new Vector2(0.7f), _camera));

        //List<string> layerNames = ["one", "two", ""];
        //AddParallaxLayers(layers, layerNames);

        //ParallaxLayer layer = new("duskwood_trees2", new Vector2Int(0, -160), new Vector2(0.9f), _camera);
        //AddParallaxLayer(layer);

        ParallaxLayer layer = new("duskwood_trees2", new Vector2Int(0, 0), new Vector2(0.9f), 0.91f, 2*Vector2.One, _camera);
        AddParallaxLayer(layer);

        layer = new("duskwood_trees2", new Vector2Int(0, -300), new Vector2(0.9f), _camera, repeatX: false, repeatY: true);
        AddParallaxLayer(layer);

        // test code
        //GetParallaxLayer("duskwood_trees2");
        //RemoveParallaxLayer("duskwood_trees2");
        //GetParallaxLayer("duskwood_trees2");
        //RemoveParallaxLayer("duskwood_trees2");

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