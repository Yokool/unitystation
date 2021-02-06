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

	public void Deserialize(NetworkReader reader)
	{
		SetMoodShift(reader.ReadInt32());
	}

	public void Serialize(NetworkWriter writer)
	{
		writer.WriteInt32(GetMoodShift());
	}

}
