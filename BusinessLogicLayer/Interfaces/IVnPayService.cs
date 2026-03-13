using BusinessLogicLayer.DTOs;
using Microsoft.AspNetCore.Http;

namespace BusinessLogicLayer.Interfaces;

public interface IVnPayService
{
    /// <summary>Tạo URL redirect sang VNPay để thanh toán</summary>
    string CreatePaymentUrl(long orderId, decimal amount, string orderInfo, string ipAddress);

    /// <summary>Xác thực và parse kết quả callback từ VNPay</summary>
    VnPayCallbackResult ProcessCallback(IQueryCollection queryParams);
}
