using Mirror;

public class UpdateMoodMessage : IMessageBase
{
	/// <summary>
	/// The updated overall mood state of the player.
	/// </summary>
	private MoodState moodState;

	private MoodEventType[] affectingTypes;

	public UpdateMoodMessage()
	{

	}

	public UpdateMoodMessage(MoodState moodState, MoodEventType[] affectingTypes)
	{
		SetMoodState(moodState);
		SetCurrentAffectingTypes(affectingTypes);
	}


	private void SetMoodState(MoodState moodState)
	{
		this.moodState = moodState;
	}

	public MoodState GetMoodState()
	{
		return this.moodState;
	}

	private void SetCurrentAffectingTypes(MoodEventType[] affectingTypes)
	{
		this.affectingTypes = affectingTypes;
	}

	public MoodEventType[] GetCurrentAffectingTypes()
	{
		return affectingTypes;
	}

	public void Deserialize(NetworkReader reader)
	{
		SetMoodState((MoodState)reader.ReadInt32());
		
		int length = reader.ReadInt32();
		MoodEventType[] curEventStrings = new MoodEventType[length];

		for(int i = 0; i < length; ++i)
		{
			curEventStrings[i] = (MoodEventType)reader.ReadInt32();
		}

		SetCurrentAffectingTypes(curEventStrings);

	}

	public void Serialize(NetworkWriter writer)
	{
		writer.WriteInt32((int)GetMoodState());

		MoodEventType[] currentEventStrings = GetCurrentAffectingTypes();
		writer.WriteInt32(currentEventStrings.Length);

		for(int i = 0; i < currentEventStrings.Length; ++i)
		{
			writer.WriteInt32((int)currentEventStrings[i]);
		}

	}
}
