using Godot;

namespace ConnectMore.Scripts.Game;

public partial class DiscBackground : Node2D
{
    [Export] public Color ColorOverride { get; set; }
    
    [Export] public Sprite2D Sprite { get; set; }

    public int TargetSize { get; set; }

    public override void _Ready()
    {
        this.UpdateVisual();
    }
    
    public void UpdateVisual()
    {
        Vector2 textureSize = this.Sprite.Texture.GetSize();

        float scaleX = this.TargetSize / textureSize.X;
        float scaleY = this.TargetSize / textureSize.Y;

        this.Sprite.Scale = new Vector2(scaleX, scaleY);
        this.Sprite.Modulate = this.ColorOverride;
    }
}