using DG.Tweening;
using UnityEngine;

public static class TweenUtility
{
    public static void KillSafe(this Tween tween, bool complete = false)
    {
        tween?.Kill(complete);
    }

    public static void CompleteSafe(this Tween tween)
    {
        tween?.Complete();
    }

    public static void InvokeSafe(this TweenCallback callback)
    {
        callback?.Invoke();
    }

    public static Sequence DelayedCall(TweenCallback callback, float delay)
    {
        return DOTween.Sequence()
                      .AppendCallback(callback)
                      .PrependInterval(delay);
    }

    public static void DelayedCall(this Tween tween, TweenCallback callback, float delay)
    {
        tween?.Kill();
        tween = DOTween.Sequence()
                       .AppendCallback(callback)
                       .PrependInterval(delay);
    }
}

public static class TweenExtenstions
{
    public static Tweener DOInteger(this Animator target, int idHash, int endValue, float duration)
    {
        return DOTween.To(() => target.GetInteger(idHash), x => target.SetInteger(idHash, x), endValue, duration)
                                      .SetTarget(target);
    }

    public static Tweener DOFloat(this Animator target, int idHash, float endValue, float duration)
    {
        return DOTween.To(() => target.GetFloat(idHash), x => target.SetFloat(idHash, x), endValue, duration)
                                      .SetTarget(target);
    }
}
