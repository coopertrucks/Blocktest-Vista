using Shared.Code.Components;
namespace Blocktest.Rendering;

public enum Layer {
    Top = 0,
    Player = 3,
    Default = 2,
    ForegroundBlocks = 1,
    BackgroundBlocks = 4,
    Parallax = 5
}

public sealed class Renderable {
    public readonly Transform Transform;
    public Drawable? Appearance;
    
    public Color RenderColor;
    public float LayerValue;

    public Renderable(Transform transform, Layer layer = Layer.Default, Drawable? appearance = null,
                      Color? renderColor = null) {
        Transform = transform;
        LayerValue = (float)layer;
        Appearance = appearance;
        RenderColor = renderColor ?? Color.White;
    }

    public Renderable(Transform transform, float layer = (float)Layer.Default, Drawable? appearance = null,
                      Color? renderColor = null)
    {
        Transform = transform;
        LayerValue = layer;
        Appearance = appearance;
        RenderColor = renderColor ?? Color.White;
    }

    public void Draw(SpriteBatch spriteBatch, Vector2 cameraPosition) {
        if (Appearance == null) {
            return;
        }

        spriteBatch.Draw(Appearance.Texture, Transform.Position - cameraPosition, Appearance.Bounds, RenderColor,
            Transform.Rotation, Transform.Origin, Transform.Scale,
            SpriteEffects.None, 0);
    }
}