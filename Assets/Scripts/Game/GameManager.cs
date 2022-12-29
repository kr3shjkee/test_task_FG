using System;
using System.Threading.Tasks;
using Signals.BoardSignals;
using Signals.UiSignals;
using UI;
using Zenject;

namespace Game
{
    public class GameManager : IInitializable, IDisposable
    {
        private const float FINISH_TIME = 4f;
        
        private readonly BoardController _boardController;
        private readonly SignalBus _signalBus;

        public GameManager(BoardController boardController, SignalBus signalBus)
        {
            _boardController = boardController;
            _signalBus = signalBus;
        }
        
        public void Initialize()
        {
            SubscribeSignals();
        }

        public void Dispose()
        {
            UnsubscribeSignals();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<StartGameSignal>(StartGame);
            _signalBus.Subscribe<BackToFilesSignal>(BackToFileMenu);
            _signalBus.Subscribe<RestartGameSignal>(RestartGame);
            _signalBus.Subscribe<FinishGameSignal>(FinishGame);
        }

        private void UnsubscribeSignals()
        {
            _signalBus.Unsubscribe<StartGameSignal>(StartGame);
            _signalBus.Unsubscribe<BackToFilesSignal>(BackToFileMenu);
            _signalBus.Unsubscribe<RestartGameSignal>(RestartGame);
            _signalBus.Unsubscribe<FinishGameSignal>(FinishGame);
        }

        private void StartGame()
        {
            _boardController.Initialize();
        }

        private void BackToFileMenu()
        {
            _boardController.ClearBoard();
            _boardController.Dispose();
        }

        private void RestartGame()
        {
            _boardController.ClearBoard();
            _boardController.Dispose();
            _boardController.Initialize();
        }

        private async void FinishGame()
        {
            _signalBus.Fire<ShowFinishSignal>();
            await Task.Delay(TimeSpan.FromSeconds(FINISH_TIME));
            _signalBus.Fire<BackToFilesSignal>();
        }
    }
}