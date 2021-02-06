
/// <summary>
/// The underlying value logically matters. Do not change.
/// For clarity - keep this enum ordered.
/// The order matters as its the order from the worst to the best mood. Jumps from one category to another are defined by <see cref="PlayerMood.neurocity"/>.
/// The order also affects <see cref="MoodIndicator"/> where the index of the <see cref="MoodIndicator.moodStateImages"/> directly maps to the underlying value of
/// the value inside this enum.
///
/// 
/// <br></br>
/// <br></br>
/// For details see:
/// <br></br>
/// <see cref="PlayerMood.GetOverallMood"/>
/// <br></br>
/// <see cref="MoodIndicator.UpdateIndicator(MoodState)"/> and <see cref="MoodIndicator.moodStateImages"/>
/// 
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
