using UnityEngine;

public class FireBulletVisualDecorator : IBulletVisualDecorator
{
    public void ApplyVisual(BulletScript bullet)
    {
        var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.red;
    }
}