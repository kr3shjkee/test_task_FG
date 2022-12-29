using FilesRead;
using Game;
using Signals.BoardSignals;
using Signals.UiSignals;
using UI;
using UnityEngine;

namespace Zenject
{
    public class MainSceneInstaller : MonoInstaller
    {
        [SerializeField] private UiManager uiManagerPrefab;
        [SerializeField] private Point pointPrefab;
        [SerializeField] private Element elementPrefab;
        [SerializeField] private Line linePrefab;
        
        public override void InstallBindings()
        {
            
            SignalBusInstaller.Install(Container);

            Container.Bind<UiManager>().FromComponentInNewPrefab(uiManagerPrefab).AsSingle().NonLazy();
            Container.Bind<FileReader>().AsSingle().NonLazy();
            
            Container.BindFactory<PointSetting, Point, Point.Factory>().FromComponentInNewPrefab(pointPrefab);
            Container.BindFactory<ElementSetting, Element, Element.Factory>().FromComponentInNewPrefab(elementPrefab);
            Container.BindFactory<Line, Line.Factory>().FromComponentInNewPrefab(linePrefab);
            Container.Bind<BoardController>().AsSingle().NonLazy();
            
            Container.BindInterfacesAndSelfTo<GameManager>().AsSingle().NonLazy();
            
            BindUiSignals();
            BindLogicSignals();
        }

        private void BindUiSignals()
        {
            Container.DeclareSignal<FirstVariantShowSignal>();
            Container.DeclareSignal<SecondVariantShowSignal>();
            Container.DeclareSignal<ThirdVariantShowSignal>();
            Container.DeclareSignal<AcceptFileSignal>();
            Container.DeclareSignal<RestartGameSignal>();
            Container.DeclareSignal<BackToFilesSignal>();
            Container.DeclareSignal<FileErrorSignal>();
            Container.DeclareSignal<CloseErrorPanelSignal>();
            Container.DeclareSignal<ShowFinishSignal>();
        }

        private void BindLogicSignals()
        {
            Container.DeclareSignal<StartGameSignal>();
            Container.DeclareSignal<OnElementClickSignal>();
            Container.DeclareSignal<OnPointForMoveClickSignal>();
            Container.DeclareSignal<FinishGameSignal>();
        }
    }
}