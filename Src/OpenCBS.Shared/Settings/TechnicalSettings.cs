﻿// Octopus MFS is an integrated suite for managing a Micro Finance Institution: 
// clients, contracts, accounting, reporting and risk
// Copyright © 2006,2007 OCTO Technology & OXUS Development Network
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License along
// with this program; if not, write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301 USA.
//
// Website: http://www.opencbs.com
// Contact: contact@opencbs.com

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;
using System.Security.AccessControl;

namespace OpenCBS.Shared.Settings
{
    [Serializable]
    public static class TechnicalSettings
    {
        private static Version _version;

        public const string RegistryPathTemplate = @"Software\Open Octopus Ltd\OpenCBS\{0}";

        private static bool _useOnlineMode;
        private static bool _useFrapidMode;
        private static string _remotingServer;
        private static int _remotingServerPort;

        private static readonly Dictionary<string, string> Settings = new Dictionary<string, string>();

        private static Version GetVersion()
        {
            if (_version != null) return _version;

            var assembly = Assembly.GetExecutingAssembly();
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(assembly.Location);
            return (_version = new Version(fileVersionInfo.FileVersion));
        }

        public static string GetDisplayVersion()
        {
            var version = GetVersion();
            var textVersion = string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
            var attribute =
                (AssemblyGitRevision)
                (Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyGitRevision), true).FirstOrDefault());
            if (attribute == null) return textVersion;
            var revision = attribute.Revision;
            revision = revision.Length > 7 ? revision.Substring(0, 7) : revision;
            return textVersion + "." + revision;
        }

        private static string _databaseName;
        public static string DatabaseName
        {
            get
            {
                if (UseFrapidMode)
                    return _databaseName;
                return GetValue("DATABASE_NAME", "OpenCBS");
            }
            set
            {
                if (UseFrapidMode)
                    _databaseName = value;
                else
                    SetValue("DATABASE_NAME", value);
            }
        }

        public static bool UseDemoDatabase
        {
            get
            {
                if (UseFrapidMode)
                    return false;
                bool ret;
                return bool.TryParse(GetValue("USE_DEMO_DATABASE", "False"), out ret) && ret;
            }
            set { SetValue("USE_DEMO_DATABASE", value.ToString()); }
        }

        private static string _databaseServerName;
        public static string DatabaseServerName
        {
            get {
                if (UseFrapidMode)
                    return _databaseServerName;
                return GetValue("DATABASE_SERVER_NAME", @"SERVER\OGBESERVER");
            }
            set
            {
                if (UseFrapidMode)
                    _databaseServerName = value;
                else
                    SetValue("DATABASE_SERVER_NAME", value);
            }
        }

        private static string databaseLoginName;
        public static string DatabaseLoginName
        {
            get {
                if (UseFrapidMode)
                    return databaseLoginName;
                return GetValue("DATABASE_LOGIN_NAME", "OgbeAhiara"); }
            set {
                if (UseFrapidMode)
                    databaseLoginName = value;
                else
                    SetValue("DATABASE_LOGIN_NAME", value); }
        }

        private static string databasePassword;
        public static string DatabasePassword
        {
            get {
                if (UseFrapidMode)
                    return databasePassword;
                return GetValue("DATABASE_PASSWORD", "ogbemfb"); }
            set {
                if (UseFrapidMode)
                    databasePassword = value;
                else
                    SetValue("DATABASE_PASSWORD", value); }
        }
        
        public static string ReportPath
        {
            get { return GetValue("REPORT_PATH", String.Empty); }
            set { SetValue("REPORT_PATH", value); }
        }

        public static string ScriptPath
        {
            get { return GetValue("SCRIPT_PATH", String.Empty); }
            set { SetValue("SCRIPT_PATH", value); }
        }

        public static List<string> AvailableDatabases
        {
            get
            {
                var retval = new List<string>();
                string val = GetValue("DATABASE_LIST", string.Empty);
                val = val.Replace(" ", "");
                if (string.IsNullOrEmpty(val)) return retval;

                retval.AddRange(val.Split(','));
                return retval;
            }
        }

        public static void AddAvailableDatabase(string database)
        {
            string val = GetValue("DATABASE_LIST", string.Empty);
            if (!string.IsNullOrEmpty(val))
            {
                val += "," + database;
            }
            SetValue("DATABASE_LIST", val);
        }

        public static string CurrentVersion
        {
            get
            {
                var version = GetVersion();
                return string.Format("{0}.{1}.0.0", version.Major, version.Minor);
            }
        }

        public static string SoftwareVersion
        {
            get { return "v" + CurrentVersion; }
        }

        public static bool UseOnlineMode
        {
            get { return _useOnlineMode; }
            set { _useOnlineMode = value; }
        }

        public static bool UseFrapidMode
        {
            get { return _useFrapidMode; }
            set { _useFrapidMode = value; }
        }

        public static bool SentQuestionnaire
        {
            get { return Convert.ToBoolean(GetValue("SENT_QUESTIONNAIRE", "True")); }
            set { SetValue("SENT_QUESTIONNAIRE", value ? "True" : "False"); }
        }

        public static string RemotingServer
        {
            get
            {
                return _remotingServer;
            }
            set
            {
                _remotingServer = value;
            }
        }

        public static int RemotingServerPort
        {
            get { return _remotingServerPort; }
            set { _remotingServerPort = value; }
        }

        private static int databaseTimeout = 300;

        public static int DatabaseTimeout
        {
            get
            {
                if (UseFrapidMode)
                    return databaseTimeout;
                string dbTimeoutStr = GetValue("DATABASE_TIMEOUT", "300");
                int dbTimeout;
                return int.TryParse(dbTimeoutStr, out dbTimeout) ? dbTimeout : 300;
            }
        }

        public static int DebugLevel
        {
            get { return 0; }
        }

        public static bool CheckSettings()
        {
            var values = new[]
            {
                DatabaseLoginName,
                DatabaseName,
                DatabasePassword,
            };
            return values.All(value => !string.IsNullOrEmpty(value));
        }

        public static string GetRegistryPath()
        {
            var version = GetVersion();
            var textVersion = string.Format("{0}.{1}.0.0", version.Major, version.Minor);
            return string.Format(RegistryPathTemplate, textVersion);
        }

        private static RegistryKey OpenRegistryKey()
        {
            var path = GetRegistryPath();

            //ApplicationRegistry.Find();  
                         
            RegistryKey userRegistryBase32 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry32);
            RegistryKey userReg32 = userRegistryBase32.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            RegistryKey lmRegistryBase32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            RegistryKey lmReg32 = lmRegistryBase32.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            RegistryKey userRegistryBase64 = RegistryKey.OpenBaseKey(RegistryHive.CurrentUser, RegistryView.Registry64);
            RegistryKey userReg64 = userRegistryBase64.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            RegistryKey lmRegistryBase64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
            RegistryKey lmReg64 = lmRegistryBase64.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            
            //return Registry.CurrentUser.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl)
            //    ?? Registry.LocalMachine.OpenSubKey(path, RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);

            return lmReg32;
        }

        private static void SetValue(string key, string value)
        {
            Settings[key] = value;
            using (var reg = OpenRegistryKey())
            {
                if (null == reg) return;
                reg.SetValue(key, value);
            }
        }

        public static string GetValue(string key, string defaultValue)
        {
            if (Settings.ContainsKey(key))
            {
                return Settings[key];
            }

            using (var reg = OpenRegistryKey())
            {
                if (null == reg) return defaultValue;
                string value = reg.GetValue(key, defaultValue).ToString();
                Settings[key] = value;
                return value;
            }
        }

        public static void EnsureKeyExists()
        {
            var path = GetRegistryPath();
            
            //using (var key = Registry.CurrentUser.OpenSubKey(path) ?? Registry.LocalMachine.OpenSubKey(path))
            using (var key = OpenRegistryKey())
            {
                if (key != null) return;
            }

            //using (var key = Registry.CurrentUser.CreateSubKey(path))
            RegistryKey lmRegistryBase32 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32);
            //RegistryKey lmRegistryBase64 = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);            
            using (var key = lmRegistryBase32.CreateSubKey(path))
            {
                key.SetValue("DATABASE_LIST", string.Empty);
                key.SetValue("DATABASE_LOGIN_NAME", DatabaseLoginName);
                key.SetValue("DATABASE_NAME", DatabaseName);
                key.SetValue("DATABASE_PASSWORD", DatabasePassword);
                key.SetValue("DATABASE_SERVER_NAME", DatabaseServerName);
                key.SetValue("DATABASE_TIMEOUT", DatabaseTimeout);
                key.SetValue("USE_DEMO_DATABASE", UseDemoDatabase);
            }
        }
    }
}
