using ClickerTestTask.Infrastructure.Network;
using ClickerTestTask.Features.Dogs.Data;

namespace ClickerTestTask.Features.Dogs.Services
{
    public sealed class DogApiService
    {
        private const string BaseUrl = "https://dogapi.dog/api/v2/breeds";

        private readonly RequestQueue _requestQueue;
        private readonly UnityWebRequestClient _httpClient;

        public DogApiService(
            RequestQueue requestQueue,
            UnityWebRequestClient httpClient)
        {
            _requestQueue = requestQueue;
            _httpClient = httpClient;
        }

        public RequestHandle<DogBreedListResponse> GetBreeds()
        {
            string url = BaseUrl + "?page%5Bsize%5D=10&page%5Bnumber%5D=1";

            return _requestQueue.Enqueue(
                token => _httpClient.GetAsync<DogBreedListResponse>(
                    url,
                    token));
        }

        public RequestHandle<DogBreedResponse> GetBreedById(string id)
        {
            string url = $"{BaseUrl}/{id}";

            return _requestQueue.Enqueue(
                token => _httpClient.GetAsync<DogBreedResponse>(
                    url,
                    token));
        }
    }
}