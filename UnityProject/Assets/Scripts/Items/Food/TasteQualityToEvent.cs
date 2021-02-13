using System.Collections.Generic;

namespace MoodSystem
{
	public static class TasteQualityToEvent
	{

		private static readonly Dictionary<TasteQuality, MoodEventType> tasteQualityToEvent = new Dictionary<TasteQuality, MoodEventType>()
	{
		{
			TasteQuality.VOMIT_INDUCING,
			MoodEventType.HAD_BITE_HORRIBLE
		},
		{
			TasteQuality.VERY_BAD,
			MoodEventType.HAD_BITE_VERY_BAD_FOOD
		},
		{
			TasteQuality.BAD,
			MoodEventType.HAD_BITE_BAD_FOOD
		},
		{
			TasteQuality.GOOD,
			MoodEventType.HAD_BITE_GOOD_FOOD
		},
		{
			TasteQuality.VERY_GOOD,
			MoodEventType.HAD_BITE_VERY_GOOD_FOOD
		},
		{
			TasteQuality.EUPHORIC,
			MoodEventType.HAD_BITE_WONDERFUL_FOOD
		},

	};

		public static MoodEventType ToEventType(TasteQuality tasteQuality)
		{
			return tasteQualityToEvent[tasteQuality];
		}

	}
}
