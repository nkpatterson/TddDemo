using System.Collections.Generic;

namespace TddDemo.Security
{
    public class Group
    {
        public string Name { get; set; }
        public IList<User> Users { get; set; }

        public Group()
        {
            Users = new List<User>();
        }

        public void AddUser(User user)
        {
            Users.Add(user);
        }
    }
}
