using MyArchitecture.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyArchitecture.Unity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Button))]
    public sealed class ButtonViewSignal : MonoBehaviour
    {
        [SerializeField] private Button button;

        private readonly ViewSignal _clicked = new();

        public ViewSignal Clicked => _clicked;

        public Observable<Unit> AsObservable()
            => _clicked.AsObservable();

        private void Awake()
        {
            if (button == null)
            {
                button = GetComponent<Button>();
            }

            button.onClick.AddListener(OnClicked);
        }

        private void OnDestroy()
        {
            if (button != null)
            {
                button.onClick.RemoveListener(OnClicked);
            }

            _clicked.Dispose();
        }

        private void OnClicked()
            => _clicked.Publish();
    }
}
