using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace VehicleTracker.Api.Storage.Rest
{
    public class RestRepository<TKey, T, TFilter> : IRepository<TKey, T, TFilter>
    {
        private readonly HttpClient _client;

        private readonly IQueryStringFilterTransformer<TFilter> _filterTransformer;

        public RestRepository(string baseUri, IQueryStringFilterTransformer<TFilter> filterTransformer)
        {
            _client = new HttpClient
            {
                BaseAddress = new Uri(baseUri)
            };
            _filterTransformer = filterTransformer;
        }

        public async Task<T> Create(T item)
        {
            var result = await _client.PostAsJsonAsync("", item);
            result.EnsureSuccessStatusCode();
            return item;
        }

        public async Task Delete(TKey id)
        {
            var result = await _client.DeleteAsync(id.ToString());
            result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<T>> List(TFilter filter)
        {
            var filterQuery = _filterTransformer.Transform(filter);
            var result = await _client.GetAsync(filterQuery);
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<IEnumerable<T>>();
        }

        public async Task<T> Get(TKey id)
        {
            var result = await _client.GetAsync(id.ToString());
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<T>();
        }

        public async Task<T> Update(TKey id, T item)
        {
            var result = await _client.PutAsJsonAsync(id.ToString(), item);
            result.EnsureSuccessStatusCode();
            return item;
        }
    }
}
