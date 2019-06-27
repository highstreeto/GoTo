using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Repositories {
    internal class InMemoryTripOfferRepository : ITripOfferRepository {
        private readonly List<TripOffer> offers;

        public InMemoryTripOfferRepository() {
            offers = new List<TripOffer>();
        }

        public IEnumerable<TripOffer> Query()
            => offers
            .Where(o
                => o.StartTime >= DateTime.Now
            );

        public void Add(TripOffer offer) {
            offers.Add(offer);
        }
    }
}
