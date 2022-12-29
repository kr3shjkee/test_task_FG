using Base;
using Signals.UiSignals;

namespace UI
{
    public class SecondVariantButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<SecondVariantShowSignal>();
        }
    }
}