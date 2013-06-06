using System;

using Libgpgme;

namespace ListKeys
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Context ctx = new Context();
			
			KeyStore store = ctx.KeyStore;
			
			Key[] publickeys = store.GetKeyList("", false);
			Key[] secretkeys = store.GetKeyList("", true);
			
			Console.WriteLine("Public PGP keys currently saved in your store:");
			foreach (Key key in publickeys)
			{
				Console.WriteLine("Key " + key.Fingerprint);
				Console.WriteLine("\tUser: {0}\n", key.Uid.ToString());
			}
			
			Console.WriteLine("\nSecret PGP keys currently saved in your store:");
			foreach (Key key in secretkeys)
			{
				Console.WriteLine("Key " + key.Fingerprint);
				Console.WriteLine("\tUser: {0}\n", key.Uid.ToString());
			}

		}
	}
}

