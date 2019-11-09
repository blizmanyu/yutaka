﻿using System;
using System.Diagnostics;
using System.IO;

namespace Yutaka.Diagnostics
{
	public class FfmpegProcess : Process
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="startTime"></param>
		/// <param name="length"></param>
		/// <param name="source"></param>
		/// <param name="fps"></param>
		/// <param name="width"></param>
		/// <param name="createWindow"></param>
		/// <returns></returns>
		public static Process StartCreatingPalette(TimeSpan startTime, TimeSpan length, string source, int fps = 24, int width = 960, bool createWindow = true)
		{
			#region Parameter Check
			var errorMsg = "";

			if (startTime.TotalSeconds < 0)
				errorMsg = String.Format("{0}<startTime> must be at least 0.{1}", errorMsg, Environment.NewLine);
			if (length.TotalSeconds < 1)
				errorMsg = String.Format("{0}<length> must be at least 1.{1}", errorMsg, Environment.NewLine);
			if (String.IsNullOrWhiteSpace(source))
				errorMsg = String.Format("{0}<source> is required.{1}", errorMsg, Environment.NewLine);
			else if (!File.Exists(source))
				errorMsg = String.Format("{0}<source> doesn't exist.{1}", errorMsg, Environment.NewLine);
			if (fps < 1)
				errorMsg = String.Format("{0}<fps> must be at least 1.{1}", errorMsg, Environment.NewLine);
			if (width < 1)
				errorMsg = String.Format("{0}<width> must be at least 1.{1}", errorMsg, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(errorMsg))
				throw new Exception(String.Format("{0}{1}", errorMsg, Environment.NewLine));
			#endregion Parameter Check

			var fi = new FileInfo(source);
			var NameWithoutExtension = fi.Name.Replace(fi.Extension, "");
			fi = null;
			var args = String.Format("-y -ss {0:hh\\:mm\\:ss\\.fff} -t {1:0.000} -i \"{2}\" -vf fps={3},scale={4}:-1:flags=lanczos,palettegen \"C:\\TEMP\\{5} {0:hhmm\\ ssff}.png\"",
				startTime, length.TotalSeconds, source, fps, width, NameWithoutExtension);

			Console.Write("\nargs: {0}\n\n", args);

			try {
				var psi = new ProcessStartInfo("ffmpeg", args) {
					CreateNoWindow = !createWindow,
					UseShellExecute = false,
				};

				return Start(psi);
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}{2}Exception thrown in FfmpegProcess.StartCreatingPalette(TimeSpan startTime, TimeSpan length, string source, int fps = 24, int width = 960, bool createWindow = true){2}{2}{1}{2}{2}", ex.Message, ex.ToString(), Environment.NewLine, createWindow));

				throw new Exception(String.Format("{0}{2}{2}Exception thrown in INNER EXCEPTION of FfmpegProcess.StartCreatingPalette(TimeSpan startTime, TimeSpan length, string source, int fps = 24, int width = 960, bool createWindow = true){2}{2}{1}{2}{2}", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, createWindow));
			}
		}

		//public static Process laksdjf(TimeSpan startTime, TimeSpan length, string source, string output, bool? overwriteAll = null, int fps = 24, int width = 960, bool createThumbnail = false, bool createWindow = true)
		//{

		//}
	}
}