using UnityEngine;
using System;
using System.Collections.Generic;

namespace MoodSystem
{
	/// <summary>
	/// An <see cref="IMoodSpriteProvider"/> component.
	/// Constructs the MoodState dictionary from a Sprite array.
	/// The sprite array must be of the same length as <see cref="MoodState"/>.
	/// A sprite at position 0 in the array will represent the MoodState that is defined first in <see cref="MoodState"/>, with the
	/// lowest underlying value.
	/// </summary>
	public class ArrayMoodSprites : MonoBehaviour, IMoodSpriteProvider
	{
		/// <summary>
		/// The Sprite array which is used to set the <see cref="spritesDictionary"/> that is returned to the <see cref="IMoodSpriteProvider.GetSprites"/>
		/// method call.
		///
		/// A sprite with index 0 will represent the <see cref="MoodState"/> with the lowest underlying value.
		/// A sprite with index 1 will represent the <see cref="MoodState"/> with the second lowest underlying value.
		/// ...
		///
		/// The sprites list has to have the exact length as <see cref="MoodState"/>
		/// </summary>
		[SerializeField]
		private Sprite[] sprites;

		private Dictionary<MoodState, Sprite> spritesDictionary = new Dictionary<MoodState, Sprite>();

		private void Awake()
		{
			// Will check for any errors and initialize the spritesDictionary
			SetSprites(sprites);
		}

		/// <summary>
		/// Setter for <see cref="sprites"/>.
		/// Will check for any errors and display them in the Debug console also updates
		/// <see cref="spritesDictionary"/>.
		/// </summary>
		private void SetSprites(Sprite[] sprites)
		{

			int moodStateLength = Enum.GetValues(typeof(MoodState)).Length;

			if (sprites.Length < moodStateLength)
			{
				Debug.LogError($"{nameof(ArrayMoodSprites)} error: The provided {nameof(sprites)} array is smaller than the number of {nameof(MoodState)} enums. Please ensure that the length of the array is {moodStateLength}");
				return;
			}

			if (sprites.Length > moodStateLength)
			{
				Debug.LogError($"{nameof(ArrayMoodSprites)} error: The provided {nameof(sprites)} array is larger than the number of {nameof(MoodState)} enums. While this won't cause an error. The exceeding sprites won't ever be displayed. Ensure that the length of the array smaller than or equal to {moodStateLength}");
				return;
			}

			this.sprites = sprites;

			UpdateDictionary();
		}

		/// <summary>
		/// Initializes/Updates <see cref="spritesDictionary"/> from the set <see cref="sprites"/> field.
		/// </summary>
		private void UpdateDictionary()
		{
			MoodState[] moodStateEnums = (MoodState[])Enum.GetValues(typeof(MoodState));

			// ^GetValues is sorted with the binary values, not arithmetically so sort them
			Array.Sort(moodStateEnums);

			for (int i = 0; i < moodStateEnums.Length; ++i)
			{
				MoodState moodStateEnum = moodStateEnums[i];
				spritesDictionary.Add(moodStateEnum, sprites[i]);
			}
		}

		public Dictionary<MoodState, Sprite> GetSprites()
		{
			return spritesDictionary;
		}
	}
}


