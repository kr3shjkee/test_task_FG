﻿using Game;

namespace Signals.BoardSignals
{
    public class OnElementClickSignal
    {
        public readonly Element Element;

        public OnElementClickSignal(Element element)
        {
            Element = element;
        }
    }
}