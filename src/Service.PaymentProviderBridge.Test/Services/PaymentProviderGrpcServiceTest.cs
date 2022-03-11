using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Domain.Models;
using Service.PaymentProviderBridge.Test.Settings;

namespace Service.PaymentProviderBridge.Test.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		private readonly ILogger<PaymentProviderGrpcService> _logger;

		public PaymentProviderGrpcService(ILogger<PaymentProviderGrpcService> logger) => _logger = logger;

		public ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			SettingsModel settings = Program.Settings;

			string url = SetTransactionId(settings.ServiceUrl, request.TransactionId)
				.Replace("#ok-url#", SetTransactionId(settings.OkUrl, request.TransactionId), StringComparison.OrdinalIgnoreCase)
				.Replace("#fail-url#", SetTransactionId(settings.FailUrl, request.TransactionId), StringComparison.OrdinalIgnoreCase)
				.Replace("#callback-url#", settings.CallbackUrl, StringComparison.OrdinalIgnoreCase)
				.Replace("#info#", $"{request.Amount} {request.Currency}", StringComparison.OrdinalIgnoreCase);

			_logger.LogDebug("Redirecting user to pay system url: {url}", url);

			return ValueTask.FromResult(new ProviderDepositGrpcResponse
			{
				RedirectUrl = url
			});
		}

		private static string SetTransactionId(string urlTemplate, Guid? id) => urlTemplate.Replace("#transaction-id#", id.ToString(), StringComparison.OrdinalIgnoreCase);
	}
}