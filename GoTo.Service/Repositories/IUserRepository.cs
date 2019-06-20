using GoTo.Service.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GoTo.Service.Repositories {
    public interface IUserRepository {
        IEnumerable<User> Query();

        User FindByLoginName(string loginName);

        void Add(User user);
    }
}
