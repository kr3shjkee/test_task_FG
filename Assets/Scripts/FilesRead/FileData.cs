using System.Collections.Generic;
using UnityEngine;

namespace FilesRead
{
    public class FileData
    {
        private int _elementsCount;
        private int _pointsCount;
        private Vector2[] _points; 
        private int[] _startElementsPos; 
        private int[] _finishElementsPos;
        private int _movesCount;
        private Dictionary<int, List<int>> _movesFromPoint;

        private char _separator = ',';

        public int ElementsCount => _elementsCount;
        public int PointsCount => _pointsCount;
        public Vector2[] Points => _points;
        public int[] StartElementsPos => _startElementsPos;
        public int[] FinishElementsPos => _finishElementsPos;
        public int MovesCount => _movesCount;
        public Dictionary<int, List<int>> MovesFromPoint => _movesFromPoint;

        public FileData(string[] fileStrings)
        {
            _elementsCount = int.Parse(fileStrings[0]);
            
            _pointsCount = int.Parse(fileStrings[1]);

            _points = new Vector2[_pointsCount];
            int fileStringCount = 2;
            for (int i = 0; i < _points.Length; i++)
            {
                var parsedStrings = fileStrings[fileStringCount].Split(_separator);
                _points[i] = new Vector2(int.Parse(parsedStrings[0]),int.Parse(parsedStrings[1]));
                fileStringCount++;
            }

            _startElementsPos = new int[_elementsCount];
            var parsedString = fileStrings[fileStringCount].Split(_separator);
            for (int i = 0; i < _startElementsPos.Length; i++)
            {
                _startElementsPos[i] = int.Parse(parsedString[i]);
            }
            fileStringCount++;
            
            _finishElementsPos = new int[_elementsCount];
            parsedString = fileStrings[fileStringCount].Split(_separator);
            for (int i = 0; i < _finishElementsPos.Length; i++)
            {
                _finishElementsPos[i] = int.Parse(parsedString[i]);
            }
            fileStringCount++;

            _movesCount = int.Parse(fileStrings[fileStringCount]);
            fileStringCount++;

            _movesFromPoint = new Dictionary<int, List<int>>();
            for (int i = 1; i < _pointsCount+1; i++)
            {
                var stringCountForCycle = fileStringCount;
                List<int> movesForPoint = new List<int>();
                for (int j = 0; j < _movesCount; j++)
                {
                    parsedString = fileStrings[stringCountForCycle].Split(_separator);
                    if (parsedString[0] == i.ToString())
                        movesForPoint.Add(int.Parse(parsedString[1]));
                    else if (parsedString[1] == i.ToString())
                        movesForPoint.Add(int.Parse(parsedString[0]));
                    stringCountForCycle++;
                }
                
                _movesFromPoint.Add(i, movesForPoint);
            }
        }
    }
}