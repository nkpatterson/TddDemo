using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using TddDemo.Security;
using System.Linq;
using System;
using TddDemo.Security.Tests.AuthorizationServiceTests;
using Moq;
using System.Runtime.Caching;

namespace TddDemo.Security.Tests
{
    [TestClass]
    public class When_calling_AuthorizeActivity
    {
        const string UserName = "nkpatterson";
        const string ActivityName = "View Admin Page";

        private IRepository<User> _users;
        private IRepository<Activity> _activities;
        private IRepository<Group> _groups;
        private IRepository<Permission> _permissions;

        private Mock<ObjectCache> _cache;

        [TestInitialize]
        public void TestInitialize()
        {
            var user = new User { UserName = UserName };
            var activity = new Activity { Name = ActivityName };
            var permission = new Permission { User = user, Activity = activity };

            _users = new FakeRepository<User>();
            _activities = new FakeRepository<Activity>();
            _permissions = new FakeRepository<Permission>();
            _groups = new FakeRepository<Group>();
            _cache = new Mock<ObjectCache>();

            _users.Save(user);
            _activities.Save(activity);
            _permissions.Save(permission);
        }

        [TestMethod]
        public void It_returns_false_if_not_authorized()
        {
            // Arrange
            _permissions.DeleteAll();
            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, ActivityName);
            
            // Assert
            Assert.IsFalse(isAuthorized, "User is authorized but shouldn't be!");
        }

        [TestMethod]
        public void It_returns_true_if_user_is_authorized()
        {
            // Arrange
            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, ActivityName);

            // Assert
            Assert.IsTrue(isAuthorized, "User is NOT authorized but should be.");
        }

        [TestMethod]
        public void It_returns_true_if_user_is_in_an_authorized_group()
        {
            // Arrange
            var group = new Group { Name = "Admins" };
            group.AddUser(_users.First());
            _permissions.DeleteAll();

            var permission = new Permission { Group = group, Activity = _activities.First() };
            _permissions.Save(permission);

            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, ActivityName);

            // Assert
            Assert.IsTrue(isAuthorized, "User is NOT authorized but should be via group membership.");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Did not catch an expected ArgumentException.")]
        public void It_throws_ArgumentException_if_user_does_not_exist()
        {
            // Arrange
            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity("blahblahblah", ActivityName);

            // Assert
            // [ExpectedException]
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Did not catch an expected ArgumentException.")]
        public void it_throws_ArgumentException_if_activity_does_not_exist()
        {
            // Arrange
            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, "badactivity");

            // Assert
            // [ExpectedException]
        }

        [TestMethod]
        public void It_caches_result()
        {
            // Arrange
            _cache.Setup(c => c.Set(It.IsAny<string>(), true, 
                It.IsAny<CacheItemPolicy>(), null));

            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, ActivityName);

            // Assert
            _cache.VerifyAll();
        }

        [TestMethod]
        public void It_returns_cached_result()
        {
            // Arrange
            _cache.Setup(c => c.Contains(It.IsAny<string>(), null)).Returns(true);
            _cache.Setup(c => c.Get(It.IsAny<string>(), null)).Returns(false);

            var authSvc = CreateService();

            // Act
            var isAuthorized = authSvc.AuthorizeActivity(UserName, ActivityName);

            // Assert
            _cache.VerifyAll();
            Assert.IsFalse(isAuthorized);
        }

        private AuthorizationService CreateService()
        {
            return new AuthorizationService(_users, _activities, 
                _permissions, _groups, _cache.Object);
        }
    }
}
