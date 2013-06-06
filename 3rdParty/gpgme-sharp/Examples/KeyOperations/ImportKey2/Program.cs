using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using Libgpgme;

namespace ImportKey2
{
    class Program
    {
        static void Main(string[] args)
        {
            // Load PGP key from file
            String pgpkeytext = File.ReadAllText(@"daniel.pub");

            // Create GPGME context
            Context ctx = new Context();

            /* Windows / C# uses Unicode to encode text 
             * (a string uses 2 bytes for each character). 
             * 
             * We need to convert the String into a byte array.
             * Since pgpkeytext contains only ASCII you can use
             * either ASCIIEncoding or UTF8Encoding.
             */
            UTF8Encoding encoder = new UTF8Encoding();
            byte[] keydata = encoder.GetBytes(pgpkeytext);

            // Create a memory stream ..
            MemoryStream memstream = new MemoryStream(keydata);
            // .. and use this stream for GPGME
            GpgmeStreamData keystream = new GpgmeStreamData(memstream);

            KeyStore store = ctx.KeyStore;
            ImportResult rst = store.Import(keystream);

            // Show the results
            Console.WriteLine("Keys imported {0}\n"
                + "Keys skipped: {1}\n"
                + "Keys unchanged: {2}\n"
                + "Keys not imported: {3}\n",
                rst.Imported,
                rst.SkippedNewKeys,
                rst.Unchanged,
                rst.NotImported);

            if (rst.Imports != null)
            {
                Console.WriteLine("Found keys: ");
                foreach (ImportStatus status in rst.Imports)
                {
                    Console.WriteLine("Key fingerprint: {0}\n",
                        status.Fpr);
                }
            }

            return;
        }
    }
}
