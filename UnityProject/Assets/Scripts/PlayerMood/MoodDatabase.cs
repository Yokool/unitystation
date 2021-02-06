using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MoodDatabase
{
	/// <summary>
	/// Consider looking at <see cref="PlayerMood.neurocity"/> when deciding for the value of the mood event. The initialized value is the default standard
	/// for all objects with a mood component.
	/// </summary>
	private static readonly Dictionary<MoodEventType, MoodEvent> moodEventPrototypes = new Dictionary<MoodEventType, MoodEvent>()
	{
		{
			MoodEventType.HAD_BITE_GOOD_FOOD,
			new MoodEvent("I've had some tasty food!", 50, 60f)
		},
		{
			MoodEventType.HAD_BITE_VERY_GOOD_FOOD,
			new MoodEvent("I've had some exquisite food!", 100, 120f)
		},
		{
			MoodEventType.HAD_BITE_WONDERFUL_FOOD,
			new MoodEvent("I've NEVER tasted anything more WONDERFUL!", 250, 240f)
		},
		{
			MoodEventType.HAD_BITE_BAD_FOOD,
			new MoodEvent("I've eaten some off-tasting food.", -50, 240f)
		},
		{
			MoodEventType.HAD_BITE_VERY_BAD_FOOD,
			new MoodEvent("I've eaten some disgusting food.", -100, 120f)
		},
		{
			MoodEventType.HAD_BITE_HORRIBLE,
			new MoodEvent("I've eaten some HORRIBLE food. The memory of it BURNT onto my tongue continues to plague my mind and stomach!", -250, 240f)
		}
	};

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
