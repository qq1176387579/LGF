using UnityEngine;
using UnityEditor;
using System.Collections;
using System;
namespace LGF.Editor
{
    public class ScriptKeywordReplace : UnityEditor.AssetModificationProcessor
    {
        public static void OnWillCreateAsset(string path)
        {
            path = path.Replace(".meta", "");
            int index = path.LastIndexOf(".");
            if (index <= 0) return;

            string file = path.Substring(index);
            if (file != ".cs" && file != ".js" && file != ".boo") return;
            string fileExtension = file;

            index = Application.dataPath.LastIndexOf("Assets");
            path = Application.dataPath.Substring(0, index) + path;
            file = System.IO.File.ReadAllText(path);

            file = file.Replace("#CREATIONDATE#", System.DateTime.Now.ToString());
            file = file.Replace("#AUTHORNAME#", Environment.UserName);
            //file = file.Replace("#SMARTDEVELOPERS#", PlayerSettings.companyName);
            //file = file.Replace("#FILEEXTENSION#", fileExtension);
            System.IO.File.WriteAllText(path, file);
            AssetDatabase.Refresh();
        }
    }

}
