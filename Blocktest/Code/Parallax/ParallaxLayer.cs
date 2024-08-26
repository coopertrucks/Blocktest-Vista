using Blocktest.Rendering;
using Shared.Code;
using Shared.Code.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Blocktest.Parallax;

public sealed class ParallaxLayer
{
    public Drawable Image;
    public Renderable Renderable;
    private List<Renderable> _repeatRenderables;

    private Vector2Int _parallaxOffset;
    private Vector2 _parallaxValue;
    private float _parallaxLayer;

    private readonly Camera _camera;
    private Transform _layerTransform;
    private Vector2Int _layerPosition;

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, Camera renderCamera)
    {
        string path = @"Graphics\Parallax\" + imageName;
        _parallaxOffset = parallaxOffset;
        _parallaxValue = parallaxValue;
        _camera = renderCamera;

        Debug.WriteLine(path);
        Image = new Drawable(path); // cool for single image parallaxes

        // rendering here
        _layerPosition = ((Vector2Int)_camera.Position / ((Vector2Int)_parallaxValue)) + _parallaxOffset - new Vector2Int(Image.Bounds.Width, 0);
        //_layerTransform = new(_layerPosition);
        //Renderable = new Renderable(_layerTransform, Layer.Parallax, Image, Color.White);


        Debug.WriteLine("parallax rendered");

        _repeatRenderables = new List<Renderable>();
        _repeatRenderables.Capacity = (_camera.RenderTarget.Width / Image.Bounds.Width) + 2;
        Debug.WriteLine(_repeatRenderables.Capacity);
        Debug.WriteLine(_repeatRenderables.Count);

        for (int i = 0; i < _repeatRenderables.Capacity; i++)
        {
            Vector2Int tempPosition = new(_layerPosition.X + (i * Image.Bounds.Width), _layerPosition.Y);
            Transform tempTransform = new(tempPosition);
            _repeatRenderables.Add(new Renderable(tempTransform, Layer.Parallax, Image, Color.White));
            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }
    }

    public void Draw()
    {
        if ((int)_camera.Position.X > _layerPosition.X + Image.Bounds.Width)
        {
            Debug.WriteLine("shift right!");
            int skip = (int)((_camera.Position.X - (float)_layerPosition.X) / (float)Image.Bounds.Width); // number of images to skip when leaping camera
            skip = Math.Max(1, skip);
            _layerPosition += new Vector2Int(skip * Image.Bounds.Width, 0);
        }
        else if ((int)_camera.Position.X < _layerPosition.X)
        {
            Debug.WriteLine("shift left!");
            int skip = 1 + (int)(((float)_layerPosition.X - _camera.Position.X)/(float)Image.Bounds.Width);
            skip = Math.Max(1, skip);
            _layerPosition -= new Vector2Int(skip * Image.Bounds.Width, 0);
        }

        for (int i = 0; i < _repeatRenderables.Capacity; i++)
        {
            _camera.RenderedComponents.Remove(_repeatRenderables[i]);

            Vector2Int tempPosition = new(_layerPosition.X + (i * Image.Bounds.Width), _layerPosition.Y);
            Transform tempTransform = new(tempPosition);
            _repeatRenderables[i] = new Renderable(tempTransform, Layer.Parallax, Image, Color.White);

            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }

        //_camera.RenderedComponents.Remove(Renderable);

        //_layerPosition = _parallaxOffset + ((Vector2Int)_camera.Position / ((Vector2Int)_parallaxValue));
        //_layerTransform = new(_layerPosition, null, null, 0);
        //Renderable = new Renderable(_layerTransform, Layer.Parallax, Image, Color.White);

        //CheckPosition();

        //Debug.WriteLine(-Renderable.Transform.Position.X); left end of parallax img
        //Debug.WriteLine(Renderable.Appearance.Size.X - Renderable.Transform.Position.X); right end of parallax img

        //_camera.RenderedComponents.Add(Renderable);
    }

    private void CheckPosition()
    {
        if (-Renderable.Transform.Position.X > 0)
        {
            Debug.WriteLine("left edge showing");
        }
        if ((Renderable.Appearance.Size.X - Renderable.Transform.Position.X) < _camera.RenderTarget.Width)
        {
            Debug.WriteLine("right edge showing");
        }
    }
}