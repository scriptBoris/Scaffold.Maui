namespace ScaffoldLib.Maui.Internal;

internal class Button : ButtonSam.Maui.Button
{
#if ANDROID
    protected override void AnimationPressedStart(float x, float y)
    {
        float newX = (float)Width / 2f;
        float newY = (float)Height / 2f;
        base.AnimationPressedStart(newX, newY);
    }
#endif
}
