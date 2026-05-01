using MyArchitecture.Core;
using R3;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MyArchitecture.Unity
{
    public readonly struct DragViewSignalData
    {
        public DragViewSignalData(
            Vector2 position,
            Vector2 delta,
            Vector2 pressPosition,
            int pointerId)
        {
            Position = position;
            Delta = delta;
            PressPosition = pressPosition;
            PointerId = pointerId;
        }

        public Vector2 Position { get; }
        public Vector2 Delta { get; }
        public Vector2 PressPosition { get; }
        public int PointerId { get; }
    }

    [DisallowMultipleComponent]
    public sealed class DragViewSignal :
        MonoBehaviour,
        IBeginDragHandler,
        IDragHandler,
        IEndDragHandler
    {
        private readonly ViewSignal<DragViewSignalData> _began = new();
        private readonly ViewSignal<DragViewSignalData> _dragging = new();
        private readonly ViewSignal<DragViewSignalData> _ended = new();

        public ViewSignal<DragViewSignalData> Began => _began;
        public ViewSignal<DragViewSignalData> Dragging => _dragging;
        public ViewSignal<DragViewSignalData> Ended => _ended;

        public Observable<DragViewSignalData> BeganAsObservable()
            => _began.AsObservable();

        public Observable<DragViewSignalData> DraggingAsObservable()
            => _dragging.AsObservable();

        public Observable<DragViewSignalData> EndedAsObservable()
            => _ended.AsObservable();

        public void OnBeginDrag(PointerEventData eventData)
            => _began.Publish(CreateData(eventData));

        public void OnDrag(PointerEventData eventData)
            => _dragging.Publish(CreateData(eventData));

        public void OnEndDrag(PointerEventData eventData)
            => _ended.Publish(CreateData(eventData));

        private void OnDestroy()
        {
            _began.Dispose();
            _dragging.Dispose();
            _ended.Dispose();
        }

        private static DragViewSignalData CreateData(PointerEventData eventData)
        {
            return new DragViewSignalData(
                eventData.position,
                eventData.delta,
                eventData.pressPosition,
                eventData.pointerId);
        }
    }
}
