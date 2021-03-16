using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace XSpy.Socket
{
    public class HubMethods
    {
        public ConcurrentDictionary<Guid, string> UserByGroup;
        public ConcurrentDictionary<Guid, string> ClientsByUserId;
        public ConcurrentDictionary<string, ConcurrentBag<double>> DecibelsByUserId;

        public HubMethods()
        {
            UserByGroup = new ConcurrentDictionary<Guid, string>();
            ClientsByUserId = new ConcurrentDictionary<Guid, string>();
            DecibelsByUserId = new ConcurrentDictionary<string, ConcurrentBag<double>>();
        }

        public bool IgnoreAudio(string userId, double decibels)
        {
            if (!DecibelsByUserId.TryGetValue(userId, out var lastDbs))
            {
                DecibelsByUserId[userId] = new ConcurrentBag<double>();
                return false;
            }

            if (decibels < 43)
            {
                if (lastDbs.Count(lasts => lasts < 43) >= 20)
                {
                    return true;
                }

                lastDbs.Add(decibels);
            }
            else
            {
                lastDbs.Clear();
            }

            return false;
        }

        public string GetConnectionByUserId(Guid userId)
        {
            if (!ClientsByUserId.TryGetValue(userId, out var connection))
                return null;

            return connection;
        }

        public ConcurrentDictionary<Guid, string> GetUserInGroups() => UserByGroup;
        public List<string> GetGroups() => UserByGroup.Values.Distinct().ToList();
    }
}