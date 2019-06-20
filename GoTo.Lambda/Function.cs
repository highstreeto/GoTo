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

namespace GoTo.Lambda
{
    public class Function
    {
        private static readonly ITripSearcher searcher = new TripSearcherFake();

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context)
        {
            // Skill currently only in German so set culture statical
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            if (input.Request is LaunchRequest) {
                return ResponseBuilder.Ask(
                    Properties.Speech.Starter,
                    null
                );
            } else if (input.Request is IntentRequest intentRequest) {
                var intent = intentRequest.Intent;
                if (intent.Name == Properties.Resources.TripSearchIntentName) {
                    var sourceSlot = intent.Slots[Properties.Resources.TripSearchSrcSlotName];
                    var destintationSlot = intent.Slots[Properties.Resources.TripSearchDstSlotName];

                    var source = sourceSlot.Value;
                    var destination = destintationSlot.Value;
                    var time = DateTime.Now;

                    context.Logger.LogLine($"Start search for {source} -> {destination}");

                    var response = new ProgressiveResponse();
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
                                source, destination,
                                bestTrip.StartTime.ToString("HH:mm"),
                                bestTrip.Provider),
                            string.Format(Properties.Speech.FoundTripsTitle, source, destination),
                            BuildTripsCard(trips)
                        );
                    } else {
                        return ResponseBuilder.Tell(
                            string.Format(Properties.Speech.NoTripsFound, source, destination)
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
                content.AppendLine($"{trip.StartLocation} ({trip.StartTime}) => {trip.EndLocation} ({trip.EndTime}) - {trip.Duration}");
            }
            return content.ToString();
        }
    }
}
