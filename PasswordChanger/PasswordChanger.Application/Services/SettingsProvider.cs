//-----------------------------------------------------------------------
// <copyright file="SettingsProvider" company="">
//     Copyright &year& (c) . All rights reserved.
// </copyright>
// <author>Nikolay.Kostadinov</author>
//-----------------------------------------------------------------------

namespace PasswordChanger.Application.Services
{
    using Contracts;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;


    /// <summary>
    /// Summary description for $classname$
    /// </summary>
    public class SettingsProvider : ISettingsProvider
    {
        private readonly string settingsType;
        public SettingsProvider(string settingsTypeParam)
        {
            this.settingsType = settingsTypeParam;
        }
        public IDictionary<string, object> GetSettings()
        {
            var result = new Dictionary<string, object>();
            var settings = ConfigurationManager.AppSettings;

            foreach (var key in settings.Keys)
            {
                result.Add(key.ToString(), settings[key.ToString()]);
            }

            return result;
        }
    }
}
