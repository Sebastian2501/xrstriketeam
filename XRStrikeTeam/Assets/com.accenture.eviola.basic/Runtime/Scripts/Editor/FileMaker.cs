using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Text;
using UnityEngine;

namespace Accenture.eviola.Editor
{
    public class FileMaker
    {
        /// <summary>
        /// create a file given its full path
        /// </summary>
        static public bool MakeFile(string fullPath, byte[] content, bool skipIfFileExists=true) {
            if (File.Exists(fullPath)) {
                if (skipIfFileExists)
                {
                    Debug.LogWarning(fullPath + " already exists");
                    return false;
                }
                else { 
                    File.Delete(fullPath);
                }
            }
            try
            {
                using (FileStream fs = File.Create(fullPath))
                {
                    fs.Write(content, 0, content.Length);
                }
#if UNITY_EDITOR
                 UnityEditor.AssetDatabase.Refresh();
#endif
                return true;
            }
            catch (System.Exception e) { 
                Debug.LogException(e);
                return false;
            }
        }

        /// <summary>
        /// create a file given its path relative to Assets
        /// </summary>
        static public bool MakeFileInAssets(string assetsPath, byte[] content, bool skipIfFileXists=true) {
            return MakeFile(Path.Join(Application.dataPath, assetsPath), content, skipIfFileXists);
        }

        static public bool MakeFile(string fullPath, string txt="", bool skipIfFileExists=true) { return MakeFile(fullPath, Encoding.ASCII.GetBytes(txt), skipIfFileExists); }
        static public bool MakeFileInAssets(string assetsPath, string txt="", bool skipIfFileXists = true) { return MakeFileInAssets(assetsPath, txt, skipIfFileXists); }

        static public bool MakeSourceCodeFromTemplate(string fullPath, string template, Dictionary<string, string> tokens, bool skipIfFileExists=true) { 
            string txt = Text.ApplyTokensToTemplate(template, tokens);
            return MakeFile(fullPath, txt, skipIfFileExists);
        }

        static public bool MakeSourceCodeFromTemplateInAssets(string assetsPath, string template, Dictionary<string, string> tokens, bool skipIfFileExists = true) {
            return MakeSourceCodeFromTemplate(Path.Join(Application.dataPath, assetsPath), template, tokens, skipIfFileExists);
        }
    }
}