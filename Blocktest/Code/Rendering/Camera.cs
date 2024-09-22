using Blocktest.Code.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace Blocktest.Rendering;

public sealed class Camera {
    private static readonly int EnumCount = Enum.GetValues(typeof(Layer)).Length;
    private readonly Color _backgroundColor;
    private readonly Vector2 _size;

    public readonly RenderableContainer RenderableComponents;
    public readonly RenderTarget2D RenderTarget;
    public readonly RenderTarget2D LightingRenderTarget;
    public Vector2 Position;

    public Rectangle RenderLocation;

    private Color _layerColor;
    private Effect? _setWhite;
    private Stopwatch _debugStopwatch;
    private HashSet<Renderable>? _renderedComponents;

    public Camera(Vector2 position, Vector2 size, GraphicsDevice graphicsDevice, Color? backgroundColor = null) {
        Position = position;
        _size = size;
        _backgroundColor = backgroundColor ?? Color.CornflowerBlue;
        RenderTarget = new RenderTarget2D(graphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.DiscardContents);

        RenderableComponents = new RenderableContainer();

        LightingRenderTarget = new RenderTarget2D(graphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        _setWhite = BlocktestGame.ContentManager?.Load<Effect>("Graphics/Effects/setWhite");

        _debugStopwatch = Stopwatch.StartNew();
    }

    public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        _debugStopwatch.Restart();

        graphics.SetRenderTarget(RenderTarget);
        graphics.Clear(_backgroundColor);

        // draw background parallax
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

        _renderedComponents = RenderableComponents.BackgroundParallax;
        foreach (Renderable component in _renderedComponents)
        {
            if (component.Appearance == null)
            {
                continue;
            }

            Vector2 worldPosition = component.Transform.Position;
            Vector2 cameraPosition = worldPosition - Position;

            if (worldPosition.X + component.Appearance.Bounds.Width < Position.X &&
                worldPosition.X > Position.X + _size.X &&
                worldPosition.Y + component.Appearance.Bounds.Height < Position.Y &&
                worldPosition.Y > Position.Y + _size.Y)
            {
                continue;
            }

            Vector2 flippedPosition = new(cameraPosition.X,
                RenderTarget.Height - cameraPosition.Y - component.Appearance.Bounds.Height);

            Rectangle positionBounds = new((int)flippedPosition.X, (int)flippedPosition.Y,
                (int)(component.Appearance.Bounds.Width * component.Transform.Scale.X),
                (int)(component.Appearance.Bounds.Height * component.Transform.Scale.Y));

            spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
                component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
                component.LayerValue / (EnumCount + 1));
        }

        spriteBatch.End();

        // draw background blocks
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        foreach (Renderable component in _renderedComponents)
        {
            if (component.Appearance == null)
            {
                continue;
            }

            Vector2 worldPosition = component.Transform.Position;
            Vector2 cameraPosition = worldPosition - Position;

            if (worldPosition.X + component.Appearance.Bounds.Width < Position.X &&
                worldPosition.X > Position.X + _size.X &&
                worldPosition.Y + component.Appearance.Bounds.Height < Position.Y &&
                worldPosition.Y > Position.Y + _size.Y)
            {
                continue;
            }

            Vector2 flippedPosition = new(cameraPosition.X,
                RenderTarget.Height - cameraPosition.Y - component.Appearance.Bounds.Height);

            Rectangle positionBounds = new((int)flippedPosition.X, (int)flippedPosition.Y,
                (int)(component.Appearance.Bounds.Width * component.Transform.Scale.X),
                (int)(component.Appearance.Bounds.Height * component.Transform.Scale.Y));

            spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
                component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
                component.LayerValue / (EnumCount + 1));
        }

        //spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

        //foreach (Renderable component in RenderedComponents)
        //{
        //    if (component.Appearance == null)
        //    {
        //        continue;
        //    }

        //    Vector2 worldPosition = component.Transform.Position;
        //    Vector2 cameraPosition = worldPosition - Position;

        //    if (worldPosition.X + component.Appearance.Bounds.Width < Position.X &&
        //        worldPosition.X > Position.X + _size.X &&
        //        worldPosition.Y + component.Appearance.Bounds.Height < Position.Y &&
        //        worldPosition.Y > Position.Y + _size.Y)
        //    {
        //        continue;
        //    }

        //    Vector2 flippedPosition = new(cameraPosition.X,
        //        RenderTarget.Height - cameraPosition.Y - component.Appearance.Bounds.Height);

        //    Rectangle positionBounds = new((int)flippedPosition.X, (int)flippedPosition.Y,
        //        (int)(component.Appearance.Bounds.Width * component.Transform.Scale.X),
        //        (int)(component.Appearance.Bounds.Height * component.Transform.Scale.Y));

        //    spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
        //        component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
        //        component.LayerValue / (EnumCount + 1));
        //}

        //spriteBatch.End();

        //graphics.SetRenderTarget(LightingRenderTarget);
        ////spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, effect: _setWhite);
        //foreach (Renderable component in RenderedComponents)
        //{
        //    if (component.Appearance == null)
        //    {
        //        continue;
        //    }

        //    Vector2 worldPosition = component.Transform.Position;
        //    Vector2 cameraPosition = worldPosition - Position;

        //    if (worldPosition.X + component.Appearance.Bounds.Width < Position.X &&
        //        worldPosition.X > Position.X + _size.X &&
        //        worldPosition.Y + component.Appearance.Bounds.Height < Position.Y &&
        //        worldPosition.Y > Position.Y + _size.Y)
        //    {
        //        continue;
        //    }

        //    Vector2 flippedPosition = new(cameraPosition.X,
        //        RenderTarget.Height - cameraPosition.Y - component.Appearance.Bounds.Height);

        //    Rectangle positionBounds = new((int)flippedPosition.X, (int)flippedPosition.Y,
        //        (int)(component.Appearance.Bounds.Width * component.Transform.Scale.X),
        //        (int)(component.Appearance.Bounds.Height * component.Transform.Scale.Y));

        //    float _layerValue = 2.5f*(1 - component.LayerValue / (EnumCount + 1));
        //    _layerValue = (float)Math.Clamp(_layerValue, 0.0, 1.0);
        //    //Debug.WriteLine(_layerValue);
        //    //_layerColor = new Color(_layerValue, _layerValue, _layerValue);
        //    _setWhite?.Parameters["color"].SetValue(_layerValue);
        //    spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, effect: _setWhite);
        //    spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
        //        component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
        //        component.LayerValue / (EnumCount + 1));
        //    spriteBatch.End();
        //}

        //spriteBatch.End();

        graphics.SetRenderTarget(null);

        Debug.WriteLine(_debugStopwatch.ElapsedMilliseconds); // track performance
    }

    public Vector2 CameraToWorldPos(Vector2 mouseState) => new(
        (mouseState.X - RenderLocation.X) / RenderLocation.Width * RenderTarget.Width + Position.X, Position.Y +
        RenderTarget.Height -
        (mouseState.Y - RenderLocation.Y) / RenderLocation.Height * RenderTarget.Height);
}