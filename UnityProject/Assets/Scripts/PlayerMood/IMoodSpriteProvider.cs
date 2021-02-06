using UnityEngine;
using System.Collections.Generic;

public interface IMoodSpriteProvider
{
	Dictionary<MoodState, Sprite> GetSprites();
}

