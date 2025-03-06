using System.IO;
using System.Linq;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Android;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager;

namespace YVR.Player.Editor
{
    public class PlayerGradleGeneration : IPostGenerateGradleAndroidProject
    {
        public int callbackOrder => 9999;

        public void OnPostGenerateGradleAndroidProject(string path)
        {
            Debug.Log("YPlayer PostGenerateGradleAndroidProject Path:" + path);

            string gradlePath = Path.Combine(path, "build.gradle");
            var template = new Template(gradlePath);

            AddMedia3Dependence(template);
            AddCommonSDKExclude(template);

            var directoryInfo = new DirectoryInfo(path).Parent;
            if (directoryInfo != null)
            {
                string parentPath = directoryInfo.FullName;
                string propertiesPath = Path.Combine(parentPath, "gradle.properties");
                var properties = new Properties(propertiesPath);

                properties.SetProperty("android.useAndroidX", "true");
                properties.SetProperty("android.suppressUnsupportedCompileSdk", "34");
                properties.Save();
            }
        }

        private void AddMedia3Dependence(Template template)
        {
            template.AddDependency("androidx.media3:media3-exoplayer", "1.4.1");
            template.AddDependency("androidx.media3:media3-exoplayer-dash", "1.4.1");
            template.AddDependency("androidx.media3:media3-common", "1.4.1");
            template.AddDependency("androidx.media3:media3-exoplayer-hls", "1.4.1");
            template.Save();
        }

        private void AddCommonSDKExclude(Template template)
        {
            ListRequest listRequest = Client.List(true, true);
            int waitCount = 0;
            while (listRequest.Status != StatusCode.Success && waitCount < 10)
            {
                waitCount++;
                Thread.Sleep(1000);
            }

            if (listRequest.Status != StatusCode.Success) return;
            bool containsCommonSDK =
                listRequest.Result.Any((item) => item.name.Contains("com.yvr.android-device.common"));
            if (containsCommonSDK)
            {
                template.AddExclude("androidx.core", "core");
                template.AddExclude("androidx.annotation", "annotation");
                template.Save();
            }
        }

        public class Template
        {
            private static class Parsing
            {
                public static string GetVersion(string text)
                {
                    return new System.Text.RegularExpressions.Regex(
                        "com.android.tools.build:gradle:([0-9]+\\.[0-9]+\\.[0-9]+)").Match(text).Groups[1].Value;
                }

                public static int GoToSection(string section, List<string> lines)
                {
                    return GoToSection(section, 0, lines);
                }

                public static int GoToSection(string section, int start, List<string> lines)
                {
                    var sections = section.Split('.');

                    int p = start - 1;
                    for (int i = 0; i < sections.Length; i++)
                    {
                        p = FindInScope("\\s*" + sections[i] + "\\s*\\{\\s*", p + 1, lines);
                    }

                    if (p == -1)
                    {
                        p = AddSection(section, lines);
                    }

                    return p;
                }

                public static int FindInScope(string search, int start, List<string> lines)
                {
                    var regex = new System.Text.RegularExpressions.Regex(search);

                    int depth = 0;

                    for (int i = start; i < lines.Count; i++)
                    {
                        if (depth == 0 && regex.IsMatch(lines[i]))
                        {
                            return i;
                        }

                        // count the number of open and close braces. If we leave the current scope, break
                        if (lines[i].Contains("{"))
                        {
                            depth++;
                        }

                        if (lines[i].Contains("}"))
                        {
                            depth--;
                        }

                        if (depth < 0)
                        {
                            break;
                        }
                    }

                    return -1;
                }

                public static int GetScopeEnd(int start, List<string> lines)
                {
                    int depth = 0;
                    for (int i = start; i < lines.Count; i++)
                    {
                        // count the number of open and close braces. If we leave the current scope, break
                        if (lines[i].Contains("{"))
                        {
                            depth++;
                        }

                        if (lines[i].Contains("}"))
                        {
                            depth--;
                        }

                        if (depth < 0)
                        {
                            return i;
                        }
                    }

                    return -1;
                }
            }

            private readonly string _templatePath;
            private readonly List<string> _lines;

