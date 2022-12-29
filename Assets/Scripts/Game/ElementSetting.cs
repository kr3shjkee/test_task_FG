using UnityEngine;

namespace Game
{
    public class ElementSetting
    {
        public readonly int ElementID;
        public readonly Vector2 LocalPosition;

        public ElementSetting(int elementID, Vector2 localPosition)
        {
            ElementID = elementID;
            LocalPosition = localPosition;
        }
    }
}