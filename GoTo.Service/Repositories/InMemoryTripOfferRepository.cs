using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Repositories {
    internal class InMemoryTripOfferRepository : ITripOfferRepository {
        private readonly List<TripOffer> offers = new List<TripOffer>();

        public IEnumerable<TripOffer> Query()
            => offers;

        public void Add(TripOffer offer) {
            offers.Add(offer);
        }
    }
}
