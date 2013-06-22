using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace Next
{
    public static class RestClientExt
    {
        public static Task<IRestResponse<T>> ExecuteTaskAsync<T>(this RestClient client, IRestRequest request) where T : new()
        {
            var tcs = new TaskCompletionSource<IRestResponse<T>>();
            RestRequestAsyncHandle asyncHandle = client.ExecuteAsync<T>(request, tcs.SetResult);
            return tcs.Task;
        }

        public static async Task<IRestResponse<T>> ExecuteTaskAsync<T>(this RestClient client, IRestRequest request, CancellationToken cancellationToken) where T : new()
        {
            var tcs = new TaskCompletionSource<IRestResponse<T>>();
            var asyncHandle = client.ExecuteAsync<T>(request, tcs.SetResult);
            using (cancellationToken.Register(asyncHandle.Abort))
            {
                return await tcs.Task;
            }
        }
    }
}