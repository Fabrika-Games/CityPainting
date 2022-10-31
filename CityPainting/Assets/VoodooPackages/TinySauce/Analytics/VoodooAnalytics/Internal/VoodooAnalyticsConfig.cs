using JetBrains.Annotations;

namespace Voodoo.Sauce.Internal.Analytics
{
    public static class VoodooAnalyticsConfig
    { 
        [CanBeNull] public static AnalyticsConfig AnalyticsConfig { get; set; }
    }
}