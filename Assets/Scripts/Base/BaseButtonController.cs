using System;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Base
{
    public abstract class BaseButtonController : MonoBehaviour
    {
        protected SignalBus _signalBus;
        
        [Inject]
        public void Construct(SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        private Button _button;

        protected void Awake()
        {
            _button = GetComponent<Button>();
        }

        protected void Start()
        {
            _button.onClick.AddListener(OnClick);
        }

        protected void OnDestroy()
        {
            _button.onClick.RemoveListener(OnClick);
        }

        protected abstract void OnClick();
    }
}