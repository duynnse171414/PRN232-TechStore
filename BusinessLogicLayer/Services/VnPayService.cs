using System.Net;
using System.Security.Cryptography;
using System.Text;
using BusinessLogicLayer.DTOs;
using BusinessLogicLayer.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.Services;

public class VnPayService : IVnPayService
{
    private readonly string _tmnCode;
    private readonly string _hashSecret;
    private readonly string _paymentUrl;
    private readonly string _returnUrl;
    private readonly string _version;

    public VnPayService(IConfiguration config)
    {
        _tmnCode = config["VnPay:TmnCode"]!;
        _hashSecret = config["VnPay:HashSecret"]!;
        _paymentUrl = config["VnPay:PaymentUrl"]!;
        _returnUrl = config["VnPay:ReturnUrl"]!;
        _version = config["VnPay:Version"] ?? "2.1.0";
    }

    public string CreatePaymentUrl(long orderId, decimal amount, string orderInfo, string ipAddress)
    {
        var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow,
            TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"));

        var vnpParams = new SortedDictionary<string, string>
        {
            { "vnp_Version", _version },
            { "vnp_Command", "pay" },
            { "vnp_TmnCode", _tmnCode },
            { "vnp_Amount", ((long)(amount * 100)).ToString() },
            { "vnp_CreateDate", now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", "VND" },
            { "vnp_IpAddr", ipAddress },
            { "vnp_Locale", "vn" },
            { "vnp_OrderInfo", orderInfo },
            { "vnp_OrderType", "other" },
            { "vnp_ReturnUrl", _returnUrl },
            { "vnp_TxnRef", orderId.ToString() },
            { "vnp_ExpireDate", now.AddMinutes(15).ToString("yyyyMMddHHmmss") }
        };

        // Build query string from sorted params
        var queryString = BuildQueryString(vnpParams);

        // HMAC-SHA512 hash
        var hash = HmacSha512(_hashSecret, queryString);
        var paymentUrl = $"{_paymentUrl}?{queryString}&vnp_SecureHash={hash}";

        return paymentUrl;
    }

    public VnPayCallbackResult ProcessCallback(IQueryCollection queryParams)
    {
        var vnpParams = new SortedDictionary<string, string>();
        string receivedHash = "";

        foreach (var (key, value) in queryParams)
        {
            if (key == "vnp_SecureHash" || key == "vnp_SecureHashType")
            {
                if (key == "vnp_SecureHash")
                    receivedHash = value.ToString();
                continue;
            }

            if (key.StartsWith("vnp_"))
                vnpParams[key] = value.ToString();
        }

        // Rebuild query string and verify hash
        var queryString = BuildQueryString(vnpParams);
        var computedHash = HmacSha512(_hashSecret, queryString);
        var isValid = string.Equals(computedHash, receivedHash, StringComparison.OrdinalIgnoreCase);

        var result = new VnPayCallbackResult
        {
            IsValid = isValid,
            ResponseCode = vnpParams.GetValueOrDefault("vnp_ResponseCode", ""),
            TransactionStatus = vnpParams.GetValueOrDefault("vnp_TransactionStatus", ""),
            OrderInfo = vnpParams.GetValueOrDefault("vnp_OrderInfo", ""),
            BankCode = vnpParams.GetValueOrDefault("vnp_BankCode", ""),
            PayDate = vnpParams.GetValueOrDefault("vnp_PayDate", "")
        };

        if (long.TryParse(vnpParams.GetValueOrDefault("vnp_TxnRef", "0"), out var orderId))
            result.OrderId = orderId;

        if (long.TryParse(vnpParams.GetValueOrDefault("vnp_TransactionNo", "0"), out var txnNo))
            result.VnPayTransactionNo = txnNo;

        if (long.TryParse(vnpParams.GetValueOrDefault("vnp_Amount", "0"), out var rawAmount))
            result.Amount = rawAmount / 100m;

        // Payment is successful when both ResponseCode and TransactionStatus are "00"
        result.IsSuccess = isValid
            && result.ResponseCode == "00"
            && result.TransactionStatus == "00";

        return result;
    }

    /// <summary>Build URL-encoded query string from sorted dictionary</summary>
    private static string BuildQueryString(SortedDictionary<string, string> parameters)
    {
        var sb = new StringBuilder();
        foreach (var (key, value) in parameters)
        {
            if (sb.Length > 0) sb.Append('&');
            sb.Append(WebUtility.UrlEncode(key));
            sb.Append('=');
            sb.Append(WebUtility.UrlEncode(value));
        }
        return sb.ToString();
    }

    /// <summary>HMAC-SHA512 hash</summary>
    private static string HmacSha512(string key, string data)
    {
        var keyBytes = Encoding.UTF8.GetBytes(key);
        var dataBytes = Encoding.UTF8.GetBytes(data);

        using var hmac = new HMACSHA512(keyBytes);
        var hashBytes = hmac.ComputeHash(dataBytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
