using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace XSpy.Socket
{
    public class HubMethods
    {
        public ConcurrentDictionary<Guid, string> UserByGroup;
        //conn id + device id
        public ConcurrentDictionary<string, string> ClientsByDeviceId;

        // device id + user token
        public ConcurrentDictionary<string, Guid> UsersByDeviceId;

        public HubMethods()
        {
            UserByGroup = new ConcurrentDictionary<Guid, string>();
            ClientsByDeviceId = new ConcurrentDictionary<string, string>();
            UsersByDeviceId = new ConcurrentDictionary<string, Guid>();
        }

   
        public string GetConnectionByDeviceId(string device)
        {
            if (!ClientsByDeviceId.TryGetValue(device, out var connection))
                return null;

            return connection;
        }

        public ConcurrentDictionary<Guid, string> GetUserInGroups() => UserByGroup;
        public List<string> GetGroups() => UserByGroup.Values.Distinct().ToList();
    }
}