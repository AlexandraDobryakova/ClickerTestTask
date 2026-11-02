using Cysharp.Threading.Tasks;
using UnityEngine.Networking;
using System.Threading;
using UnityEngine;
using System;

namespace ClickerTestTask.Infrastructure.Network
{
    public class UnityWebRequestClient
    {
        public async UniTask<T> GetAsync<T>(
            string url,
            CancellationToken cancellationToken,
            string accept = "application/json")
        {
            using (var request = UnityWebRequest.Get(url))
            {
                request.timeout = 20;
                request.SetRequestHeader("Accept", accept);
                request.SetRequestHeader(
                    "User-Agent",
                    "ClickerTestTask/1.0");

                try
                {
                    await request.SendWebRequest()
                        .ToUniTask(cancellationToken: cancellationToken);

                    if (request.result != UnityWebRequest.Result.Success)
                    {
                        throw new Exception(
                            $"HTTP request failed. " +
                            $"URL: {url}\n" +
                            $"Result: {request.result}\n" +
                            $"Response code: {request.responseCode}\n" +
                            $"Error: {request.error}\n" +
                            $"Response: {request.downloadHandler?.text}");
                    }

                    var json = request.downloadHandler.text;

                    Debug.Log($"GET SUCCESS: {url}");
                    Debug.Log($"Response: {json}");

                    var result = JsonUtility.FromJson<T>(json);

                    if (result == null)
                        throw new Exception($"JSON parsing returned null. URL: {url}");

                    return result;
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception exception)
                {
                    Debug.LogError(
                        $"GET REQUEST ERROR\n" +
                        $"URL: {url}\n" +
                        $"Result: {request.result}\n" +
                        $"Response code: {request.responseCode}\n" +
                        $"Error: {request.error}\n" +
                        $"Response: {request.downloadHandler?.text}\n" +
                        $"Exception: {exception}");

                    throw;
                }
            }
        }

        public async UniTask<Texture2D> GetTextureAsync(
            string url,
            CancellationToken cancellationToken)
        {
            using (var request = UnityWebRequestTexture.GetTexture(url))
            {
                request.timeout = 20;

                await request.SendWebRequest()
                    .ToUniTask(cancellationToken: cancellationToken);

                if (request.result != UnityWebRequest.Result.Success)
                {
                    throw new Exception(
                        $"Texture request failed: {url}\n" +
                        $"Error: {request.error}");
                }

                return DownloadHandlerTexture.GetContent(request);
            }
        }
    }
}