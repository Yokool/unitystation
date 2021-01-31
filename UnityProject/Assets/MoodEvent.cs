
/// <summary>
/// Represents an event that affects a single player.
/// </summary>
public class MoodEvent
{

	#region MEMBER_VARIABLES

	private int moodShift;

	private string eventName;

	private float timeLeft;

	private MoodEventType eventType;

	#endregion


	#region CONSTRUCTORS

	public MoodEvent(string eventName, int moodShift, float timeLeft)
	{
		SetEventName(eventName);
		SetTimeLeft(timeLeft);
		SetMoodShift(moodShift);
	}

	public MoodEvent(MoodEvent copySource)
	{
		CopyDataFrom(copySource);
	}

	private void CopyDataFrom(MoodEvent copy)
	{
		SetEventName(copy.eventName);
		SetTimeLeft(copy.timeLeft);
		SetMoodShift(copy.moodShift);
		SetEventType(copy.eventType);
	}

	#endregion

	public bool IsPositive()
	{
		return moodShift > 0;
	}

	public bool IsNeutral()
	{
		return moodShift == 0;
	}


	#region GETTERS_SETTERS

	private void SetEventName(string eventName)
	{
		this.eventName = eventName;
	}

	public string GetEventName()
	{
		return this.eventName;
	}

	public void SetEventType(MoodEventType eventType)
	{
		this.eventType = eventType;
	}

	public MoodEventType GetEventType()
	{
		return this.eventType;
	}

	private void SetTimeLeft(float timeLeft)
	{
		this.timeLeft = timeLeft;
	}

	public float GetTimeLeft()
	{
		return this.timeLeft;
	}

	private void SetMoodShift(int moodShift)
	{
		this.moodShift = moodShift;
	}

	public void SubtractTime(float time)
	{
		float nextTime = this.timeLeft - time;
		SetTimeLeft(nextTime);
	}

	public int GetMoodShift()
	{
		return this.moodShift;
	}

	public bool IsFinished()
	{
		return GetTimeLeft() <= 0f;
	}


	#endregion



}
