using System;
using System.Threading.Tasks;
using Service.PaymentDeposit.Domain.Models;

namespace Service.PaymentProviderBridge.Test.Services
{
	public class PaymentProviderGrpcService : IPaymentProviderGrpcService
	{
		public ValueTask<ProviderDepositGrpcResponse> DepositAsync(ProviderDepositGrpcRequest request)
		{
			throw new NotImplementedException();
		}
	}
}