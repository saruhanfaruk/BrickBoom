using DG.Tweening;
using UnityEngine;

public class AnimationHandler : IAnimationHandler
{
    private IBackgroundColorChanger backgroundColorChanger;

    public AnimationHandler(IBackgroundColorChanger colorChanger)
    {
        backgroundColorChanger = colorChanger;
    }

    public void PlayResetAnimation(Color finishColor, Color defaultColor)
    {
        Sequence sequence = DOTween.Sequence();
        for (int i = 0; i < 3; i++)
        {
            sequence.Append(DOTween.To(() => backgroundColorChanger.GetCurrentColor(), x => backgroundColorChanger.ChangeColor(x), Color.white, .1f));
            sequence.Append(DOTween.To(() => backgroundColorChanger.GetCurrentColor(), x => backgroundColorChanger.ChangeColor(x), finishColor, .1f));
        }
        sequence.Append(DOTween.To(() => backgroundColorChanger.GetCurrentColor(), x => backgroundColorChanger.ChangeColor(x), defaultColor, .1f));
        sequence.Play();
    }
}