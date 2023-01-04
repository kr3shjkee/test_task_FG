using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Signals.BoardSignals;
using UnityEngine;
using Zenject;

namespace Game
{
    public class Element : MonoBehaviour
    {
        public class Factory : PlaceholderFactory<ElementSetting, Element>
        {
        }

        public enum State
        {
            None,
            Example,
            Clicked,
            OnFinish
        }

        private const float MOVE_SPEED = 1.2f;

        [SerializeField] private SpriteRenderer elementIcon;
        [SerializeField] private GameObject elementBg;
        
        private SignalBus _signalBus;
        private int _elementID;
        private Color _elementColor;
        private Vector2 _localPosition;
        private int _pointIdForFinish;
        private Dictionary<int, Color> _colors;

        public int ElementID => _elementID;
        public Vector2 LocalPosition => _localPosition;

        public State CurrentState;
        public int PointIdForFinish => _pointIdForFinish;

        [Inject]
        public void Construct(ElementSetting elementSetting, SignalBus signalBus)
        {
            _elementID = elementSetting.ElementID;
            _localPosition = elementSetting.LocalPosition;
            _signalBus = signalBus;
        }


        public void Initialize()
        {
            InitColorDictionary();
            transform.localPosition = (Vector3)LocalPosition + new Vector3(0,0,-1);
            elementIcon.color = TryToGetColor();
            CurrentState = State.None;
        }
        
        public void SetSelected(bool isOn)
        {
            elementBg.SetActive(isOn);
        }
        
        public void DestroySelf()
        {
            Destroy(gameObject);
        }

        public async UniTask MoveToPoint(Vector2 position)
        {
            Vector3 pos = (Vector3)position + new Vector3(0, 0, -1);
            await transform.DOMove(pos, MOVE_SPEED);
        }

        public void SetPointIdForFinish(int value)
        {
            if (value < 1)
            {
                Debug.Log("Wrong pointIdForFinish on element " + _elementID);
                return;
            }

            _pointIdForFinish = value;
        }
        
        private void OnMouseUpAsButton()
        {
            OnClick();
        }
        
        private void OnClick()
        {
            _signalBus.Fire(new OnElementClickSignal(this));
        }

        private void InitColorDictionary()
        {
            _colors = new Dictionary<int, Color>
            {
                {1, Color.blue},
                {2, Color.cyan},
                {3, Color.green},
                {4, Color.red},
                {5, Color.yellow},
                {6, Color.magenta},
                {7, Color.white},
                {8, Color.grey}
            };
        }

        private Color TryToGetColor()
        {
            try
            {
                return _colors[_elementID];
            }
            catch (Exception e)
            {
                Debug.Log("Need to add more colors in dictionary");
                Debug.Log(e);
                return Color.black;
            }
        }
        
    }
}