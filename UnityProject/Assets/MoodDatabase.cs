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
			MoodEventType.HAD_BITE_GOOD_FOOD,
			new MoodEvent("I've had a bite of tasty food", 101, 60f)
		},
		{
			MoodEventType.HAD_BITE_BAD_FOOD,
			new MoodEvent("I've had a bite of disgusting food", -101, 60f)
		}
	};

	[UnityEditor.Callbacks.DidReloadScripts]
	private static void MoodPrototypesCheck()
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

		MoodPrototypesCheck();

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
