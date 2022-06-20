namespace GameFoundation.BlueprintFlow.Signals
{
    public class LoadBlueprintDataSignal
    {
        public string Url;
        public string Hash;
    }

    public class LoadBlueprintDataSuccessedSignal
    {
        
    }

    public class LoadBlueprintDataProgressSignal
    {
        public float percent;
    }
}