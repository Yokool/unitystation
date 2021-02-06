using UnityEngine;
using UnityEngine.UI;
using Mirror;
using System.Collections.Generic;

[RequireComponent(typeof(Image))]
public class MoodIndicator : MonoBehaviour
{

	private IMoodSpriteProvider moodStateSpriteProvider;

	private Image moodImage;

	private List<string> cachedEventLines = null;

	private void OnEnable()
	{
		moodImage = GetComponent<Image>();
		moodStateSpriteProvider = GetComponent<IMoodSpriteProvider>();
	}

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

	public void MoodOnClick()
	{
		Chat.AddExamineMsgToClient("How I feel:");
		if(cachedEventLines == null)
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

