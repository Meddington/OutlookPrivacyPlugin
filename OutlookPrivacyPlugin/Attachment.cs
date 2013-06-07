using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OutlookPrivacyPlugin
{
	public class Attachment
	{
		public string FileName { get; set; }
		public string DisplayName { get; set; }
		public object AttachmentType { get; set; }
		public string TempFile { get; set; }
		public bool Encrypted { get; set; }
	}
}
