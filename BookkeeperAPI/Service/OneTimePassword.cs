namespace BookkeeperAPI.Service
{
    #region usings
    using System;
    using System.Text;
    #endregion
    public static class OneTimePassword
    {
        public static int Generate()
        {
            long ticks = DateTime.UtcNow.Ticks;
            long totalSeconds = Convert.ToInt64(TimeSpan.FromTicks(ticks).TotalSeconds);
            string tot = totalSeconds.ToString();
            int otp = 0;
            for (int i = 0; i < 6; i++)
            {
                otp += (int)((tot[tot.Length - i - 1] - 48) * Math.Pow(10, i));
            }

            do
            {
                Random random = new Random();
                int numberOfRandomNumbersInOTP = random.Next(1, 4);
                for (int i = 1; i <= numberOfRandomNumbersInOTP; i++)
                {
                    Random rand = new Random();
                    int randomIndex = rand.Next(0, 6);
                    StringBuilder otpStr = new StringBuilder(otp.ToString());
                    otpStr[randomIndex] = random.Next(0, 10).ToString()[0];
                    otp = Convert.ToInt32(otpStr.ToString());
                }
            } while (false);

            otp = Convert.ToInt32(otp.ToString().PadRight(6, '0'));
            return otp < 0 ? otp * -1 : otp;
        }
    }
}
