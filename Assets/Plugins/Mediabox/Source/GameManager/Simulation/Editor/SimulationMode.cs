﻿using Mediabox.GameKit.Bundles;
using Mediabox.GameKit.GameDefinition;
using Mediabox.GameManager.Editor;
using Mediabox.GameManager.Editor.Build;
using UnityEditor;

namespace Mediabox.GameManager.Simulation.Editor {
	[InitializeOnLoad]
	public static class SimulationMode {
		public static ISimulationNativeAPI SimulationModeNativeApi => simulationRunner.SimulationModeNativeApi;
		public static string BundleName {
			get => simulationRunner.BundleName;
			set => simulationRunner.BundleName = value;
		}

		static readonly SimulationModeRunner simulationRunner;

		public static bool AutoSimulate {
			get => simulationRunner.AutoSimulate;
			set => simulationRunner.AutoSimulate = value;
		}

		public static bool IsInSimulationMode => simulationRunner.IsInSimulationMode;
		static SimulationMode() {
			simulationRunner = new SimulationModeRunner(new UnityEditorPrefs(), CreateNativeAPI);
			EditorApplication.update += Update;
			EditorApplication.playModeStateChanged += OnplayModeStateChanged;
		}

		static void OnplayModeStateChanged(PlayModeStateChange change) {
			switch (change) {
				case PlayModeStateChange.ExitingPlayMode:
					simulationRunner.OnDestroy();
					break;
				case PlayModeStateChange.EnteredEditMode:
					simulationRunner.Start();
					break;
			}
		}

		static void Update() {
			LoadDefaultSceneOnPlayMode.enabled = AutoSimulate;
			simulationRunner.Update();
		}
		
		public static void StartSimulationMode() {
			simulationRunner.StartSimulationMode();
		}

		static ISimulationNativeAPI CreateNativeAPI() {
			var gameDefinitionSettings = AssetDatabase.LoadAssetAtPath<GameDefinitionSettings>(GameDefinitionSettings.SettingsPath);
			var gameDefinitionBuildSettings = AssetDatabase.LoadAssetAtPath<GameDefinitionBuildSettings>(GameDefinitionBuildSettings.SettingsPath);
			return BundleManager.UseEditorBundles ? new EditorNativeAPI(BundleName, gameDefinitionSettings) : new EditorBuildNativeAPI(BundleName, gameDefinitionSettings, gameDefinitionBuildSettings);
		}

		public static void StopSimulationMode() {
			simulationRunner.StopSimulationMode();
		}
	}
}