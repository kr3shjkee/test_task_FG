using Base;
using Signals.UiSignals;

namespace UI
{
    public class FirstVariantButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<FirstVariantShowSignal>();
        }
    }
}