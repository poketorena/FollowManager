using System;
using System.Collections.Generic;

namespace FollowManager.Account
{
    /// <summary>
    /// UserDataの等価比較をサポートするメソッドを提供します。
    /// </summary>
    public class UserDataEqualityComparer : IEqualityComparer<UserData>
    {
        /// <summary>
        /// UserDataの等価比較をサポートします。
        /// </summary>
        /// <param name="x">1つ目のUserData</param>
        /// <param name="y">2つ目のUserData</param>
        /// <returns></returns>
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

        /// <summary>
        /// ユーザーIdを使ってハッシュコードを取得します。
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
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
