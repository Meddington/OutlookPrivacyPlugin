
using System;
using System.Collections.Generic;

namespace OutlookPrivacyPlugin
{
	public class KeyItem
	{
		public string KeyDisplay { get; set; }
		public string Key { get; set; }
		public string Expiry { get; set; }
		public string KeyId { get; set; }
	}

	internal class KeyItemSorter : IComparer<KeyItem>
	{
		public int Compare(KeyItem x, KeyItem y)
		{
			return x.KeyDisplay.CompareTo(y.KeyDisplay);
		}
	}
}