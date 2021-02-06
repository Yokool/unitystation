using UnityEngine;
using System;
using System.Collections.Generic;

public class ArrayMoodSprites : MonoBehaviour, IMoodSpriteProvider
{
	[SerializeField]
	private Sprite[] sprites;

	private Dictionary<MoodState, Sprite> spritesDictionary = new Dictionary<MoodState, Sprite>();

	private void Awake()
	{
		// Will check for any errors and initialize the spritesDictionary
		SetSprites(sprites);
	}

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

	private void UpdateDictionary()
	{
		MoodState[] moodStateEnums = (MoodState[])Enum.GetValues(typeof(MoodState));

		// GetValues is sorted with the binary values, not arithmetically
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

