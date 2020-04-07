using System;
using System.Collections.Generic;
using System.IO;
using NDesk.Options;

namespace UnityGUIDReplacer
 {
     class MainClass
     {
         //path to list of old GUIDs
         const string OldGUIDsPath = "/GUIDsUniq.txt";
         
         //path to list of new GUIDs. You can generate them easily with System.Guid.NewGuid().ToString("N")
         const string NewGUIDsPath = "/newGUIDs.txt";
         
         //path to the list of files in which we want to alter GUIDs. These include all .meta, .mat, .prefab, and .unity files
         const string FileListPath = "/listOfFiles.txt";
         
         static private string _projectPath = "./";
         
         static private OptionSet _optionSet
             = new OptionSet
         {
             { "p=", v => _projectPath = v },
         };
         
         public static void Main (string[] args)
         {
             _optionSet.Parse(args);
             
             var m = new MainClass();
             
             m.ReplaceGUID();
         }
         
         public void ReplaceGUID() 
         {
             //build a dictionary out of the GUIDs
             var oldGUIDsList = File.ReadAllLines(_projectPath + OldGUIDsPath);
             var newGUIDsList = File.ReadAllLines(_projectPath + NewGUIDsPath);
             
             var GUIDDict = new Dictionary<string, string>();
             
             for(var i = 0; i < oldGUIDsList.Length; i++) {
                 GUIDDict.Add(oldGUIDsList[i], newGUIDsList[i]);
             }
             //Get the list of files to modify
             var fileList = File.ReadAllLines(_projectPath + FileListPath);
             
             foreach (string f in fileList) 
             {
                 var fileLines = File.ReadAllLines(_projectPath + f);
                 
                 for(var i = 0; i < fileLines.Length; i++) 
                 {
                     //find all instances of the string "guid: " and grab the next 32 characters as the old GUID
                     if (fileLines[i].Contains("guid: "))
                     {
                         var index = fileLines[i].IndexOf("guid: ") + 6;
                         var oldGUID = fileLines[i].Substring(index, 32); 
                         
                         //use that as a key to the dictionary and find the value
                         //replace those 32 characters with the new GUID value
                     
                         if(GUIDDict.ContainsKey(oldGUID))
                         {
                             fileLines[i] = fileLines[i].Replace(oldGUID, GUIDDict[oldGUID]);
                             Console.WriteLine("replaced \"" + oldGUID + "\" with \"" + GUIDDict[oldGUID] + "\" in file " + f);
                         }
                         else
                         {
                             Console.WriteLine("GUIDDict did not contain the key " + oldGUID);
                         }
                     }
                 }
                 //Write the lines back to the file
                 File.WriteAllLines(_projectPath + f, fileLines);
             }
         }
     }
 }