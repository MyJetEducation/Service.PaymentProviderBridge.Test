using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Service.PaymentDeposit.Domain.Models;

namespace Service.PaymentProviderBridge.Test.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		private readonly ILogger<PaymentProviderGrpcService> _logger;

		public PaymentProviderGrpcService(ILogger<PaymentProviderGrpcService> logger) => _logger = logger;

		public async ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			string urlTemplate = Program.ReloadedSettings(model => model.ServiceUrl).Invoke();

			string url = urlTemplate
				.Replace("#transaction-id#", request.TransactionId.ToString(), StringComparison.OrdinalIgnoreCase)
				.Replace("#ok-url#", "http://somedomain.ru/ok.html", StringComparison.OrdinalIgnoreCase)
				.Replace("#fail-url#", "http://somedomain.ru/fail.html", StringComparison.OrdinalIgnoreCase)
				.Replace("#callback-url#", "http://api.dfnt.work/api/v1/paymentdeposit/callback-test", StringComparison.OrdinalIgnoreCase);

			_logger.LogDebug("Redirecting user to pay system url: {url}", url);

			return await ValueTask.FromResult(new ProviderDepositGrpcResponse
			{
				RedirectUrl = url
			});
		}
	}
}