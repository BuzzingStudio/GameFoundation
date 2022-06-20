namespace GameFoundation.Network.WebService
{
    using System;
    using GameFoundation.Network.Utils;
    using Newtonsoft.Json;
    using Zenject;

    public class ClientWrappedHttpRequestData : WrappedHttpRequestData, IPoolable<IMemoryPool>, IDisposable
    {
        [JsonIgnore]
        public IMemoryPool Pool;

        public void OnDespawned()             { this.Pool = null; }
        public void OnSpawned(IMemoryPool p1) { this.Pool = p1; }
        public void Dispose()                 { this.Pool.Despawn(this); }
    }
}