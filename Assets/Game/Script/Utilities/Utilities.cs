using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilitie
{
	public static void Shuffle<T>(this IList<T> list)
	{
		// Source: http://stackoverflow.com/questions/273313/randomize-a-listt-in-c-sharp
		System.Random rng = new System.Random();
		int n = list.Count;
		while (n > 1)
		{
			n--;
			int k = rng.Next(n + 1);
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}
}
