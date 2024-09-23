using Blocktest.Code.Rendering;
using Microsoft.Xna.Framework.Graphics;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;

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
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

        RenderableComponents = new RenderableContainer();

        LightingRenderTarget = new RenderTarget2D(graphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        _setWhite = BlocktestGame.ContentManager?.Load<Effect>("Graphics/Effects/setWhite");

        _debugStopwatch = Stopwatch.StartNew();
    }

    public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        _debugStopwatch.Restart();

        graphics.SetRenderTarget(LightingRenderTarget);
        graphics.Clear(Color.Black);

        graphics.SetRenderTarget(RenderTarget);
        graphics.Clear(_backgroundColor);

        LiteOrderedDraw(graphics, spriteBatch);
        //OrderedDraw(graphics, spriteBatch);

        graphics.SetRenderTarget(null);

        Debug.WriteLine(_debugStopwatch.ElapsedMilliseconds); // track performance
    }

    public void LiteOrderedDraw(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        // draw background parallax
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.BackgroundParallax;
        DrawCallExternal(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.Parallax);

        // draw background blocks
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.BackgroundBlocks;
        DrawCallExternal(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.BackgroundBlocks);

        // draw player
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.Players;
        DrawCallExternal(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.Player);

        // draw foreground blocks
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.ForegroundBlocks;
        DrawCallExternal(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.ForegroundBlocks);

        // draw foreground parallax
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.ForegroundParallax;
        DrawCallExternal(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.Top);
    }


    public void OrderedDraw(GraphicsDevice graphics, SpriteBatch spriteBatch)
    {
        // draw background parallax
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.BackgroundParallax;
        SpritebatchDrawCall(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallLayeredLighting(spriteBatch, _renderedComponents, _setWhite);

        // draw background blocks
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.BackgroundBlocks;
        SpritebatchDrawCall(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.BackgroundBlocks);

        // draw player
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.Players;
        SpritebatchDrawCall(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.Player);

        // draw foreground blocks
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.ForegroundBlocks;
        SpritebatchDrawCall(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallBatchedLighting(spriteBatch, _renderedComponents, _setWhite, (float)Layer.ForegroundBlocks);

        // draw foreground parallax
        graphics.SetRenderTarget(RenderTarget);
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);
        _renderedComponents = RenderableComponents.ForegroundParallax;
        SpritebatchDrawCall(spriteBatch, _renderedComponents);
        spriteBatch.End();

        graphics.SetRenderTarget(LightingRenderTarget);
        DrawCallLayeredLighting(spriteBatch, _renderedComponents, _setWhite);
    }

    public void DrawCallExternal(SpriteBatch spriteBatch, HashSet<Renderable> renderedComponents)
    {
        foreach (Renderable component in renderedComponents)
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
    }

    public void DrawCallBatchedLighting(SpriteBatch spriteBatch, HashSet<Renderable> renderedComponents, Effect? effects, float layerValue)
    {
        float _layerColor = 1 - layerValue / (EnumCount + 1);
        _setWhite?.Parameters["color"].SetValue(_layerColor);

        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, effect: _setWhite);

        foreach (Renderable component in renderedComponents)
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
    }

    public void DrawCallLayeredLighting(SpriteBatch spriteBatch, HashSet<Renderable> renderedComponents, Effect? effects)
    {
        // may need to sort renderedComponents for parallax?
        foreach (Renderable component in renderedComponents)
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

            float _layerColor = 1 - component.LayerValue / (EnumCount + 1);
            _setWhite?.Parameters["color"].SetValue(_layerColor);
            spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.NonPremultiplied, SamplerState.PointClamp, effect: _setWhite);
            spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
                component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
                component.LayerValue / (EnumCount + 1));
            spriteBatch.End();
        }
    }

    public Vector2 CameraToWorldPos(Vector2 mouseState) => new(
        (mouseState.X - RenderLocation.X) / RenderLocation.Width * RenderTarget.Width + Position.X, Position.Y +
        RenderTarget.Height -
        (mouseState.Y - RenderLocation.Y) / RenderLocation.Height * RenderTarget.Height);
}