            internal Template(string templatePath)
            {
                _templatePath = templatePath;
                _lines = File.ReadAllLines(_templatePath).ToList();
            }

            public void Save()
            {
                File.WriteAllLines(_templatePath, _lines);
            }

            public void AddRepository(string section, string name)
            {
                int sectionIndex = Parsing.GoToSection($"{section}.repositories", _lines);
                if (Parsing.FindInScope($"{name}\\(\\)", sectionIndex + 1, _lines) == -1)
                {
                    _lines.Insert(Parsing.GetScopeEnd(sectionIndex + 1, _lines), $"\t\t{name}()");
                }
            }

            public void AddDependency(string name, string version)
            {
                int dependencies = Parsing.GoToSection("dependencies", _lines);
                int target = Parsing.FindInScope(Regex.Escape(name), dependencies + 1, _lines);
                if (target != -1)
                {
                    _lines[target] = $"\timplementation '{name}:{version}'";
                }
                else
                {
                    _lines.Insert(Parsing.GetScopeEnd(dependencies + 1, _lines),
                        $"\timplementation '{name}:{version}'");
                }
            }

            public void RemoveDependency(string name)
            {
                int dependencies = Parsing.GoToSection("dependencies", _lines);
                int target = Parsing.FindInScope(Regex.Escape(name), dependencies + 1, _lines);
                if (target != -1)
                {
                    _lines.RemoveAt(target);
                }
            }

            public void AddExclude(string group, string module)
            {
                int configurationsImplementation = Parsing.GoToSection("configurations.implementation", _lines);
                int target = Parsing.FindInScope(Regex.Escape(module), configurationsImplementation + 1, _lines);
                if (target != -1)
                {
                    _lines[target] = $"\texclude group:'{group}',module:'{module}'";
                }
                else
                {
                    _lines.Insert(Parsing.GetScopeEnd(configurationsImplementation + 1, _lines),
                        $"\texclude group:'{group}',module:'{module}'");
                }
            }

            public void RemoveExclude(string module)
            {
                int configurationsImplementation = Parsing.GoToSection("configurations.implementation", _lines);
                int target = Parsing.FindInScope(Regex.Escape(module), configurationsImplementation + 1, _lines);
                if (target != -1)
                {
                    _lines.RemoveAt(target);
                }
            }

            public void RemoveAndroidSetting(string name)
            {
                int android = Parsing.GoToSection("android", _lines);
                int target = Parsing.FindInScope(Regex.Escape(name), android + 1, _lines);
                if (target != -1)
                {
                    _lines.RemoveAt(target);
                }
            }

            private static int AddSection(string section, List<string> lines)
            {
                lines.Add($"{section} {{");
                lines.Add($"}}");
                int index = lines.Count - 2;
                return index;
            }
        }

        public class Properties
        {
            private readonly string _propertiesPath;
            private readonly List<string> _lines;

            internal Properties(string propertiesPath)
            {
                _propertiesPath = propertiesPath;
                _lines = File.ReadAllLines(_propertiesPath).ToList();
            }

            public void Save()
            {
                File.WriteAllLines(_propertiesPath, _lines);
            }

            private int FindProperty(string name)
            {
                int p = -1;
                string propStr = name + "=";
                for (int i = 0; i < _lines.Count; i++)
                {
                    if (_lines[i].StartsWith(propStr))
                    {
                        p = i;
                        break;
                    }
                }

                return p;
            }

            public void SetProperty(string name, string value)
            {
                int line = FindProperty(name);
                if (line == -1)
                {
                    _lines.Add($"{name}={value}");
                }
                else
                {
                    _lines[line] = $"{name}={value}";
                }
            }

            public void RemoveProperty(string name)
            {
                int line = FindProperty(name);
                if (line != -1)
                {
                    _lines.RemoveAt(line);
                }
            }

            public bool TryGetProperty(string name, out string value)
            {
                int line = FindProperty(name);
                if (line != -1)
                {
                    value = _lines[line].Split('=')[1];
                    return true;
                }
                else
                {
                    value = null;
                    return false;
                }
            }
        }
    }
}