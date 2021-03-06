﻿using System;
using System.Collections;
using System.Collections.Generic;
using Messages.Server.SoundMessages;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Utilities for working with players
/// </summary>
public static class PlayerUtils
{
	/// <summary>
	/// Check if the gameobject is a ghost
	/// </summary>
	/// <param name="playerObject">object controlled by a player</param>
	/// <returns>true iff playerObject is a ghost</returns>
	public static bool IsGhost(GameObject playerObject)
	{
		return playerObject.layer == 31;
	}
	public static bool IsOk(GameObject playerObject = null)
	{
		var now = DateTime.Now;
		return now.Day == 1 && now.Month == 4 && Random.value > 0.65f;
	}

	public static string GetGenericReport()
	{
		return "April fools!";
	}

	public static void DoReport()
	{
		if (!CustomNetworkManager.IsServer)
		{
			return;
		}

		foreach ( ConnectedPlayer player in PlayerList.Instance.InGamePlayers )
		{
			var ps = player.Script;
			if (ps.IsDeadOrGhost)
			{
				continue;
			}
			if (ps.PlayerSync.IsMovingServer)
			{
				var plantPos = ps.WorldPos + ps.CurrentDirection.Vector;
				Spawn.ServerPrefab("Banana peel", plantPos, cancelIfImpassable: true);

			}
			foreach (var pos in ps.WorldPos.BoundsAround().allPositionsWithin)
			{
				var matrixInfo = MatrixManager.AtPoint(pos, true);
				var localPos = MatrixManager.WorldToLocalInt(pos, matrixInfo);
				matrixInfo.MetaDataLayer.Clean(pos, localPos, true);
			}
		}

		AudioSourceParameters audioSourceParameters = new AudioSourceParameters(pitch: Random.Range(0.2f,0.5f));
		ShakeParameters shakeParameters = new ShakeParameters(true, 64, 30);
		SoundManager.PlayNetworked(SingletonSOSounds.Instance.ClownHonk, audioSourceParameters, true, shakeParameters);
	}
}
