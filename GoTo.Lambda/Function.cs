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

        private static readonly string completeFailCounter = "countCompleteFail";

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context) {
            // Skill currently only in German so set culture statical
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var session = input.Session;
            if (input.Request is LaunchRequest) {
                if (session.Attributes == null)
                    session.Attributes = new Dictionary<string, object>();

                return ResponseBuilder.AskWithCard(
                    Properties.Speech.Starter,
                    Properties.Speech.StarterTitle,
                    Properties.Speech.StarterContent,
                    null,
                    session
                );
            } else if (input.Request is IntentRequest intentRequest) {
                context.Logger.LogLine($"IntentRequest {input.Session}, Attributes: {input.Session?.Attributes}");

                var intent = intentRequest.Intent;
                if (intent.Name == Properties.Resources.TripSearchIntentName) {
                    if (session.Attributes.ContainsKey(completeFailCounter) && (int)input.Session.Attributes[completeFailCounter] >= 3) {
                        return ResponseBuilder.Tell(
                            Properties.Speech.CompleteFail
                        );
                    }

                    var sourceSlot = intent.Slots[Properties.Resources.TripSearchSrcSlotName];
                    var destintationSlot = intent.Slots[Properties.Resources.TripSearchDstSlotName];

                    var source = sourceSlot.Value;
                    var foundSources = await searcher.FindDestinationByName(source);

                    var destination = destintationSlot.Value;
                    var foundDestinations = await searcher.FindDestinationByName(destination);

                    if (foundSources.Count() != 1 && foundDestinations.Count() != 1) {
                        IncreaseCounter(session, completeFailCounter);

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.SourceAndDestinationNotFound, source, destination),
                            Properties.Speech.SourceAndDestinationNotFoundTitle,
                            string.Format("Die Orte {0} und {1} kenne ich nicht. Versuch es bitte noch einaml von vorne.",
                                source, destination),
                            null,
                            session
                        );
                    } else if (foundSources.Count() != 1) {
                        input.Session.Attributes["destinationDst"] = foundDestinations.First();

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.SourceNotFound, source),
                            Properties.Speech.SourceNotFoundTitle,
                            string.Format("Den Startort {0} kenne ich lieder nicht. Versuch es vielleicht mit {1}.",
                                source, foundSources.First().Name),
                            null,
                            session
                        );
                    } else if (foundDestinations.Count() != 1) {
                        input.Session.Attributes["sourceDst"] = foundSources.First();

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.DestinationNotFound, destination),
                            Properties.Speech.DestinationNotFoundTitle,
                            string.Format("Den Zielort {0} kenne ich leider nicht. Versuch es vielleicht mit {1}.",
                                destination, foundDestinations.First().Name),
                            null,
                            session
                        );
                    }

                    var time = DateTime.Now;
                    return await SearchForTrips(context, input, foundSources.First(), foundDestinations.First(), time);
                } else if (intent.Name == Properties.Resources.SpecifyLocationIntentName) {
                    return ResponseBuilder.Tell("WIP");
                } else {
                    // TODO Better response for unknown intent
                    return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
                }
            } else {
                return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
            }
        }

        private void IncreaseCounter(Session session, string counter) {
            if (session.Attributes.ContainsKey(counter)) {
                session.Attributes[counter] =
                    ((int)session.Attributes[counter]) + 1;
            } else {
                session.Attributes[counter] = 1;
            }
        }

        private async Task<SkillResponse> SearchForTrips(ILambdaContext context, SkillRequest input, Destination start, Destination end, DateTime time) {
            context.Logger.LogLine($"Start search for {start} -> {end}");

            var response = new ProgressiveResponse(input);
            await response.SendSpeech(
                string.Format(Properties.Speech.SearchingForTrips, start.Name, end.Name)
            );

            var trips = (await searcher.SearchForTripsAsync(start.Name, end.Name, time))
                .OrderBy(t => t.StartTime)
                .ThenBy(t => t.Duration)
                .ToList();
            context.Logger.LogLine($"Finish search for {start} -> {end}: Found {trips.Count()}, Best: {trips.FirstOrDefault()}");

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
                    string.Format(Properties.Speech.NoTripsFound, start.Name, end.Name),
                    string.Format(Properties.Speech.NoTripsFound, start.Name, end.Name),
                    ""
                );
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
