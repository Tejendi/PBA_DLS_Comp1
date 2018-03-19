﻿using System;

namespace PrimeCheckerComponent.Services
{
    public struct PrimeCheckerService
    {
        public static bool IsPrime(long number)
        {
            if (number == 1)
                return false;

            if (number == 2)
                return true;

            if (number % 2 == 0)
                return false;

            long boundary = (long)Math.Floor(Math.Sqrt(number));

            for (int i = 3; i <= boundary; i += 2)
            {
                if (number % i == 0)
                    return false;
            }

            return true;
        }

        public static int CountPrimes(long start, long end)
        {

            int result = 0;

            if (start > end)
                return result;

            while (start < end)
            {

                if (IsPrime(start))
                    result++;

                start++;
            }

            return result;
        }
    }
}
