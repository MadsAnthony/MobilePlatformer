﻿using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_5 && !(UNITY_5_0 || UNITY_5_1 || UNITY_5_2)
using UnityEditor.SceneManagement;
#endif
#if !UNITY_5
using Opencoding.XCodeEditor;
#endif
using UnityEngine;

namespace Opencoding.Console.Editor
{
	static class BuildPostProcessor
	{
#if !UNITY_5
		[PostProcessBuild(200)]
		public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
		{
			if (target != BuildTarget.iPhone)
				return;

			var project = new XCProject(pathToBuiltProject);
				project.ApplyMod(Application.dataPath, Path.Combine(DebugConsoleEditorSettings.OpencodingDirectoryLocation, "Console/Editor/fixup.projmods"));
				project.Save();
		}
#endif

		[PostProcessScene]
		public static void OnPostprocessScene()
		{
			if (EditorApplication.isPlaying)
				return;

			var debugConsoles = UnityEngine.Object.FindObjectsOfType<DebugConsole>();
		    if (debugConsoles.Length > 1)
		    {
#if UNITY_4_5 || UNITY_4_6 || UNITY_4_7 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
                throw new InvalidOperationException("More than one debug console in the scene " +
		                                            EditorApplication.currentScene);
#else
                throw new InvalidOperationException("More than one debug console in the scene " + 
                    EditorSceneManager.GetActiveScene().name);
#endif
            }

            if (debugConsoles.Length == 0)
				return;

			var debugConsole = debugConsoles[0];
			if(debugConsole.Settings.OnlyInDevBuilds && !EditorUserBuildSettings.development)
				UnityEngine.Object.DestroyImmediate(debugConsole.gameObject);

		    bool isEnabledByDefine = String.IsNullOrEmpty(debugConsole.Settings.EnableIfDefined) ||
		                             EditorUserBuildSettings.activeScriptCompilationDefines.Contains(
		                                 debugConsole.Settings.EnableIfDefined);

		    if (isEnabledByDefine && !String.IsNullOrEmpty(debugConsole.Settings.DisableIfDefined) && EditorUserBuildSettings.activeScriptCompilationDefines.Contains(
                                    debugConsole.Settings.DisableIfDefined))
		    {
		        isEnabledByDefine = false;
		    }

            if (!isEnabledByDefine)
				UnityEngine.Object.DestroyImmediate(debugConsole.gameObject);

			if (debugConsole.Settings.AutoSetVersion)
			{
#if UNITY_IOS || UNITY_ANDROID || !UNITY_5
				debugConsole.Settings.GameVersion = PlayerSettings.bundleVersion;
#else
				debugConsole.Settings.GameVersion = Application.version;
#endif
				EditorUtility.SetDirty(debugConsole.Settings);
			}

#if UNITY_ANDROID
		    ConformAndroidManifest();
#endif
		    VerifyLinkXmlFile();

		}

	    private static void ConformAndroidManifest()
        {
            var manifestFilePath = "Assets/Plugins/Android/OpenCoding/AndroidManifest.xml";
            var text = File.ReadAllText(manifestFilePath);
	        var regEx = new Regex("android:authorities=(\".*?\")");
	        var newContent = regEx.Replace(text, "android:authorities=\"" + PlayerSettings.bundleIdentifier + ".fileprovider\"");
            if(newContent != text)
                File.WriteAllText(manifestFilePath, newContent);
	    }

	    private static void VerifyLinkXmlFile()
	    {
	        var linkXmlContent = "<linker>\n" +
	                             "       <assembly fullname=\"UnityEngine\">\n" +
	                             "               <type fullname=\"UnityEngine.TouchScreenKeyboard\" preserve=\"all\"/>\n" +
	                             "       </assembly>\n" +
	                             "</linker>";
            if (!File.Exists("Assets/link.xml"))
	        {
	            File.WriteAllText("Assets/link.xml", linkXmlContent);
                Debug.LogWarning("The link.xml file was missing from the Assets folder. This is required for TouchConsole Pro to work on AOT builds (iOS and any platform using IL2CPP). The file has been created with the default content.");
	            return;
	        }

	        var content = File.ReadAllText("Assets/link.xml");
            if(!content.Contains("UnityEngine.TouchScreenKeyboard"))
                throw new InvalidOperationException("Couldn't find TouchScreenKeyboard listed in Assets/link.xml. This is required for TouchConsole Pro to work on AOT builds (iOS and any platform using IL2CPP). See www.opencoding.net/TouchConsolePro/technical_documentation.php#stripping for an explanation of how to fix this.");
	    }
	}

	
}