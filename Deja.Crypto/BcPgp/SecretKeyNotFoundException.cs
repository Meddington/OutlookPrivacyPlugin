using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deja.Crypto.BcPgp
{
	/// <summary>
	/// Secret key could not be found
	/// </summary>
	public class SecretKeyNotFoundException : CryptoException
	{
		public SecretKeyNotFoundException(string message)
			: base(message)
		{
		}
	}
}
