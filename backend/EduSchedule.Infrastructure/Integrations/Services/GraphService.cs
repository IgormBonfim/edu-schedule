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

        public async Task<DeltaResults<UserResult>> GetUsersDeltaAsync(string? deltaLink = null, int top = 999, CancellationToken cancellationToken = default)
        {
            var changedUsers = new List<UserResult>();
            string? nextDeltaLink = null;
            string? nextDeltaToken = null;

            string? requestUrl = deltaLink;

            if (string.IsNullOrEmpty(requestUrl))
                requestUrl = $"{GRAPH_URL}users/delta?$select=id,displayName,mail&$top={top}";

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
                
                nextDeltaLink = response.OdataNextLink;
                if (!string.IsNullOrEmpty(response.OdataDeltaLink))
                {
                    nextDeltaLink = response.OdataDeltaLink;
                    nextDeltaToken = ExtractDeltaToken(response.OdataDeltaLink);
                }

                return new DeltaResults<UserResult>(changedUsers, nextDeltaToken, nextDeltaLink);
            }
            
            return new DeltaResults<UserResult>(changedUsers, null, null);
        }

        public async Task<DeltaResults<EventResult>> GetUserEventsDeltaAsync(
            string studentId, 
            string? deltaLink = null, 
            CancellationToken cancellationToken = default)
        {
            var changedEvents = new List<EventResult>();
            string? requestUrl = deltaLink;

            if (string.IsNullOrEmpty(requestUrl))
            {
                var start = "2000-01-01T00:00:00Z";
                var end = "2026-12-31T23:59:59Z";
                
                requestUrl = $"{GRAPH_URL}users/{studentId}/calendarView/delta" +
                            $"?startDateTime={start}&endDateTime={end}" +
                            "&$select=id,subject,start,end";
            }

            var response = await _graphClient.Users[studentId].CalendarView.Delta
                .WithUrl(requestUrl)
                .GetAsDeltaGetResponseAsync(cancellationToken: cancellationToken);

            if (response?.Value != null)
            {
                foreach (var ev in response.Value)
                {
                    bool isDeleted = ev.AdditionalData != null && 
                                    ev.AdditionalData.ContainsKey("@removed");

                    changedEvents.Add(new EventResult(
                        ev.Id!,
                        ev.Subject ?? "Sem Assunto",
                        ev.Start?.ToDateTimeOffset(),
                        ev.End?.ToDateTimeOffset(),
                        isDeleted
                    ));
                }

                var nextLink = response.OdataNextLink ?? response.OdataDeltaLink;

                return new DeltaResults<EventResult>(changedEvents, response.OdataDeltaLink, nextLink);
            }

            return new DeltaResults<EventResult>(changedEvents, null, null);
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