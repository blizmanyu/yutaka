﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yutaka.IO
{
	public class ImageUtil : FileUtil
	{
		public ImageUtil()
		{
			IgnoreListFolders = new List<string>();
			IgnoreListFileMasks = new List<string>();
		}
	}
}