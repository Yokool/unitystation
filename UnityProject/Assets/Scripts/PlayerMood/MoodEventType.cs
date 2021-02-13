namespace MoodSystem
{
	/// <summary>
	/// Represents an "ID" of a single <see cref="MoodEvent"/>.
	/// <see cref="MoodEvent"/> objects are instantiated through their enum, see <see cref="MoodDatabase.GetMoodEventInstance(MoodEventType)"/>.
	/// When you want to add a new MoodEvent, first add an enum entry here, then go into <see cref="MoodDatabase"/> and
	/// add an entry to <see cref="MoodDatabase.moodEventPrototypes"/>.
	/// </summary>
	public enum MoodEventType
	{
		HAD_BITE_GOOD_FOOD,
		HAD_BITE_VERY_GOOD_FOOD,
		HAD_BITE_WONDERFUL_FOOD,
		HAD_BITE_BAD_FOOD,
		HAD_BITE_VERY_BAD_FOOD,
		HAD_BITE_HORRIBLE
	}
}

