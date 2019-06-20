using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GoTo.Service.Domain;

namespace GoTo.Service.Repositories {
    public class InMemoryUserRepository : IUserRepository {
        private readonly List<User> users;

        public InMemoryUserRepository() {
            users = new List<User>();
        }

        public IEnumerable<User> Query()
            => users;

        public User FindByLoginName(string loginName) {
            return Query()
                .Where(user => user.LoginName == loginName)
                .SingleOrDefault();
        }

        public void Add(User user) {
            if (FindByLoginName(user.LoginName) != null)
                throw new ArgumentException($"A user with the same login name '{user.LoginName}' already exists!", nameof(user));

            users.Add(user);
        }
    }
}
