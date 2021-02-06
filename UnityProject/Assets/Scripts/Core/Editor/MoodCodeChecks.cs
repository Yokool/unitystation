using UnityEngine;
using System;

public static class MoodCodeChecks
{
	[UnityEditor.Callbacks.DidReloadScripts]
#pragma warning disable IDE0051 // Remove unused private members
	private static void MoodStateChecks()
#pragma warning restore IDE0051 // Remove unused private members
	{

		int moodStateLength = Enum.GetValues(typeof(MoodState)).Length;

		if(moodStateLength < 3)
		{
			Debug.LogError($"Mood System Logical Error: {nameof(MoodState)} must have at least 3 members.");
			return;
		}

	}

}
