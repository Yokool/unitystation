using Mirror;

public class MoodEventMessage : IMessageBase
{

	private int moodShift = 0;

	public MoodEventMessage()
	{

	}

	public MoodEventMessage(int moodShift)
	{
		SetMoodShift(moodShift);
	}

	public void SetMoodShift(int moodShift)
	{
		this.moodShift = moodShift;
	}

	public int GetMoodShift()
	{
		return this.moodShift;
	}

	public string MoodUpdateString()
	{
		int shift = GetMoodShift();

		if (shift == 0)
		{
			return "I feel indifferent.";
		}
		else if (shift >= 0)
		{
			return "My mood gets better.";
		}
		else
		{
			return "My mood gets worse.";
		}

	}

	public void Deserialize(NetworkReader reader)
	{
		SetMoodShift(reader.ReadInt32());
	}

	public void Serialize(NetworkWriter writer)
	{
		writer.WriteInt32(GetMoodShift());
	}

}
