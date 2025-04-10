using UnityEngine;

public interface IBackgroundColorChanger
{
    void ChangeColor(Color color);
    Color GetCurrentColor();
}