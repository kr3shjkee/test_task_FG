using UnityEngine;

namespace Game
{
    public class PointSetting
    {
        public readonly Vector2 LocalPosition;
        public readonly int PointID;

        public PointSetting(int pointID, Vector2 localPosition)
        {
            PointID = pointID;
            LocalPosition = localPosition;
        }
    }
}