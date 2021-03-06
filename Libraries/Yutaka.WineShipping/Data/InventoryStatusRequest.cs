﻿using System;

namespace Yutaka.WineShipping
{
	public class InventoryStatusRequest
	{
		public Authentication Authentication { get; set; }
		public string Warehouse { get; set; }
		public string[] ItemNumbers { get; set; }
		public bool? IncludeTotalRecordCount { get; set; }
		public int? Skip { get; set; }
		public int? Top { get; set; }

		public string ToJson()
		{
			var json = String.Format("{{ {0}", Authentication.ToJson());

			if (!String.IsNullOrWhiteSpace(Warehouse))
				json = String.Format("{0}, \"Warehouse\": \"{1}\"", json, Warehouse);

			if (ItemNumbers != null && ItemNumbers.Length > 0)
				json = String.Format("{0}, \"ItemNumbers\": [ \"{1}\" ]", json, String.Join(",", ItemNumbers));

			if (IncludeTotalRecordCount != null)
				json = String.Format("{0}, \"IncludeTotalRecordCount\": {1}", json, IncludeTotalRecordCount.ToString().ToLower());

			if (Skip != null)
				json = String.Format("{0}, \"Skip\": {1}", json, Skip);

			if (Top != null)
				json = String.Format("{0}, \"Top\": {1}", json, Top);

			return String.Format("{0} }}", json);
		}
	}
}