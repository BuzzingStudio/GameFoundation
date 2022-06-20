namespace GameFoundation.Network.ApiHandler
{
    using GameFoundation.GameManager;
    using GameFoundation.Models;
    using GameFoundation.Network.Authentication;
    using GameFoundation.Network.Interface;
    using GameFoundation.Network.Utils;
    using GameFoundation.Network.WebService;
    using GameFoundation.Utilities.LogService;

    public class LoginHttpRequest : BaseHttpRequest<LoginResponseData>
    {
        private readonly DataLoginServices       dataLoginServices;
        private readonly GameFoundationLocalData localData;
        private readonly HandleLocalDataServices handleLocalDataServices;
        private readonly PlayerState             mechPlayerState;

        public LoginHttpRequest(ILogService logger, DataLoginServices dataLoginServices, GameFoundationLocalData localData,
            HandleLocalDataServices handleLocalDataServices, PlayerState mechPlayerState) : base(logger)
        {
            this.dataLoginServices       = dataLoginServices;
            this.localData               = localData;
            this.handleLocalDataServices = handleLocalDataServices;
            this.mechPlayerState         = mechPlayerState;
        }
        public override void Process(LoginResponseData responseData)
        {
            var jwtToken       = responseData.JwtToken;
            var refreshToken   = responseData.RefreshToken;
            var expirationTime = responseData.ExpirationTime;
            var email          = responseData.Email;
            var fullName       = responseData.Fullname;
            var avatar         = responseData.Picture;
            var wallet         = responseData.WalletAddress;
            if (string.IsNullOrEmpty(jwtToken)) return;
            this.SaveDataToLocalData(jwtToken, refreshToken, expirationTime, email, fullName, avatar, wallet);
        }

        private void SaveDataToLocalData(string jwtToken, string refreshToken, long expirationTime, string email, string fullName, string avatar, string wallet)
        {
            this.localData.ServerToken.JwtToken       = jwtToken;
            this.localData.ServerToken.RefreshToken   = refreshToken;
            this.localData.ServerToken.ExpirationTime = expirationTime;
            this.localData.ServerToken.WalletAddress  = wallet;
            switch (this.dataLoginServices.currentTypeLogin)
            {
                case TypeLogIn.Facebook:
                    this.localData.UserDataLogin.LastLogin = TypeLogIn.Facebook;
                    this.mechPlayerState.PlayerData.Name   = this.localData.UserDataLogin.FacebookLogin.UserName;
                    this.mechPlayerState.PlayerData.Avatar = this.localData.UserDataLogin.FacebookLogin.URLImage;
                    break;
                case TypeLogIn.Google:
                    this.localData.UserDataLogin.LastLogin = TypeLogIn.Google;
                    this.mechPlayerState.PlayerData.Name   = this.localData.UserDataLogin.GoogleLogin.UserName;
                    this.mechPlayerState.PlayerData.Avatar = this.localData.UserDataLogin.GoogleLogin.URLImage;
                    break;
                case TypeLogIn.OTPCode:
                    this.localData.UserDataLogin.LastLogin = TypeLogIn.OTPCode;
                    this.mechPlayerState.PlayerData.Name   = fullName;
                    this.mechPlayerState.PlayerData.Avatar = avatar ?? "";
                    this.mechPlayerState.PlayerData.Email  = email;
                    break;
                case TypeLogIn.None:
                    break;
            }

            this.handleLocalDataServices.Save(this.localData,true);
            this.dataLoginServices.Status.Value = AuthenticationStatus.Authenticated;
        }

        public override void ErrorProcess(int statusCode)
        {
            switch (statusCode)
            {
                case 1:
                    // ToDo
                    break;
                case 2:
                    this.dataLoginServices.Status.Value = this.dataLoginServices.currentTypeLogin == TypeLogIn.Google ? AuthenticationStatus.FailWithGoogleToken : AuthenticationStatus.FailWithFbToken;
                    break;
                case 3:
                    this.dataLoginServices.Status.Value = AuthenticationStatus.InvalidRefreshToken;
                    break;
                case 4:
                    this.dataLoginServices.Status.Value = AuthenticationStatus.RefreshTokenNotFound;
                    break;
                case 5:
                    this.dataLoginServices.Status.Value = AuthenticationStatus.InvalidOTP;
                    break;
                case 6:
                    this.dataLoginServices.Status.Value = AuthenticationStatus.InvalidEmail;
                    break;
                default:
                    base.ErrorProcess(statusCode);
                    break;
            }
        }
    }
    
    [HttpRequestDefinition("login/authentication")]
    public class LoginRequestData : IHttpRequestData
    {
        public string DeviceToken { get; set; }
        public string FbToken     { get; set; }
        public string GgToken     { get; set; }
    }
    
    /// <summary>
    /// 1. unauthorize
    /// 2. invalid token
    /// 3. refresh token is invalid
    /// 4. refresh token not found
    /// 5. Invalid otp
    /// 6. Invalid email
    /// </summary>
    public class LoginResponseData : IHttpResponseData
    {
        public static int OtpExpireTime  = 180; // OTP expire time in seconds.
        public static int ResendableTime = 10;  // after ResendableTime seconds, user can request the OTP again.
        public static int TimeRemain     = 10;  // maximum: 10 times to send.
        
        public string JwtToken       { get; set; }
        public string RefreshToken   { get; set; }
        public long   ExpirationTime { get; set; }
        public string WalletAddress  { get; set; }
        public string Email          { get; set; }
        public string Fullname       { get; set; }
        public string Picture        { get; set; }
    }
}