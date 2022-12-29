using Base;
using Signals.UiSignals;

namespace UI
{
    public class BackToFilesButtonClick : BaseButtonController
    {
        protected override void OnClick()
        {
            _signalBus.Fire<BackToFilesSignal>();
        }
    }
}