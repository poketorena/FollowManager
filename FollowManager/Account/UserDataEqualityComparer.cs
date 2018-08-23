using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FollowManager.Account
{
    public class UserDataEqualityComparer : IEqualityComparer<UserData>
    {
        public bool Equals(UserData x, UserData y)
        {
            if (x == null && y == null)
            {
                return true;
            }
            else if (x == null || y == null)
            {
                return false;
            }
            else if (x.User.Id == null || y.User.Id == null)
            {
                return false;
            }
            else if (x.User.Id.Value == y.User.Id.Value)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(UserData obj)
        {
            if (obj.User.Id == null)
            {
                throw new ArgumentNullException("UserIdがnullです。");
            }
            var hCode = obj.User.Id.Value;
            return hCode.GetHashCode();
        }
    }
}
