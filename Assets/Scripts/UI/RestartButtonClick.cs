using Base;
using Signals.UiSignals;

namespace UI
{
    public class RestartButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<RestartGameSignal>();
        }
    }
}