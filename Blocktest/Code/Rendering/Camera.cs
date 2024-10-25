using System.Diagnostics;

namespace Blocktest.Rendering;

public sealed class Camera {
    private static readonly int EnumCount = Enum.GetValues(typeof(Layer)).Length;
    private readonly Color _backgroundColor;
    private readonly Vector2 _size;
    private readonly float _subScale;

    public readonly HashSet<Renderable> RenderedComponents = [];
    public readonly RenderTarget2D RenderTarget;
    public Vector2 Position;

    public Rectangle RenderLocation;

    public Camera(Vector2 position, Vector2 size, float subScale, GraphicsDevice graphicsDevice, Color? backgroundColor = null) {
        Position = position;
        _size = size;
        _subScale = subScale;
        _backgroundColor = backgroundColor ?? Color.CornflowerBlue;
        RenderTarget = new RenderTarget2D(graphicsDevice, (int)(size.X * subScale), (int)(size.Y * subScale), false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
    }

    public Camera(Vector2 position, Vector2 size, GraphicsDevice graphicsDevice, Color? backgroundColor = null) 
        : this(position, size, 1.0f, graphicsDevice, backgroundColor)
    {

    }

    public void Draw(GraphicsDevice graphics, SpriteBatch spriteBatch) {
        graphics.SetRenderTarget(RenderTarget);
        graphics.Clear(_backgroundColor);
        
        spriteBatch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp);

        foreach (Renderable component in RenderedComponents) {
            if (component.Appearance == null) {
                continue;
            }

            //Vector2 worldPosition = new Vector2(component.Transform.Position.X * _subScale, component.Transform.Position.Y * _subScale);
            Vector2 worldPosition = component.Transform.Position;
            Vector2 cameraPosition = worldPosition - Position;

            if (worldPosition.X + component.Appearance.Bounds.Width < Position.X &&
                worldPosition.X > Position.X + _size.X &&
                worldPosition.Y + component.Appearance.Bounds.Height < Position.Y &&
                worldPosition.Y > Position.Y + _size.Y) {
                continue;
            }

            Vector2 flippedPosition = new((_subScale * cameraPosition.X),
                RenderTarget.Height - (_subScale * cameraPosition.Y + component.Appearance.Bounds.Height));

            Rectangle positionBounds = new((int)flippedPosition.X, (int)flippedPosition.Y,
                (int)(component.Appearance.Bounds.Width * component.Transform.Scale.X * _subScale),
                (int)(component.Appearance.Bounds.Height * component.Transform.Scale.Y * _subScale));

            spriteBatch.Draw(component.Appearance.Texture, positionBounds, component.Appearance.Bounds,
                component.RenderColor, component.Transform.Rotation, component.Transform.Origin, SpriteEffects.None,
                (float)component.Layer / EnumCount);
        }

        spriteBatch.End();

        graphics.SetRenderTarget(null);
    }

    public Vector2 CameraToWorldPos(Vector2 mouseState) => new(
        (mouseState.X - RenderLocation.X) / RenderLocation.Width * RenderTarget.Width + Position.X, Position.Y +
        RenderTarget.Height -
        (mouseState.Y - RenderLocation.Y) / RenderLocation.Height * RenderTarget.Height);
}