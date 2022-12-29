using System;
using FilesRead;
using Signals.BoardSignals;
using Signals.UiSignals;
using TMPro;
using UnityEngine;
using Zenject;

namespace UI
{
    public class UiManager : MonoBehaviour
    {
        [SerializeField] private GameObject fileChoosePanel;
        [SerializeField] private GameObject fileViewObject;
        [SerializeField] private GameObject gamePanel;
        [SerializeField] private TextMeshProUGUI fileText;
        [SerializeField] private GameObject errorPanel;
        [SerializeField] private GameObject acceptButton;
        [SerializeField] private GameObject finishPanel;

        private SignalBus _signalBus;
        private FileReader _fileReader;

        [Inject]
        public void Construct(SignalBus signalBus, FileReader fileReader)
        {
            _signalBus = signalBus;
            _fileReader = fileReader;
        }

        private void Start()
        {
            SubscribeSignals();
            InitFileChooseUi();
        }

        private void OnDestroy()
        {
            UnsubscribeSignals();
        }

        private void SubscribeSignals()
        {
            _signalBus.Subscribe<FirstVariantShowSignal>(FirstVariantShow);
            _signalBus.Subscribe<SecondVariantShowSignal>(SecondVariantShow);
            _signalBus.Subscribe<ThirdVariantShowSignal>(ThirdVariantShow);
            _signalBus.Subscribe<AcceptFileSignal>(StartGame);
            _signalBus.Subscribe<BackToFilesSignal>(InitFileChooseUi);
            _signalBus.Subscribe<FileErrorSignal>(ShowError);
            _signalBus.Subscribe<CloseErrorPanelSignal>(CloseErrorWindow);
            _signalBus.Subscribe<ShowFinishSignal>(ShowMainMenuAfterFinish);
        }

        private void UnsubscribeSignals()
        {
            _signalBus.Unsubscribe<FirstVariantShowSignal>(FirstVariantShow);
            _signalBus.Unsubscribe<SecondVariantShowSignal>(SecondVariantShow);
            _signalBus.Unsubscribe<ThirdVariantShowSignal>(ThirdVariantShow);
            _signalBus.Unsubscribe<AcceptFileSignal>(StartGame);
            _signalBus.Unsubscribe<BackToFilesSignal>(InitFileChooseUi);
            _signalBus.Unsubscribe<FileErrorSignal>(ShowError);
            _signalBus.Unsubscribe<CloseErrorPanelSignal>(CloseErrorWindow);
            _signalBus.Unsubscribe<ShowFinishSignal>(ShowMainMenuAfterFinish);
        }

        private void InitFileChooseUi()
        {
            fileChoosePanel.SetActive(true);
            fileText.text = "";
            fileViewObject.SetActive(false);
            gamePanel.SetActive(false);
            finishPanel.SetActive(false);
        }

        private void FirstVariantShow()
        {
            fileViewObject.SetActive(true);
            acceptButton.SetActive(true);
            fileText.text = "";
            foreach (string fileString in _fileReader.GetFileText(1))
            {
                fileText.text += fileString + "\n";
            }
        }

        private void SecondVariantShow()
        {
            fileViewObject.SetActive(true);
            acceptButton.SetActive(true);
            fileText.text = "";
            foreach (string fileString in _fileReader.GetFileText(2))
            {
                fileText.text += fileString + "\n";
            }
        }

        private void ThirdVariantShow()
        {
            fileViewObject.SetActive(true);
            acceptButton.SetActive(true);
            fileText.text = "";
            foreach (string fileString in _fileReader.GetFileText(3))
            {
                fileText.text += fileString + "\n";
            }
        }

        private void StartGame()
        {
            fileChoosePanel.SetActive(false);
            gamePanel.SetActive(true);
            _signalBus.Fire<StartGameSignal>();
        }

        private void ShowError()
        {
            acceptButton.SetActive(false);
            errorPanel.SetActive(true);
        }

        private void CloseErrorWindow()
        {
            errorPanel.SetActive(false);
        }

        private void ShowMainMenuAfterFinish()
        {
            finishPanel.SetActive(true);
        }
    }
}