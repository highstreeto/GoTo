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
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace GoTo.Lambda {
    public class Function {
        private static readonly ITripSearcher searcher
            //= new TripSearcherFake();
            = new GoToTripSearcher(Properties.Resources.SearchService);

        private static readonly string completeFailCounter = "countCompleteFail";
        private static readonly string locationFailCounter = "countLocationFail";

        private static readonly string geoLocationPermission = "alexa::devices:all:geolocation:read";

        public async Task<SkillResponse> FunctionHandler(SkillRequest input, ILambdaContext context) {
            // Skill currently only in German so set culture statical
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("de-DE");

            var session = input.Session;
            if (session.Attributes == null)
                session.Attributes = new Dictionary<string, object>();

            if (input.Request is LaunchRequest) {
                return ResponseBuilder.AskWithCard(
                    Properties.Speech.Starter,
                    Properties.Speech.StarterTitle,
                    Properties.Speech.StarterContent,
                    null,
                    session
                );
            } else if (input.Request is IntentRequest intentRequest) {
                var intent = intentRequest.Intent;

                context.Logger.LogLine($"IntentRequest {intent.Name}, Attributes: {string.Join(";", input.Session?.Attributes.Select(kp => $"{kp.Key}: {kp.Value}"))}");
                context.Logger.LogLine($"Slots {string.Join(";", intent.Slots.Select(s => $"{s.Key}: {s.Value.Value}"))}");

                if (intent.Name == Properties.Resources.TripSearchIntentName) {
                    if (GetCounter(session, completeFailCounter) >= 3) {
                        return ResponseBuilder.Tell(
                            Properties.Speech.CompleteFail
                        );
                    }

                    var sourceSlot = intent.Slots[Properties.Resources.TripSearchSrcSlotName];
                    var destintationSlot = intent.Slots[Properties.Resources.TripSearchDstSlotName];

                    var source = sourceSlot.Value;
                    IEnumerable<Destination> foundSources = null;
                    if (string.IsNullOrWhiteSpace(source)) {
                        if (input.Context.Geolocation == null) {
                            return ResponseBuilder.TellWithAskForPermissionConsentCard(
                                Properties.Speech.RequestGeoLocation,
                                new[] { geoLocationPermission }.AsEnumerable(),
                                session
                            );
                        } else {
                            var coords = input.Context.Geolocation.Coordinate;
                            context.Logger.LogLine($"Geo lat: {coords.Latitude}, lon: {coords.Longitude}");
                            foundSources = await searcher.FindDestinationByGeo(
                                coords.Latitude,
                                coords.Longitude
                            );
                        }
                    } else {
                        foundSources = await searcher.FindDestinationByName(source);
                    }

                    var destination = destintationSlot.Value;
                    var foundDestinations = await searcher.FindDestinationByName(destination);

                    if (foundSources.Count() != 1 && foundDestinations.Count() != 1) {
                        IncreaseCounter(session, completeFailCounter);

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.SourceAndDestinationNotFound, source, destination),
                            Properties.Speech.SourceAndDestinationNotFoundTitle,
                            string.Format("Die Orte {0} und {1} kenne ich nicht. Versuche es bitte noch einmal von vorne.",
                                source, destination),
                            null,
                            session
                        );
                    } else if (foundSources.Count() != 1) {
                        input.Session.Attributes["destinationDst"] = foundDestinations.First();

                        if (string.IsNullOrWhiteSpace(source)) {
                            return ResponseBuilder.AskWithCard(
                                string.Format(Properties.Speech.SourceGeoNotFound),
                                Properties.Speech.SourceNotFoundTitle,
                                "Den Startort konnte ich lieder nicht ermitteln. Versuche es vielleicht diesen explizit zu nennen.",
                                null,
                                session
                            );
                        } else {
                            return ResponseBuilder.AskWithCard(
                                string.Format(Properties.Speech.SourceNotFound, source, foundSources.First().Name),
                                Properties.Speech.SourceNotFoundTitle,
                                string.Format("Den Startort {0} kenne ich lieder nicht. Versuche es mit: Der Ort ist {1}.",
                                    source, foundSources.First().Name),
                                null,
                                session
                            );
                        }
                    } else if (foundDestinations.Count() != 1) {
                        input.Session.Attributes["sourceDst"] = foundSources.First();

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.DestinationNotFound, destination, foundDestinations.First().Name),
                            Properties.Speech.DestinationNotFoundTitle,
                            string.Format("Den Zielort {0} kenne ich leider nicht. Versuche es mit: Der Ort ist {1}.",
                                destination, foundDestinations.First().Name),
                            null,
                            session
                        );
                    }

                    var time = DateTime.Now;
                    return await SearchForTrips(context, input, foundSources.First(), foundDestinations.First(), time);
                } else if (intent.Name == Properties.Resources.SpecifyLocationIntentName) {
                    if (GetCounter(session, locationFailCounter) >= 3) {
                        return ResponseBuilder.Tell(
                            Properties.Speech.LocationFail
                        );
                    }

                    var source = GetAttributeAs<Destination>(session, "sourceDst");
                    var destination = GetAttributeAs<Destination>(session, "destinationDst");
                    if (source == null && destination == null) {
                        return ResponseBuilder.Ask(
                            Properties.Speech.WrongOrderSpecLoc,
                            null
                        );
                    }

                    var locationSlot = intent.Slots[Properties.Resources.SpecifyLocationLocSlotName]; ;
                    var location = locationSlot.Value;
                    var foundLocations = await searcher.FindDestinationByName(location);
                    if (foundLocations.Count() != 1) {
                        IncreaseCounter(session, locationFailCounter);

                        return ResponseBuilder.AskWithCard(
                            string.Format(Properties.Speech.DestinationNotFound, location, foundLocations.First().Name),
                            source == null
                                ? Properties.Speech.SourceNotFoundTitle
                                : Properties.Speech.DestinationNotFoundTitle,
                            string.Format("Den {0} {1} kenne ich leider nicht. Versuche es mit: Der Ort ist {2}.",
                                source == null ? "Startort" : "Zielort",
                                location, foundLocations.First().Name),
                            null,
                            session
                        );
                    } else {
                        if (source == null)
                            source = foundLocations.First();
                        if (destination == null)
                            destination = foundLocations.First();

                        var time = DateTime.Now;
                        return await SearchForTrips(context, input, source, destination, time);
                    }
                } else {
                    // TODO Better response for unknown intent
                    return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
                }
            } else {
                return ResponseBuilder.Tell(Properties.Speech.InvalidRequest);
            }
        }

        private T GetAttributeAs<T>(Session session, string attr) {
            var value = session.Attributes.GetValueOrDefault(attr, null);
            if (value == null)
                return default(T);
            else
                return JsonConvert.DeserializeObject<T>(value.ToString());
        }

        private int GetCounter(Session session, string counter) {
            if (session.Attributes.ContainsKey(counter)) {
                var value = session.Attributes[counter];
                Console.WriteLine($"{value} ({value.GetType().Name})");
                if (value is long lvalue) {
                    return (int)lvalue;
                }
                return (int)value;
            } else
                return 0;
        }

        private void IncreaseCounter(Session session, string counter) {
            if (session.Attributes.ContainsKey(counter)) {
                session.Attributes[counter] = GetCounter(session, counter) + 1;
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
