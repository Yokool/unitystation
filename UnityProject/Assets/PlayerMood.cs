using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

public class PlayerMood : NetworkBehaviour
{

	/// <summary>
	/// All of the current mood events that this player is affected by.
	///
	/// Two events of the same type or name can be present in the list. They both attribute to the final overall mood value.
	/// 
	/// </summary>
	private List<MoodEvent> currentAffectingMoodEvents = new List<MoodEvent>();
	/// <summary>
	/// How much time before we update the mood of this component on the server. Is constant for all the components but update times
	/// may vary depending when the component was created.
	/// </summary>
	private const float MOOD_UPDATE_TIME = 1f;

	/// <summary>
	/// How much mood shift it requires to go from one <see cref="MoodState"/> to the other.
	/// Affects positive and negative shifts equally.
	/// </summary>
	[SerializeField]
	private int neurocity = 100;

	public override void OnStartServer()
	{
		base.OnStartServer();
		UpdateManager.Add(ServerPeriodicMoodUpdate, MOOD_UPDATE_TIME);
	}

	public override void OnStartClient()
	{
		base.OnStartClient();
		Debug.Log("Registering messages.");
		NetworkClient.RegisterHandler<UpdateMoodMessage>(ClientMoodUpdate);
		NetworkClient.RegisterHandler<MoodEventMessage>(ClientEventUpdate);

		// Get the updated information from the server when the client starts
		CmdRequestMoodUpdateMessage();
	}

	private void OnDestroy()
	{
		if (isServer)
		{
			UpdateManager.Remove(CallbackType.PERIODIC_UPDATE, ServerPeriodicMoodUpdate);
		}
	}

	[Command]
	public void CmdRequestMoodUpdateMessage()
	{
		ServerSendMoodUpdateToClient();
	}

	
	[Server]
	private void ServerSendMoodUpdateToClient()
	{
		
		MoodState overallMood = GetOverallMood();

		string[] currentEventNames = GetMoodLines().ToArray();
		UpdateMoodMessage updateMoodMessage = new UpdateMoodMessage(overallMood, currentEventNames);

		connectionToClient.Send(updateMoodMessage);
	}

	[Server]
	private void ServerSendNewEventToClient(MoodEvent changingEvent, bool removed = false)
	{
		int eventShift = changingEvent.GetMoodShift();

		// If we're getting rid of an event that is in the client's mood event list
		// Then the event's shift is 
		//
		if (removed)
		{
			eventShift *= -1;
		}

		MoodEventMessage moodEventMessage = new MoodEventMessage(eventShift);
		connectionToClient.Send(moodEventMessage);
	}

	private void ClientMoodUpdate(UpdateMoodMessage msg)
	{
		MoodIndicator indicator = UIManager.Instance.moodIndicator;

		if (indicator == null)
		{
			Debug.LogError($"{nameof(MoodIndicator)} is null. Can't update indicator with the updated mood.");
			return;
		}

		indicator.UpdateIndicator(msg.GetMoodState());
		indicator.SetCachedEventLines(msg.GetCurrentEventStrings());
	}

	private void ClientEventUpdate(MoodEventMessage msg)
	{
		string chatString = msg.MoodUpdateString();
		Chat.AddExamineMsgToClient(chatString);
	}

	[Server]
	public void ServerPeriodicMoodUpdate()
	{
		MoodEvent[] moodCopy = new MoodEvent[currentAffectingMoodEvents.Count];

		List<int> toRemove = new List<int>();
		currentAffectingMoodEvents.CopyTo(moodCopy);

		for (int i = 0; i < moodCopy.Length; ++i)
		{
			MoodEvent affectingEvent = moodCopy[i];

			affectingEvent.SubtractTime(MOOD_UPDATE_TIME);

			if (affectingEvent.IsFinished())
			{
				toRemove.Add(i);
			}
		}

		for(int i = 0; i < toRemove.Count; ++i)
		{
			int indexToRemove = toRemove[i];
			ServerRemoveMoodAt(indexToRemove);
		}
	}

