using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Deja.Crypto.BcPgp
{
	/// <summary>
	/// Unable to verify signature
	/// </summary>
	public class VerifyException : CryptoException
	{
		public VerifyException(string message)
			: base(message)
		{
		}
	}
}
