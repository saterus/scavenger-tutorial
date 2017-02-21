using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace ListSampling
{
	public static class ExtensionMethods
	{
		public static T SampleFrom<T> (this IList<T> list)
		{
			return list [Random.Range (0, list.Count)];
		}
	}

}
