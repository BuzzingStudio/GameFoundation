namespace GameFoundation.Network.ApiHandler
{
    using GameFoundation.BlueprintFlow.BlueprintControlFlow;
    using GameFoundation.Network.Interface;
    using GameFoundation.Network.Utils;
    using GameFoundation.Network.WebService;
    using GameFoundation.Utilities;
    using GameFoundation.Utilities.LogService;

    /// <summary>
    /// Get blueprint download link from server
    /// </summary>
    public class BlueprintDownloadRequest : BaseHttpRequest<GetBlueprintResponseData>
    {
        #region zenject

        private readonly BlueprintReaderManager blueprintReaderManager;

        #endregion
        
        public BlueprintDownloadRequest(ILogService logger, BlueprintReaderManager blueprintReaderManager) : base(logger)
        {
            this.blueprintReaderManager = blueprintReaderManager;
        }

        public override void Process(GetBlueprintResponseData responseDataData)
        {
            this.Logger.Log($"Blueprint download link: {responseDataData.Url}");
            this.blueprintReaderManager.LoadBlueprint(responseDataData.Url, responseDataData.Hash);
        }
    }
    
    [HttpRequestDefinition("blueprint/get")]
    public class GetBlueprintRequestData : IHttpRequestData
    {
        [Required]
        public string Version { set; get; }
    }

    public class GetBlueprintResponseData : IHttpResponseData
    {
        public string Url  { set; get; }
        public string Hash { set; get; }
    }
}
