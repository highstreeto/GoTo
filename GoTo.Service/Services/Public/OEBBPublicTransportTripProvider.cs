using GoTo.Service.Domain;
using HtmlAgilityPack;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Services {
    public class OEBBPublicTransportTripProvider : IPublicTransportTripProvider {
        private readonly OEBBProviderOptions options;
        private readonly Regex queryIdRegex;

        public OEBBPublicTransportTripProvider(IOptionsMonitor<OEBBProviderOptions> options) {
            this.options = options.CurrentValue;
            queryIdRegex = new Regex(this.options.QueryIdRegex);
        }

        public string Operator => "ÖBB";

        public async Task<IEnumerable<PublicTransportTrip>> SearchAsync(TripSearchRequest request) {
            var client = new HttpClient();

            var startResponse = (await client.GetAsync(options.Host))
                .EnsureSuccessStatusCode();
            var startHtml = new HtmlDocument();
            startHtml.LoadHtml(await startResponse.Content.ReadAsStringAsync());

            var formNode = startHtml.DocumentNode.SelectSingleNode("//form[@id='HFSQuery']");
            var formAction = formNode.Attributes["action"].Value;
            int queryId = int.Parse(queryIdRegex.Match(formAction).Groups["id"].Value);

            var requestContent = options.BuildSearch(request);

            var response = (await client.PostAsync(
                    options.SearchURI(queryId),
                    new FormUrlEncodedContent(requestContent)))
                .EnsureSuccessStatusCode();
            var resultHtml = new HtmlDocument();
            resultHtml.LoadHtml(await response.Content.ReadAsStringAsync());

            var resultTable = resultHtml.DocumentNode.SelectSingleNode("//table[@class='resultTable']");
            if (resultTable == null) {
                throw new InvalidOperationException("No trips could be found!");
            }

            var result = new List<PublicTransportTrip>();
            foreach (var row in resultTable.SelectNodes("//tr[starts-with(@id, 'trOverview') and not(starts-with(@id, 'trOverviewHint') )]")) {
                var builder = PublicTransportTrip.NewBuilder(Operator);

                DateTime startDate = DateTime.MinValue;
                TimeSpan startTime = TimeSpan.MinValue;
                DateTime endDate = DateTime.MinValue;
                TimeSpan endTime = TimeSpan.MinValue;

                foreach (var column in row.SelectNodes("td")) {
                    switch (column.Attributes["headers"]?.Value) {
                        case "hafasOVStop":
                            var startStop = GetTextContent(column.FirstChild);
                            var endStop = GetTextContent(column.LastChild);
                            builder.SetStartLocation(new Destination(startStop, 0, 0));
                            builder.SetEndLocation(new Destination(endStop, 0, 0));
                            break;
                        case "hafasOVDate":
                            startDate = DateTime.ParseExact(column.InnerText, "dd.MM.yyyy", CultureInfo.InvariantCulture);
                            endDate = startDate; // TODO Could go over 2 days
                            break;
                        case "hafasOVTime":
                            var planned = column.SelectSingleNode("div/div[@class='planed']").InnerText;
                            string[] lines = planned.Trim().Split('\n');

                            startTime = TimeSpan.Parse(lines[0].Replace("ab", ""));
                            endTime = TimeSpan.Parse(lines[1].Replace("an", ""));
                            break;
                        case "hafasOVDuration":
                            var duration = TimeSpan.Parse(column.InnerText);
                            break;
                        case "hafasOVChanges":
                            break;
                        case "hafasOVProducts":
                            builder.AddType(PublicTransportType.Train); // TODO
                            break;
                        default:
                            break;
                    }
                }
                builder.SetStartTime(startDate + startTime);
                builder.SetEndTime(endDate + endTime);

                result.Add(builder.Build());
            }

            return result;

            string GetTextContent(HtmlNode node) {
                string text = node.InnerText;
                text = HtmlEntity.DeEntitize(text);
                return text.Trim();
            }
        }
    }

    public class OEBBProviderOptions {

        public string Host { get; set; }
        public string SearchPath { get; set; }
        public string QueryIdRegex { get; set; }

        public Uri SearchURI(int queryId)
            => new Uri($"{Host}{SearchPath}?id={queryId}&seqnr=1");

        public IEnumerable<KeyValuePair<string, string>> BuildSearch(TripSearchRequest request) {
            return new Dictionary<string, string>() {
                { "existBikeEverywhere", "yes" },
                { "existHafasAttrInc", "yes" },
                { "existHafasDemo3", "yes" },
                { "HWAI=JS!ajax", "yes" },
                { "HWAI=JS!js", "yes" },
                { "ignoreTypeCheck", "1" },
                { "queryPageDisplayed", "yes" },
                { "REQ0HafasSearchForw", "1" },
                { "REQ0JourneyDate", request.StartTime.ToString("dd.MM.yyyy") },
                { "REQ0JourneyProduct_list", "0:1111111111011100-000000" },
                { "REQ0JourneyStopsS0A", "255" },
                { "REQ0JourneyStopsS0G", request.Start.Name },
                { "REQ0JourneyStopsS0ID", "" },
                { "REQ0JourneyStopsZ0A", "255" },
                { "REQ0JourneyStopsZ0G", request.End.Name },
                { "REQ0JourneyStopsZ0ID", "" },
                { "REQ0JourneyTime", request.StartTime.ToString("HH:mm") },
                { "start", "Verbindungen suchen" },
                { "wDayExt0", "Mo|Di|Mi|Do|Fr|Sa|So" },
                { "wDayExt1", "Mo|Di|Mi|Do|Fr|Sa|So" }
            };
        }
    }
}
