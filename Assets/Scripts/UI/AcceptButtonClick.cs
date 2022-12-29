using Base;
using Signals.UiSignals;

namespace UI
{
    public class AcceptButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<AcceptFileSignal>();
        }
    }
}