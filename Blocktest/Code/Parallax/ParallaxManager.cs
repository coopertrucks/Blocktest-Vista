using Blocktest.Rendering;
using Shared.Code;
using Shared.Code.Components;
using System.Diagnostics;
namespace Blocktest.Parallax;

public sealed class ParallaxManager
{
    public Drawable Image;
    public Renderable Renderable;


    private Vector2Int _parallaxOffset;
    private Vector2 _parallaxValue;
    private float _parallaxLayer;

    private readonly Camera _camera;
    private Transform _layerTransform;
    private Vector2Int _layerPosition;

    public ParallaxManager(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, Camera renderCamera)
    {
        string path = @"Graphics\Parallax\" + imageName;
        _parallaxOffset = parallaxOffset;
        _parallaxValue = parallaxValue;
        _camera = renderCamera;

        Debug.WriteLine(path);
        Image = new Drawable(path); // cool for single image parallaxes

        // rendering here
        _layerPosition = _parallaxOffset + ((Vector2Int)_camera.Position / ((Vector2Int)_parallaxValue));
        _layerTransform = new(_layerPosition, null, null, 0);
        Renderable = new Renderable(_layerTransform, Layer.Parallax, Image, Color.White);

        _camera.RenderedComponents.Add(Renderable);
        Debug.WriteLine("parallax rendered");
    }

    public void Draw()
    {
        _camera.RenderedComponents.Remove(Renderable);

        _layerPosition = _parallaxOffset + ((Vector2Int)_camera.Position / ((Vector2Int)_parallaxValue));
        _layerTransform = new(_layerPosition, null, null, 0);
        Renderable = new Renderable(_layerTransform, Layer.Parallax, Image, Color.White);

        _camera.RenderedComponents.Add(Renderable);
    }
}