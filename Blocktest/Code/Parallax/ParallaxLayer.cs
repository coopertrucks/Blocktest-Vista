using Blocktest.Rendering;
using Shared.Code;
using Shared.Code.Components;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
namespace Blocktest.Parallax;

public sealed class ParallaxLayer
{
    public readonly Drawable Image;
    public readonly Vector2Int Offset;
    public readonly Vector2 Value;
    public readonly float ZLevel;

    private readonly List<Renderable> _repeatRenderables;
    private readonly Camera _camera;

    private Vector2Int _layerPosition;
    private Vector2Int _layerPositionBase;
    private float _zLevel;

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, float parallaxLayer, Camera renderCamera)
    {
        string path = @"Graphics\Parallax\" + imageName;
        Offset = parallaxOffset;
        Value = parallaxValue;
        ZLevel = parallaxLayer;
        _zLevel = (float)Layer.Parallax + ZLevel;
        _camera = renderCamera;

        Image = new Drawable(path); // cool for single image parallaxes

        // default left starting position for parallax array
        _layerPosition = (Vector2Int)(_camera.Position * Value + (Vector2)Offset - new Vector2(Image.Bounds.Width, 0));
        _layerPositionBase = (Vector2Int)Vector2.Zero;

        _repeatRenderables = new List<Renderable>
        {
            Capacity = (_camera.RenderTarget.Width / Image.Bounds.Width) + 2
        };

        for (int i = 0; i < _repeatRenderables.Capacity; i++)
        {
            Vector2Int tempPosition = new(_layerPosition.X + (i * Image.Bounds.Width), _layerPosition.Y);
            Transform tempTransform = new(tempPosition);
            _repeatRenderables.Add(new Renderable(tempTransform, _zLevel, Image, Color.White));
            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }
    }

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, Camera renderCamera)
        : this(imageName, parallaxOffset, parallaxValue, parallaxValue.X, renderCamera)
    {

    }

    //public void Initialize(float trueZLevel)
    //{
    //    _zLevel = trueZLevel;


    //}

    public void Draw()
    {
        _layerPosition = _layerPositionBase + (Vector2Int)(_camera.Position * Value + (Vector2)Offset - new Vector2(Image.Bounds.Width, 0)); // updates position

        if ((int)Math.Round(_camera.Position.X) > _layerPosition.X + Image.Bounds.Width + Image.Bounds.Width / 10) // checks if right edge off screen w/ bound, to skip to right
        {
            int skip = (int)Math.Round((_camera.Position.X - _layerPosition.X) / Image.Bounds.Width); // number of images to skip when leaping camera
            skip = Math.Max(1, skip);
            _layerPositionBase += new Vector2Int(skip * Image.Bounds.Width, 0);
        }
        else if ((int)Math.Round(_camera.Position.X) < _layerPosition.X + Image.Bounds.Width / 10) // checks if left edge off screen w/ bound, to skip to left
        {
            int skip = 1 + (int)Math.Round(((float)_layerPosition.X - _camera.Position.X) / (float)Image.Bounds.Width);
            skip = Math.Max(1, skip);
            _layerPositionBase -= new Vector2Int(skip * Image.Bounds.Width, 0);
        }

        for (int i = 0; i < _repeatRenderables.Capacity; i++) // true drawing action
        {
            _camera.RenderedComponents.Remove(_repeatRenderables[i]);

            Vector2Int tempPosition = new(_layerPosition.X + (i * Image.Bounds.Width), _layerPosition.Y);
            Transform tempTransform = new(tempPosition);
            _repeatRenderables[i] = new Renderable(tempTransform, _zLevel, Image, Color.White);

            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }
    }
}