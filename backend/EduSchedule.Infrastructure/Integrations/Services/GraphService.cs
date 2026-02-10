using System.Web;
using EduSchedule.Domain.Integrations.Models;
using EduSchedule.Domain.Integrations.Services.Interfaces;
using Microsoft.Graph;
using Microsoft.Graph.Models;

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
            var changedUsers = new List<UserResult>();
            string nextDeltaToken = string.Empty;

            string? requestUrl = deltaLink;

            if (string.IsNullOrEmpty(requestUrl))
            {
                requestUrl = string.IsNullOrEmpty(deltaToken) 
                    ? $"{GRAPH_URL}users/delta?$select=id,displayName,mail&$top={top}"
                    : $"{GRAPH_URL}users/delta?deltatoken={deltaToken}";
            }

            var response = await _graphClient.Users.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);

            if (response?.Value != null)
            {
                foreach (var user in response.Value)
                {
                    bool isDeleted = user.AdditionalData != null && 
                                        user.AdditionalData.ContainsKey("@removed");

                    changedUsers.Add(new UserResult(
                        user.Id!, 
                        user.DisplayName!, 
                        user.Mail!, 
                        isDeleted
                    ));
                }
                
                if (!string.IsNullOrEmpty(response.OdataDeltaLink))
                    nextDeltaToken = ExtractDeltaToken(response.OdataDeltaLink);

                return new UsersDeltaResult(changedUsers, nextDeltaToken, response.OdataNextLink);
            }
            
            return new UsersDeltaResult(changedUsers, null, null);
        }

        public async Task<EventsDeltaResult> GetEventsDeltaAsync(string studentId, string? deltaToken, CancellationToken cancellationToken = default)
        {
            string requestUrl = string.IsNullOrEmpty(deltaToken)
                ? $"{GRAPH_URL}users/{studentId}/events/delta"
                : $"{GRAPH_URL}users/{studentId}/events/delta?deltatoken={deltaToken}";

            var response = await _graphClient.Users[studentId].Events.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);

            throw new Exception();
        }

        public async Task<UserResult?> GetUserAsync(string userId, CancellationToken cancellationToken = default)
        {
            var response = await _graphClient.Users[userId].GetAsync();

            if (response == null)
                return null;

            return new UserResult(response.Id!, response.DisplayName!, response.Mail!, false);
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