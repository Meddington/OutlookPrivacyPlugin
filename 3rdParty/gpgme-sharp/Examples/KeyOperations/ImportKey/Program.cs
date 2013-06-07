using System;
using System.Collections.Generic;
using System.Text;

using Libgpgme;

namespace ImportKey
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create GPGME context
            Context ctx = new Context();

            // Open a PGP public key from file "daniel.pub"
            GpgmeFileData keyfile = new GpgmeFileData(@"daniel.pub");

            // Import the PGP key
            KeyStore store = ctx.KeyStore;
            ImportResult rst = store.Import(keyfile);
            
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
