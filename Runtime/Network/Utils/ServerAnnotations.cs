#if CLIENT
// fake require Attribute
namespace GameFoundation.Network.Utils
{
    using System;

    public class RequiredAttribute : Attribute
    {
    }
}
#endif