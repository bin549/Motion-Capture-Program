using SFB;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class FolderUtils : MonoBehaviour
{
    public static string CheckDirectory(string path)
    {
        return Directory.Exists(path) ? GetDirectoryPath(path) : null;
    }

    public static DirectoryInfo SafeCreateDirectory(string path)
    {
        return Directory.Exists(path) ? null : Directory.CreateDirectory(path);
    }

    public static string SelectFolder()
    {
        var paths = StandaloneFileBrowser.OpenFolderPanel("Select Folder", Application.dataPath, true);
        string path = "";
        foreach (var p in paths)
        {
            path += p;
        }
        return path; 
    }

    public static string[] GetFilterdFiles(string directory, string[] extensions)
    {
        if (directory == "")
        {
            directory = Environment.CurrentDirectory;
        }
        DirectoryInfo mydir = new DirectoryInfo(directory);
        FileInfo[] f = mydir.GetFiles();
        List<FileInfo> f2 = new List<FileInfo>();
        List<string> f3 = new List<string>();
        foreach (FileInfo file in f)
        {
            foreach (var extension in extensions)
            {
                if (file.Extension.Equals(extension))
                {
                    f2.Add(file);
                    continue;
                }
            }
        }

        foreach (FileInfo file in f2)
        {
            f3.Add(file.ToString());
        }
        return f3.ToArray();
    }
    private static string GetDirectoryPath(string directory)
    {
        var directoryPath = Path.GetFullPath(directory);
        if (!directoryPath.EndsWith("\\"))
        {
            directoryPath += "\\";
        }
        if (Path.GetPathRoot(directoryPath) == directoryPath)
        {
            return directory;
        }
        return Path.GetDirectoryName(directoryPath) + Path.DirectorySeparatorChar;
    }
}
