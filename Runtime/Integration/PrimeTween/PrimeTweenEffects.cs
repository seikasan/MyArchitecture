#if PRIME_TWEEN || MYARCHITECTURE_PRIME_TWEEN
using System;
using MyArchitecture.Core;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace MyArchitecture.Integration
{
    public static class PrimeTweenEffects
    {
        private const float CompletionThreshold = 0.9999f;

        public static Tween FadeTo(
            this ICanUseTween _,
            CanvasGroup target,
            float alpha,
            float duration,
            Ease ease = Ease.OutQuad)
            => FadeTo(target, alpha, duration, ease);

        public static Tween FadeIn(
            this ICanUseTween _,
            CanvasGroup target,
            float duration,
            Ease ease = Ease.OutQuad,
            bool enableInput = true,
            bool activateBeforeStart = true)
            => FadeIn(target, duration, ease, enableInput, activateBeforeStart);

        public static Tween FadeOut(
            this ICanUseTween _,
            CanvasGroup target,
            float duration,
            Ease ease = Ease.InQuad,
            bool disableInputImmediately = true,
            bool deactivateOnComplete = false)
            => FadeOut(target, duration, ease, disableInputImmediately, deactivateOnComplete);

        public static Tween SetVisible(
            this ICanUseTween _,
            CanvasGroup target,
            bool visible,
            float duration,
            Ease ease = Ease.OutQuad,
            bool setActive = false)
            => SetVisible(target, visible, duration, ease, setActive);

        public static Tween FadeTo(
            CanvasGroup target,
            float alpha,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startAlpha = target.alpha;
            return Tween01(
                duration,
                progress => target.alpha = Mathf.LerpUnclamped(startAlpha, alpha, progress),
                ease);
        }

        public static Tween FadeIn(
            CanvasGroup target,
            float duration,
            Ease ease = Ease.OutQuad,
            bool enableInput = true,
            bool activateBeforeStart = true)
        {
            EnsureNotNull(target, nameof(target));

            if (activateBeforeStart)
            {
                target.gameObject.SetActive(true);
            }

            if (enableInput)
            {
                target.interactable = true;
                target.blocksRaycasts = true;
            }

            return FadeTo(target, 1f, duration, ease);
        }

        public static Tween FadeOut(
            CanvasGroup target,
            float duration,
            Ease ease = Ease.InQuad,
            bool disableInputImmediately = true,
            bool deactivateOnComplete = false)
        {
            EnsureNotNull(target, nameof(target));

            if (disableInputImmediately)
            {
                target.interactable = false;
                target.blocksRaycasts = false;
            }

            var startAlpha = target.alpha;
            var completed = false;

            return Tween01(
                duration,
                progress =>
                {
                    target.alpha = Mathf.LerpUnclamped(startAlpha, 0f, progress);

                    if (!completed && progress >= CompletionThreshold)
                    {
                        completed = true;

                        if (deactivateOnComplete)
                        {
                            target.gameObject.SetActive(false);
                        }
                    }
                },
                ease);
        }

        public static Tween SetVisible(
            CanvasGroup target,
            bool visible,
            float duration,
            Ease ease = Ease.OutQuad,
            bool setActive = false)
        {
            EnsureNotNull(target, nameof(target));

            return visible
                ? FadeIn(target, duration, ease, enableInput: true, activateBeforeStart: setActive)
                : FadeOut(target, duration, ease, disableInputImmediately: true, deactivateOnComplete: setActive);
        }

        public static Tween MoveTo(
            this ICanUseTween _,
            Transform target,
            Vector3 position,
            float duration,
            Ease ease = Ease.OutQuad)
            => MoveTo(target, position, duration, ease);

        public static Tween MoveLocalTo(
            this ICanUseTween _,
            Transform target,
            Vector3 localPosition,
            float duration,
            Ease ease = Ease.OutQuad)
            => MoveLocalTo(target, localPosition, duration, ease);

        public static Tween MoveLocalBy(
            this ICanUseTween _,
            Transform target,
            Vector3 localDelta,
            float duration,
            Ease ease = Ease.OutQuad)
            => MoveLocalBy(target, localDelta, duration, ease);

        public static Tween MoveTo(
            Transform target,
            Vector3 position,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.position;
            return Tween01(
                duration,
                progress => target.position = Vector3.LerpUnclamped(startPosition, position, progress),
                ease);
        }

        public static Tween MoveLocalTo(
            Transform target,
            Vector3 localPosition,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.localPosition;
            return Tween01(
                duration,
                progress => target.localPosition = Vector3.LerpUnclamped(startPosition, localPosition, progress),
                ease);
        }

        public static Tween MoveLocalBy(
            Transform target,
            Vector3 localDelta,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            return MoveLocalTo(target, target.localPosition + localDelta, duration, ease);
        }

        public static Tween AnchoredMoveTo(
            this ICanUseTween _,
            RectTransform target,
            Vector2 anchoredPosition,
            float duration,
            Ease ease = Ease.OutQuad)
            => AnchoredMoveTo(target, anchoredPosition, duration, ease);

        public static Tween AnchoredMoveBy(
            this ICanUseTween _,
            RectTransform target,
            Vector2 anchoredDelta,
            float duration,
            Ease ease = Ease.OutQuad)
            => AnchoredMoveBy(target, anchoredDelta, duration, ease);

        public static Tween SlideInFrom(
            this ICanUseTween _,
            RectTransform target,
            Vector2 fromOffset,
            float duration,
            Ease ease = Ease.OutCubic)
            => SlideInFrom(target, fromOffset, duration, ease);

        public static Tween SlideOutTo(
            this ICanUseTween _,
            RectTransform target,
            Vector2 toOffset,
            float duration,
            Ease ease = Ease.InCubic)
            => SlideOutTo(target, toOffset, duration, ease);

        public static Tween AnchoredMoveTo(
            RectTransform target,
            Vector2 anchoredPosition,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.anchoredPosition;
            return Tween01(
                duration,
                progress => target.anchoredPosition = Vector2.LerpUnclamped(startPosition, anchoredPosition, progress),
                ease);
        }

        public static Tween AnchoredMoveBy(
            RectTransform target,
            Vector2 anchoredDelta,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            return AnchoredMoveTo(target, target.anchoredPosition + anchoredDelta, duration, ease);
        }

        public static Tween SlideInFrom(
            RectTransform target,
            Vector2 fromOffset,
            float duration,
            Ease ease = Ease.OutCubic)
        {
            EnsureNotNull(target, nameof(target));

            var endPosition = target.anchoredPosition;
            target.anchoredPosition = endPosition + fromOffset;
            return AnchoredMoveTo(target, endPosition, duration, ease);
        }

        public static Tween SlideOutTo(
            RectTransform target,
            Vector2 toOffset,
            float duration,
            Ease ease = Ease.InCubic)
        {
            EnsureNotNull(target, nameof(target));

            return AnchoredMoveBy(target, toOffset, duration, ease);
        }

        public static Tween ScaleTo(
            this ICanUseTween _,
            Transform target,
            Vector3 scale,
            float duration,
            Ease ease = Ease.OutQuad)
            => ScaleTo(target, scale, duration, ease);

        public static Tween ScaleFromTo(
            this ICanUseTween _,
            Transform target,
            Vector3 fromScale,
            Vector3 toScale,
            float duration,
            Ease ease = Ease.OutQuad)
            => ScaleFromTo(target, fromScale, toScale, duration, ease);

        public static Sequence PopIn(
            this ICanUseTween _,
            Transform target,
            float duration,
            Vector3? targetScale = null,
            Vector3? overshootScale = null,
            Ease firstEase = Ease.OutCubic,
            Ease secondEase = Ease.InOutSine)
            => PopIn(target, duration, targetScale, overshootScale, firstEase, secondEase);

        public static Tween PunchScale(
            this ICanUseTween _,
            Transform target,
            Vector3 strength,
            float duration,
            int frequency = 4,
            float damping = 2f)
            => PunchScale(target, strength, duration, frequency, damping);

        public static Tween BreatheScale(
            this ICanUseTween _,
            Transform target,
            Vector3 minScale,
            Vector3 maxScale,
            float duration,
            int cycles = 1,
            bool restoreOriginal = true)
            => BreatheScale(target, minScale, maxScale, duration, cycles, restoreOriginal);

        public static Tween ScaleTo(
            Transform target,
            Vector3 scale,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startScale = target.localScale;
            return Tween01(
                duration,
                progress => target.localScale = Vector3.LerpUnclamped(startScale, scale, progress),
                ease);
        }

        public static Tween ScaleFromTo(
            Transform target,
            Vector3 fromScale,
            Vector3 toScale,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            target.localScale = fromScale;
            return ScaleTo(target, toScale, duration, ease);
        }

        public static Sequence PopIn(
            Transform target,
            float duration,
            Vector3? targetScale = null,
            Vector3? overshootScale = null,
            Ease firstEase = Ease.OutCubic,
            Ease secondEase = Ease.InOutSine)
        {
            EnsureNotNull(target, nameof(target));

            var finalScale = targetScale ?? Vector3.one;
            var overshoot = overshootScale ?? finalScale * 1.12f;
            target.localScale = Vector3.zero;

            var sequence = Sequence.Create();
            sequence.Chain(ScaleTo(target, overshoot, duration * 0.7f, firstEase));
            sequence.Chain(ScaleTo(target, finalScale, duration * 0.3f, secondEase));
            return sequence;
        }

        public static Tween PunchScale(
            Transform target,
            Vector3 strength,
            float duration,
            int frequency = 4,
            float damping = 2f)
        {
            EnsureNotNull(target, nameof(target));

            frequency = Mathf.Max(1, frequency);
            damping = Mathf.Max(0.01f, damping);

            var origin = target.localScale;
            return Tween01(
                duration,
                progress =>
                {
                    var wave = Mathf.Sin(progress * frequency * Mathf.PI * 2f);
                    var falloff = Mathf.Pow(1f - progress, damping);
                    target.localScale = origin + strength * (wave * falloff);

                    if (progress >= CompletionThreshold)
                    {
                        target.localScale = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween BreatheScale(
            Transform target,
            Vector3 minScale,
            Vector3 maxScale,
            float duration,
            int cycles = 1,
            bool restoreOriginal = true)
        {
            EnsureNotNull(target, nameof(target));

            cycles = Mathf.Max(1, cycles);
            var origin = target.localScale;

            return Tween01(
                duration,
                progress =>
                {
                    var wave = (Mathf.Sin(progress * cycles * Mathf.PI * 2f - Mathf.PI * 0.5f) + 1f) * 0.5f;
                    target.localScale = Vector3.LerpUnclamped(minScale, maxScale, wave);

                    if (restoreOriginal && progress >= CompletionThreshold)
                    {
                        target.localScale = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween RotateLocalTo(
            this ICanUseTween _,
            Transform target,
            Vector3 localEulerAngles,
            float duration,
            Ease ease = Ease.OutQuad)
            => RotateLocalTo(target, localEulerAngles, duration, ease);

        public static Tween RotateLocalBy(
            this ICanUseTween _,
            Transform target,
            Vector3 localEulerDelta,
            float duration,
            Ease ease = Ease.OutQuad)
            => RotateLocalBy(target, localEulerDelta, duration, ease);

        public static Tween RotateZBy(
            this ICanUseTween _,
            Transform target,
            float localZDelta,
            float duration,
            Ease ease = Ease.OutQuad)
            => RotateLocalBy(target, new Vector3(0f, 0f, localZDelta), duration, ease);

        public static Tween WiggleRotation(
            this ICanUseTween _,
            Transform target,
            Vector3 strength,
            float duration,
            int frequency = 5,
            float damping = 1.5f)
            => WiggleRotation(target, strength, duration, frequency, damping);

        public static Tween RotateLocalTo(
            Transform target,
            Vector3 localEulerAngles,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startRotation = target.localRotation;
            var endRotation = Quaternion.Euler(localEulerAngles);
            return Tween01(
                duration,
                progress => target.localRotation = Quaternion.LerpUnclamped(startRotation, endRotation, progress),
                ease);
        }

        public static Tween RotateLocalBy(
            Transform target,
            Vector3 localEulerDelta,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startRotation = target.localRotation;
            var endRotation = startRotation * Quaternion.Euler(localEulerDelta);
            return Tween01(
                duration,
                progress => target.localRotation = Quaternion.LerpUnclamped(startRotation, endRotation, progress),
                ease);
        }

        public static Tween WiggleRotation(
            Transform target,
            Vector3 strength,
            float duration,
            int frequency = 5,
            float damping = 1.5f)
        {
            EnsureNotNull(target, nameof(target));

            frequency = Mathf.Max(1, frequency);
            damping = Mathf.Max(0.01f, damping);

            var origin = target.localRotation;
            return Tween01(
                duration,
                progress =>
                {
                    var wave = Mathf.Sin(progress * frequency * Mathf.PI * 2f);
                    var falloff = Mathf.Pow(1f - progress, damping);
                    target.localRotation = origin * Quaternion.Euler(strength * (wave * falloff));

                    if (progress >= CompletionThreshold)
                    {
                        target.localRotation = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween ShakeLocalPosition(
            this ICanUseTween _,
            Transform target,
            Vector3 strength,
            float duration,
            float damping = 1.5f)
            => ShakeLocalPosition(target, strength, duration, damping);

        public static Tween ShakeAnchoredPosition(
            this ICanUseTween _,
            RectTransform target,
            Vector2 strength,
            float duration,
            float damping = 1.5f)
            => ShakeAnchoredPosition(target, strength, duration, damping);

        public static Tween ShakeCamera(
            this ICanUseTween _,
            Camera target,
            Vector3 strength,
            float duration,
            float damping = 1.5f)
            => ShakeCamera(target, strength, duration, damping);

        public static Tween ShakeLocalPosition(
            Transform target,
            Vector3 strength,
            float duration,
            float damping = 1.5f)
        {
            EnsureNotNull(target, nameof(target));

            damping = Mathf.Max(0.01f, damping);
            var origin = target.localPosition;

            return Tween01(
                duration,
                progress =>
                {
                    var falloff = Mathf.Pow(1f - progress, damping);
                    var offset = new Vector3(
                        UnityEngine.Random.Range(-strength.x, strength.x),
                        UnityEngine.Random.Range(-strength.y, strength.y),
                        UnityEngine.Random.Range(-strength.z, strength.z));

                    target.localPosition = origin + offset * falloff;

                    if (progress >= CompletionThreshold)
                    {
                        target.localPosition = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween ShakeAnchoredPosition(
            RectTransform target,
            Vector2 strength,
            float duration,
            float damping = 1.5f)
        {
            EnsureNotNull(target, nameof(target));

            damping = Mathf.Max(0.01f, damping);
            var origin = target.anchoredPosition;

            return Tween01(
                duration,
                progress =>
                {
                    var falloff = Mathf.Pow(1f - progress, damping);
                    var offset = new Vector2(
                        UnityEngine.Random.Range(-strength.x, strength.x),
                        UnityEngine.Random.Range(-strength.y, strength.y));

                    target.anchoredPosition = origin + offset * falloff;

                    if (progress >= CompletionThreshold)
                    {
                        target.anchoredPosition = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween ShakeCamera(
            Camera target,
            Vector3 strength,
            float duration,
            float damping = 1.5f)
        {
            EnsureNotNull(target, nameof(target));

            return ShakeLocalPosition(target.transform, strength, duration, damping);
        }

        public static Tween GraphicColorTo(
            this ICanUseTween _,
            Graphic target,
            Color color,
            float duration,
            Ease ease = Ease.OutQuad)
            => GraphicColorTo(target, color, duration, ease);

        public static Tween Flash(
            this ICanUseTween _,
            Graphic target,
            Color flashColor,
            float duration,
            bool restoreOriginal = true)
            => Flash(target, flashColor, duration, restoreOriginal);

        public static Tween SpriteColorTo(
            this ICanUseTween _,
            SpriteRenderer target,
            Color color,
            float duration,
            Ease ease = Ease.OutQuad)
            => SpriteColorTo(target, color, duration, ease);

        public static Tween SpriteFadeTo(
            this ICanUseTween _,
            SpriteRenderer target,
            float alpha,
            float duration,
            Ease ease = Ease.OutQuad)
            => SpriteFadeTo(target, alpha, duration, ease);

        public static Tween GraphicColorTo(
            Graphic target,
            Color color,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startColor = target.color;
            return Tween01(
                duration,
                progress => target.color = Color.LerpUnclamped(startColor, color, progress),
                ease);
        }

        public static Tween Flash(
            Graphic target,
            Color flashColor,
            float duration,
            bool restoreOriginal = true)
        {
            EnsureNotNull(target, nameof(target));

            var origin = target.color;
            return Tween01(
                duration,
                progress =>
                {
                    var weight = Mathf.Sin(progress * Mathf.PI);
                    target.color = Color.LerpUnclamped(origin, flashColor, weight);

                    if (restoreOriginal && progress >= CompletionThreshold)
                    {
                        target.color = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween SpriteColorTo(
            SpriteRenderer target,
            Color color,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startColor = target.color;
            return Tween01(
                duration,
                progress => target.color = Color.LerpUnclamped(startColor, color, progress),
                ease);
        }

        public static Tween SpriteFadeTo(
            SpriteRenderer target,
            float alpha,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startColor = target.color;
            var endColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
            return SpriteColorTo(target, endColor, duration, ease);
        }

        public static Tween Blink(
            this ICanUseTween _,
            CanvasGroup target,
            float minAlpha,
            float maxAlpha,
            float duration,
            int cycles = 2,
            bool restoreOriginal = true)
            => Blink(target, minAlpha, maxAlpha, duration, cycles, restoreOriginal);

        public static Tween Blink(
            CanvasGroup target,
            float minAlpha,
            float maxAlpha,
            float duration,
            int cycles = 2,
            bool restoreOriginal = true)
        {
            EnsureNotNull(target, nameof(target));

            cycles = Mathf.Max(1, cycles);
            var origin = target.alpha;

            return Tween01(
                duration,
                progress =>
                {
                    var wave = Mathf.Abs(Mathf.Sin(progress * cycles * Mathf.PI));
                    target.alpha = Mathf.LerpUnclamped(minAlpha, maxAlpha, wave);

                    if (restoreOriginal && progress >= CompletionThreshold)
                    {
                        target.alpha = origin;
                    }
                },
                Ease.Linear);
        }

        public static Tween TypeText(
            this ICanUseTween _,
            Text target,
            string text,
            float duration,
            Ease ease = Ease.Linear)
            => TypeText(target, text, duration, ease);

        public static Tween CountText(
            this ICanUseTween _,
            Text target,
            int startValue,
            int endValue,
            float duration,
            string format = null,
            Ease ease = Ease.OutQuad)
            => CountText(target, startValue, endValue, duration, format, ease);

        public static Tween TypeText(
            Text target,
            string text,
            float duration,
            Ease ease = Ease.Linear)
        {
            EnsureNotNull(target, nameof(target));

            text ??= string.Empty;
            var lastLength = -1;

            return Tween01(
                duration,
                progress =>
                {
                    var length = Mathf.Clamp(
                        Mathf.FloorToInt(text.Length * progress),
                        0,
                        text.Length);

                    if (progress >= CompletionThreshold)
                    {
                        length = text.Length;
                    }

                    if (length != lastLength)
                    {
                        target.text = text.Substring(0, length);
                        lastLength = length;
                    }
                },
                ease);
        }

        public static Tween CountText(
            Text target,
            int startValue,
            int endValue,
            float duration,
            string format = null,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            return CountValue(
                startValue,
                endValue,
                duration,
                value => target.text = FormatInt(value, format),
                ease);
        }

        public static Tween CountValue(
            this ICanUseTween _,
            int startValue,
            int endValue,
            float duration,
            Action<int> onValueChanged,
            Ease ease = Ease.OutQuad)
            => CountValue(startValue, endValue, duration, onValueChanged, ease);

        public static Tween CountValue(
            int startValue,
            int endValue,
            float duration,
            Action<int> onValueChanged,
            Ease ease = Ease.OutQuad)
        {
            if (onValueChanged == null)
            {
                throw new ArgumentNullException(nameof(onValueChanged));
            }

            var lastValue = int.MinValue;
            return Tween01(
                duration,
                progress =>
                {
                    var value = Mathf.RoundToInt(Mathf.LerpUnclamped(startValue, endValue, progress));

                    if (progress >= CompletionThreshold)
                    {
                        value = endValue;
                    }

                    if (value != lastValue)
                    {
                        onValueChanged(value);
                        lastValue = value;
                    }
                },
                ease);
        }

        public static Sequence ShowPanel(
            this ICanUseTween _,
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideFromOffset,
            float duration,
            Ease ease = Ease.OutCubic)
            => ShowPanel(canvasGroup, rectTransform, slideFromOffset, duration, ease);

        public static Sequence HidePanel(
            this ICanUseTween _,
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideToOffset,
            float duration,
            Ease ease = Ease.InCubic,
            bool deactivateOnComplete = false)
            => HidePanel(canvasGroup, rectTransform, slideToOffset, duration, ease, deactivateOnComplete);

        public static Sequence ShowPanel(
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideFromOffset,
            float duration,
            Ease ease = Ease.OutCubic)
        {
            EnsureNotNull(canvasGroup, nameof(canvasGroup));
            EnsureNotNull(rectTransform, nameof(rectTransform));

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;

            var sequence = Sequence.Create();
            sequence.Group(FadeIn(canvasGroup, duration, ease));
            sequence.Group(SlideInFrom(rectTransform, slideFromOffset, duration, ease));
            return sequence;
        }

        public static Sequence HidePanel(
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideToOffset,
            float duration,
            Ease ease = Ease.InCubic,
            bool deactivateOnComplete = false)
        {
            EnsureNotNull(canvasGroup, nameof(canvasGroup));
            EnsureNotNull(rectTransform, nameof(rectTransform));

            var sequence = Sequence.Create();
            sequence.Group(FadeOut(canvasGroup, duration, ease, deactivateOnComplete: deactivateOnComplete));
            sequence.Group(SlideOutTo(rectTransform, slideToOffset, duration, ease));
            return sequence;
        }

        private static Tween Tween01(
            float duration,
            Action<float> onProgress,
            Ease ease)
        {
            if (onProgress == null)
            {
                throw new ArgumentNullException(nameof(onProgress));
            }

            if (duration <= 0f)
            {
                onProgress(1f);
                return default;
            }

            return Tween.Custom(
                startValue: 0f,
                endValue: 1f,
                duration: duration,
                onValueChange: onProgress,
                ease: ease);
        }

        private static string FormatInt(
            int value,
            string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString();
            }

            return string.Format(format, value);
        }

        private static void EnsureNotNull(
            UnityEngine.Object target,
            string paramName)
        {
            if (target == null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
    }
}
#endif
