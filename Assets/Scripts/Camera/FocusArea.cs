using UnityEngine;
using UnityEngine.PlayerLoop;

public struct FocusArea
{
    public Vector2 Center;
    public Vector2 Velocity;

    private float Left;
    private float Right;
    private float Top;
    private float Bottom;


    public FocusArea(Bounds TargetBounds, Vector2 size)
    {
        Left = TargetBounds.center.x - size.x / 2;
        Right = TargetBounds.center.x + size.x / 2;
        Top = TargetBounds.min.y + size.y;
        Bottom = TargetBounds.min.y;

        Velocity = Vector2.zero;
        Center = new Vector2((Left + Right) / 2, (Top + Bottom) / 2);
    }

    public void Update(Bounds Target)
    {
        float shiftX = 0;

        if (Target.min.x < Left)
            shiftX = Target.min.x - Left;
        else if (Target.max.x > Right)
            shiftX = Target.max.x - Right;

        Left += shiftX;
        Right += shiftX;

        float shiftY = 0;

        if (Target.min.y < Bottom)
            shiftY = Target.min.y - Bottom;
        else if (Target.max.y > Top)
            shiftY = Target.max.y - Top;

        Bottom += shiftY;
        Top += shiftY;

        Center = new Vector2((Left + Right) / 2, (Top + Bottom) / 2);
        Velocity = new Vector2(shiftX, shiftY);
    }
}