	/// <summary>
	/// Returns a list of names of all the current afflicting events.
	/// A single entry relates to a single afflicting mood event.
	/// If two or more events with the same name (not enum ID) are present, then the line will end with (Nx) where N is the number of events with the same name.
	/// 
	/// <br></br>
	/// Example:
	/// Had a smoke.
	/// Ate good food. (2x)
	/// </summary>
	/// <returns></returns>
	public List<string> GetMoodLines()
	{
		List<string> lines = new List<string>();
		List<MoodEventType> processedTypes = new List<MoodEventType>();

		for (int i = 0; i < currentAffectingMoodEvents.Count; ++i)
		{
			MoodEvent affectingMood = currentAffectingMoodEvents[i];
			MoodEventType moodEventType = affectingMood.GetEventType();

			string eventName = affectingMood.GetEventName();
			
			if (processedTypes.Contains(moodEventType))
			{
				continue;
			}
			
			int sameEventCount = currentAffectingMoodEvents.Count((otherMood) => {
				return moodEventType.Equals(otherMood.GetEventType());
			});

			if (sameEventCount > 1)
			{
				eventName += $" ({sameEventCount}x)";
			}

			processedTypes.Add(moodEventType);
			lines.Add(eventName);
		}


		return lines;

	}

	

	[Server]
	public void ServerAddMood(MoodEventType moodEventType)
	{
		MoodEvent instance = MoodDatabase.GetMoodEventInstance(moodEventType);
		ServerAddMood(instance);
	}

	[Server]
	public void ServerAddMood(MoodEvent moodEvent)
	{
		currentAffectingMoodEvents.Add(moodEvent);
		ServerSendMoodUpdateToClient();
		ServerSendNewEventToClient(moodEvent, false);
	}

	[Server]
	public void ServerRemoveMood(MoodEvent moodEvent)
	{

		if (!currentAffectingMoodEvents.Contains(moodEvent))
		{
			Logger.LogError("Tried to remove a mood event that is not in the moodList."); // TODO: MAKE LOGGING MESSAGE BETTER
			return;
		}

		currentAffectingMoodEvents.Remove(moodEvent);
		ServerSendNewEventToClient(moodEvent, true);
		ServerSendMoodUpdateToClient();
	}

	[Server]
	private void ServerRemoveMoodAt(int index)
	{
		MoodEvent moodEvent = currentAffectingMoodEvents[index];
		ServerRemoveMood(moodEvent);
	}

	public int GetOverallMoodValue()
	{
		int endSum = 0;
		for(int i = 0; i < currentAffectingMoodEvents.Count; ++i)
		{
			MoodEvent moodEvent = currentAffectingMoodEvents[i];
			endSum += moodEvent.GetMoodShift();
		}
		return endSum;
	}

	private void SetNeurocity(int neurocity)
	{
		// Having to change the overall mood by 0 to get to the next category doesn't make sense.
		this.neurocity = neurocity != 0 ? neurocity : 1;
	}

	private int GetNeurocity()
	{
		return this.neurocity;
	}
	
	public MoodState GetOverallMood()
	{
		int overallMoodValue = GetOverallMoodValue();

		// Done so -0.75 is -1 and not 0
		int moodID = Mathf.CeilToInt(Math.Abs(overallMoodValue) / (float)GetNeurocity());
		moodID *= (int)Mathf.Sign(overallMoodValue);

		MoodState[] values = (MoodState[])Enum.GetValues(typeof(MoodState));
		
		int min = (int)values.Min();
		int max = (int)values.Max();

		moodID = Mathf.Clamp(moodID, min, max);

		return (MoodState)moodID;
	}

	public bool IsAfflictedWith(MoodEventType afflictionType)
	{
		return currentAffectingMoodEvents.Any((moodEvent) => { return moodEvent.GetEventType().Equals(afflictionType); });
	}
	

}
