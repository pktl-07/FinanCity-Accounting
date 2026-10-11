namespace FC.Core
{
    using System;

    public static class DecimalExtension
    {
        public static decimal ToRoundUp(this decimal value)
        {
            return decimal.Round(value, 2, MidpointRounding.AwayFromZero);
        }
    }
}
