using GoTo.Service.Domain;
using Optional;
using System.Collections.Generic;

namespace GoTo.Service.Repositories {
    public interface IDestinationRepository {
        IEnumerable<Destination> Query();

        Option<Destination> FindByName(string name);

        Option<Destination> FindByGeo(double lat, double lon);

        void Add(Destination destination);
    }
}
