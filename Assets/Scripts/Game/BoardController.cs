using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using FilesRead;
using Mono.Cecil;
using Signals.BoardSignals;
using UnityEngine;
using Zenject;

namespace Game
{
    public class BoardController 
    {
        private const float POINTS_OFFSET = 4f;
        
        private readonly Point.Factory _pointFactory;
        private readonly Element.Factory _elementFactory;
        private readonly Line.Factory _lineFactory;
        private readonly SignalBus _signalBus;
        private readonly FileReader _fileReader;

        private Point[] _points;
        private Point[] _examplePoints;
        private Element[] _elements;
        private Element[] _exampleElements;
        private List<Line> _lines = new List<Line>();
        private bool _isBoardBlocked;

        public BoardController(Point.Factory pointFactory, Element.Factory elementFactory, SignalBus signalBus, 
            FileReader fileReader, Line.Factory lineFactory)
        {
            _pointFactory = pointFactory;
            _elementFactory = elementFactory;
            _lineFactory = lineFactory;
            _signalBus = signalBus;
            _fileReader = fileReader;
            
        }
        
        public void Initialize()
        {
            _signalBus.Subscribe<OnElementClickSignal>(OnElementClick);
            _signalBus.Subscribe<OnPointForMoveClickSignal>(GenerateMoveWay);
            GenerateMainPoints();
            GenerateExamplePoints();
            GenerateViewPointLines(_points);
            GenerateViewPointLines(_examplePoints);
            GenerateMainElements();
            GenerateExampleElements();
        }

        public void Dispose()
        {
            _signalBus.Unsubscribe<OnElementClickSignal>(OnElementClick);
            _signalBus.Unsubscribe<OnPointForMoveClickSignal>(GenerateMoveWay);
        }

        public void ClearBoard()
        {
            foreach (var point in _points)
                point.DestroySelf();
            _points = null;

            foreach (var point in _examplePoints)
                point.DestroySelf();
            _examplePoints = null;

            foreach (var element in _elements)
                element.DestroySelf();
            _elements = null;

            foreach (var element in _exampleElements)
                element.DestroySelf();
            _exampleElements = null;

            foreach (var line in _lines)
                line.DestroySelf();
            _lines.Clear();
        }

        private void GenerateMainPoints()
        {
            _points = new Point[_fileReader.Data.PointsCount];
            for (int i = 0; i < _points.Length; i++)
            {
                var point = _pointFactory.Create(new PointSetting(i+1, _fileReader.Data.Points[i]));
                point.Initialize();
                _points[i] = point;
            }
        }
        
        private void GenerateExamplePoints()
        {
            _examplePoints = new Point[_points.Length];
            for (int i = 0; i < _points.Length; i++)
            {
                var point = _pointFactory.Create(new PointSetting(i+1, _points[i].LocalPosition));
                point.Initialize();
                point.transform.localPosition += new Vector3(-3, -5, 0);
                point.CurrentState = Point.State.Example;
                _examplePoints[i] = point;
            }
        }

        private void GenerateMainElements()
        {
            _elements = new Element[_fileReader.Data.ElementsCount];
            for (int i = 0; i < _elements.Length; i++)
            {
                var point = _points[_fileReader.Data.StartElementsPos[i]-1];
                var element = _elementFactory.Create(new ElementSetting(i + 1, point.transform.localPosition));
                element.Initialize();
                point.CurrentState = Point.State.Busy;
                point.CurrentElementId = element.ElementID;
                element.pointIdForFinish = _fileReader.Data.FinishElementsPos[i];
                _elements[i] = element;
            }
        }

        private void GenerateExampleElements()
        {
            _exampleElements = new Element[_fileReader.Data.ElementsCount];
            for (int i = 0; i < _exampleElements.Length; i++)
            {
                var point = _examplePoints[_fileReader.Data.FinishElementsPos[i]-1];
                var element = _elementFactory.Create(new ElementSetting(i + 1, point.transform.localPosition));
                element.Initialize();
                element.CurrentState = Element.State.Example;
                _exampleElements[i] = element;
            }
        }

        private void GenerateViewPointLines(Point[] points)
        {
            foreach (var point in points)
            {
                foreach (var move in _fileReader.Data.MovesFromPoint[point.PointID])
                {
                    var line = _lineFactory.Create();
                    line.Initialize(point.transform.localPosition, points[move-1].transform.localPosition);
                    _lines.Add(line);
                }
            }
        }

        private void OnElementClick(OnElementClickSignal signal)
        {
            
            if (_isBoardBlocked || signal.Element.CurrentState != Element.State.None)
                return;

            signal.Element.CurrentState = Element.State.Clicked;
            
            foreach (var element in _elements)
            {
                element.SetSelected(false);
                if (element.CurrentState != Element.State.OnFinish)
                    element.CurrentState = Element.State.None;
            }

            foreach (var point in _points)
            {
                if (point.CurrentState == Point.State.Selected)
                    point.SetSelected(false);
            }
            signal.Element.SetSelected(true);
            signal.Element.CurrentState = Element.State.Clicked;
            FindPointsForMove(signal.Element.ElementID);
        }

