using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Repositories {
    public interface IDestinationRepository {
        IEnumerable<Destination> Query();

        Destination FindByName(string name);

        Destination FindByGeo(double lat, double lon);

        void Add(Destination destination);
    }
}
