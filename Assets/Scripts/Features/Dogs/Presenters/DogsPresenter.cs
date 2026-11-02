using ClickerTestTask.Features.Dogs.Services;
using ClickerTestTask.Infrastructure.Network;
using ClickerTestTask.Features.Dogs.Views;
using ClickerTestTask.Features.Dogs.Data;
using ClickerTestTask.Core.Navigation;
using Cysharp.Threading.Tasks;
using UnityEngine;
using Zenject;
using System;

namespace ClickerTestTask.Features.Dogs.Presenters
{
    public sealed class DogsPresenter : IInitializable, IDisposable
    {
        private readonly DogsView _view;
        private readonly DogApiService _dogApi;
        private readonly DogBreedItemView.Factory _itemFactory;
        private readonly NavigationController _navigation;

        private RequestHandle<DogBreedListResponse> _listHandle;
        private RequestHandle<DogBreedResponse> _detailsHandle;

        private bool _isActive;
        private string _selectedBreedId;

        public DogsPresenter(
            DogsView view,
            DogApiService dogApi,
            DogBreedItemView.Factory itemFactory,
            NavigationController navigation)
        {
            _view = view;
            _dogApi = dogApi;
            _itemFactory = itemFactory;
            _navigation = navigation;
        }

        public void Initialize()
        {
            _navigation.TabChanged += OnTabChanged;
            _view.CloseClicked += OnPopupCloseClicked;

            if (_navigation.CurrentTab == NavigationTab.Dogs)
                EnterTab();
        }

        public void Dispose()
        {
            _navigation.TabChanged -= OnTabChanged;
            _view.CloseClicked -= OnPopupCloseClicked;

            ExitTab();
        }

        private void OnTabChanged(NavigationTab tab)
        {
            if (tab == NavigationTab.Dogs)
                EnterTab();
            else
                ExitTab();
        }

        private void EnterTab()
        {
            if (_isActive)
                return;

            _isActive = true;
            LoadBreedsAsync().Forget();
        }

        private void ExitTab()
        {
            if (!_isActive)
                return;

            _isActive = false;

            _listHandle?.Cancel();
            _detailsHandle?.Cancel();

            _listHandle = null;
            _detailsHandle = null;
            _selectedBreedId = null;

            _view.ShowLoader(false);
            _view.HidePopup();
        }

        private async UniTaskVoid LoadBreedsAsync()
        {
            _view.ClearList();
            _view.ShowLoader(true);

            var handle = _dogApi.GetBreeds();
            _listHandle = handle;

            try
            {
                DogBreedListResponse response = await handle.Result;

                if (!_isActive || _listHandle != handle)
                    return;

                if (response?.data == null)
                {
                    Debug.LogError("Dog API вернул пустой список пород.");
                    return;
                }

                foreach (DogBreedDto breed in response.data)
                {
                    if (breed?.attributes == null)
                        continue;

                    DogBreedItemView item = _itemFactory.Create();

                    item.Initialize(
                        breed.id,
                        breed.attributes.name
                    );

                    item.BreedClicked += OnBreedClicked;
                }
            }
            catch (OperationCanceledException)
            {
                // Запрос отменён.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);
            }
            finally
            {
                if (_listHandle == handle)
                {
                    _listHandle = null;
                    _view.ShowLoader(false);
                }
            }
        }

        private void OnBreedClicked(string breedId)
        {
            LoadBreedDetailsAsync(breedId).Forget();
        }

        private async UniTaskVoid LoadBreedDetailsAsync(string breedId)
        {
            if (string.IsNullOrEmpty(breedId))
                return;

            _detailsHandle?.Cancel();
            _detailsHandle = null;

            _selectedBreedId = breedId;

            _view.ShowPopupLoader();

            var handle = _dogApi.GetBreedById(breedId);
            _detailsHandle = handle;

            try
            {
                DogBreedResponse response = await handle.Result;

                if (!_isActive ||
                    _detailsHandle != handle ||
                    _selectedBreedId != breedId)
                    return;

                if (response?.data?.attributes == null)
                {
                    _view.ShowPopupError(
                        "Не удалось получить информацию о породе."
                    );
                    return;
                }

                _view.ShowPopup(
                    response.data.attributes.name,
                    response.data.attributes.description
                );
            }
            catch (OperationCanceledException)
            {
                // Предыдущий запрос отменён.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception);

                if (_isActive &&
                    _detailsHandle == handle &&
                    _selectedBreedId == breedId)
                {
                    _view.ShowPopupError(
                        "Не удалось загрузить информацию о собаке."
                    );
                }
            }
            finally
            {
                if (_detailsHandle == handle)
                    _detailsHandle = null;
            }
        }

        private void OnPopupCloseClicked()
        {
            _detailsHandle?.Cancel();
            _detailsHandle = null;
            _selectedBreedId = null;

            _view.HidePopup();
        }
    }
}