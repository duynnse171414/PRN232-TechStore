namespace BusinessLogicLayer.DTOs;

/// <summary>Request tạo URL thanh toán VNPay</summary>
public class VnPayCreateUrlRequest
{
    public long OrderId { get; set; }
}

/// <summary>Response chứa URL redirect sang VNPay</summary>
public class VnPayCreateUrlResponse
{
    public long OrderId { get; set; }
    public string PaymentUrl { get; set; }
}

/// <summary>Kết quả trả về từ VNPay callback (Return URL / IPN)</summary>
public class VnPayCallbackResult
{
    public bool IsValid { get; set; }
    public bool IsSuccess { get; set; }
    public long OrderId { get; set; }
    public long VnPayTransactionNo { get; set; }
    public string ResponseCode { get; set; }
    public string TransactionStatus { get; set; }
    public decimal Amount { get; set; }
    public string BankCode { get; set; }
    public string OrderInfo { get; set; }
    public string PayDate { get; set; }
}
