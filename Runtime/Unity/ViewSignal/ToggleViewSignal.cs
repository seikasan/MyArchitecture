using MyArchitecture.Core;
using R3;
using UnityEngine;
using UnityEngine.UI;

namespace MyArchitecture.Unity
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Toggle))]
    public sealed class ToggleViewSignal : MonoBehaviour
    {
        [SerializeField] private Toggle toggle;

        private readonly ViewSignal<bool> _changed = new();

        public ViewSignal<bool> Changed => _changed;

        public bool IsOn => toggle != null && toggle.isOn;

        public Observable<bool> AsObservable()
            => _changed.AsObservable();

        private void Awake()
        {
            if (toggle == null)
            {
                toggle = GetComponent<Toggle>();
            }

            toggle.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnDestroy()
        {
            if (toggle != null)
            {
                toggle.onValueChanged.RemoveListener(OnValueChanged);
            }

            _changed.Dispose();
        }

        private void OnValueChanged(bool value)
            => _changed.Publish(value);
    }
}
