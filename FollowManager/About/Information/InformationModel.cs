using System.Reflection;

namespace FollowManager.About.Information
{
    public class InformationModel
    {
        // パブリックメソッド

        /// <summary>
        /// 現在のアプリケーションバージョンを取得します。
        /// </summary>
        /// <returns></returns>
        public string GetVersion() => Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
    }
}
