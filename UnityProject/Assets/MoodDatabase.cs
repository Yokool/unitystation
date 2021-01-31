using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoodDatabase
{
	/// <summary>
	/// 
	/// </summary>
	private static readonly Dictionary<MoodEventType, MoodEvent> moodEventPrototypes = new Dictionary<MoodEventType, MoodEvent>()
	{
		{
			MoodEventType.DEBUG_POSITIVE,
			new MoodEvent("DEBUG_POSITIVE", 101, 60f)
		},
		{
			MoodEventType.DEBUG_NEGATIVE,
			new MoodEvent("DEBUG_NEGATIVE", -101, 60f)
		},
		{
			MoodEventType.DEBUG_NEUTRAL,
			new MoodEvent("DEBUG_NEUTRAL", 0, 60f)
		}
	};

	[UnityEditor.Callbacks.DidReloadScripts]
#pragma warning disable IDE0051 // Remove unused private members
	private static void MoodPrototypesCheck()
#pragma warning restore IDE0051 // Remove unused private members
	{

		MoodEventType[] moodEvents = (MoodEventType[])Enum.GetValues(typeof(MoodEventType));

		for(int i = 0; i < moodEvents.Length; ++i)
		{
			MoodEventType moodEvent = moodEvents[i];

			if (!moodEventPrototypes.ContainsKey(moodEvent))
			{
				Debug.LogWarning($"Mood System Warning: {nameof(moodEventPrototypes)} doesn't have a prototype entry for a {moodEvent}. No object will be instantiated when such enum value is entered.");
			}

		}


	}

	static MoodDatabase()
	{
		foreach(KeyValuePair<MoodEventType, MoodEvent> moodEventAndType in moodEventPrototypes.AsEnumerable())
		{
			moodEventAndType.Value.SetEventType(moodEventAndType.Key);
		}
	}

	public static MoodEvent GetMoodEventInstance(MoodEventType moodEventType)
	{

		if (!moodEventPrototypes.ContainsKey(moodEventType))
		{
			return null;
		}

		MoodEvent prototype = moodEventPrototypes[moodEventType];
		return new MoodEvent(prototype);
	}


}
