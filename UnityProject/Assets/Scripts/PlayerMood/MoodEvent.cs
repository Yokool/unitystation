
namespace MoodSystem
{
	/// <summary>
	/// Represents an event that affects a single player.
	/// <see cref="MoodEvent"/>s affect an object with a <see cref="PlayerMood"/> component.
	/// </summary>
	public class MoodEvent
	{

		#region MEMBER_VARIABLES

		/// <summary>
		/// Immutable.
		/// How much the event shifts the mood of a <see cref="PlayerMood"/> when it affects it.
		/// </summary>
		private int moodShift;

		/// <summary>
		/// Immutable.
		/// The name of the event, shown inside the player Chat when the user clicks the <see cref="MoodIndicator"/>. 
		/// </summary>
		private string eventName;

		/// <summary>
		/// Mutable.
		/// How much time is left for this <see cref="MoodEvent"/> instance until it gets removed from the <see cref="MoodEvent"/>
		/// component.
		///
		/// Also represents the default immutable time of this event prototype that the instance will be on the player.
		/// </summary>
		private float timeLeft;

		/// <summary>
		/// Immutable
		/// Represents the "id" of a <see cref="MoodEvent"/> prototype by which instances of this class are created through
		/// <see cref="MoodDatabase.GetMoodEventInstance(MoodEventType)"/>.
		/// </summary>
		private MoodEventType eventType;

		#endregion


		#region CONSTRUCTORS

		internal MoodEvent(string eventName, int moodShift, float timeLeft)
		{
			SetEventName(eventName);
			SetTimeLeft(timeLeft);
			SetMoodShift(moodShift);
		}

		internal MoodEvent(MoodEvent copySource)
		{
			CopyDataFrom(copySource);
		}

		internal void CopyDataFrom(MoodEvent copy)
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

		internal void SetEventName(string eventName)
		{
			this.eventName = eventName;
		}

		public string GetEventName()
		{
			return this.eventName;
		}

		internal void SetEventType(MoodEventType eventType)
		{
			this.eventType = eventType;
		}

		public MoodEventType GetEventType()
		{
			return this.eventType;
		}

		public void SetTimeLeft(float timeLeft)
		{
			this.timeLeft = timeLeft;
		}

		public float GetTimeLeft()
		{
			return this.timeLeft;
		}

		internal void SetMoodShift(int moodShift)
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
}