        private void FindPointsForMove(int elementID)
        {
            var startPoint = GetPointIdByElement(elementID);
            List<int> firstPointsForMove = new List<int>();
            firstPointsForMove.AddRange(_fileReader.Data.MovesFromPoint[startPoint]);
            List<int> tempList = new List<int>();
            bool isHavePointsForSearch = true;
            while (isHavePointsForSearch)
            {
                foreach (var point in firstPointsForMove)
                {
                    if (_points[point - 1].CurrentState == Point.State.None)
                    {
                        _points[point-1].SetSelected(true);
                        tempList.Add(_points[point-1].PointID);
                    }
                }

                if (tempList.Count>0)
                {
                    firstPointsForMove.Clear();
                    foreach (var point in tempList)
                    {
                        firstPointsForMove.AddRange(_fileReader.Data.MovesFromPoint[point]);
                    }
                }
                else if (tempList.Count == 0)
                    isHavePointsForSearch = false;
                
                tempList.Clear();
            }
        }

        private int GetPointIdByElement(int elementID)
        {
            foreach (var point in _points)
            {
                if (elementID == point.CurrentElementId)
                    return point.PointID;
            }

            return 0;
        }

        private async void GenerateMoveWay(OnPointForMoveClickSignal signal) 
        {
            if (_isBoardBlocked)
                return;

            _isBoardBlocked = true;
            
            var finishPoint = _points.FirstOrDefault(pnt => pnt.PointID == signal.PointID);
            var elementForMove = _elements.FirstOrDefault(elem => elem.CurrentState == Element.State.Clicked);
            var firstPoint = GetPointIdByElement(elementForMove.ElementID);
            List<int> selectedPoints = new List<int>();
            foreach (var point in _points)
            {
                if (point.CurrentState == Point.State.Selected)
                    selectedPoints.Add(point.PointID);
            }

            List<int> firstPointsForMove = new List<int>();
            List<int> pointsAfterMove = new List<int>();
            Stack<Point> wayPoints = new Stack<Point>();
            wayPoints.Push(finishPoint);
            int price = 1;
            firstPointsForMove.AddRange(_fileReader.Data.MovesFromPoint[firstPoint]);

            bool isAlgorithmEnd = false;
            while (true)
            {
                
                foreach (var point in firstPointsForMove)
                {
                    var currentPoint = _points.FirstOrDefault(pnt => pnt.PointID == point);
                    if (currentPoint.CurrentState == Point.State.Selected)
                    {
                        if (currentPoint.PointID == finishPoint.PointID)
                        {
                            currentPoint.WayNumber = price;
                            currentPoint.CurrentState = Point.State.None;
                            pointsAfterMove.Clear();
                            break;
                        }
                        currentPoint.WayNumber = price;
                        currentPoint.CurrentState = Point.State.None;
                        pointsAfterMove.AddRange(_fileReader.Data.MovesFromPoint[point]);
                    }
                    
                }

                if (pointsAfterMove.Count == 0)
                    break;
                
                price++;
                firstPointsForMove.Clear();
                firstPointsForMove.AddRange(pointsAfterMove);
                pointsAfterMove.Clear();
            }
                
            bool isWayStackReady = false;
            while (!isWayStackReady)
            {
                foreach (var point in _fileReader.Data.MovesFromPoint[wayPoints.Peek().PointID])
                {
                    var currentPoint = _points.FirstOrDefault(pnt => pnt.PointID == point);
                    var diff = wayPoints.Peek().WayNumber - currentPoint.WayNumber;
                    if (diff == 1 && currentPoint.PointID!=firstPoint && currentPoint.WayNumber!=0)
                    {
                        wayPoints.Push(currentPoint);
                        break;
                    }
                    else if (currentPoint.PointID == firstPoint)
                    {
                        isWayStackReady = true;
                        break;
                    }
                }
            }

            while (wayPoints.Count>0)
            {
                await Move(elementForMove, wayPoints.Pop().PointID);
            }

            
            elementForMove.SetSelected(false);
            elementForMove.CurrentState = Element.State.None;
            foreach (var point in _points)
            {
                point.WayNumber = 0;
                if (point.CurrentState == Point.State.Selected)
                    point.SetSelected(false);
            }
            _isBoardBlocked = false;
            CheckBoardOnWin();
        }

        private async UniTask Move(Element element, int pointId)
        {
            var pointWithElement = _points.FirstOrDefault(point => point.CurrentElementId == element.ElementID);
            var nextPoint = _points.FirstOrDefault(point => point.PointID == pointId);
            pointWithElement.CurrentElementId = 0;
            pointWithElement.CurrentState = Point.State.None;
            nextPoint.CurrentElementId = element.ElementID;
            nextPoint.SetSelected(false);
            nextPoint.CurrentState = Point.State.Busy;
            await element.MoveToPoint(nextPoint.transform.localPosition);
        }

        private void CheckBoardOnWin()
        {
            int finishedElementsCount = 0;
            foreach (var element in _elements)
            {
                if (element.pointIdForFinish == GetPointIdByElement(element.ElementID))
                {
                    element.CurrentState = Element.State.OnFinish;
                    finishedElementsCount++;
                }
            }

            if (finishedElementsCount == _elements.Length)
                _signalBus.Fire<FinishGameSignal>();
            else
                Debug.Log(finishedElementsCount + " elements finished from " + _elements.Length);
        }
    }
}