using Mirror;

public class UpdateMoodMessage : IMessageBase
{
	/// <summary>
	/// The updated overall mood state of the player.
	/// </summary>
	private MoodState moodState;

	/// <summary>
	/// Contains a list of string representing the names of all the active <see cref="MoodEvent"/>s on the receiving player of this message.
	/// </summary>
	private string[] currentEventStrings;

	public UpdateMoodMessage()
	{

	}

	public UpdateMoodMessage(MoodState moodState, string[] currentEventStrings)
	{
		SetMoodState(moodState);
		SetCurrentEventStrings(currentEventStrings);
	}


	private void SetMoodState(MoodState moodState)
	{
		this.moodState = moodState;
	}

	public MoodState GetMoodState()
	{
		return this.moodState;
	}

	private void SetCurrentEventStrings(string[] currentEventStrings)
	{
		this.currentEventStrings = currentEventStrings;
	}

	public string[] GetCurrentEventStrings()
	{
		return this.currentEventStrings;
	}

	public void Deserialize(NetworkReader reader)
	{
		SetMoodState((MoodState)reader.ReadInt32());
		
		int length = reader.ReadInt32();
		string[] curEventStrings = new string[length];

		for(int i = 0; i < length; ++i)
		{
			curEventStrings[i] = reader.ReadString();
		}

		SetCurrentEventStrings(curEventStrings);

	}

	public void Serialize(NetworkWriter writer)
	{
		writer.WriteInt32((int)GetMoodState());
		
		string[] currentEventStrings = GetCurrentEventStrings();
		writer.WriteInt32(currentEventStrings.Length);

		for(int i = 0; i < currentEventStrings.Length; ++i)
		{
			writer.WriteString(currentEventStrings[i]);
		}

	}
}
