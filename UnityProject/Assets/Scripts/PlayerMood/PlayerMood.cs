using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;

namespace MoodSystem
{
	/// <summary>
	/// The component that represents the Player object's mood.
	/// </summary>
	public class PlayerMood : NetworkBehaviour
	{

		/// <summary>
		///
		/// Only serverside contains actual information about the component.
		/// All of the current mood events that this player is affected by.
		/// Two events of the same type or name can be present in the list.
		///
		/// </summary>

		private List<MoodEvent> currentAffectingMoodEvents = new List<MoodEvent>();
		/// <summary>
		/// Serverside config var
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

		/// <summary>
		/// Clientside
		/// Used on the client to hold the MoodState of the last received mood message.
		/// Initialized to null, before the client gets the first message from the server.
		/// </summary>
		private MoodState? lastClientMoodState = null;

		public override void OnStartServer()
		{
			base.OnStartServer();
			UpdateManager.Add(ServerPeriodicMoodUpdate, MOOD_UPDATE_TIME);
		}

		public override void OnStartClient()
		{
			base.OnStartClient();
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

		/// <summary>
		/// Clientside command
		/// A command that requests server information about this <see cref="PlayerMood"/> component.
		/// </summary>
		[Command]
		public void CmdRequestMoodUpdateMessage()
		{
			ServerSendMoodUpdateToClient();
		}

		/// <summary>
		/// Serverside
		/// Sends the Server information about this <see cref="PlayerMood"/> to the associated client.
		/// </summary>
		[Server]
		private void ServerSendMoodUpdateToClient()
		{

			MoodState overallMood = GetOverallMood();

			UpdateMoodMessage updateMoodMessage = new UpdateMoodMessage(overallMood, GetAffectingMoodEventTypes());

			connectionToClient.Send(updateMoodMessage);
		}

		/// <summary>
		/// Serverside
		/// Returns a list of all the <see cref="MoodEvent"/> instances by their <see cref="MoodEventType"/> id.
		/// </summary>
		private MoodEventType[] GetAffectingMoodEventTypes()
		{
			return currentAffectingMoodEvents.Select((moodEvent) => { return moodEvent.GetEventType(); }).ToArray();
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

		/// <summary>
		/// Clientside
		/// The method called when the client receives the updated information about its object on the server.
		/// Updates the <see cref="MoodIndicator"/> and prints a message if the mood chanched from one category to another.
		/// </summary>
		/// <param name="msg"></param>
		private void ClientMoodUpdate(UpdateMoodMessage msg)
		{
			MoodState msgMoodState = msg.GetMoodState();
			List<string> currentEventStrings = PlayerMood.GetMoodLines(msg.GetCurrentAffectingTypes());

			ClientPrintMoodUpdateMsg(msgMoodState);
			lastClientMoodState = msgMoodState;

			ClientUpdateIndicator(msgMoodState, currentEventStrings);
		}

		/// <summary>
		/// Clientisde
		/// </summary>
		/// <param name="newMoodState"></param>
		private void ClientPrintMoodUpdateMsg(MoodState newMoodState)
		{
			if (!lastClientMoodState.HasValue)
			{
				return;
			}

			int newMoodStateInt = (int)newMoodState;
			int lastClientMoodStateInt = (int)lastClientMoodState;

			if (newMoodState == lastClientMoodState)
			{
				return;
			}

			if (newMoodStateInt > lastClientMoodStateInt)
			{
				Chat.AddExamineMsgToClient("My mood gets better.");
			}
			else
			{
				Chat.AddExamineMsgToClient("My mood gets worse.");
			}

		}

		/// <summary>
		/// Clientside
		/// Updates the mood indicator with the param <see cref="MoodState"/> and caches the event lines passed also
		/// through the parameter.
		/// </summary>
		/// <param name="msgMoodState"></param>
		/// <param name="currentEventStrings"></param>
		private void ClientUpdateIndicator(MoodState msgMoodState, List<string> currentEventStrings)
		{
			MoodIndicator indicator = UIManager.Instance.moodIndicator;

			if (indicator == null)
			{
				Debug.LogError($"{nameof(MoodIndicator)} is null. Can't update indicator with the updated mood.");
				return;
			}

			indicator.UpdateIndicator(msgMoodState);
			indicator.SetCachedEventLines(currentEventStrings);
		}

		/// <summary>
		/// Called on the client from the server, when a new <see cref="MoodEvent"/> is added to this player on the server or if an already present event is removed
		/// on the server from this client.
		/// </summary>
		/// <param name="msg"></param>
		private void ClientEventUpdate(MoodEventMessage msg)
		{
			// No implementation as of now.
		}

		/// <summary>
		/// Serverside.
		/// Called periodically as defined by <see cref="MOOD_UPDATE_TIME"/>.
		/// Removes <see cref="MoodEvent"/> instances from this component when they expire.
		/// </summary>
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

			for (int i = 0; i < toRemove.Count; ++i)
			{
				int indexToRemove = toRemove[i];
				ServerRemoveMoodAt(indexToRemove);
			}
		}

		/// <summary>
		/// Returns a list of names of the <see cref="MoodEvent"/> prototypes by their <see cref="MoodEventType"/> id.
		/// A single entry relates to a single afflicting mood event.
		/// If two or more events with the same name (not enum ID) are present, then the line will end with (Nx) where N
		/// is the number of events with the same
		/// <see cref="MoodEventType"/> ID.
		/// 
		/// <br></br>
		/// Example:
		/// Had a smoke.
		/// Ate good food. (2x)
		/// </summary>
		public static List<string> GetMoodLines(MoodEventType[] moodEventTypes)
		{
			List<string> lines = new List<string>();
			List<MoodEventType> processedTypes = new List<MoodEventType>();

			for (int i = 0; i < moodEventTypes.Length; ++i)
			{
				MoodEventType moodEventType = moodEventTypes[i];

				string eventName = MoodDatabase.GetMoodEventInstance(moodEventType).GetEventName();

				if (processedTypes.Contains(moodEventType))
				{
					continue;
				}

				int sameEventCount = 1;

				for (int j = i + 1; j < moodEventTypes.Length; ++j)
				{
					MoodEventType differentMember = moodEventTypes[j];

					if (differentMember.Equals(moodEventType))
					{
						++sameEventCount;
					}

				}

				if (sameEventCount > 1)
				{
					eventName += $" ({sameEventCount}x)";
				}

				processedTypes.Add(moodEventType);
				lines.Add(eventName);
			}


			return lines;

		}

		/// <summary>
		/// Serverside.
		/// Returns the <see cref="MoodEvent"/> names for all the affecting events on this component.
		/// </summary>
		/// <returns></returns>
		public List<string> GetMoodLines()
		{

			List<MoodEventType> moodEventTypes = new List<MoodEventType>();

			for (int i = 0; i < currentAffectingMoodEvents.Count; ++i)
			{
				moodEventTypes.Add(currentAffectingMoodEvents[i].GetEventType());
			}

			return PlayerMood.GetMoodLines(moodEventTypes.ToArray());

		}

		/// <summary>
		/// Serverside
		/// Adds a mood on a server by its <see cref="MoodEventType"/> prototype ID.
		/// Sends an update to the client owner of this component.
		/// </summary>
		/// <param name="moodEventType"></param>
		[Server]
		public void ServerAddMood(MoodEventType moodEventType)
		{
			MoodEvent instance = MoodDatabase.GetMoodEventInstance(moodEventType);
			ServerAddMood(instance);
		}

		/// <summary>
		/// Serverside
		/// Adds a <see cref="MoodEvent"/> instance by reference.
		/// Used internally by this component.
		/// Use of <see cref="ServerAddMood(MoodEventType)"/> is preferred.
		/// </summary>
		/// <param name="moodEvent"></param>
		[Server]
		private void ServerAddMood(MoodEvent moodEvent)
		{
			currentAffectingMoodEvents.Add(moodEvent);
			ServerSendMoodUpdateToClient();
			ServerSendNewEventToClient(moodEvent, false);
		}

		/// <summary>
		/// Serverside.
		/// Used internally.
		/// Removes a <see cref="MoodEvent"/> by reference.
		/// <see cref="MoodEvent"/>s are not removed publicly as of now, as they are managed
		/// by the <see cref="ServerPeriodicMoodUpdate"/> method.
		/// </summary>
		/// <param name="moodEvent"></param>
		[Server]
		private void ServerRemoveMood(MoodEvent moodEvent)
		{

			if (!currentAffectingMoodEvents.Contains(moodEvent))
			{
				Logger.LogErrorFormat("Tried to remove a mood event of type: {0}.", Category.Unknown, moodEvent.GetEventType());
				return;
			}

			currentAffectingMoodEvents.Remove(moodEvent);
			ServerSendNewEventToClient(moodEvent, true);
			ServerSendMoodUpdateToClient();
		}

		/// <summary>
		/// Serverside.
		/// Used internally.
		/// Removes a MoodEvent instance at the param index from the <see cref="currentAffectingMoodEvents"/> list.
		/// </summary>
		[Server]
		private void ServerRemoveMoodAt(int index)
		{
			MoodEvent moodEvent = currentAffectingMoodEvents[index];
			ServerRemoveMood(moodEvent);
		}

		public int GetOverallMoodValue()
		{
			int endSum = 0;
			for (int i = 0; i < currentAffectingMoodEvents.Count; ++i)
			{
				MoodEvent moodEvent = currentAffectingMoodEvents[i];
				endSum += moodEvent.GetMoodShift();
			}
			return endSum;
		}

		public void SetNeurocity(int neurocity)
		{
			// Having to change the overall mood by 0 to get to the next category doesn't make sense.
			this.neurocity = neurocity != 0 ? neurocity : 1;
		}

		public int GetNeurocity()
		{
			return this.neurocity;
		}

		/// <summary>
		/// Gets the overall mood of the player returned as a <see cref="MoodState"/> enum.
		/// </summary>
		/// <returns></returns>
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

		/// <summary>
		/// Currently serverside.
		/// Checks if a player is afflicted with an <see cref="MoodEvent"/> by its <see cref="MoodEventType"/> id.
		/// </summary>
		/// <param name="afflictionType"></param>
		/// <returns></returns>
		[Server]
		public bool IsAfflictedWith(MoodEventType afflictionType)
		{
			return currentAffectingMoodEvents.Any((moodEvent) => { return moodEvent.GetEventType().Equals(afflictionType); });
		}


	}
}

