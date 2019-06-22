using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using Amazon.Lambda.Core;
using GoTo.Lambda.Domain;
using GoTo.Lambda.Services;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GoTo.Lambda {
    public class Function {
        private static readonly ITripSearcher searcher
            = new TripSearcherFake();
        //= new GoToTripSearcher(Properties.Resources.SearchService);

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context) {
            // Skill currently only in German so set culture statical
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            if (input.Request is LaunchRequest) {
                return ResponseBuilder.AskWithCard(
                    Properties.Speech.Starter,
                    Properties.Speech.StarterTitle,
                    Properties.Speech.StarterContent,
                    null
                );
            } else if (input.Request is IntentRequest intentRequest) {
                var intent = intentRequest.Intent;
                if (intent.Name == Properties.Resources.TripSearchIntentName) {
                    var sourceSlot = intent.Slots[Properties.Resources.TripSearchSrcSlotName];
                    var destintationSlot = intent.Slots[Properties.Resources.TripSearchDstSlotName];

                    var source = sourceSlot.Value;
                    var foundSource = await searcher.FindDestinationByName(source);

                    var destination = destintationSlot.Value;
                    var foundDestination = await searcher.FindDestinationByName(destination);

                    if (!foundSource.Any() && !foundDestination.Any()) {
                        return ResponseBuilder.Ask(
                            string.Format(Properties.Speech.SourceAndDestinationNotFound, source, destination),
                            null
                        );
                    } else if (!foundSource.Any()) {
                        return ResponseBuilder.Ask(
                            string.Format(Properties.Speech.SourceNotFound, source),
                            null
                        );
                    } else if (!foundDestination.Any()) {
                        return ResponseBuilder.Ask(
                            string.Format(Properties.Speech.DestinationNotFound, destination),
                            null
                        );
                    }

                    var time = DateTime.Now;

                    context.Logger.LogLine($"Start search for {source} -> {destination}");

                    var response = new ProgressiveResponse(input);
                    await response.SendSpeech(Properties.Speech.SearchingForTrips);

                    var trips = (await searcher.SearchForTripsAsync(source, destination, time))
                        .OrderBy(t => t.StartTime)
                        .ThenBy(t => t.Duration)
                        .ToList();
                    context.Logger.LogLine($"Finish search for {source} -> {destination}: Found {trips.Count()}, Best: {trips.FirstOrDefault()}");

                    if (trips.Any()) {
                        var bestTrip = trips.First();
                        return ResponseBuilder.TellWithCard(
                            string.Format(Properties.Speech.FoundBestTrip,
                                bestTrip.StartLocation, bestTrip.EndLocation,
                                bestTrip.StartTime.ToString("HH:mm"),
                                bestTrip.Provider),
                            string.Format(Properties.Speech.FoundTripsTitle,
                                bestTrip.StartLocation,
                                bestTrip.EndLocation),
                            BuildTripsCard(trips)
                        );
                    } else {
                        return ResponseBuilder.TellWithCard(
                            string.Format(Properties.Speech.NoTripsFound, source, destination),
                            string.Format(Properties.Speech.NoTripsFound, source, destination),
                            ""
                        );
                    }
                } else {
                    // TODO Better response for unknown intent
                    return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
                }
            } else {
                return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
            }
        }

        private string BuildTripsCard(IEnumerable<Trip> trips) {
            var content = new StringBuilder();
            foreach (var trip in trips) {
                content.AppendLine(
                    $"Von {trip.StartLocation} ({trip.StartTime.ToString("HH:mm")})\n" +
                    $"Nach {trip.EndLocation} ({trip.EndTime.ToString("HH:mm")})\n" +
                    $"Dauer: {trip.Duration.TotalHours:F2} h / Mit: {trip.Provider}");
                content.AppendLine("-------------");
            }
            return content.ToString();
        }
    }
}
