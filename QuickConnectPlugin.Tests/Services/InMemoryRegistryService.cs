using System;
using System.Collections.Generic;
using QuickConnectPlugin.Services;

namespace QuickConnectPlugin.Tests.Services {

    public class InMemoryRegistryService : IRegistryService {

        private List<String> sessions = new List<String>();
        private List<String> hostKeys = new List<String>();

        public InMemoryRegistryService(ICollection<String> sessions) {
            this.sessions.AddRange(sessions);
        }

        public InMemoryRegistryService(ICollection<String> sessions, ICollection<String> hostKeys) {
            this.sessions.AddRange(sessions);
            this.hostKeys.AddRange(hostKeys);
        }

        public ICollection<String> GetPuttySessions() {
            return new List<String>(sessions);
        }

        public ICollection<String> GetPuttyHostKeys() {
            return new List<String>(hostKeys);
        }
    }
}
