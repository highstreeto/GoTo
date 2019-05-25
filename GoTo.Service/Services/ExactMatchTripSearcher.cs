using GoTo.Service.Domain;
using GoTo.Service.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Services {
    public class ExactMatchTripSearcher : ITripSearcher {
        private readonly ILogger logger;

        private readonly ITripOfferRepository tripOfferRepo;
        private readonly IEnumerable<IPublicTransportTripProvider> publicTransportProviders;

        public ExactMatchTripSearcher(
                ILogger<ExactMatchTripSearcher> logger,
                ITripOfferRepository tripOfferRepo,
                IEnumerable<IPublicTransportTripProvider> publicTransportProviders) {
            this.logger = logger;
            this.tripOfferRepo = tripOfferRepo;
            this.publicTransportProviders = publicTransportProviders;
        }

        public async Task<IEnumerable<Trip>> SearchAsync(TripSearchRequest request) {
            // Search for offered trips
            var offeredTrips = tripOfferRepo.Query()
                .Where(t => request.Start.Equals(t.StartLocation))
                .Where(t => request.End.Equals(t.EndLocation))
                .Where(t => t.StartTime >= request.StartTime);

            // Search for trips provided by public transport
            var publicTrips = new List<Trip>();
            foreach (var provider in publicTransportProviders) {
                try {
                    var providerResult = await provider.SearchAsync(request);
                    publicTrips.AddRange(providerResult);
                } catch (Exception ex) {
                    logger.LogWarning(ex, "Provider '{0}' failed while searching for trips!", provider.GetType().Name);
                }
            }

            // Concat and order by duration
            return offeredTrips
                .Concat(publicTrips)
                .OrderBy(t => t.StartTime)
                .ThenBy(t => t.EstimatedDuration);
        }
    }
}
