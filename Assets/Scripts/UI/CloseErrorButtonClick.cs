using Base;
using Signals.UiSignals;

namespace UI
{
    public class CloseErrorButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<CloseErrorPanelSignal>();
        }
    }
}