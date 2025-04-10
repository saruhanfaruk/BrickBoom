using UnityEngine;

public class UIElementAdjuster : IUIElementAdjuster
{
    private RectTransform gameBackground;
    private RectTransform shapeSpawnerPanel;

    public UIElementAdjuster(RectTransform gameBackground, RectTransform shapeSpawnerPanel)
    {
        this.gameBackground = gameBackground;
        this.shapeSpawnerPanel = shapeSpawnerPanel;
    }

    public void AdjustUIElements()
    {
        float size = (Screen.height - Screen.width) / 2;

        if (gameBackground != null)
            gameBackground.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Screen.width);
        if (shapeSpawnerPanel != null)
            shapeSpawnerPanel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size);
    }
}
