using System;
using System.Text;
using System.IO;

using Libgpgme;

namespace DataBufferTest
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			// Create some sample data
			byte[] bytedata = new byte[1024];
			for(int i=0; i < bytedata.Length; i++)
				bytedata[i] = (byte) (i % 256);
			
			// Create memory based buffer for GPGME
			GpgmeMemoryData memdata = new GpgmeMemoryData();
			// write sample data into the GPGME memory based buffer
			Console.WriteLine("Bytes written: " + memdata.Write(bytedata, bytedata.Length));
			
			// Set the cursor to the beginning
			Console.WriteLine("Seek to begin: " + memdata.Seek(0, SeekOrigin.Begin));
			
			// Re-read the data into a tempory buffer
			byte[] tmp = new byte[bytedata.Length];
			Console.WriteLine("Bytes read: " + memdata.Read(tmp));
			
			// Create stream based buffer (CBS)
			MemoryStream memstream = new MemoryStream(tmp);
			GpgmeStreamData streamdata = new GpgmeStreamData(memstream);
			
			// ..
			Console.WriteLine("Bytes written: " + streamdata.Write(bytedata, bytedata.Length));
			Console.WriteLine("Seek to begin: " + streamdata.Seek(0, SeekOrigin.Begin));
			byte[] tmp2 = new byte[bytedata.Length];
			Console.WriteLine("Bytes read: " + streamdata.Read(tmp2));
			
			return;
		}
	}
}

