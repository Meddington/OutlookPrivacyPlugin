using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deja.Crypto.BcPgp
{
	/// <summary>
	/// Public key could not be found
	/// </summary>
	public class PublicKeyNotFoundException : CryptoException
	{
		public PublicKeyNotFoundException(string message)
			: base(message)
		{
		}
	}
}
