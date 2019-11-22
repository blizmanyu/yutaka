﻿using System;
using System.IO;

namespace Yutaka.IO2
{
	public static class FileUtil
	{
		#region Fields
		public static readonly DateTime UNIX_TIME = new DateTime(1970, 1, 1);
		public static readonly int FIVE_HUNDRED_TWELVE_KB = (int) Math.Pow(2, 19);
		#endregion Fields

		#region Private Methods
		/// <summary>
		/// Fast file copy with big buffers. If &lt;destFileName&gt; exists, it will be overwritten.
		/// </summary>
		/// <param name="sourceFileName">The file to copy.</param>
		/// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
		/// <seealso cref="https://www.codeproject.com/Tips/777322/A-Faster-File-Copy"/>
		private static bool FastCopy(string sourceFileName, string destFileName)
		{
			int read;
			var array_length = FIVE_HUNDRED_TWELVE_KB;
			var dataArray = new byte[array_length];

			try {
				using (var fsread = new FileStream(sourceFileName, FileMode.Open, FileAccess.Read, FileShare.None, array_length)) {
					using (var bwread = new BinaryReader(fsread)) {
						using (var fswrite = new FileStream(destFileName, FileMode.Create, FileAccess.Write, FileShare.None, array_length)) {
							using (var bwwrite = new BinaryWriter(fswrite)) {
								for (; ; ) {
									read = bwread.Read(dataArray, 0, array_length);
									if (0 == read)
										break;
									bwwrite.Write(dataArray, 0, read);
								}
							}
						}
					}
				}

				return true;
			}

			catch (Exception ex) {
				#region Log
				string log;

				if (ex.InnerException == null)
					log = String.Format("{0}{2}Exception thrown in FileUtil.FastCopy(string sourceFileName='{3}', string destFileName='{4}'){2}{1}{2}{2}", ex.Message, ex.ToString(), Environment.NewLine, sourceFileName, destFileName);
				else
					log = String.Format("{0}{2}Exception thrown in INNER EXCEPTION of FileUtil.FastCopy(string sourceFileName='{3}', string destFileName='{4}'){2}{1}{2}{2}", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, sourceFileName, destFileName);

				Console.Write("\n{0}", log);
				#endregion Log

				return false;
			}
		}
		#endregion Private Methods

		#region Public Methods
		/// <summary>
		/// Returns a new filename with " - Copy" appended to it.
		/// </summary>
		/// <param name="filename"></param>
		/// <returns></returns>
		public static string AutoRename(string filename)
		{
			if (String.IsNullOrWhiteSpace(filename))
				throw new Exception(String.Format("<filename> is required.{0}", Environment.NewLine));

			var extension = Path.GetExtension(filename);

			while (File.Exists(filename))
				filename = filename.Replace(extension, String.Format(" - Copy{0}", extension));

			return filename;
		}

		/// <summary>
		/// Copies an existing file to a new file.
		/// </summary>
		/// <param name="sourceFileName">The file to copy.</param>
		/// <param name="destFileName">The name of the destination file. This cannot be a directory.</param>
		/// <param name="overwriteOption">The <see cref="OverwriteOption"/> to use.</param>
		/// <returns></returns>
		public static bool Copy(string sourceFileName, string destFileName, OverwriteOption overwriteOption = OverwriteOption.Skip)
		{
			#region Input Check
			var log = "";

			if (String.IsNullOrWhiteSpace(sourceFileName))
				log = String.Format("{0}<sourceFileName> is required.{1}", log, Environment.NewLine);
			else if (!File.Exists(sourceFileName))
				log = String.Format("{0}<sourceFileName> doesn't exist.{1}", log, Environment.NewLine);
			if (String.IsNullOrWhiteSpace(destFileName))
				log = String.Format("{0}<destFileName> is required.{1}", log, Environment.NewLine);
			if (destFileName.ToUpper().Equals(sourceFileName.ToUpper()))
				log = String.Format("{0}<sourceFileName> and <destFileName> are the same.{1}", log, Environment.NewLine);

			if (!String.IsNullOrWhiteSpace(log)) {
				log = String.Format("{0}Exception thrown in FileUtil.Copy(string sourceFileName, string destFileName, OverwriteOption overwriteOption).{1}", log, Environment.NewLine);
				Console.Write("\n{0}\n", log);
				return false;
			}
			#endregion Input Check

			try {
				var destFileExists = File.Exists(destFileName);

				switch (overwriteOption) {
					#region case OverwriteOption.Overwrite:
					case OverwriteOption.Overwrite:
						return FastCopy(sourceFileName, destFileName);
					#endregion
					#region case OverwriteOption.Skip:
					case OverwriteOption.Skip:
						if (destFileExists)
							return false;

						return FastCopy(sourceFileName, destFileName);
					#endregion
					#region case OverwriteOption.KeepBoth:
					case OverwriteOption.KeepBoth:
						if (destFileExists)
							return FastCopy(sourceFileName, AutoRename(destFileName));

						return FastCopy(sourceFileName, destFileName);
					#endregion
					#region case OverwriteOption.Smart:
					case OverwriteOption.Smart:
						if (destFileExists) {
							var fi1 = new FileInfo(sourceFileName);
							var fi2 = new FileInfo(destFileName);

							if (fi1.Length == fi2.Length)
								return true;

							return FastCopy(sourceFileName, AutoRename(destFileName));
						}

						return FastCopy(sourceFileName, destFileName);
					#endregion
					default:
						return false;
				}
			}

			catch (Exception ex) {
				#region Log
				log = "";

				if (ex.InnerException == null)
					log = String.Format("{0}{2}Exception thrown in FileUtil.Copy(string sourceFileName='{3}', string destFileName='{4}', OverwriteOption overwriteOption='{5}'){2}{1}{2}{2}", ex.Message, ex.ToString(), Environment.NewLine, sourceFileName, destFileName, overwriteOption.ToString());
				else
					log = String.Format("{0}{2}Exception thrown in INNER EXCEPTION of FileUtil.Copy(string sourceFileName='{3}', string destFileName='{4}', OverwriteOption overwriteOption='{5}'){2}{1}{2}{2}", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, sourceFileName, destFileName, overwriteOption.ToString());

				Console.Write("\n{0}", log);
				#endregion Log

				return false;
			}
		}
		#endregion Public Methods
	}
}