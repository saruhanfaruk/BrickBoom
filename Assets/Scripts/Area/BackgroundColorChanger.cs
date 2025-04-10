using UnityEngine;
using UnityEngine.UI;

public class BackgroundColorChanger : IBackgroundColorChanger
{
    private Image backgroundImage;

    public BackgroundColorChanger(Image image)
    {
        backgroundImage = image;
    }

    public void ChangeColor(Color color)
    {
        backgroundImage.color = color;
    }

    public Color GetCurrentColor()
    {
        return backgroundImage.color;
    }
}
