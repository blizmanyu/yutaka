﻿using System;
using System.Collections.Generic;
using System.IO;

namespace Yutaka.IO
{
	public class YuVideo
	{
		#region Fields
		private const int MEDIA_CREATED_FIELD = 208;
		private const int DATE_RELEASED_FIELD = 209;
		private readonly Guid CLSID_Shell = Guid.Parse("13709620-C279-11CE-A49E-444553540000");
		private char[] charactersToRemove = new char[] { (char) 8206, (char) 8207 };
		public DateTime CreationTime;
		public DateTime DateReleased;
		public DateTime LastAccessTime;
		public DateTime LastWriteTime;
		public DateTime MediaCreated;
		public DateTime MinDateTime;
		public DateTime MinDateTimeThreshold = new DateTime(1970, 1, 1); // based on Unix time //
		public long Size;
		public string DirectoryName;
		public string Extension;
		public string FullName;
		public string Name;
		public string NameWithoutExtension;
		public string NewFilename;
		public string NewFolder;
		public string ParentFolder;
		#endregion Fields

		public YuVideo(string filename)
		{
			var fi = new FileInfo(filename);
			CreationTime = fi.CreationTime;
			LastAccessTime = fi.LastAccessTime;
			LastWriteTime = fi.LastWriteTime;
			Size = fi.Length;
			DirectoryName = fi.DirectoryName;
			Extension = fi.Extension.ToLower();
			FullName = fi.FullName;
			Name = fi.Name;
			NameWithoutExtension = Name.Replace(fi.Extension, "");
			ParentFolder = fi.Directory.Name;
			fi = null;

			SetMediaCreated();
			SetDateReleased();
			SetMinDateTime();
			SetNewFolderAndFilename();
		}

		private void SetMediaCreated()
		{
			try {
				dynamic shell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_Shell));
				var folder = shell.NameSpace(DirectoryName);
				var file = folder.ParseName(Name);
				var label = folder.GetDetailsOf(null, MEDIA_CREATED_FIELD);

				if (label.ToUpper().Equals("MEDIA CREATED")) {
					var value = folder.GetDetailsOf(file, MEDIA_CREATED_FIELD).Trim();

					// Removing the suspect characters
					foreach (char c in charactersToRemove)
						value = value.Replace((c).ToString(), "").Trim();

					// If the value string is empty, return DateTime.MinValue, otherwise return the "Media Created" date
					MediaCreated = String.IsNullOrWhiteSpace(value) ? DateTime.MinValue : DateTime.Parse(value);
				}

				else {
					Console.Write("\n**********");
					Console.Write("\n{0} is NOT the Media Created field", MEDIA_CREATED_FIELD);
					Console.Write("\n**********");
					MediaCreated = new DateTime();
				}
			}

			catch (Exception) {
				MediaCreated = new DateTime();
			}
		}

		private void SetDateReleased()
		{
			try {
				dynamic shell = Activator.CreateInstance(Type.GetTypeFromCLSID(CLSID_Shell));
				var folder = shell.NameSpace(DirectoryName);
				var file = folder.ParseName(Name);
				var label = folder.GetDetailsOf(null, DATE_RELEASED_FIELD);

				if (label.ToUpper().Equals("DATE RELEASED")) {
					var value = folder.GetDetailsOf(file, DATE_RELEASED_FIELD).Trim();

					// Removing the suspect characters
					foreach (char c in charactersToRemove)
						value = value.Replace((c).ToString(), "").Trim();

					// If the value string is empty, return DateTime.MinValue, otherwise return the "Media Created" date
					DateReleased = String.IsNullOrWhiteSpace(value) ? DateTime.MinValue : DateTime.Parse(value);
				}

				else {
					Console.Write("\n**********");
					Console.Write("\n{0} is NOT the Date Relased field", DATE_RELEASED_FIELD);
					Console.Write("\n**********");
					DateReleased = new DateTime();
				}
			}

			catch (Exception) {
				DateReleased = new DateTime();
			}
		}

		private void SetMinDateTime()
		{
			MinDateTime = DateTime.Now;

			if (MediaCreated != null && MinDateTimeThreshold < MediaCreated && MediaCreated < MinDateTime)
				MinDateTime = MediaCreated; // prioritize MediaCreated //

			else if (DateReleased != null && MinDateTimeThreshold < DateReleased && DateReleased < MinDateTime)
				MinDateTime = DateReleased; // prioritize DateReleased //

			else {
				if (CreationTime != null && MinDateTimeThreshold < CreationTime && CreationTime < MinDateTime)
					MinDateTime = CreationTime;

				if (LastWriteTime != null && MinDateTimeThreshold < LastWriteTime && LastWriteTime < MinDateTime)
					MinDateTime = LastWriteTime;
			}
		}

		private void SetNewFolderAndFilename()
		{
			#region Special Cases
			var specialFolders1 = new string[,] {
				// search term, new folder name //
				{ @"_JFUL", @"_Jful\", },
				{ @"JFUL", @"_Jful\", },
			};

			for (int i = 0; i < specialFolders1.Length / 2; i++) {
				if (FullName.ToUpper().Contains(specialFolders1[i, 0])) {
					if (specialFolders1[i, 1].StartsWith(@"yyyy\"))
						NewFolder = specialFolders1[i, 1].Replace("yyyy", MinDateTime.ToString("yyyy"));
					else
						NewFolder = specialFolders1[i, 1];

					NewFilename = Name;
					return; // only match one, then return //
				}
			}
			#endregion Special Cases

			#region Default: Everything else
			int year;
			var ignoreList = new List<string> { "TV", "VIDEOS", "ANIME", "MOVIES", "MUSIC VIDEOS", "TEST", "_TEST", };
			if (ignoreList.Exists(x => ParentFolder.ToUpper().Equals(x)) || (int.TryParse(ParentFolder, out year) && (MinDateTimeThreshold.Year <= year && year <= DateTime.Now.Year)))
				NewFolder = String.Format(@"{0:yyyy}\", MinDateTime);
			else
				NewFolder = String.Format(@"{0:yyyy}\{1}\", MinDateTime, ParentFolder);

			NewFilename = Name;
			#endregion Default: Everything else
		}
	}
}