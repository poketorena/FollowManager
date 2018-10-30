using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace FollowManager.About.License
{
    public class LicenseModel
    {
        // パブリックメソッド

        /// <summary>
        /// アセンブリからライセンスのコレクションを取得します。
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Tuple<string, string, string>> GetLicenses()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var licenses = assembly.GetManifestResourceNames().Where(name => name.StartsWith("FollowManager.Resources.Licenses."));
            var list = new List<Tuple<string, string, string>>();
            foreach (var license in licenses)
            {
                using (var streamReader = new StreamReader(assembly.GetManifestResourceStream(license)))
                {
                    var name = streamReader.ReadLine();
                    var url = streamReader.ReadLine();
                    streamReader.ReadLine();
                    var text = streamReader.ReadToEnd();
                    list.Add(new Tuple<string, string, string>(name, url, text));
                }
            }
            return list;
        }
    }
}
