using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentDepositRepository.Domain.Models;
using Service.PaymentProviderBridge.Test.Models;
using Service.PaymentProviderBridge.Test.Settings;

namespace Service.PaymentProviderBridge.Test.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		private readonly ILogger<PaymentProviderGrpcService> _logger;
		private static readonly HttpClient Client;

		static PaymentProviderGrpcService() => Client = new HttpClient();

		public PaymentProviderGrpcService(ILogger<PaymentProviderGrpcService> logger) => _logger = logger;

		public ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			SettingsModel settings = Program.Settings;

			string externalUrl = SetTransactionId(settings.ServiceUrl, request.TransactionId)
				.Replace("#ok-url#", SetTransactionId(settings.OkUrl, request.TransactionId), StringComparison.OrdinalIgnoreCase)
				.Replace("#fail-url#", SetTransactionId(settings.FailUrl, request.TransactionId), StringComparison.OrdinalIgnoreCase)
				.Replace("#callback-url#", settings.CallbackUrl, StringComparison.OrdinalIgnoreCase)
				.Replace("#info#", $"{request.Amount} {request.Currency}", StringComparison.OrdinalIgnoreCase);

			DepositRegisterResponse registerResponse;
			using (HttpResponseMessage response = Client.GetAsync(externalUrl).Result)
				using (HttpContent content = response.Content)
				registerResponse = JsonConvert.DeserializeObject<DepositRegisterResponse>(content.ReadAsStringAsync().Result);

			if (registerResponse == null)
			{
				_logger.LogError("External id not recieved for url {url} with request {@request}!", externalUrl, request);

				return ValueTask.FromResult(new ProviderDepositGrpcResponse
				{
					State = TransactionState.Error
				});
			}

			_logger.LogDebug("Accepted response {@registerResponse} by url {url} for request {@request}!", registerResponse, externalUrl, request);

			return ValueTask.FromResult(new ProviderDepositGrpcResponse
			{
				State = GetState(registerResponse.State),
				ExternalId = registerResponse.ExternalId,
				RedirectUrl = registerResponse.RedirectUrl
			});
		}

		private static TransactionState GetState(string state) =>
			state switch {
				"accept" => TransactionState.Accepted,
				"reject" => TransactionState.Rejected,
				"approve" => TransactionState.Approved, 
				_ => TransactionState.Error};

		private static string SetTransactionId(string urlTemplate, Guid? id) => urlTemplate.Replace("#transaction-id#", id.ToString(), StringComparison.OrdinalIgnoreCase);
	}
}