using UnityEngine;
using UnityEngine.UI;

public static class Extensions
{
    public const int GridCellSize = 256;
    public const int GridCellGap = 0;
    public static Color SetAlpha(this Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
    public static Direction GetOppositeDirection(Direction direction)
    {
        if (direction == Direction.Up)
            direction = Direction.Down;
        else if (direction == Direction.Down)
            direction = Direction.Up;
        else if (direction == Direction.Left)
            direction = Direction.Right;
        else
            direction = Direction.Left;
        return direction;
    }
    public static Camera GetUICamera()
    {
        return Camera.main;
    }
}