namespace GameFoundation.Network.ApiHandler
{
    using GameFoundation.Network.Authentication;
    using GameFoundation.Network.Interface;
    using GameFoundation.Network.Utils;
    using GameFoundation.Network.WebService;
    using GameFoundation.Utilities.LogService;

    public class SendOTPRequest : BaseHttpRequest<SendOtpResponseData>
    {
        private readonly DataLoginServices dataLoginServices;
        public SendOTPRequest(ILogService logger, DataLoginServices dataLoginServices) : base(logger) { this.dataLoginServices = dataLoginServices; }
        public override void Process(SendOtpResponseData responseDataData) { this.dataLoginServices.SendCodeStatus.Value       = SendCodeStatus.Sent; }

        public override void ErrorProcess(int statusCode)
        {
            switch (statusCode)
            {
                case 0:
                    this.dataLoginServices.SendCodeStatus.Value = SendCodeStatus.EmailIsInvalid;
                    break;
                case 1:
                    this.dataLoginServices.SendCodeStatus.Value = SendCodeStatus.TooManyRequest;
                    break;
            }
        }
    }
    
    
    [HttpRequestDefinition("otp/send")]
    public class SendOtpRequestData : IHttpRequestData
    {
        public string Email { get; set; }
    }

    /// <summary>
    /// 0. Email is not whitedlist
    /// 1. Invalid email
    /// 2. Too many request
    /// </summary>
    public class SendOtpResponseData : IHttpResponseData
    {
        public int    Status { get; set; }
        public string Code   { get; set; }
    }

    public class SendOtpApiCode
    {
        public const int EmailIsNotWhitelisted = 0;
        public const int InvalidEmail          = 1;
        public const int TooManyRequest        = 2;
    }
}