using System;

namespace Helpers
{
    public static class ComparisonExtensions
    {
        public static bool IsOneOf<T>(this T toTest, params T[] tests)
        {
            foreach (T test in tests)
                if (toTest.Equals(test))
                    return true;
            return false;
        }

        public static bool IsBetween<T>(this T toTest, T first, T second, bool firstInclusive = false, bool secondInclusive = false) where T: IComparable
        {
            if (firstInclusive && toTest.CompareTo(first) == 0)
                return true;
            if (secondInclusive && toTest.CompareTo(second) == 0)
                return true;
            return toTest.CompareTo(first) > 0 && toTest.CompareTo(second) < 0;
        }
    }
}