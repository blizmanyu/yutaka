﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Yutaka.IO;

namespace Yutaka.Diagnostics.Tests
{
	class Program
	{
		private static bool consoleOut = true; // default = false //

		#region Fields
		#region Static Externs
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		const int SW_HIDE = 0;
		#endregion

		// Constants //
		const string PROGRAM_NAME = "Yutaka.Diagnostics.Tests";
		const string TIMESTAMP = @"[HH:mm:ss] ";

		// PIVs //
		private static DateTime startTime = DateTime.Now;
		private static FileUtil _fileUtil = new FileUtil();
		private static int errorCount = 0;
		private static int totalCount = 0;
		private static int errorCountThreshold = 7;
		private static double errorPerThreshold = 0.07;
		#endregion

		static void Main(string[] args)
		{
			StartProgram();
			Test_CreateAnimatedGif2();
			EndProgram();
		}

		// Modified Nov 19, 2019 // Created Nov 19, 2019 //
		private static void Test_CreateAnimatedGif2()
		{
			double i, startTime, duration;
			string folder, filename, source, destFolder;

			#region Video 1
			startTime = 0.0;
			duration = 1585;
			folder = @"asdf\";
			filename = @"asdf.mp4";
			source = Path.Combine(folder, filename);
			destFolder = Path.Combine(@"G:\TEMP\", filename.Replace(".mp4", ""));

			// 1st 5min //
			for (i = startTime; i < startTime + 300; i += 10)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);

			// Middle //
			for (i = startTime + 350; i < duration-305; i += 60)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);

			// Last 5min //
			for (i = duration - 300; i < duration; i += 10)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);
			#endregion Video 1

			#region Video 2
			startTime = 30.0;
			duration = 2162;
			folder = @"asdf\";
			filename = @"asdf.mp4";
			source = Path.Combine(folder, filename);
			destFolder = Path.Combine(@"G:\TEMP\", filename.Replace(".mp4", ""));

			// 1st 5min //
			for (i = startTime; i < startTime + 300; i += 10)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);

			// Middle //
			for (i = startTime + 350; i < duration - 305; i += 60)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);

			// Last 5min //
			for (i = duration - 300; i < duration; i += 10)
				FfmpegUtil.CreateAnimatedGif(i, source, false, -1, -1, -1, null, true, true);

			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
			_fileUtil.CreateGalleryHtml(destFolder);
			#endregion Video 2
		}

		// Modified Jan 9, 2020 // Created Jan 9, 2020 //
		private static void Test_FileUtil_CreateGalleryHtmlInEachSubfolder()
		{
			var path = @"asdf";
			_fileUtil.CreateGalleryHtmlInEachSubfolder(path);
		}

		private static void Test_FileUtil_CreateGalleryHtml()
		{
			var destFolder = @"asdf\";
			_fileUtil.CreateGalleryHtml(destFolder);
		}

		private static void Test_CreateAnimatedGif()
		{
			double startTime;
			var folder = @"asdf\";
			var filename = @"asdf.mp4";
			var source = Path.Combine(folder, filename);
			var destFolder = Path.Combine(@"G:\TEMP\", filename.Replace(".mp4", ""));

			//for (int i = 0; i < 300; i += 10) {
			//	startTime = i;
			//	using (var proc1 = FfmpegUtil.StartCreatingPalette(startTime, source)) {
			//		proc1.WaitForExit();

			//		using (var proc2 = FfmpegUtil.StartCreatingAnimatedGif(startTime, source, true)) {
			//			proc2.WaitForExit();

			//			using (var proc3 = FfmpegUtil.StartCreatingThumbnail(startTime, source, true)) {
			//				proc3.WaitForExit();
			//			}
			//		}
			//	}
			//}

			for (int i = 2578; i < 2878; i += 10) {
				startTime = i;
				using (var proc1 = FfmpegUtil.StartCreatingPalette(startTime, source)) {
					proc1.WaitForExit();

					using (var proc2 = FfmpegUtil.StartCreatingAnimatedGif(startTime, source, true)) {
						proc2.WaitForExit();

						using (var proc3 = FfmpegUtil.StartCreatingThumbnail(startTime, source, true)) {
							proc3.WaitForExit();
						}
					}
				}
			}

			_fileUtil.CreateGalleryHtml(destFolder);
			_fileUtil.Delete(destFolder, "*.png", SearchOption.TopDirectoryOnly);
		}

		private static void Test_GetUpTime()
		{
			Console.Write("\nUp Time: {0}", ProcessUtil.GetUpTime());
		}

		private static void Test_RestartComputer()
		{
			ProcessUtil.RestartComputer();
			ProcessUtil.RestartComputer(force: false);
			ProcessUtil.RestartComputer(waitTime: 61);
			ProcessUtil.RestartComputer(remoteCompName: "laksjdf");
			ProcessUtil.RestartComputer(createWindow: true);
		}

		private static void Test_Path_Combine()
		{
			var DefaultPaletteFolder = @"G:\TEMP\";
			var NameWithoutExtension = "NameWithoutExtension";
			var startTime = 0.0;
			Console.Write("\n\nPath: {0}\n\n", Path.Combine(DefaultPaletteFolder, NameWithoutExtension, startTime.ToString("00000.00'.png'")));
		}

		#region Start & EndProgram
		static void StartProgram()
		{
			var log = String.Format("Starting {0} program", PROGRAM_NAME);

			if (consoleOut) {
				Console.Clear();
				Console.Write("{0}{1}", DateTime.Now.ToString(TIMESTAMP), log);
			}

			else {
				var handle = GetConsoleWindow();
				ShowWindow(handle, SW_HIDE); // hide window //
			}
		}

		static void EndProgram()
		{
			var endTime = DateTime.Now;
			var ts = endTime - startTime;
			var errorPer = (double) errorCount / totalCount;

			if (errorCount > errorCountThreshold && errorPer > errorPerThreshold)
				Console.Write("\nThe number of errors is above the threshold.");

			var log = new string[4];
			log[0] = "Ending program";
			log[1] = String.Format("It took {0} to complete", ts.ToString(@"hh\:mm\:ss\.fff"));
			log[2] = String.Format("Total: {0:n0}", totalCount);
			log[3] = String.Format("Errors: {0} ({1}){2}", errorCount, errorPer.ToString("P"), Environment.NewLine + Environment.NewLine);

			if (consoleOut) {
				Console.Write("\n\n\n\n\n\n\n****************************");
				for (int i = 0; i < log.Length; i++) {
					Console.Write("\n{0}{1}", DateTime.Now.ToString(TIMESTAMP), log[i]);
				}
				Console.Write("\n.... Press any key to close the program ....");
				Console.ReadKey(true);
			}

			Environment.Exit(0); // in case you want to call this method outside of a standard successful program completion, this line will close the app //
		}
		#endregion Start & EndProgram
	}
}