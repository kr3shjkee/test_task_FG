using Base;
using Signals.UiSignals;

namespace UI
{
    public class ThirdVariantButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<ThirdVariantShowSignal>();
        }
    }
}