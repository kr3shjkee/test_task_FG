using System;
using Signals.BoardSignals;
using UnityEngine;
using UnityEngine.Serialization;
using Zenject;

namespace Game
{
    public class Point : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<PointSetting, Point>
        {
        }
        
        public enum State
        {
            None,
            Example,
            Busy,
            Selected
        }

        [SerializeField] private GameObject pointBg;

        private Vector2 _localPosition;
        private int _pointID;
        private SignalBus _signalBus;
        

        public int PointID => _pointID;
        public Vector2 LocalPosition => _localPosition;
        public State CurrentState;
        public int CurrentElementId;
        public int WayNumber;

        [Inject]
        public void Construct(PointSetting setting, SignalBus signalBus)
        {
            _pointID = setting.PointID;
            _localPosition = setting.LocalPosition;
            _signalBus = signalBus;
        }

        public void Initialize() 
        {
            float multiplierX = 1f;
            float multiplierY = 1f;
            if (_localPosition.x.ToString().Length > 1)
            {
                float coef = _localPosition.x.ToString().Length-1;
                multiplierX = (float) Math.Pow(0.1f, coef);
            }
            if (_localPosition.y.ToString().Length > 1)
            {
                float coef = _localPosition.y.ToString().Length-1;
                multiplierY = (float) Math.Pow(0.1f, coef);
            }

            transform.localPosition = new Vector3(_localPosition.x * multiplierX, 
                _localPosition.y * multiplierY, 0f);
            CurrentState = State.None;
            CurrentElementId = 0;
        }

        public void SetSelected(bool isOn)
        {
            pointBg.SetActive(isOn);
            if (isOn)
                CurrentState = State.Selected;
            else
                CurrentState = State.None;
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }
        
        private void OnMouseUpAsButton()
        {
            OnClick();
        }
        
        private void OnClick()
        {
            if (CurrentState == State.Selected)
                _signalBus.Fire(new OnPointForMoveClickSignal(_pointID));
        }
    }
}