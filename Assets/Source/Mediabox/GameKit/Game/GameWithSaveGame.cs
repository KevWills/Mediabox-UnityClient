﻿using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Mediabox.GameKit.Game {
	/// <summary>
	/// Inherit from this class, if you want to define a game with a savegame. Place it in any relevant scene / prefab that is loaded from GameManager.
	/// </summary>
	/// <typeparam name="TGameDefinition">The type of GameDefinition needs to match the one used in your GameManager.</typeparam>
	public abstract class GameWithSaveGame<TGameDefinition, TSaveGame> : GameBase<TGameDefinition> {
		/// <summary>
		/// Implement this Getter to specify the name of the SaveGame-File.
		/// </summary>
		protected abstract string SaveGameName { get; }
		/// <summary>
		/// Return a new Instance of TSaveGame here. It will be stored on the disk.
		/// </summary>
		protected abstract TSaveGame CreateSaveGame();
		/// <summary>
		/// Implement this method in order to add any logic necessary to load a TSaveGame.
		/// </summary>
		protected abstract Task LoadSaveGame(TSaveGame saveGame);
    
		public sealed override Task Save(string path) {
			var saveGamePath = Path.Combine(path, this.SaveGameName);
			var directory = Path.GetDirectoryName(saveGamePath);
			if (!Directory.Exists(directory))
				Directory.CreateDirectory(directory);
			File.WriteAllText(saveGamePath, JsonUtility.ToJson(CreateSaveGame()));
			return Task.CompletedTask;
		}

		public sealed override async Task Load(string path) {
			var saveGamePath = Path.Combine(path, this.SaveGameName);
			if (File.Exists(saveGamePath))
				await LoadSaveGame(JsonUtility.FromJson<TSaveGame>(File.ReadAllText(saveGamePath)));
		}
	}
}