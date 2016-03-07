using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ScreepsAPI_NET
{
    public class UserCode
    {
        /// <summary>
        /// The name of the branch
        /// </summary>
        public String Branch { get; set; }

        /// <summary>
        /// The modules in the branch.
        /// <para>The Key is the modules name</para>
        /// <para>The Value is the code</para>
        /// </summary>
        public Dictionary<String, String> Modules { get; set; }

        public UserCode()
        {
            this.Modules = new Dictionary<String, String>();
        }

        /// <summary>
        /// Creates a new UserCode object with the branch set
        /// </summary>
        /// <param name="branch">The branch name</param>
        public UserCode(String branch) : this()
        {
            this.Branch = branch;
        }

        /// <summary>
        /// Creates a new UserCode object with the branch set and the modules read from a folder
        /// </summary>
        /// <param name="branch">The branch name</param>
        /// <param name="folder">The folder where the scripts are located</param>
        /// <exception cref="System.IO.DirectoryNotFoundException">Thrown if the folder does not exist</exception>
        public UserCode(String branch, String folder) : this(branch)
        {
            if (!Directory.Exists(folder))
                throw new DirectoryNotFoundException(folder);

            String[] files = Directory.GetFiles(folder, "*.js", SearchOption.AllDirectories);

            foreach(String file in files)
            {
                FileInfo fi = new FileInfo(file);

                StreamReader sr = new StreamReader(file);
                String code = sr.ReadToEnd();
                sr.Close();

                this.Modules.Add(fi.Name.Replace(".js", ""), code);
            }
        }

        public static UserCode Parse(JObject obj)
        {
            try
            {
                UserCode code = new UserCode();

                code.Branch = obj.GetValue<String>("branch");

                JObject modules = obj.GetValue<JObject>("modules");

                IEnumerator<KeyValuePair<String, JToken>> enumerator = modules.GetEnumerator();

                while(enumerator.MoveNext())
                {
                    code.Modules.Add(enumerator.Current.Key, enumerator.Current.Value.Value<String>());
                }

                return code;
            }
            catch { }

            return null;
        }


    }
}
