using UnityEngine;

public class WindBulletVisualDecorator : IBulletVisualDecorator
{
    public void ApplyVisual(BulletScript bullet)
    {
        var spriteRenderer = bullet.GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            spriteRenderer.color = Color.green; 
    }
}