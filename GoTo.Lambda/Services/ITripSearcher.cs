using GoTo.Lambda.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GoTo.Lambda.Services {
    public interface ITripSearcher {
        Task<IEnumerable<Trip>> SearchForTripsAsync(string start, string end, DateTime time);
    }
}
