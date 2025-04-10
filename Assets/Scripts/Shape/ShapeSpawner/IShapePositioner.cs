using System.Collections.Generic;
using UnityEngine;

public interface IShapePositioner
{
    List<Vector2> CalculatePositions(float panelWidth, int count);
}
