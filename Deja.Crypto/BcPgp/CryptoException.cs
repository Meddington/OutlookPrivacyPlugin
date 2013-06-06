using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deja.Crypto.BcPgp
{
	/// <summary>
	/// Generic exception during crypto process.
	/// </summary>
	public class CryptoException : Exception
	{
		public CryptoException(string message)
			: base(message)
		{
		}
	}
}
