//#if PRIME_TWEEN || MYARCHITECTURE_PRIME_TWEEN
using System;
using MyArchitecture.Core;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;

namespace MyArchitecture.Integration
{
    public static class PrimeTweenAdvancedEffects
    {
        private const float CompletionThreshold = 0.9999f;
        private const string DefaultColorProperty = "_Color";
        private const string DefaultScrambleCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789!?+-*/<>";

        public static Sequence CrossFade(
            this ICanUseTween _,
            CanvasGroup from,
            CanvasGroup to,
            float duration,
            Ease fadeOutEase = Ease.InQuad,
            Ease fadeInEase = Ease.OutQuad,
            bool deactivateFromOnComplete = false)
            => CrossFade(from, to, duration, fadeOutEase, fadeInEase, deactivateFromOnComplete);

        public static Sequence CrossFade(
            CanvasGroup from,
            CanvasGroup to,
            float duration,
            Ease fadeOutEase = Ease.InQuad,
            Ease fadeInEase = Ease.OutQuad,
            bool deactivateFromOnComplete = false)
        {
            EnsureNotNull(from, nameof(from));
            EnsureNotNull(to, nameof(to));

            to.gameObject.SetActive(true);
            to.alpha = 0f;
            to.interactable = true;
            to.blocksRaycasts = true;
            from.interactable = false;
            from.blocksRaycasts = false;

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.FadeOut(from, duration, fadeOutEase, deactivateOnComplete: deactivateFromOnComplete));
            sequence.Group(PrimeTweenEffects.FadeIn(to, duration, fadeInEase));
            return sequence;
        }

        public static Sequence SwapPanel(
            this ICanUseTween _,
            CanvasGroup fromGroup,
            RectTransform fromRect,
            CanvasGroup toGroup,
            RectTransform toRect,
            Vector2 fromOutOffset,
            Vector2 toInOffset,
            float duration,
            bool deactivateFromOnComplete = true)
            => SwapPanel(fromGroup, fromRect, toGroup, toRect, fromOutOffset, toInOffset, duration, deactivateFromOnComplete);

