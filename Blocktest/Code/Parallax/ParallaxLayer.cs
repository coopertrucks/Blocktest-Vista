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
    public readonly Vector2 Scale;
    public readonly Vector2 Speed;
    public float ZLevel
    {
        get { return _zLevel - (float)Layer.Parallax; }
        set { _zLevel = (float)Layer.Parallax + value; }
    }

    private readonly List<Renderable> _repeatRenderables;
    private readonly Camera _camera;
    private readonly bool _repeatX;
    private readonly bool _repeatY;

    private Vector2Int _layerPosition;
    private Vector2 _layerPositionBase;
    private float _zLevel;

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, float parallaxLayer, Vector2 parallaxScale, Vector2 parallaxSpeed,
        Camera renderCamera, bool repeatX = true, bool repeatY = false)
    {
        string path = @"Graphics\Parallax\" + imageName;
        Image = new Drawable(path);
        Offset = parallaxOffset;
        Value = parallaxValue;
        ZLevel = parallaxLayer;
        Scale = parallaxScale;
        Speed = parallaxSpeed;
        _camera = renderCamera;
        _repeatX = repeatX;
        _repeatY = repeatY;

        _layerPositionBase = (Vector2Int)Vector2.Zero;

        _repeatRenderables = new List<Renderable>
        {
            Capacity = (_camera.RenderTarget.Width / (int)(Image.Bounds.Width * Scale.X)) + 2
        };
    }

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, Camera renderCamera, bool repeatX = true, bool repeatY = false)
        : this(imageName, parallaxOffset, parallaxValue, parallaxValue.X, Vector2.One, Vector2.Zero, renderCamera, repeatX, repeatY)
    {

    }

    public void Draw() // clean this up for the love of GOD
    {
        // left starting position for parallax array
        _layerPositionBase += Speed;
        if (_layerPositionBase.X > _camera.RenderTarget.Width) { _layerPositionBase = new(_layerPositionBase.X - _camera.RenderTarget.Width, _layerPositionBase.Y); }
        //_layerPosition = (Vector2Int)Vector2.Transform(_layerPositionBase + (_camera.Position * Value + (Vector2)Offset - (new Vector2(Image.Bounds.Width, 0)) * Scale), rotation);
        _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value + (Vector2)Offset - (new Vector2(Image.Bounds.Width, 0)) * Scale));// updates position
        Matrix rotation = Matrix.CreateFromAxisAngle(Vector3.UnitZ, (float)Math.PI / 8);

        if ((int)Math.Round(_camera.Position.X) > _layerPosition.X + (Image.Bounds.Width + Image.Bounds.Width / 10) * Scale.X) // checks if right edge off screen w/ bound, to skip to right
        {
            int skip = (int)Math.Round((_camera.Position.X - _layerPosition.X) / ((float)Image.Bounds.Width * Scale.X)); // number of images to skip when leaping camera
            skip = Math.Max(1, skip);
            _layerPositionBase += new Vector2Int((int)(skip * Image.Bounds.Width * Scale.X), 0);
        }
        else if ((int)Math.Round(_camera.Position.X) < _layerPosition.X + (Image.Bounds.Width / 10) * Scale.X) // checks if left edge off screen w/ bound, to skip to left
        {
            int skip = 1 + (int)Math.Round(((float)_layerPosition.X - _camera.Position.X) / ((float)Image.Bounds.Width * Scale.X));
            skip = Math.Max(1, skip);
            _layerPositionBase -= new Vector2Int((int)(skip * Image.Bounds.Width * Scale.X), 0);
        }

        for (int i = 0; i < _repeatRenderables.Capacity; i++) // true drawing action
        {
            Vector2Int tempPosition = new(_layerPosition.X + i * (int)(Image.Bounds.Width * Scale.X), _layerPosition.Y);
            //Debug.WriteLine(tempPosition);
            tempPosition = (Vector2Int)Vector2.Transform(tempPosition, rotation);
            //Debug.WriteLine(tempPosition);
            Transform newTransform = new(tempPosition, Scale, _camera.RenderLocation.Location, -(float)Math.PI/8);
            Renderable newRenderable = new(newTransform, _zLevel, Image, Color.White);

            if (_repeatRenderables.Count > i) // && (_repeatRenderables[i] != null)) 
            {
                _camera.RenderedComponents.Remove(_repeatRenderables[i]);
                _repeatRenderables[i] = newRenderable;
            }
            else
            { 
                _repeatRenderables.Add(newRenderable);
            }

            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }
    }
}