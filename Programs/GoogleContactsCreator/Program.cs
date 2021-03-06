﻿using System;
using System.Runtime.InteropServices;
using Yutaka.IO;
using Yutaka.Utils;

namespace GoogleContactsCreator
{
	class Program
	{
		const string PROGRAM_NAME = "GoogleContactsCreator";
		private static bool consoleOut = true; // default = false //

		#region Fields
		#region Static Externs
		[DllImport("kernel32.dll")]
		static extern IntPtr GetConsoleWindow();
		[DllImport("user32.dll")]
		static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
		const int SW_HIDE = 0;
		#endregion

		const string TIMESTAMP = @"[HH:mm:ss] ";
		private static DateTime startTime = DateTime.Now;
		private static FileUtil _fileUtil = new FileUtil();
		private static int errorCount = 0;
		private static int totalCount = 0;
		private static int errorCountThreshold = 7;
		private static double errorPerThreshold = 0.07;
		#endregion

		#region Private Helpers
		#endregion

		#region Methods
		static void Process()
		{
			var now = DateTime.Now;
			var path = String.Format(@"C:\TEMP\Contacts {0}.csv", now.ToString("yyyy MMdd HHmm ssff"));

			// Headers //
			_fileUtil.Write("Name,Given Name,Family Name,Group Membership,Phone 1 - Type,Phone 1 - Value", path);

			var givenName = "RoboCall";
			var phonePrefix = "(408) 717-";

			for (int i = 0; i < 10000; i++) {
				if (i % 477 == 0)
					_fileUtil.Write(String.Format("\n{1} {0:d4},{1},{0:d4},* myContacts,,{2}{0:d4}", i, givenName, phonePrefix), path);
				else if (i == 860)
					continue;
				else
					_fileUtil.Write(String.Format(" ::: {1}{0:d4}", i, phonePrefix), path);
			}
		}

		static void Main(string[] args)
		{
			StartProgram();
			Process();
			EndProgram();
		}

		static void StartProgram()
		{
			var log = String.Format("Starting {0} program", PROGRAM_NAME);

			if (consoleOut)
			{
				Console.Clear();
				Console.Write("{0}{1}", DateTime.Now.ToString(TIMESTAMP), log);
			}

			else
			{
				var handle = GetConsoleWindow();
				ShowWindow(handle, SW_HIDE); // hide window //
			}
		}

		static void EndProgram()
		{
			var endTime = DateTime.Now;
			var ts = endTime - startTime;
			var errorPer = (double) errorCount/totalCount;

			var log = new string[4];
			log[0] = "Ending program";
			log[1] = String.Format("It took {0} to complete", ts.ToString(@"hh\:mm\:ss\.fff"));
			log[2] = String.Format("Total: {0:n0}", totalCount);
			log[3] = String.Format("Errors: {0} ({1}){2}", errorCount, errorPer.ToString("P"), Environment.NewLine + Environment.NewLine);

			if (consoleOut)
			{
				Console.Write("\n");
				Console.Write("\n{0}{1}", DateTime.Now.ToString(TIMESTAMP), log[0]);
				Console.Write("\n{0}{1}", DateTime.Now.ToString(TIMESTAMP), log[1]);
				Console.Write("\n{0}{1}", DateTime.Now.ToString(TIMESTAMP), log[2]);
				Console.Write("\n{0}{1}", DateTime.Now.ToString(TIMESTAMP), log[3]);
				Console.Write("\n.... Press any key to close the program ....");
				Console.ReadKey(true);
			}

			Environment.Exit(0); // in case you want to call this method outside of a standard successful program completion, this line will close the app //
		}
		#endregion
	}
}