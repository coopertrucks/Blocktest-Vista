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
    private readonly bool _fixX;
    private readonly bool _fixY;

    private Vector2Int _layerPosition;
    private Vector2 _layerPositionBase;
    private float _zLevel;
    private int _countX;
    private int _countY;

    //private float _cornerAngle;

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, float parallaxLayer, Vector2 parallaxScale, Vector2 parallaxSpeed,
        Camera renderCamera, bool repeatX = true, bool repeatY = false, bool fixX = false, bool fixY = false)
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
        _fixX = fixX;
        _fixY = fixY;

        _layerPositionBase = (Vector2Int)Vector2.Zero - (new Vector2(0, Image.Bounds.Height));

        if (repeatX)
        { _countX = (_camera.RenderTarget.Width / (int)(Image.Bounds.Width * Scale.X)) + 2; }
        else
        { _countX = 1; }

        if (repeatY)
        { _countY = (_camera.RenderTarget.Height / (int)(Image.Bounds.Height * Scale.Y)) + 2; }
        else
        { _countY = 1; }

        _repeatRenderables = new List<Renderable>
        {
            Capacity = _countX * _countY
        };

        InitializeRenderables();
    }

    public ParallaxLayer(string imageName, Vector2Int parallaxOffset, Vector2 parallaxValue, Camera renderCamera, bool repeatX = true, bool repeatY = false, bool fixX = false, bool fixY = false)
        : this(imageName, parallaxOffset, parallaxValue, parallaxValue.X, Vector2.One, Vector2.Zero, renderCamera, repeatX, repeatY, fixX, fixY)
    {

    }

    public void InitializeRenderables()
    {
        _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value + (Vector2)Offset));

        int j = new();
        for (int i = 0; i < _countX * _countY; i++) // true drawing action
        {
            j = i / _countX;
            Vector2Int tempPosition = new Vector2Int(_layerPosition.X + (i - _countX * j - 1) * (int)(Image.Bounds.Width * Scale.X), _layerPosition.Y + (j - 1) * (int)(Image.Bounds.Height * Scale.Y));
            Transform newTransform = new(tempPosition, Scale, new Vector2Int(0, Image.Bounds.Height));
            Renderable newRenderable = new(newTransform, _zLevel, Image, Color.White);
            _repeatRenderables.Add(newRenderable);

            _camera.RenderedComponents.Add(_repeatRenderables[i]);
        }
    }

    public void Draw() // clean this up for the love of GOD
    {
        // left starting position for parallax array
        _layerPositionBase += Speed;
        _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value) + (Vector2)Offset);// updates position

        // skip logic
        if (_repeatX)
        {
            if ((int)Math.Round(_camera.Position.X) >= _layerPosition.X + (int)Math.Round(0.1f * Image.Bounds.Width * Scale.X)) // checks if right edge off screen w/ bound, to skip to right
            {
                int skip = (int)Math.Round((_camera.Position.X - _layerPosition.X) / (Image.Bounds.Width * Scale.X)); // number of images to skip when leaping camera
                skip = Math.Max(1, skip);
                //Debug.WriteLine("Skip Right " + skip);
                _layerPositionBase += new Vector2Int((int)(skip * Image.Bounds.Width * Scale.X), 0);
                _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value) + (Vector2)Offset);
            }
            else if ((int)Math.Round(_camera.Position.X) < _layerPosition.X - (int)Math.Round(0.9f * Image.Bounds.Width * Scale.X)) // checks if left edge off screen w/ bound, to skip to left
            {
                int skip = (int)Math.Round((_layerPosition.X - _camera.Position.X) / (Image.Bounds.Width * Scale.X));
                skip = Math.Max(1, skip);
                //Debug.WriteLine("Skip Left " + skip);
                _layerPositionBase -= new Vector2Int((int)(skip * Image.Bounds.Width * Scale.X), 0);
                _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value) + (Vector2)Offset);
            }
        }

        // 52 is a magic number
        if (_repeatY)
        {
            if ((int)Math.Round(_camera.Position.Y - _camera.RenderTarget.Height - 42) >= _layerPosition.Y + (int)Math.Round(0.2f * Image.Bounds.Width * Scale.Y))// + (1.1f * Image.Bounds.Height * Scale.Y)) // 
            {
                int skip = (int)Math.Round(((_camera.Position.Y - _camera.RenderTarget.Height) - _layerPosition.Y) / (Image.Bounds.Height * Scale.Y));
                skip = Math.Max(1, skip);
                //Debug.WriteLine("Skip Up " + skip);
                _layerPositionBase += new Vector2Int(0, (int)(skip * Image.Bounds.Height * Scale.Y));
                _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value) + (Vector2)Offset);
            }
            else if ((int)Math.Round(_camera.Position.Y - _camera.RenderTarget.Height - 42) < _layerPosition.Y - (int)Math.Round(0.8f * Image.Bounds.Height * Scale.Y)) // checks if left edge off screen w/ bound, to skip to left
            {
                int skip = (int)Math.Round((_layerPosition.Y - (_camera.Position.Y - _camera.RenderTarget.Height)) / (Image.Bounds.Height * Scale.Y));
                skip = Math.Max(1, skip);
                //Debug.WriteLine("Skip Down " + skip);
                _layerPositionBase -= new Vector2Int(0, (int)(skip * Image.Bounds.Height * Scale.Y));
                _layerPosition = (Vector2Int)(_layerPositionBase + (_camera.Position * Value) + (Vector2)Offset);
            }
        }

        // true drawing action ensues
        int i = 0;
        int j = 0;
        foreach (Renderable rendered in _repeatRenderables)
        {
            j = i / _countX;

            _camera.RenderedComponents.Remove(rendered);
            //tempPosition = new Vector2Int(_layerPosition.X + (i - _countX * j - 1) * (int)(Image.Bounds.Width * Scale.X), _layerPosition.Y + (j - 1) * (int)(Image.Bounds.Height * Scale.Y));
            rendered.Transform.Position = new Vector2Int(_layerPosition.X + (i - _countX * j - (_repeatX ? 1 : 0)) * (int)(Image.Bounds.Width * Scale.X), _layerPosition.Y + (j - (_repeatY ? 1 : 0)) * (int)(Image.Bounds.Height * Scale.Y));
            _camera.RenderedComponents.Add(rendered);

            i += 1;
        }
    }
}