using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.Collections;
using System.IO;

public class XCodeOnPostprocessBuild
{
	[PostProcessBuild]
	public static void OnPostprocessBuild(BuildTarget buildTarget, string path)
	{
		if (buildTarget != BuildTarget.iOS)
			return;
		
		// PBXProjectの初期化
		var projectPath = path + "/Unity-iPhone.xcodeproj/project.pbxproj";
		PBXProject pbxProject = new PBXProject ();
		pbxProject.ReadFromFile (projectPath);
		string targetGuid = pbxProject.TargetGuidByName ("Unity-iPhone");

		// ここに自動化の処理を記述する

		// MEMEのフレームワークを追加
		var memeFrameworkPath = Application.dataPath + "/Builds/Frameworks";
//		var memeFrameworkPath = Application.dataPath + "/Editor/Files/MEMELib.framework";
		FileUtil.CopyFileOrDirectory(memeFrameworkPath, Path.Combine(path, "Frameworks"));
		pbxProject.AddFileToBuild(targetGuid, pbxProject.AddFile("Frameworks/MEMELib.framework", "Frameworks/MEMELib.framework", PBXSourceTree.Source));

		// フレームワークの検索パスを設定・追加
		pbxProject.SetBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(inherited)");
		pbxProject.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(PROJECT_DIR)/Frameworks");

		pbxProject.SetBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited)");
		pbxProject.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "@executable_path/Frameworks");
		// Unity2017.1からしか使えない？
//		string copyFilesPhaseGuid = pbxProject.AddCopyFilesBuildPhase(targetGuid, "Embed Frameworks", "", "10");
//		pbxProject.AddFileToBuildSection(targetGuid, copyFilesPhaseGuid, fguid);

		// 設定を反映
		File.WriteAllText (projectPath, pbxProject.WriteToString ());

		// plistにCoreBluetoothの設定を追加
		var plistPath = Path.Combine(path, "Info.plist");
		var plist = new PlistDocument ();
		plist.ReadFromFile (plistPath);
		var backgroundModes = plist.root.CreateArray ("UIBackgroundModes");
		backgroundModes.AddString ("bluetooth-central");
		plist.WriteToFile (plistPath);
	}
}
