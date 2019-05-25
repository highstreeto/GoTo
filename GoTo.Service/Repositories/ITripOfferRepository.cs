using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Repositories {
    /// <summary>
    /// 
    /// </summary>
    public interface ITripOfferRepository {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<TripOffer> Query();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offer"></param>
        void Add(TripOffer offer);
    }
}
