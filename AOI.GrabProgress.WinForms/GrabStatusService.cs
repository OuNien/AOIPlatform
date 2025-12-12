using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AOI.GrabProgressWinForms
{
    public class GrabStatusService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public GrabStatusService(string baseUrl)
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(baseUrl)
            };

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<BatchInfo>> GetBatchesAsync(CancellationToken cancellationToken = default)
        {
            using var response = await _httpClient.GetAsync("/api/grabstatus/batches", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<List<BatchInfo>>(json, _jsonOptions);

            return result ?? new List<BatchInfo>();
        }

        public async Task<GrabStatusResponse?> GetBatchStatusAsync(string batchId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(batchId))
            {
                return null;
            }

            using var response = await _httpClient.GetAsync($"/api/grabstatus/batches/{Uri.EscapeDataString(batchId)}", cancellationToken);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonSerializer.Deserialize<GrabStatusResponse>(json, _jsonOptions);

            return result;
        }
    }
}
