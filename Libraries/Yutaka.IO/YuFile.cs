﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Yutaka.IO
{
	public class YuFile
	{
		#region Fields
		const int DATE_RELEASED_FIELD = 209;
		const int MEDIA_CREATED_FIELD = 208;
		const int PROPERTY_TAG_EXIF_DATE_TAKEN = 36867; // PropertyTagExifDTOrig //
		private static readonly DateTime UNIX_TIME = new DateTime(1970, 1, 1);
		private static readonly Guid CLSID_Shell = Guid.Parse("13709620-C279-11CE-A49E-444553540000");
		private static readonly char[] charactersToRemove = new char[] { (char) 8206, (char) 8207 };
		public DateTime CreationTime;
		public DateTime DateReleased;
		public DateTime DateTaken;
		public DateTime LastAccessTime;
		public DateTime LastWriteTime;
		public DateTime MediaCreated;
		public DateTime MinDateTime;
		public DateTime MinDateTimeThreshold = UNIX_TIME;
		public DateTime OldThreshold = DateTime.Now.AddYears(-7);
		#region protected static readonly string[][] SpecialFolders = new string[][] {
		protected static readonly string[][] SpecialFolders = new string[][] {
			new string[] { "babystepsanddownwarddogs", @"zz\BareSolesBearSoul\", },
			new string[] { "PreciousO23_Bucket", @"zz\Olga\", },
			new string[] { "BareSolesBearSoul", @"zz\BareSolesBearSoul\", },
			new string[] { "Consumer Reports", @"Documents\Consumer Reports\", },
			new string[] { "ConsumerReports", @"Documents\Consumer Reports\", },
			new string[] { "United Airlines", @"Documents\Itineraries\", },
			new string[] { "Clash of Clans", @"Games\Clash of Clans\", },
			new string[] { "UnitedAirlines", @"Documents\Itineraries\", },
			new string[] { "Clash Royale", @"Games\Clash Royale\", },
			new string[] { "ClashOfClans", @"Games\Clash of Clans\", },
			new string[] { "Confirmation", @"Documents\Receipts\", },
			new string[] { "Registration", @"Documents\Registrations\", },
			new string[] { "Video Player", @"zz\Video Player\", },
			new string[] { "ClashRoyale", @"Games\Clash Royale\", },
			new string[] { "DragonFruit", @"zz\DragonFruit\", },
			new string[] { "Itineraries", @"Documents\Itineraries\", },
			new string[] { "Grooming", @"Grooming\", }, // higher priority //
			new string[] { "Womens Health", @"Documents\Womens Health\", },
			new string[] { "WomensHealth", @"Documents\Womens Health\", },
			new string[] { "Men's Health", @"Documents\Mens Health\", },
			new string[] { "Mens Health", @"Documents\Mens Health\", },
			new string[] { "MensHealth", @"Documents\Mens Health\", },
			new string[] { "Philips Hue", @"Philips Hue\", },
			new string[] { "Castle Age", @"Games\Castle Age\", },
			new string[] { "Green Card", @"Documents\Green Card\", },
			new string[] { "Maximum PC", @"Documents\Maximum PC\", },
			new string[] { "PhilipsHue", @"Philips Hue\", },
			new string[] { "US Airways", @"Documents\Itineraries\", },
			new string[] { "CastleAge", @"Games\Castle Age\", },
			new string[] { "Fantasica", @"Games\Fantasica\", },
			new string[] { "GreenCard", @"Documents\GreenCard\", },
			new string[] { "Instagram", @"zz\Instagram\", },
			new string[] { "Itinerary", @"Documents\Itineraries\", },
			new string[] { "MaximumPC", @"Documents\Maximum PC\", },
			new string[] { "Messenger", @"Apps\Messenger\", },
			new string[] { "Steamgirl", @"zz\Steamgirl\", },
			new string[] { "Thank You", @"Documents\Receipts\", },
			new string[] { "USAirways", @"Documents\Itineraries\", },
			new string[] { "Cash App", @"Apps\Cash App\", },
			new string[] { "Checkout", @"Documents\Receipts\", },
			new string[] { "Facebook", @"Apps\Facebook\", },
			new string[] { "Invoices", @"Documents\Invoices\", },
			new string[] { "Messages", @"Apps\Messages\", },
			new string[] { "Passport", @"Documents\Passport\", },
			new string[] { "Patricia", @"zz\Patricia\", },
			new string[] { "PC Gamer", @"Documents\PC Gamer\", },
			new string[] { "Receipts", @"Documents\Receipts\", },
			new string[] { "Snapchat", @"zz\Snapchat\", },
			new string[] { "ThankYou", @"Documents\Receipts\", },
			new string[] { "Unsplash", @"Unsplash\", },
			new string[] { "WhatsApp", @"Apps\WhatsApp\", },
			new string[] { "Bitmoji", @"Apps\Bitmoji\", },
			new string[] { "CashApp", @"Apps\Cash App\", },
			new string[] { "Invoice", @"Documents\Invoices\", },
			new string[] { "License", @"Documents\License\", },
			new string[] { "Netflix", @"Apps\Netflix\", },
			new string[] { "OkCupid", @"zz\OkCupid\", },
			new string[] { "P Shots", @"zz\P Shots\", },
			new string[] { "Pandora", @"Apps\Pandora\", },
			new string[] { "PCGamer", @"Documents\PC Gamer\", },
			new string[] { "Receipt", @"Documents\Receipts\", },
			new string[] { "Samsung", @"Apps\Samsung\", },
			new string[] { "Spotify", @"Apps\Spotify\", },
			new string[] { "Tattoos", @"Tattoos\", },
			new string[] { "Twitter", @"Apps\Twitter\", },
			new string[] { "Vanessa", @"zz\Vanessa\", },
			new string[] { "Welcome", @"Documents\Receipts\", },
			new string[] { "YouTube", @"Apps\YouTube\", },
			new string[] { "Amazon", @"Documents\Amazon\", },
			new string[] { "Poses", @"Poses\", }, // higher priority //
			new string[] { "Bumble", @"zz\Bumble\", },
			new string[] { "Chrome", @"Apps\Chrome\", },
			new string[] { "London", @"zz\London\", },
			new string[] { "Nanami", @"Nanami\", },
			new string[] { "Shirts", @"Shirts\", },
			new string[] { "Tattoo", @"Tattoos\", },
			new string[] { "Thanks", @"Documents\Receipts\", },
			new string[] { "TikTok", @"Apps\TikTok\", },
			new string[] { "Tinder", @"zz\Tinder\", },
			new string[] { "Alekz", @"zz\Alekz\", },
			new string[] { "Bixby", @"Apps\Bixby\", },
			new string[] { "Chase", @"Documents\Chase\", },
			new string[] { "Delta", @"Documents\Itineraries\", },
			new string[] { "Gmail", @"Apps\Gmail\", },
			new string[] { "Happn", @"zz\Happn\", },
			new string[] { "Hinge", @"zz\Hinge\", },
			new string[] { "Maxim", @"Documents\Maxim\", },
			new string[] { "Sarah", @"zz\Sarah\", },
			new string[] { "Scans", @"Documents\Scans\", },
			new string[] { "Shirt", @"Shirts\", },
			new string[] { "Sleep", @"Apps\Sleep\", },
			new string[] { "Alex", @"zz\Alekz\", },
			new string[] { "Arpy", @"zz\Arpy\", },
			new string[] { "ETNT", @"Documents\ETNT\", },
			new string[] { "FICO", @"Documents\FICO\", },
			new string[] { "Ikea", @"Documents\Ikea\", },
			new string[] { "Leah", @"zz\Leah\", },
			new string[] { "Line", @"Apps\Line\", },
			new string[] { "Maps", @"Apps\Maps\", },
			new string[] { "Marc", @"zz\Olga\", },
			new string[] { "Mely", @"zz\Mely\", },
			new string[] { "Olga", @"zz\Olga\", },
			new string[] { "Pose", @"Poses\", },
			new string[] { "Scan", @"Documents\Scans\", },
			new string[] { "Turo", @"Apps\Turo\", },
			new string[] { "Uber", @"Apps\Uber\", },
			new string[] { "Car", @"Documents\Car\", },
			new string[] { "Jas", @"zz\Jas\", },
			new string[] { "zMe", @"zMe\", },
			new string[] { "GQ", @"Documents\GQ\", },
			new string[] { "Me", @"zMe\", },
			new string[] { "MH", @"Documents\Mens Health\", },
			// Keep these last //
			new string[] { "zz", @"zz\", },
			new string[] { "Screenshots", @"Documents\Screenshots\", },
			new string[] { "Screenshot", @"Documents\Screenshots\", },
			new string[] { "Documents", @"Documents\", },
			new string[] { "Games", @"Games\", },
			new string[] { "Apps", @"Apps\", },
		};
		#endregion protected static readonly string[][] SpecialFolders
		public long Size;
		public string DirectoryName;
		public string Extension;
		public string FullName;
		public string Name;
		public string NameWithoutExtension;
		public string NewFilename;
		public string NewFolder;
		public string ParentFolder;
		public string Root;
		#endregion Fields

		public YuFile(string filename = null)
		{
			if (String.IsNullOrWhiteSpace(filename))
				throw new Exception(String.Format("<filename> is required.{0}Exception thrown in YuFile Constructor.{0}", Environment.NewLine));

			var fi = new FileInfo(filename);
			fi.IsReadOnly = false;
			#region CreationTime = fi.CreationTime;
			try {
				CreationTime = fi.CreationTime;
			}
			catch (Exception) {
				CreationTime = new DateTime();
			}
			#endregion CreationTime = fi.CreationTime;
			LastAccessTime = fi.LastAccessTime;
			#region LastWriteTime = fi.LastWriteTime;
			try {
				LastWriteTime = fi.LastWriteTime;
			}
			catch (Exception) {
				LastWriteTime = new DateTime();
			}
			#endregion LastWriteTime = fi.LastWriteTime;
			MinDateTimeThreshold = UNIX_TIME;
			Size = fi.Length;
			DirectoryName = fi.DirectoryName;
			Extension = fi.Extension;
			FullName = fi.FullName.Replace(Extension, Extension.ToLower());
			Name = fi.Name.Replace(Extension, Extension.ToLower());
			NameWithoutExtension = fi.Name.Replace(Extension, "");
			NewFilename = Name;
			NewFolder = "";
			ParentFolder = fi.Directory.Name;
			Root = Path.GetPathRoot(filename);
			fi = null;

			SetDateReleased();
			SetDateTaken();
			SetMediaCreated();
			SetMinDateTime();
			SetNewFolderAndFilename();
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

		// Retrieves the datetime WITHOUT loading the whole image //
		private void SetDateTaken()
		{
			var r = new Regex(":");

			try {
				using (var fs = new FileStream(FullName, FileMode.Open, FileAccess.Read)) {
					using (var img = Image.FromStream(fs, false, false)) {
						var propItem = img.GetPropertyItem(PROPERTY_TAG_EXIF_DATE_TAKEN);
						var dateTaken = r.Replace(Encoding.UTF8.GetString(propItem.Value), "-", 2);
						DateTaken = DateTime.Parse(dateTaken);
					}
				}
			}

			catch (Exception) {
				DateTaken = new DateTime();
			}
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

		private void SetMinDateTime()
		{
			MinDateTime = DateTime.Now;

			if (DateTaken != null && MinDateTimeThreshold < DateTaken && DateTaken < MinDateTime)
				MinDateTime = DateTaken; // prioritize DateTaken //

			else if (MediaCreated != null && MinDateTimeThreshold < MediaCreated && MediaCreated < MinDateTime)
				MinDateTime = MediaCreated; // prioritize MediaCreated //

			else {
				if (CreationTime != null && MinDateTimeThreshold < CreationTime && CreationTime < MinDateTime)
					MinDateTime = CreationTime;

				if (LastWriteTime != null && MinDateTimeThreshold < LastWriteTime && LastWriteTime < MinDateTime)
					MinDateTime = LastWriteTime;

				if (DateReleased != null && MinDateTimeThreshold < DateReleased && DateReleased < MinDateTime)
					MinDateTime = DateReleased;
			}
		}

		private void SetNewFolderAndFilename()
		{
			int result;
			var fullnameUpper = FullName.ToUpper();
			var parentFolderUpper = ParentFolder.ToUpper();
			var bypassList = new List<string> { "_PROCESS THESE", "_TEST", "_UNPROCESSED", "100ANDRO", "101_PANA", "102_PANA", "103_PANA", "APPS", "CAMERA", "CAMERA ROLL", "DOCUMENTS", "DOWNLOAD", "DOWNLOADS", "GAMES", "IMAGES", "OLD", "PICTURES", "SCREENSHOT", "SCREENSHOTS", "TEST", "XPERIA TL", };

			#region Special Folders
			for (int i = 0; i < SpecialFolders.Length; i++) {
				if (Regex.IsMatch(FullName, String.Format(@"[^a-zA-Z]{0}[^a-zA-Z]", SpecialFolders[i][0]), RegexOptions.IgnoreCase)) {
					if (SpecialFolders[i][1].ToUpper().Contains(parentFolderUpper))
						NewFolder = SpecialFolders[i][1];
					else if (bypassList.Exists(x => x.Equals(parentFolderUpper) || int.TryParse(ParentFolder, out result)))
						NewFolder = SpecialFolders[i][1];
					else
						NewFolder = String.Format(@"{0}{1}\", SpecialFolders[i][1], ParentFolder);

					if (MinDateTime < OldThreshold) {
						if (NewFolder.StartsWith(@"Apps\"))
							NewFolder = NewFolder.Replace(@"Apps\", @"Apps\Old\");
						else if (NewFolder.StartsWith(@"zMe\"))
							NewFolder = NewFolder.Replace(@"zMe\", @"zMe\Old\");
						else if (NewFolder.StartsWith(@"Games\"))
							NewFolder = NewFolder.Replace(@"Games\", @"Games\Old\");
						else if (NewFolder.StartsWith(@"Documents\"))
							NewFolder = NewFolder.Replace(@"Documents\", @"Documents\Old\");
					}

					return; // only match one, then return //
				}
			}
			#endregion Special Folders

			#region Default: Everything else
			if (MinDateTime < OldThreshold)
				NewFolder = @"Old\";

			if (bypassList.Exists(x => x.Equals(parentFolderUpper)) || int.TryParse(ParentFolder, out result))
				NewFolder = String.Format(@"{0}{1}\", NewFolder, MinDateTime.Year);
			else
				NewFolder = String.Format(@"{0}{1}\{2}\", NewFolder, MinDateTime.Year, ParentFolder);
			#endregion Default: Everything else
		}
	}
}