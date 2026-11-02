using ClickerTestTask.Features.Clicker.Presenters;
using ClickerTestTask.Features.Weather.Presenters;
using ClickerTestTask.Features.Weather.Services;
using ClickerTestTask.Features.Dogs.Presenters;
using ClickerTestTask.Features.Clicker.Config;
using ClickerTestTask.Features.Clicker.Models;
using ClickerTestTask.Features.Dogs.Services;
using ClickerTestTask.Features.Weather.Views;
using ClickerTestTask.Infrastructure.Network;
using ClickerTestTask.Features.Clicker.Views;
using ClickerTestTask.Features.Dogs.Views;
using ClickerTestTask.Core.Navigation;
using UnityEngine;
using Zenject;

namespace ClickerTestTask.Core.Installers
{
    public class SceneInstaller : MonoInstaller
    {
        [Header("Dogs Factory")]
        [SerializeField] private DogBreedItemView _dogBreedItemPrefab;
        [SerializeField] private Transform _dogListRoot;
        [SerializeField] private DogsView _dogsView;

        [Header("Clicker")]
        [SerializeField] private ClickerConfig _clickerConfig;

        public override void InstallBindings()
        {
            Container.Bind<NavigationView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.Bind<NavigationController>()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<NavigationPresenter>()
                .AsSingle();

            Container.Bind<RequestQueue>()
                .AsSingle();

            Container.Bind<UnityWebRequestClient>()
                .AsSingle();

            Container.Bind<WeatherApiService>()
                .AsSingle();

            Container.Bind<DogApiService>()
                .AsSingle();

            Container.Bind<DogsView>()
                .FromInstance(_dogsView)
                .AsSingle();

            Container.BindFactory<DogBreedItemView, DogBreedItemView.Factory>()
                .FromComponentInNewPrefab(_dogBreedItemPrefab)
                .UnderTransform(_dogListRoot);

            Container.BindInterfacesAndSelfTo<DogsPresenter>()
                .AsSingle();

            Container.Bind<WeatherView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<WeatherPresenter>()
                .AsSingle();

            Container.Bind<ClickerConfig>()
                .FromInstance(_clickerConfig)
                .AsSingle();

            Container.Bind<ClickerModel>()
                .AsSingle();

            Container.Bind<ClickerView>()
                .FromComponentInHierarchy()
                .AsSingle();

            Container.BindInterfacesAndSelfTo<ClickerPresenter>()
                .AsSingle();
        }
    }
}