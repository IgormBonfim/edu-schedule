using System.Web;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using Microsoft.Graph;

namespace EduSchedule.Infrastructure.Integrations.Services
{
    public class GraphService : IGraphService
    {
        private readonly GraphServiceClient _graphClient;
        private const string GRAPH_URL = "https://graph.microsoft.com/v1.0/";

        public GraphService(GraphServiceClient graphClient)
        {
            _graphClient = graphClient;
        }

        public async Task<UsersDeltaResult> GetUsersDeltaAsync(string? deltaToken = null, string? deltaLink = null, int top = 999, CancellationToken cancellationToken = default)
        {
            var changedIds = new List<string>();
            string nextDeltaToken = string.Empty;

            string requestUrl = deltaLink;

            if (string.IsNullOrEmpty(requestUrl))
            {
                requestUrl = string.IsNullOrEmpty(deltaToken) 
                    ? $"{GRAPH_URL}users/delta?$select=id&$top={top}"
                    : $"{GRAPH_URL}users/delta?deltatoken={deltaToken}";
            }

            var response = await _graphClient.Users.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);

            if (response != null)
            {
                if (response.Value != null)
                    changedIds.AddRange(response.Value.Select(u => u.Id!));
                
                if (!string.IsNullOrEmpty(response.OdataDeltaLink))
                    nextDeltaToken = ExtractDeltaToken(response.OdataDeltaLink);

                return new UsersDeltaResult(changedIds, nextDeltaToken, response.OdataNextLink);
            }
            
            return new UsersDeltaResult(changedIds, null, null);
        }

        public async Task<EventsDeltaResults> GetEventsDeltaAsync(string studentId, string? deltaToken, CancellationToken cancellationToken = default)
        {
            string requestUrl = string.IsNullOrEmpty(deltaToken)
                ? $"{GRAPH_URL}users/{studentId}/events/delta"
                : $"{GRAPH_URL}users/{studentId}/events/delta?deltatoken={deltaToken}";

            var response = await _graphClient.Users[studentId].Events.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);

            throw new Exception();
        }

        private string ExtractDeltaToken(string? url)
        {
            if (string.IsNullOrEmpty(url)) return string.Empty;

            var uri = new Uri(url);
            var query = HttpUtility.ParseQueryString(uri.Query);
            return query.Get("deltatoken") ?? string.Empty;
        }
    }
}