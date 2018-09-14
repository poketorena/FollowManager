using FollowManager.Account;
using FollowManager.Tab;

namespace FollowManager.MultiBinding.MultiParameter
{
    /// <summary>
    /// タブのデータとブロックしてブロック解除するユーザーデータ
    /// </summary>
    public class BlockAndBlockReleaseRequest
    {
        // パブリックプロパティ

        /// <summary>
        /// タブのデータ
        /// </summary>
        public TabData TabData { get; set; }

        /// <summary>
        /// ユーザーデータ
        /// </summary>
        public UserData UserData { get; set; }
    }
}
