
namespace MoodSystem
{
	/// <summary>
	/// The underlying value logically matters. Do not change.
	/// 
	/// Keep this enum ordered (for visual clarity) by their underlying values.
	/// The order matters as its the order from the worst to the best mood.
	/// An enum entry with the lowest underlying value is the worst mood.
	/// An enum entry with the lowest underlying value 0 is the neutral mood.
	/// An enum entry with the highest underlying value is the best mood.
	/// 
	/// The rest of the mood categories are defined between them.
	/// For example (-1) is the first bad mood.
	/// (-2) is the second bad mood.
	///
	/// The order also matters because the <see cref="ArrayMoodSprites.sprites"/>'s sprites are sorted in the
	/// order of this enum.
	/// 
	/// <br></br>
	/// <br></br>
	/// Details on why order matters see:
	/// <br></br>
	/// <see cref="PlayerMood.GetOverallMood"/>
	/// <br></br>
	/// <see cref="ArrayMoodSprites.sprites"/>
	/// </summary>
	public enum MoodState
	{

		SUICIDAL = -4,
		VERY_UNHAPPY = -3,
		UNHAPPY = -2,
		SLIGHTLY_UNHAPPY = -1,
		NEUTRAL = 0,
		SLIGHTLY_HAPPY = 1,
		HAPPY = 2,
		VERY_HAPPY = 3,
		EUPHORIOUS = 4

	}
}

