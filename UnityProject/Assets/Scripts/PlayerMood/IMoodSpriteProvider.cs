using UnityEngine;
using System.Collections.Generic;

namespace MoodSystem
{
	/// <summary>
	/// An interface used to get the sprites for a <see cref="MoodIndicator"/>.
	/// </summary>
	public interface IMoodSpriteProvider
	{
		/// <summary>
		/// Return a Dictionary that associates a MoodState (of the player / mood indicator owner) to a Sprite
		/// used in the <see cref="MoodIndicator"/> to which this interface is attached to (through a component).
		/// </summary>
		Dictionary<MoodState, Sprite> GetSprites();
	}
}


