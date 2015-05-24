
using System;
using System.Collections.Generic;

namespace OutlookPrivacyPlugin
{
	public class GnuKey
	{
		public string KeyDisplay { get; set; }
		public string Key { get; set; }
		public string Expiry { get; set; }
		public string KeyId { get; set; }
	}

	internal class GnuKeySorter : IComparer<GnuKey>
	{
		public int Compare(GnuKey x, GnuKey y)
		{
			return x.KeyDisplay.CompareTo(y.KeyDisplay);
		}
	}
}