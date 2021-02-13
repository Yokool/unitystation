using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;


namespace MoodSystem
{
	/// <summary>
	/// An indicator of the player mood.
	/// Requires an <see cref="Image"/> component that will display the final sprite and a <see cref="IMoodSpriteProvider"/> derived component.
	/// </summary>
	[RequireComponent(typeof(Image))]
	public class MoodIndicator : MonoBehaviour
	{
		/// <summary>
		/// Contains a Dictionary that translates a <see cref="MoodState"/> to a <see cref="Sprite"/> used when updating
		/// the mood.
		/// </summary>
		private IMoodSpriteProvider moodStateSpriteProvider;

		private Image moodImage;

		/// <summary>
		/// Contains a cached list of <see cref="PlayerMood.GetMoodLines()"/> for the player on the server.
		/// </summary>
		private List<string> cachedEventLines = null;

		private void OnEnable()
		{
			moodImage = GetComponent<Image>();

			if(TryGetComponent(out IMoodSpriteProvider provider))
			{
				moodStateSpriteProvider = provider;
			}
			else
			{
				Debug.LogError($"This {nameof(MoodIndicator)} component on gameObject: {gameObject} doesn't have a {nameof(IMoodSpriteProvider)} component.");
			}
			
		}

		/// <summary>
		/// Updates the indicator to the <see cref="MoodState"/> passed in the parameter.
		/// </summary>
		public void UpdateIndicator(MoodState newMoodState)
		{
			Dictionary<MoodState, Sprite> supportedSprites = moodStateSpriteProvider.GetSprites();

			if (!supportedSprites.ContainsKey(newMoodState))
			{
				Debug.LogError($"The object {gameObject} doesn't have a sprite for the mood - {newMoodState}.");
				return;
			}

			Sprite moodSprite = supportedSprites[newMoodState];
			moodImage.sprite = moodSprite;

		}

		public void SetCachedEventLines(List<string> cachedEventLines)
		{
			this.cachedEventLines = cachedEventLines;
		}

		/// <summary>
		/// Displays the <see cref="cachedEventLines"/> to the chat on the client.
		/// </summary>
		public void MoodOnClick()
		{
			Chat.AddExamineMsgToClient("How I feel:");
			if (cachedEventLines == null)
			{
				return;
			}

			for (int i = 0; i < cachedEventLines.Count; ++i)
			{
				string line = cachedEventLines[i];
				Chat.AddExamineMsgToClient(line);
			}

		}

	}
}


