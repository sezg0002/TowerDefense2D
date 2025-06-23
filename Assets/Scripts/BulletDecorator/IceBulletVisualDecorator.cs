using UnityEngine;

public class IceBulletVisualDecorator : IBulletVisualDecorator
{
    public void ApplyVisual(BulletScript bullet)
    {
        var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.blue;
    }
}