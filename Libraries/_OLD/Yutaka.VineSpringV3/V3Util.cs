﻿using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Yutaka.VineSpringV3.Data20191126;

namespace Yutaka.VineSpring
{
	public class V3Util
	{
		#region Fields
		public const string MOCK_SERVER_URL = @"https://private-anon-ba1d162474-vinespring.apiary-mock.com/";
		public const string DEBUGGIN_PROXY_URL  = @"https://private-anon-ba1d162474-vinespring.apiary-proxy.com/";
		public const string PRODUCTION_URL      = @"https://api.vinespring.com/";
		public const string TIME_FORMAT = @"yyyy-MM-ddT00:00:00.000Z";
		private readonly DateTime DOB_THRESHOLD;
		private readonly DateTime MIN_DATE = new DateTime(2000,1,1);
		private readonly DateTime MAX_DATE = DateTime.Now.AddYears(1);

		public Uri BaseAddress;
		public string ApiKey;
		#endregion Fields

		#region Constructor
		public V3Util(string apiKey = null, string baseAddress = null)
		{
			if (String.IsNullOrWhiteSpace(apiKey))
				throw new Exception(String.Format("<apiKey> is required. Exception thrown in Constructor V3Util(string apiKey = null, Uri baseAddress = null).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(baseAddress))
				baseAddress = MOCK_SERVER_URL;

			ApiKey = apiKey;
			BaseAddress = new Uri(baseAddress);
			DOB_THRESHOLD = DateTime.Now.AddYears(-100);
		}
		#endregion Constructor

		#region Methods
		public void DisplayResponse(Task<string> response, bool pretty=true)
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

		#region Customers
		public async Task<string> CreateCustomer(Customer customer)
		{
			if (customer == null)
				throw new Exception(String.Format("<customer> is required. Exception thrown in V3Util.CreateCustomer(Customer customer).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customer.Email))
				throw new Exception(String.Format("<customer.Email> is required. Exception thrown in V3Util.CreateCustomer(Customer customer).{0}", Environment.NewLine));

			try {
				var str = String.Format("{{ \"email\": \"{0}\"", customer.Email);

				if (!String.IsNullOrWhiteSpace(customer.FullName))
					str = String.Format("{0}, \"fullName\": \"{1}\"", str, customer.FullName);
				if (!String.IsNullOrWhiteSpace(customer.AltEmail))
					str = String.Format("{0}, \"altEmail\": \"{1}\"", str, customer.AltEmail);
				if (!String.IsNullOrWhiteSpace(customer.Company))
					str = String.Format("{0}, \"company\": \"{1}\"", str, customer.Company);
				if (customer.Dob != null && customer.Dob > DOB_THRESHOLD)
					str = String.Format("{0}, \"dob\": \"{1}\"", str, customer.Dob.Value.ToString(TIME_FORMAT));
				if (customer.IsTaxExempt != null)
					str = String.Format("{0}, \"isTaxExempt\": {1}", str, customer.IsTaxExempt.ToString().ToLower());
				if (!String.IsNullOrWhiteSpace(customer.Phone))
					str = String.Format("{0}, \"phone\": \"{1}\"", str, customer.Phone);
				if (!String.IsNullOrWhiteSpace(customer.Source))
					str = String.Format("{0}, \"source\": \"{1}\"", str, customer.Source);

				str = String.Format("{0} }}", str);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(str, System.Text.Encoding.Default, "application/json")) {
						using (var response = await httpClient.PostAsync("customers", content)) {
							var responseData = await response.Content.ReadAsStringAsync();
							return responseData;
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Email: {3}{2}Exception thrown in V3Util.CreateCustomer(Customer customer)", ex.Message, ex.ToString(), Environment.NewLine, customer.Email));
				else
					throw new Exception(String.Format("{0}{2}Email: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.CreateCustomer(Customer customer)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customer.Email));
			}
		}

		public async Task<string> DeleteCustomer(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.DeleteCustomer(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.DeleteAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.DeleteCustomer(string customerId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.DeleteCustomer(string customerId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> GetCustomer(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.GetCustomer(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.GetCustomer(string customerId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.GetCustomer(string customerId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> ResetPassword(string email)
		{
			if (String.IsNullOrWhiteSpace(email))
				throw new Exception(String.Format("<email> is required. Exception thrown in V3Util.ResetPassword(string email).{0}", Environment.NewLine));

			try {
				var str = String.Format("{{ \"email\": \"{0}\" }}", email);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(str, System.Text.Encoding.Default, "application/json")) {
						using (var response = await httpClient.PostAsync("customers/passwordReset", content)) {
							var responseData = await response.Content.ReadAsStringAsync();
							return responseData;
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Email: {3}{2}Exception thrown in V3Util.ResetPassword(string email)", ex.Message, ex.ToString(), Environment.NewLine, email));
				else
					throw new Exception(String.Format("{0}{2}Email: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.ResetPassword(string email)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, email));
			}
		}

		public async Task<string> UpdateCustomer(Customer customer)
		{
			if (customer == null)
				throw new Exception(String.Format("<customer> is required. Exception thrown in V3Util.UpdateCustomer(Customer customer).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customer.Id))
				throw new Exception(String.Format("<customer.Id> is required. Exception thrown in V3Util.UpdateCustomer(Customer customer).{0}", Environment.NewLine));

			try {
				var str = String.Format("{{ \"fullName\": \"{0}\"", customer.FullName);

				if (!String.IsNullOrWhiteSpace(customer.AltEmail))
					str = String.Format("{0}, \"altEmail\": \"{1}\"", str, customer.AltEmail);
				if (!String.IsNullOrWhiteSpace(customer.Company))
					str = String.Format("{0}, \"company\": \"{1}\"", str, customer.Company);
				if (customer.Dob != null && customer.Dob > DOB_THRESHOLD)
					str = String.Format("{0}, \"dob\": \"{1}\"", str, customer.Dob.Value.ToString(TIME_FORMAT));
				if (customer.IsTaxExempt != null)
					str = String.Format("{0}, \"isTaxExempt\": {1}", str, customer.IsTaxExempt.ToString().ToLower());
				if (!String.IsNullOrWhiteSpace(customer.Phone))
					str = String.Format("{0}, \"phone\": \"{1}\"", str, customer.Phone);
				if (!String.IsNullOrWhiteSpace(customer.Source))
					str = String.Format("{0}, \"source\": \"{1}\"", str, customer.Source);
				if (!String.IsNullOrWhiteSpace(customer.UpdatedBy))
					str = String.Format("{0}, \"updatedBy\": \"{1}\"", str, customer.UpdatedBy);

				str = String.Format("{0} }}", str);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(str, System.Text.Encoding.Default, "application/json")) {
						var requestUri = new Uri(BaseAddress, String.Format("customers/{0}", customer.Id));
						using (var request = new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = requestUri, Content = content }) {
							using (var response = await httpClient.SendAsync(request)) {
								var responseData = await response.Content.ReadAsStringAsync();
								return responseData;
							}
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Id: {3}{2}Exception thrown in V3Util.UpdateCustomer(Customer customer)", ex.Message, ex.ToString(), Environment.NewLine, customer.Id));
				else
					throw new Exception(String.Format("{0}{2}Id: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.UpdateCustomer(Customer customer)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customer.Id));
			}
		}

		#region Address
		public async Task<string> CreateAddress(Address address, string customerId)
		{
			if (address == null)
				throw new Exception(String.Format("<address> is required. Exception thrown in V3Util.CreateAddress(Address address, string customerId).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.CreateAddress(Address address, string customerId).{0}", Environment.NewLine));

			try {
				var str = "{  \"address\": { ";

				if (!String.IsNullOrWhiteSpace(address.City))
					str = String.Format("{0} \"city\": \"{1}\"", str, address.City);
				if (address.IsInternational != null)
					str = String.Format("{0}, \"isInternational\": {1}", str, address.IsInternational.ToString().ToLower());
				if (!String.IsNullOrWhiteSpace(address.Line1))
					str = String.Format("{0}, \"line1\": \"{1}\"", str, address.Line1);
				if (!String.IsNullOrWhiteSpace(address.Line2))
					str = String.Format("{0}, \"line2\": \"{1}\"", str, address.Line2);
				if (!String.IsNullOrWhiteSpace(address.Country))
					str = String.Format("{0}, \"country\": \"{1}\"", str, address.Country);
				if (!String.IsNullOrWhiteSpace(address.PostalCode))
					str = String.Format("{0}, \"postalCode\": \"{1}\"", str, address.PostalCode);
				if (!String.IsNullOrWhiteSpace(address.Name))
					str = String.Format("{0}, \"name\": \"{1}\"", str, address.Name);
				if (!String.IsNullOrWhiteSpace(address.State))
					str = String.Format("{0}, \"state\": \"{1}\"", str, address.State);

				str = String.Format("{0} }}}}", str);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(str, System.Text.Encoding.Default, "application/json")) {
						using (var response = await httpClient.PostAsync(String.Format("customers/{0}/addresses", customerId), content)) {
							var responseData = await response.Content.ReadAsStringAsync();
							return responseData;
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in V3Util.CreateAddress(Address address, string customerId)", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.CreateAddress(Address address, string customerId)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> DeleteAddress(string addressId, string customerId)
		{
			if (String.IsNullOrWhiteSpace(addressId))
				throw new Exception(String.Format("<addressId> is required. Exception thrown in V3Util.DeleteAddress(string addressId, string customerId).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.DeleteAddress(string addressId, string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}/addresses/{1}", customerId, addressId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.DeleteAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.DeleteAddress(string addressId='{3}', string customerId='{4}')", ex.Message, ex.ToString(), Environment.NewLine, addressId, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.DeleteAddress(string addressId='{3}', string customerId='{4}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, addressId, customerId));
			}
		}

		public async Task<string> ListAllAddresses(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.ListAllAddresses(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}/addresses", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in V3Util.ListAllAddresses(string customerId)", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllAddresses(string customerId)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> UpdateAddress(Address address, string customerId)
		{
			if (address == null)
				throw new Exception(String.Format("<address> is required. Exception thrown in V3Util.UpdateAddress(Address address, string customerId).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(address.Id))
				throw new Exception(String.Format("<address.Id> is required. Exception thrown in V3Util.UpdateAddress(Address address, string customerId).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.UpdateAddress(Address address, string customerId).{0}", Environment.NewLine));

			try {
				var str = "{  \"address\": { ";

				str = String.Format("{0} \"city\": \"{1}\"", str, address.City ?? "");
				if (address.IsInternational != null)
					str = String.Format("{0}, \"isInternational\": {1}", str, address.IsInternational.ToString().ToLower());
				str = String.Format("{0}, \"line1\": \"{1}\"", str, address.Line1 ?? "");
				str = String.Format("{0}, \"line2\": \"{1}\"", str, address.Line2 ?? "");
				str = String.Format("{0}, \"country\": \"{1}\"", str, address.Country ?? "");
				str = String.Format("{0}, \"postalCode\": \"{1}\"", str, address.PostalCode ?? "");
				str = String.Format("{0}, \"name\": \"{1}\"", str, address.Name ?? "");
				str = String.Format("{0}, \"state\": \"{1}\"", str, address.State ?? "");
				str = String.Format("{0}, \"country\": \"{1}\"", str, address.Country ?? "");
				str = String.Format("{0} }}}}", str);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(str, System.Text.Encoding.Default, "application/json")) {
						var requestUri = new Uri(BaseAddress, String.Format("customers/{0}/addresses/{1}", customerId, address.Id));
						using (var request = new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = requestUri, Content = content }) {
							using (var response = await httpClient.SendAsync(request)) {
								var responseData = await response.Content.ReadAsStringAsync();
								return responseData;
							}
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in V3Util.UpdateAddress(Address address, string customerId)", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}CustomerId: {3}{2}Exception thrown in INNER EXCEPTION of V3Util.UpdateAddress(Address address, string customerId)", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}
		#endregion Address

		public async Task<string> ListAllAllocations(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.ListAllAllocations(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}/allocations", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllAllocations(string customerId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllAllocations(string customerId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> ListAllNotes(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.ListAllNotes(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}/notes", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent("", System.Text.Encoding.Default, "application/json")) {
						using (var response = await httpClient.PostAsync(str, content)) {
							var responseData = await response.Content.ReadAsStringAsync();
							return responseData;
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllNotes(string customerId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllNotes(string customerId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}

		public async Task<string> ListAllRecentOrders(string customerId)
		{
			if (String.IsNullOrWhiteSpace(customerId))
				throw new Exception(String.Format("<customerId> is required. Exception thrown in V3Util.ListAllRecentOrders(string customerId).{0}", Environment.NewLine));

			try {
				var str = String.Format("customers/{0}/orders", customerId);
				Console.Write("\n{0}", str);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(str)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllRecentOrders(string customerId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, customerId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllRecentOrders(string customerId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, customerId));
			}
		}
		#endregion Customers

		#region Orders
		public async Task<string> ListAllOrdersByIds(string commaSeparatedIds, string paginationKey=null)
		{
			if (String.IsNullOrWhiteSpace(commaSeparatedIds))
				throw new Exception(String.Format("<commaSeparatedIds> is required. Exception thrown in V3Util.ListAllOrdersByIds(string commaSeparatedIds, string paginationKey).{0}", Environment.NewLine));

			try {
				var endpoint = String.Format("orders?ids={0}", commaSeparatedIds.Replace(",", "%2C"));
				if (!String.IsNullOrWhiteSpace(paginationKey))
					endpoint = String.Format("{0}&paginationKey={1}", endpoint, paginationKey);
				Console.Write("\n{0}", endpoint);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllOrdersByIds(string commaSeparatedIds='{3}', string paginationKey='{4}')", ex.Message, ex.ToString(), Environment.NewLine, commaSeparatedIds, paginationKey));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllOrdersByIds(string commaSeparatedIds='{3}', string paginationKey='{4}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, commaSeparatedIds, paginationKey));
			}
		}

		public async Task<string> ListAllOrdersByDate(DateTime startDate, DateTime endDate, string paginationKey = null)
		{
			#region Input Validation
			var now = DateTime.Now;
			var minDate = new DateTime(2000,1,1);
			var maxDate = now.AddMonths(3);

			if (startDate < minDate)
				startDate = minDate;
			if (startDate > now)
				startDate = now;
			if (endDate < minDate)
				endDate = minDate;
			if (endDate > maxDate)
				endDate = maxDate;
			#endregion Input Validation

			try {
				var start = startDate.ToString(TIME_FORMAT).Replace(":", "%3A");
				var end = endDate.ToString(TIME_FORMAT).Replace(":", "%3A");
				var endpoint = String.Format("orders?startDate={0}&endDate={1}", start, end);
				if (!String.IsNullOrWhiteSpace(paginationKey))
					endpoint = String.Format("{0}&paginationKey={1}", endpoint, paginationKey);
				Console.Write("\n{0}", endpoint);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllOrdersByDate(DateTime startDate='{3}', DateTime endDate='{4}', string paginationKey='{5}')", ex.Message, ex.ToString(), Environment.NewLine, startDate, endDate, paginationKey));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllOrdersByDate(DateTime startDate='{3}', DateTime endDate='{4}', string paginationKey='{5}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, startDate, endDate, paginationKey));
			}
		}

		public async Task<string> GetOrder(string orderId)
		{
			if (String.IsNullOrWhiteSpace(orderId))
				throw new Exception(String.Format("<orderId> is required. Exception thrown in V3Util.GetOrder(string orderId).{0}", Environment.NewLine));

			try {
				var endpoint = String.Format("orders/{0}", orderId);
				Console.Write("\n{0}", endpoint);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.GetOrder(string orderId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, orderId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.GetOrder(string orderId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, orderId));
			}
		}

		public async Task<string> UpdateOrder(Order order)
		{
			if (order == null)
				throw new Exception(String.Format("<order> is required. Exception thrown in V3Util.UpdateOrder(Order order).{0}", Environment.NewLine));
			if (String.IsNullOrWhiteSpace(order.Id))
				throw new Exception(String.Format("<order.id> is required. Exception thrown in V3Util.UpdateOrder(Order order).{0}", Environment.NewLine));

			try {
				var endpoint = String.Format("orders/{0}", order.Id);
				Console.Write("\n{0}", endpoint);

				var strContent = String.Format("{{ \"fulfillmentHouse\": \"{0}\"", order.FulfillmentHouse ?? "");

				if (order.ShipDate != null && MIN_DATE < order.ShipDate && order.ShipDate < MAX_DATE)
					strContent = String.Format("{0}, \"shipDate\": \"{1}\"", strContent, order.ShipDate.Value.ToString(TIME_FORMAT));
				if (!String.IsNullOrWhiteSpace(order.Status))
					strContent = String.Format("{0}, \"status\": \"{1}\"", strContent, order.Status);
				if (order.Tags != null && order.Tags.Length > 0)
					strContent = String.Format("{0}, \"tags\": [ \"{1}\" ]", strContent, String.Join(",", order.Tags));

				strContent = String.Format("{0} }}", strContent);
				Console.Write("\n{0}", strContent);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var content = new StringContent(strContent, System.Text.Encoding.Default, "application/json")) {
						var requestUri = new Uri(BaseAddress, endpoint);
						using (var request = new HttpRequestMessage { Method = new HttpMethod("PATCH"), RequestUri = requestUri, Content = content }) {
							using (var response = await httpClient.SendAsync(request)) {
								var responseData = await response.Content.ReadAsStringAsync();
								return responseData;
							}
						}
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.UpdateOrder(Order order='{3}')", ex.Message, ex.ToString(), Environment.NewLine, order.Id));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.UpdateOrder(Order order='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, order.Id));
			}
		}

		public async Task<string> ListAllTransactionsByOrderId(string orderId)
		{
			if (String.IsNullOrWhiteSpace(orderId))
				throw new Exception(String.Format("<orderId> is required. Exception thrown in V3Util.ListAllTransactionsByOrderId(string orderId).{0}", Environment.NewLine));

			try {
				var endpoint = String.Format("orders/{0}/transactions", orderId);
				Console.Write("\n{0}", endpoint);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllTransactionsByOrderId(string orderId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, orderId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllTransactionsByOrderId(string orderId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, orderId));
			}
		}

		public async Task<string> ListAllTransactionsByDate(DateTime startDate, DateTime endDate, string paginationKey = null)
		{
			#region Input Validation
			var now = DateTime.Now;
			var minDate = new DateTime(2000,1,1);
			var maxDate = now.AddMonths(3);

			if (startDate < minDate)
				startDate = minDate;
			if (startDate > now)
				startDate = now;
			if (endDate < minDate)
				endDate = minDate;
			if (endDate > maxDate)
				endDate = maxDate;
			#endregion Input Validation

			try {
				var start = startDate.ToString(TIME_FORMAT).Replace(":", "%3A");
				var end = endDate.ToString(TIME_FORMAT).Replace(":", "%3A");
				var endpoint = String.Format("transactions?startDate={0}&endDate={1}", start, end);
				if (!String.IsNullOrWhiteSpace(paginationKey))
					endpoint = String.Format("{0}&paginationKey={1}", endpoint, paginationKey);
				Console.Write("\n{0}", endpoint);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllTransactionsByDate(DateTime startDate='{3}', DateTime endDate='{4}', string paginationKey='{5}')", ex.Message, ex.ToString(), Environment.NewLine, startDate, endDate, paginationKey));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllTransactionsByDate(DateTime startDate='{3}', DateTime endDate='{4}', string paginationKey='{5}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, startDate, endDate, paginationKey));
			}
		}
		#endregion Orders

		#region Products
		public async Task<string> GetProduct(string productId)
		{
			try {
				var endpoint = String.Format("products/{0}", productId);

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.GetProduct(string productId='{3}')", ex.Message, ex.ToString(), Environment.NewLine, productId));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.GetProduct(string productId='{3}')", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine, productId));
			}
		}

		public async Task<string> ListAllProducts()
		{
			try {
				var endpoint = "products";

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllProducts()", ex.Message, ex.ToString(), Environment.NewLine));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllProducts()", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine));
			}
		}

		public async Task<string> ListAllTags()
		{
			try {
				var endpoint = "products/tags";

				using (var httpClient = new HttpClient { BaseAddress = BaseAddress }) {
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
					httpClient.DefaultRequestHeaders.TryAddWithoutValidation("x-api-key", ApiKey);
					using (var response = await httpClient.GetAsync(endpoint)) {
						var responseData = await response.Content.ReadAsStringAsync();
						return responseData;
					}
				}
			}

			catch (Exception ex) {
				if (ex.InnerException == null)
					throw new Exception(String.Format("{0}{2}Exception thrown in V3Util.ListAllTags()", ex.Message, ex.ToString(), Environment.NewLine));
				else
					throw new Exception(String.Format("{0}{2}Exception thrown in INNER EXCEPTION of V3Util.ListAllTags()", ex.InnerException.Message, ex.InnerException.ToString(), Environment.NewLine));
			}
		}
		#endregion Products
		#endregion Methods
	}
}