        public static Sequence SwapPanel(
            CanvasGroup fromGroup,
            RectTransform fromRect,
            CanvasGroup toGroup,
            RectTransform toRect,
            Vector2 fromOutOffset,
            Vector2 toInOffset,
            float duration,
            bool deactivateFromOnComplete = true)
        {
            EnsureNotNull(fromGroup, nameof(fromGroup));
            EnsureNotNull(fromRect, nameof(fromRect));
            EnsureNotNull(toGroup, nameof(toGroup));
            EnsureNotNull(toRect, nameof(toRect));

            toGroup.gameObject.SetActive(true);
            toGroup.alpha = 0f;

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.FadeOut(fromGroup, duration, Ease.InQuad, deactivateOnComplete: deactivateFromOnComplete));
            sequence.Group(PrimeTweenEffects.SlideOutTo(fromRect, fromOutOffset, duration, Ease.InCubic));
            sequence.Group(PrimeTweenEffects.FadeIn(toGroup, duration, Ease.OutQuad));
            sequence.Group(PrimeTweenEffects.SlideInFrom(toRect, toInOffset, duration, Ease.OutCubic));
            return sequence;
        }

        public static Sequence OpenModal(
            this ICanUseTween _,
            CanvasGroup overlay,
            CanvasGroup panelGroup,
            Transform panelTransform,
            float duration,
            float overlayAlpha = 0.6f,
            float startScale = 0.92f)
            => OpenModal(overlay, panelGroup, panelTransform, duration, overlayAlpha, startScale);

        public static Sequence OpenModal(
            CanvasGroup overlay,
            CanvasGroup panelGroup,
            Transform panelTransform,
            float duration,
            float overlayAlpha = 0.6f,
            float startScale = 0.92f)
        {
            EnsureNotNull(overlay, nameof(overlay));
            EnsureNotNull(panelGroup, nameof(panelGroup));
            EnsureNotNull(panelTransform, nameof(panelTransform));

            overlay.gameObject.SetActive(true);
            panelGroup.gameObject.SetActive(true);
            overlay.alpha = 0f;
            panelGroup.alpha = 0f;
            panelTransform.localScale = Vector3.one * startScale;

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.FadeTo(overlay, overlayAlpha, duration, Ease.OutQuad));
            sequence.Group(PrimeTweenEffects.FadeIn(panelGroup, duration, Ease.OutQuad));
            sequence.Group(PrimeTweenEffects.ScaleTo(panelTransform, Vector3.one, duration, Ease.OutBack));
            return sequence;
        }

        public static Sequence CloseModal(
            this ICanUseTween _,
            CanvasGroup overlay,
            CanvasGroup panelGroup,
            Transform panelTransform,
            float duration,
            float endScale = 0.96f,
            bool deactivateOnComplete = true)
            => CloseModal(overlay, panelGroup, panelTransform, duration, endScale, deactivateOnComplete);

        public static Sequence CloseModal(
            CanvasGroup overlay,
            CanvasGroup panelGroup,
            Transform panelTransform,
            float duration,
            float endScale = 0.96f,
            bool deactivateOnComplete = true)
        {
            EnsureNotNull(overlay, nameof(overlay));
            EnsureNotNull(panelGroup, nameof(panelGroup));
            EnsureNotNull(panelTransform, nameof(panelTransform));

            overlay.interactable = false;
            overlay.blocksRaycasts = false;
            panelGroup.interactable = false;
            panelGroup.blocksRaycasts = false;

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.FadeOut(overlay, duration, Ease.InQuad, deactivateOnComplete: deactivateOnComplete));
            sequence.Group(PrimeTweenEffects.FadeOut(panelGroup, duration, Ease.InQuad, deactivateOnComplete: deactivateOnComplete));
            sequence.Group(PrimeTweenEffects.ScaleTo(panelTransform, Vector3.one * endScale, duration, Ease.InCubic));
            return sequence;
        }

        public static Sequence Toast(
            this ICanUseTween _,
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideFromOffset,
            Vector2 slideOutOffset,
            float showDuration,
            float stayDuration,
            float hideDuration)
            => Toast(canvasGroup, rectTransform, slideFromOffset, slideOutOffset, showDuration, stayDuration, hideDuration);

        public static Sequence Toast(
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 slideFromOffset,
            Vector2 slideOutOffset,
            float showDuration,
            float stayDuration,
            float hideDuration)
        {
            EnsureNotNull(canvasGroup, nameof(canvasGroup));
            EnsureNotNull(rectTransform, nameof(rectTransform));

            canvasGroup.gameObject.SetActive(true);
            canvasGroup.alpha = 0f;

            var sequence = Sequence.Create();
            sequence.Chain(PrimeTweenEffects.ShowPanel(canvasGroup, rectTransform, slideFromOffset, showDuration, Ease.OutCubic));
            sequence.Chain(Hold(stayDuration));
            sequence.Chain(PrimeTweenEffects.HidePanel(canvasGroup, rectTransform, slideOutOffset, hideDuration, Ease.InCubic, deactivateOnComplete: true));
            return sequence;
        }

        public static Sequence PressFeedback(
            this ICanUseTween _,
            Transform target,
            float duration = 0.12f,
            float pressedScale = 0.94f)
            => PressFeedback(target, duration, pressedScale);

        public static Sequence PressFeedback(
            Transform target,
            float duration = 0.12f,
            float pressedScale = 0.94f)
        {
            EnsureNotNull(target, nameof(target));

            var origin = target.localScale;
            var sequence = Sequence.Create();
            sequence.Chain(PrimeTweenEffects.ScaleTo(target, origin * pressedScale, duration * 0.45f, Ease.OutQuad));
            sequence.Chain(PrimeTweenEffects.ScaleTo(target, origin, duration * 0.55f, Ease.OutBack));
            return sequence;
        }

        public static Tween PulseScale(
            this ICanUseTween _,
            Transform target,
            float scaleMultiplier,
            float duration,
            int cycles = 1,
            bool restoreOriginal = true)
            => PulseScale(target, scaleMultiplier, duration, cycles, restoreOriginal);

        public static Tween PulseScale(
            Transform target,
            float scaleMultiplier,
            float duration,
            int cycles = 1,
            bool restoreOriginal = true)
        {
            EnsureNotNull(target, nameof(target));

            cycles = Mathf.Max(1, cycles);
            var origin = target.localScale;
            return TweenProgress(
                duration,
                progress =>
                {
                    var wave = Mathf.Abs(Mathf.Sin(progress * cycles * Mathf.PI));
                    target.localScale = Vector3.LerpUnclamped(origin, origin * scaleMultiplier, wave);

                    if (restoreOriginal && progress >= CompletionThreshold)
                    {
                        target.localScale = origin;
                    }
                });
        }

        public static Sequence ErrorFeedback(
            this ICanUseTween _,
            Transform target,
            Graphic flashTarget = null,
            float duration = 0.28f,
            float shakeStrength = 12f)
            => ErrorFeedback(target, flashTarget, duration, shakeStrength);

        public static Sequence ErrorFeedback(
            Transform target,
            Graphic flashTarget = null,
            float duration = 0.28f,
            float shakeStrength = 12f)
        {
            EnsureNotNull(target, nameof(target));

            var sequence = Sequence.Create();

            if (target is RectTransform rectTransform)
            {
                sequence.Group(PrimeTweenEffects.ShakeAnchoredPosition(rectTransform, new Vector2(shakeStrength, 0f), duration));
            }
            else
            {
                sequence.Group(PrimeTweenEffects.ShakeLocalPosition(target, new Vector3(shakeStrength * 0.01f, 0f, 0f), duration));
            }

            sequence.Group(PrimeTweenEffects.WiggleRotation(target, new Vector3(0f, 0f, 5f), duration));

            if (flashTarget != null)
            {
                sequence.Group(PrimeTweenEffects.Flash(flashTarget, Color.red, duration));
            }

            return sequence;
        }

        public static Sequence SuccessFeedback(
            this ICanUseTween _,
            Transform target,
            Graphic flashTarget = null,
            float duration = 0.35f,
            float scaleMultiplier = 1.12f)
            => SuccessFeedback(target, flashTarget, duration, scaleMultiplier);

        public static Sequence SuccessFeedback(
            Transform target,
            Graphic flashTarget = null,
            float duration = 0.35f,
            float scaleMultiplier = 1.12f)
        {
            EnsureNotNull(target, nameof(target));

            var sequence = Sequence.Create();
            sequence.Group(PulseScale(target, scaleMultiplier, duration, cycles: 1));

            if (flashTarget != null)
            {
                sequence.Group(PrimeTweenEffects.Flash(flashTarget, Color.white, duration));
            }

            return sequence;
        }

        public static Sequence StaggerFadeIn(
            this ICanUseTween _,
            CanvasGroup[] targets,
            float itemDuration,
            float interval,
            bool enableInputOnComplete = true)
            => StaggerFadeIn(targets, itemDuration, interval, enableInputOnComplete);

        public static Sequence StaggerFadeIn(
            CanvasGroup[] targets,
            float itemDuration,
            float interval,
            bool enableInputOnComplete = true)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                target.gameObject.SetActive(true);
                target.alpha = 0f;
                target.interactable = false;
                target.blocksRaycasts = false;

                var delay = interval * i;
                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress =>
                    {
                        target.alpha = progress;

                        if (enableInputOnComplete && progress >= CompletionThreshold)
                        {
                            target.interactable = true;
                            target.blocksRaycasts = true;
                        }
                    }));
            }

            return sequence;
        }

        public static Sequence StaggerFadeOut(
            this ICanUseTween _,
            CanvasGroup[] targets,
            float itemDuration,
            float interval,
            bool deactivateOnComplete = false)
            => StaggerFadeOut(targets, itemDuration, interval, deactivateOnComplete);

        public static Sequence StaggerFadeOut(
            CanvasGroup[] targets,
            float itemDuration,
            float interval,
            bool deactivateOnComplete = false)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                var startAlpha = target.alpha;
                target.interactable = false;
                target.blocksRaycasts = false;

                var delay = interval * i;
                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress =>
                    {
                        target.alpha = Mathf.LerpUnclamped(startAlpha, 0f, progress);

                        if (deactivateOnComplete && progress >= CompletionThreshold)
                        {
                            target.gameObject.SetActive(false);
                        }
                    }));
            }

            return sequence;
        }

        public static Sequence StaggerPopIn(
            this ICanUseTween _,
            Transform[] targets,
            float itemDuration,
            float interval,
            Vector3? finalScale = null,
            float overshootMultiplier = 1.12f)
            => StaggerPopIn(targets, itemDuration, interval, finalScale, overshootMultiplier);

        public static Sequence StaggerPopIn(
            Transform[] targets,
            float itemDuration,
            float interval,
            Vector3? finalScale = null,
            float overshootMultiplier = 1.12f)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                var endScale = finalScale ?? target.localScale;
                var overshootScale = endScale * overshootMultiplier;
                target.localScale = Vector3.zero;

                var delay = interval * i;
                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress =>
                    {
                        if (progress < 0.72f)
                        {
                            var localProgress = Smooth01(progress / 0.72f);
                            target.localScale = Vector3.LerpUnclamped(Vector3.zero, overshootScale, localProgress);
                            return;
                        }

                        var settleProgress = Smooth01((progress - 0.72f) / 0.28f);
                        target.localScale = Vector3.LerpUnclamped(overshootScale, endScale, settleProgress);
                    }));
            }

            return sequence;
        }

        public static Sequence StaggerSlideIn(
            this ICanUseTween _,
            RectTransform[] targets,
            Vector2 fromOffset,
            float itemDuration,
            float interval)
            => StaggerSlideIn(targets, fromOffset, itemDuration, interval);

        public static Sequence StaggerSlideIn(
            RectTransform[] targets,
            Vector2 fromOffset,
            float itemDuration,
            float interval)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                var endPosition = target.anchoredPosition;
                var startPosition = endPosition + fromOffset;
                target.anchoredPosition = startPosition;

                var delay = interval * i;
                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress => target.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, progress)));
            }

            return sequence;
        }

        public static Sequence StaggerSlideOut(
            this ICanUseTween _,
            RectTransform[] targets,
            Vector2 toOffset,
            float itemDuration,
            float interval)
            => StaggerSlideOut(targets, toOffset, itemDuration, interval);

        public static Sequence StaggerSlideOut(
            RectTransform[] targets,
            Vector2 toOffset,
            float itemDuration,
            float interval)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                var startPosition = target.anchoredPosition;
                var endPosition = startPosition + toOffset;

                var delay = interval * i;
                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress => target.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, progress)));
            }

            return sequence;
        }

        public static Sequence FanOutAnchored(
            this ICanUseTween _,
            RectTransform[] targets,
            Vector2 center,
            Vector2 spacing,
            float itemDuration,
            float interval = 0.02f)
            => FanOutAnchored(targets, center, spacing, itemDuration, interval);

        public static Sequence FanOutAnchored(
            RectTransform[] targets,
            Vector2 center,
            Vector2 spacing,
            float itemDuration,
            float interval = 0.02f)
        {
            EnsureTargets(targets, nameof(targets));

            var sequence = Sequence.Create();
            var originIndex = (targets.Length - 1) * 0.5f;

            for (var i = 0; i < targets.Length; i++)
            {
                var target = targets[i];
                EnsureNotNull(target, $"{nameof(targets)}[{i}]");

                var startPosition = target.anchoredPosition;
                var endPosition = center + spacing * (i - originIndex);
                var delay = interval * i;

                sequence.Group(DelayedSmoothProgress(
                    delay,
                    itemDuration,
                    progress => target.anchoredPosition = Vector2.LerpUnclamped(startPosition, endPosition, progress)));
            }

            return sequence;
        }

        public static Tween ArcMoveTo(
            this ICanUseTween _,
            Transform target,
            Vector3 worldPosition,
            float height,
            float duration)
            => ArcMoveTo(target, worldPosition, height, duration);

        public static Tween ArcMoveTo(
            Transform target,
            Vector3 worldPosition,
            float height,
            float duration)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.position;
            return TweenProgress(
                duration,
                progress =>
                {
                    var p = Smooth01(progress);
                    var arc = Mathf.Sin(p * Mathf.PI) * height;
                    target.position = Vector3.LerpUnclamped(startPosition, worldPosition, p) + Vector3.up * arc;
                });
        }

        public static Tween AnchoredArcMoveTo(
            this ICanUseTween _,
            RectTransform target,
            Vector2 anchoredPosition,
            float height,
            float duration)
            => AnchoredArcMoveTo(target, anchoredPosition, height, duration);

        public static Tween AnchoredArcMoveTo(
            RectTransform target,
            Vector2 anchoredPosition,
            float height,
            float duration)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.anchoredPosition;
            return TweenProgress(
                duration,
                progress =>
                {
                    var p = Smooth01(progress);
                    var arc = Mathf.Sin(p * Mathf.PI) * height;
                    target.anchoredPosition = Vector2.LerpUnclamped(startPosition, anchoredPosition, p) + Vector2.up * arc;
                });
        }

        public static Sequence FlyAndFadeOut(
            this ICanUseTween _,
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 anchoredDelta,
            float duration,
            bool deactivateOnComplete = true)
            => FlyAndFadeOut(canvasGroup, rectTransform, anchoredDelta, duration, deactivateOnComplete);

        public static Sequence FlyAndFadeOut(
            CanvasGroup canvasGroup,
            RectTransform rectTransform,
            Vector2 anchoredDelta,
            float duration,
            bool deactivateOnComplete = true)
        {
            EnsureNotNull(canvasGroup, nameof(canvasGroup));
            EnsureNotNull(rectTransform, nameof(rectTransform));

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.FadeOut(canvasGroup, duration, Ease.InQuad, deactivateOnComplete: deactivateOnComplete));
            sequence.Group(PrimeTweenEffects.AnchoredMoveBy(rectTransform, anchoredDelta, duration, Ease.OutCubic));
            return sequence;
        }

        public static Tween FillAmountTo(
            this ICanUseTween _,
            Image target,
            float fillAmount,
            float duration,
            Ease ease = Ease.OutQuad)
            => FillAmountTo(target, fillAmount, duration, ease);

        public static Tween FillAmountTo(
            Image target,
            float fillAmount,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.fillAmount;
            return Tween01(
                duration,
                progress => target.fillAmount = Mathf.LerpUnclamped(startValue, fillAmount, progress),
                ease);
        }

        public static Tween SliderValueTo(
            this ICanUseTween _,
            Slider target,
            float value,
            float duration,
            Ease ease = Ease.OutQuad)
            => SliderValueTo(target, value, duration, ease);

        public static Tween SliderValueTo(
            Slider target,
            float value,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.value;
            return Tween01(
                duration,
                progress => target.value = Mathf.LerpUnclamped(startValue, value, progress),
                ease);
        }

        public static Tween ScrollNormalizedPositionTo(
            this ICanUseTween _,
            ScrollRect target,
            Vector2 normalizedPosition,
            float duration,
            Ease ease = Ease.OutQuad)
            => ScrollNormalizedPositionTo(target, normalizedPosition, duration, ease);

        public static Tween ScrollNormalizedPositionTo(
            ScrollRect target,
            Vector2 normalizedPosition,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startPosition = target.normalizedPosition;
            return Tween01(
                duration,
                progress => target.normalizedPosition = Vector2.LerpUnclamped(startPosition, normalizedPosition, progress),
                ease);
        }

        public static Tween PreferredWidthTo(
            this ICanUseTween _,
            LayoutElement target,
            float width,
            float duration,
            Ease ease = Ease.OutQuad)
            => PreferredWidthTo(target, width, duration, ease);

        public static Tween PreferredWidthTo(
            LayoutElement target,
            float width,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.preferredWidth;
            return Tween01(
                duration,
                progress => target.preferredWidth = Mathf.LerpUnclamped(startValue, width, progress),
                ease);
        }

        public static Tween PreferredHeightTo(
            this ICanUseTween _,
            LayoutElement target,
            float height,
            float duration,
            Ease ease = Ease.OutQuad)
            => PreferredHeightTo(target, height, duration, ease);

        public static Tween PreferredHeightTo(
            LayoutElement target,
            float height,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.preferredHeight;
            return Tween01(
                duration,
                progress => target.preferredHeight = Mathf.LerpUnclamped(startValue, height, progress),
                ease);
        }

        public static Sequence ProgressBarTo(
            this ICanUseTween _,
            Image fillImage,
            Text label,
            float startValue,
            float endValue,
            float duration,
            string labelFormat = "{0:P0}")
            => ProgressBarTo(fillImage, label, startValue, endValue, duration, labelFormat);

        public static Sequence ProgressBarTo(
            Image fillImage,
            Text label,
            float startValue,
            float endValue,
            float duration,
            string labelFormat = "{0:P0}")
        {
            EnsureNotNull(fillImage, nameof(fillImage));

            fillImage.fillAmount = startValue;

            var sequence = Sequence.Create();
            sequence.Group(FillAmountTo(fillImage, endValue, duration, Ease.OutQuad));

            if (label != null)
            {
                sequence.Group(FloatValueTo(
                    startValue,
                    endValue,
                    duration,
                    value => label.text = FormatFloat(value, labelFormat),
                    Ease.OutQuad));
            }

            return sequence;
        }

        public static Tween FloatValueTo(
            this ICanUseTween _,
            float startValue,
            float endValue,
            float duration,
            Action<float> onValueChanged,
            Ease ease = Ease.OutQuad)
            => FloatValueTo(startValue, endValue, duration, onValueChanged, ease);

        public static Tween FloatValueTo(
            float startValue,
            float endValue,
            float duration,
            Action<float> onValueChanged,
            Ease ease = Ease.OutQuad)
        {
            if (onValueChanged == null)
            {
                throw new ArgumentNullException(nameof(onValueChanged));
            }

            return Tween01(
                duration,
                progress => onValueChanged(Mathf.LerpUnclamped(startValue, endValue, progress)),
                ease);
        }

        public static Tween CameraFovTo(
            this ICanUseTween _,
            Camera target,
            float fieldOfView,
            float duration,
            Ease ease = Ease.OutQuad)
            => CameraFovTo(target, fieldOfView, duration, ease);

        public static Tween CameraFovTo(
            Camera target,
            float fieldOfView,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.fieldOfView;
            return Tween01(
                duration,
                progress => target.fieldOfView = Mathf.LerpUnclamped(startValue, fieldOfView, progress),
                ease);
        }

        public static Tween CameraOrthographicSizeTo(
            this ICanUseTween _,
            Camera target,
            float orthographicSize,
            float duration,
            Ease ease = Ease.OutQuad)
            => CameraOrthographicSizeTo(target, orthographicSize, duration, ease);

        public static Tween CameraOrthographicSizeTo(
            Camera target,
            float orthographicSize,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));

            var startValue = target.orthographicSize;
            return Tween01(
                duration,
                progress => target.orthographicSize = Mathf.LerpUnclamped(startValue, orthographicSize, progress),
                ease);
        }

        public static Tween CameraFovPunch(
            this ICanUseTween _,
            Camera target,
            float addFieldOfView,
            float duration,
            int cycles = 1)
            => CameraFovPunch(target, addFieldOfView, duration, cycles);

        public static Tween CameraFovPunch(
            Camera target,
            float addFieldOfView,
            float duration,
            int cycles = 1)
        {
            EnsureNotNull(target, nameof(target));

            cycles = Mathf.Max(1, cycles);
            var origin = target.fieldOfView;
            return TweenProgress(
                duration,
                progress =>
                {
                    var wave = Mathf.Sin(progress * cycles * Mathf.PI);
                    target.fieldOfView = origin + addFieldOfView * wave;

                    if (progress >= CompletionThreshold)
                    {
                        target.fieldOfView = origin;
                    }
                });
        }

        public static Tween CameraOrthographicSizePunch(
            this ICanUseTween _,
            Camera target,
            float addSize,
            float duration,
            int cycles = 1)
            => CameraOrthographicSizePunch(target, addSize, duration, cycles);

        public static Tween CameraOrthographicSizePunch(
            Camera target,
            float addSize,
            float duration,
            int cycles = 1)
        {
            EnsureNotNull(target, nameof(target));

            cycles = Mathf.Max(1, cycles);
            var origin = target.orthographicSize;
            return TweenProgress(
                duration,
                progress =>
                {
                    var wave = Mathf.Sin(progress * cycles * Mathf.PI);
                    target.orthographicSize = origin + addSize * wave;

                    if (progress >= CompletionThreshold)
                    {
                        target.orthographicSize = origin;
                    }
                });
        }

        public static Tween MaterialColorTo(
            this ICanUseTween _,
            Material target,
            Color color,
            float duration,
            string propertyName = DefaultColorProperty,
            Ease ease = Ease.OutQuad)
            => MaterialColorTo(target, color, duration, propertyName, ease);

        public static Tween MaterialColorTo(
            Material target,
            Color color,
            float duration,
            string propertyName = DefaultColorProperty,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));
            EnsureMaterialProperty(target, propertyName);

            var startColor = target.GetColor(propertyName);
            return Tween01(
                duration,
                progress => target.SetColor(propertyName, Color.LerpUnclamped(startColor, color, progress)),
                ease);
        }

        public static Tween RendererMaterialColorTo(
            this ICanUseTween _,
            Renderer target,
            Color color,
            float duration,
            string propertyName = DefaultColorProperty,
            Ease ease = Ease.OutQuad)
            => RendererMaterialColorTo(target, color, duration, propertyName, ease);

        public static Tween RendererMaterialColorTo(
            Renderer target,
            Color color,
            float duration,
            string propertyName = DefaultColorProperty,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));
            return MaterialColorTo(target.material, color, duration, propertyName, ease);
        }

        public static Tween MaterialFloatTo(
            this ICanUseTween _,
            Material target,
            string propertyName,
            float value,
            float duration,
            Ease ease = Ease.OutQuad)
            => MaterialFloatTo(target, propertyName, value, duration, ease);

        public static Tween MaterialFloatTo(
            Material target,
            string propertyName,
            float value,
            float duration,
            Ease ease = Ease.OutQuad)
        {
            EnsureNotNull(target, nameof(target));
            EnsureMaterialProperty(target, propertyName);

            var startValue = target.GetFloat(propertyName);
            return Tween01(
                duration,
                progress => target.SetFloat(propertyName, Mathf.LerpUnclamped(startValue, value, progress)),
                ease);
        }

        public static Tween TypeTextBySpeed(
            this ICanUseTween _,
            Text target,
            string text,
            float charactersPerSecond)
            => TypeTextBySpeed(target, text, charactersPerSecond);

        public static Tween TypeTextBySpeed(
            Text target,
            string text,
            float charactersPerSecond)
        {
            EnsureNotNull(target, nameof(target));

            text ??= string.Empty;
            charactersPerSecond = Mathf.Max(1f, charactersPerSecond);
            var duration = text.Length / charactersPerSecond;
            return PrimeTweenEffects.TypeText(target, text, duration);
        }

        public static Tween TypeTextWithCursor(
            this ICanUseTween _,
            Text target,
            string text,
            float duration,
            string cursor = "_",
            Ease ease = Ease.Linear)
            => TypeTextWithCursor(target, text, duration, cursor, ease);

        public static Tween TypeTextWithCursor(
            Text target,
            string text,
            float duration,
            string cursor = "_",
            Ease ease = Ease.Linear)
        {
            EnsureNotNull(target, nameof(target));

            text ??= string.Empty;
            cursor ??= string.Empty;
            var lastLength = -1;

            return Tween01(
                duration,
                progress =>
                {
                    var length = Mathf.Clamp(Mathf.FloorToInt(text.Length * progress), 0, text.Length);

                    if (progress >= CompletionThreshold)
                    {
                        target.text = text;
                        return;
                    }

                    if (length != lastLength)
                    {
                        target.text = text.Substring(0, length) + cursor;
                        lastLength = length;
                    }
                },
                ease);
        }

        public static Tween ScrambleText(
            this ICanUseTween _,
            Text target,
            string text,
            float duration,
            string scrambleCharacters = DefaultScrambleCharacters)
            => ScrambleText(target, text, duration, scrambleCharacters);

        public static Tween ScrambleText(
            Text target,
            string text,
            float duration,
            string scrambleCharacters = DefaultScrambleCharacters)
        {
            EnsureNotNull(target, nameof(target));

            text ??= string.Empty;
            scrambleCharacters = string.IsNullOrEmpty(scrambleCharacters)
                ? DefaultScrambleCharacters
                : scrambleCharacters;

            return TweenProgress(
                duration,
                progress =>
                {
                    var revealLength = Mathf.Clamp(Mathf.FloorToInt(text.Length * progress), 0, text.Length);

                    if (progress >= CompletionThreshold)
                    {
                        target.text = text;
                        return;
                    }

                    var chars = new char[text.Length];
                    for (var i = 0; i < chars.Length; i++)
                    {
                        chars[i] = i < revealLength
                            ? text[i]
                            : scrambleCharacters[UnityEngine.Random.Range(0, scrambleCharacters.Length)];
                    }

                    target.text = new string(chars);
                });
        }

        public static Sequence CountTextWithPunch(
            this ICanUseTween _,
            Text target,
            int startValue,
            int endValue,
            float duration,
            string format = null,
            float punchScale = 0.08f)
            => CountTextWithPunch(target, startValue, endValue, duration, format, punchScale);

        public static Sequence CountTextWithPunch(
            Text target,
            int startValue,
            int endValue,
            float duration,
            string format = null,
            float punchScale = 0.08f)
        {
            EnsureNotNull(target, nameof(target));

            var sequence = Sequence.Create();
            sequence.Group(PrimeTweenEffects.CountText(target, startValue, endValue, duration, format));
            sequence.Group(PrimeTweenEffects.PunchScale(target.transform, Vector3.one * punchScale, duration, frequency: 5));
            return sequence;
        }

        private static Tween Hold(float duration)
            => TweenProgress(duration, _ => { });

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

        private static Tween TweenProgress(
            float duration,
            Action<float> onProgress)
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
                ease: Ease.Linear);
        }

        private static Tween DelayedSmoothProgress(
            float delay,
            float duration,
            Action<float> onProgress)
        {
            if (onProgress == null)
            {
                throw new ArgumentNullException(nameof(onProgress));
            }

            delay = Mathf.Max(0f, delay);
            duration = Mathf.Max(0f, duration);
            var totalDuration = delay + duration;

            if (totalDuration <= 0f)
            {
                onProgress(1f);
                return default;
            }

            var initialized = false;

            return Tween.Custom(
                startValue: 0f,
                endValue: totalDuration,
                duration: totalDuration,
                onValueChange: elapsed =>
                {
                    if (elapsed < delay)
                    {
                        if (!initialized)
                        {
                            onProgress(0f);
                            initialized = true;
                        }

                        return;
                    }

                    initialized = true;
                    var progress = duration <= 0f
                        ? 1f
                        : Mathf.Clamp01((elapsed - delay) / duration);
                    onProgress(Smooth01(progress));
                },
                ease: Ease.Linear);
        }

        private static float Smooth01(float value)
        {
            value = Mathf.Clamp01(value);
            return value * value * (3f - 2f * value);
        }

        private static string FormatFloat(
            float value,
            string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                return value.ToString();
            }

            return string.Format(format, value);
        }

        private static void EnsureTargets(
            Array targets,
            string paramName)
        {
            if (targets == null)
            {
                throw new ArgumentNullException(paramName);
            }
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

        private static void EnsureMaterialProperty(
            Material material,
            string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Material property name is empty.", nameof(propertyName));
            }

            if (!material.HasProperty(propertyName))
            {
                throw new ArgumentException($"Material does not have property: {propertyName}", nameof(propertyName));
            }
        }
    }
}
//#endif
