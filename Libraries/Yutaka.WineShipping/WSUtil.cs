﻿using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Yutaka.WineShipping
{
	public class WSUtil
	{
		#region Fields
		public const int DEFAULT_TOP = 480;
		public const string PRODUCTION_URL = @"https://services.wineshipping.com/";
		public const string TEST_URL = @"https://wsservices-test.azurewebsites.net/";

		public Uri BaseUrl;
		public string UserKey;
		public string Password;
		public string CustomerNumber;
		#endregion Fields

		#region Constructor
		public WSUtil(string userKey=null, string password=null, string customerNumber=null, string baseUrl=null)
		{
			if (String.IsNullOrWhiteSpace(userKey))
				throw new Exception(String.Format("<userKey> is required. Exception thrown in Constructor WSUtil(string baseUrl, string userKey, string password, string customerNumber).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(password))
				throw new Exception(String.Format("<password> is required. Exception thrown in Constructor WSUtil(string baseUrl, string userKey, string password, string customerNumber).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customerNumber))
				throw new Exception(String.Format("<customerNumber> is required. Exception thrown in Constructor WSUtil(string baseUrl, string userKey, string password, string customerNumber).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(baseUrl))
				baseUrl = TEST_URL;

			BaseUrl = new Uri(baseUrl);
			UserKey = userKey;
			Password = password;
			CustomerNumber = customerNumber;
		}
		#endregion Constructor

		#region Methods
		public void DisplayResponse(Task<string> response, bool pretty = true)
		{
			if (response == null || String.IsNullOrWhiteSpace(response.Result))
				return;

			if (pretty) {
				dynamic parsedJson = JsonConvert.DeserializeObject(response.Result);
				var formatted = JsonConvert.SerializeObject(parsedJson, Formatting.Indented);
				Console.Write("\n{0}", formatted);
			}

			else
				Console.Write("\n{0}", response.Result);
		}

		public async Task<string> GetInventoryStatus(string warehouse=null, string[] itemNumbers=null, bool? includeTotalRecordCount=null, int? skip=null, int? top=null)
		{
			if (includeTotalRecordCount == null)
				includeTotalRecordCount = true;
			if (skip == null)
				skip = 0;
			if (top == null)
				top = DEFAULT_TOP;

			try {
				var endpoint = "api/Inventory/GetStatus";
				var request = new InventoryStatusRequest {
					Authentication = new Authentication { UserKey = UserKey, Password = Password, CustomerNo = CustomerNumber, },
					Warehouse = warehouse,
					ItemNumbers = itemNumbers,
					IncludeTotalRecordCount = includeTotalRecordCount,
					Skip = skip,
					Top = top,
				};

				using (var httpClient = new HttpClient { BaseAddress = BaseUrl }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					using (var content = new StringContent(request.ToJson(), Encoding.Default, "application/json")) {
						using (var response = await httpClient.PostAsync(endpoint, content)) {
							var responseData = await response.Content.ReadAsStringAsync();
							return responseData;
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.GetInventoryStatus(string warehouse='{3}', string[] itemNumbers='{4}', bool? includeTotalRecordCount='{5}', int? skip='{6}', int? top='{7}')", ex.Message, ex.ToString(), Environment.NewLine, warehouse, String.Join(",", itemNumbers), includeTotalRecordCount, skip, top));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.GetInventoryStatus(string warehouse='{3}', string[] itemNumbers='{4}', bool? includeTotalRecordCount='{5}', int? skip='{6}', int? top='{7}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, warehouse, String.Join(",", itemNumbers), includeTotalRecordCount, skip, top));
			}
		}

		public int? ItemUnitToBottlesPerUnit(string itemUnit)
		{
			if (String.IsNullOrWhiteSpace(itemUnit))
				return 1;

			itemUnit = itemUnit.ToUpper();

			switch (itemUnit) {
				#region case 1.5L
				case "1500":
					return 6;
				case "1500-1-W":
					return 1;
				#endregion case 1.5L
				#region case 3L
				case "3000":
				case "3000-1-W":
					return 1;
				#endregion case 3L
				#region case 750mL
				case "750":
					return 12;
				case "750-3":
					return 3;
				case "750-3-W":
					return 3;
				#endregion case 750mL
				#region case 9L
				case "9000":
					return 1;
				#endregion case 9L
				default:
					return 1;
			}
		}

		public int? ItemUnitToBottlesPerCase(string itemUnit)
		{
			if (String.IsNullOrWhiteSpace(itemUnit))
				return 1;

			itemUnit = itemUnit.ToUpper();

			switch (itemUnit) {
				#region case 1.5L
				case "1500":
				case "1500-1-W":
					return 6;
				#endregion case 1.5L
				#region case 3L
				case "3000":
				case "3000-1-W":
					return 3;
				#endregion case 3L
				#region case 750mL
				case "750":
				case "750-3":
				case "750-3-W":
					return 12;
				#endregion case 750mL
				#region case 9L
				case "9000":
					return 1;
				#endregion case 9L
				default:
					return 1;
			}
		}
		#endregion Methods
	}
}