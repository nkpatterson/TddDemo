using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;

namespace TddDemo.Security
{
    public class AuthorizationService
    {
        private IRepository<User> _users;
        private IRepository<Activity> _activities;
        private IRepository<Permission> _permissions;
        private IRepository<Group> _groups;
        private ObjectCache _cache;

        public AuthorizationService(IRepository<User> users, IRepository<Activity> activities, 
            IRepository<Permission> permissions, IRepository<Group> groups,
            ObjectCache cache)
        {
            _users = users;
            _activities = activities;
            _permissions = permissions;
            _groups = groups;
            _cache = cache;
        }

        public bool AuthorizeActivity(string userName, string activityName)
        {
            var cacheKey = string.Concat(userName, ":", activityName);

            if (_cache.Contains(cacheKey))
                return (bool)_cache.Get(cacheKey);

            if (!_users.Any(x => x.UserName == userName))
                throw new ArgumentException("User does not exist.");
            if (!_activities.Any(x => x.Name == activityName))
                throw new ArgumentException("Activity does not exist.");

            var result = _permissions.Any(p => p.User != null && p.User.UserName == userName && p.Activity.Name == activityName) ||
                         _permissions.Any(p => p.Group.Users.Select(u => u.UserName).Contains(userName) && p.Activity.Name == activityName);

            _cache.Set(cacheKey, result, new CacheItemPolicy(), null);

            return result;
        }
    }
}
