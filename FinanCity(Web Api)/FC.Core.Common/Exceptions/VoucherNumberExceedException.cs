namespace FC.Core.Exceptions
{
    using System;

    public class VoucherNumberExceedException : Exception
    {
        public VoucherNumberExceedException()
        {
            // do nothing
        }

        public override string Message
        {
            get
            {
                return "Id Reference Number is not Valid due to the maximum value exceeding, Please go to the Id Reference Setup page to increase the Max size OR change the Prefix value";
            }
        }
    }
}
