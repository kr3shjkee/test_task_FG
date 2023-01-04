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
        private int _currentElementId;
        private int _wayNumber;

        public int PointID => _pointID;
        public Vector2 LocalPosition => _localPosition;
        
        public int CurrentElementId => _currentElementId;
        public int WayNumber => _wayNumber;
        
        public State CurrentState;

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
            SetWayNumberDefault();
            SetCurrentElementIdDefault();
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

        public void SetWayNumberDefault()
        {
            _wayNumber = 0;
        }

        public void SetWayNumber(int value)
        {
            if (value < 0)
            {
                Debug.Log("Wrong WayNumber on point " + _pointID);
                return;
            }

            _wayNumber = value;
        }

        public void SetCurrentElementIdDefault()
        {
            _currentElementId = 0;
        }

        public void SetCurrentElementId(int value)
        {
            if (value < 0)
            {
                Debug.Log("Wrong CurrentElementId on point " + _pointID);
                return;
            }

            _currentElementId = value;
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