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

        public async Task<UsersDeltaResult> GetUsersDeltaAsync(string? deltaToken, CancellationToken cancellationToken = default)
        {
            var changedIds = new List<string>();
            string nextDeltaToken = string.Empty;

            string requestUrl = string.IsNullOrEmpty(deltaToken) 
                ? $"{GRAPH_URL}delta" 
                : $"{GRAPH_URL}delta?deltatoken={deltaToken}";

            var response = await _graphClient.Users.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(requestConfiguration =>
                {
                    requestConfiguration.QueryParameters.Select = ["id"];

                }, cancellationToken: cancellationToken);

            while (response != null)
            {
                if (response.Value != null)
                    changedIds.AddRange(response.Value.Select(u => u.Id!));

                if (!string.IsNullOrEmpty(response.OdataDeltaLink))
                {
                    nextDeltaToken = ExtractDeltaToken(response.OdataDeltaLink);
                    break;
                }

                if (string.IsNullOrEmpty(response.OdataNextLink)) 
                    break;

                response = await _graphClient.Users
                    .Delta
                    .WithUrl(response.OdataNextLink)
                    .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);
            }
            
            return new UsersDeltaResult(changedIds, nextDeltaToken);
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