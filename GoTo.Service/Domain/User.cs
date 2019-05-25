using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GoTo.Service.Domain {
    public class User {
        public User(string loginName, string displayName) {
            LoginName = loginName;
            DisplayName = displayName;
        }

        public string LoginName { get; }
        public string DisplayName { get; private set; }
    }
}
