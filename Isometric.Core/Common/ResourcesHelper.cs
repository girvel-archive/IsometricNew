using System.Linq;

namespace Isometric.Core.Common
{
    public static class ResourcesHelper
    {
        public static T Added<T>(this T r1, T r2) where T : IResources
        {
            var result = (T) r1.Clone();

            for (var i = 0; i < r1.ResourceTypesNumber; i++)
            {
                result[i] += r2[i];
            }

            return result;
        }

        public static T Inverted<T>(this T r) where T : IResources
        {
            var result = (T) r.Clone();

            for (var i = 0; i < result.ResourceTypesNumber; i++)
            {
                result[i] = -r[i];
            }

            return result;
        }

        public static T Substracted<T>(this T r1, T r2) where T : IResources
        {
            return r1.Added(r2.Inverted());
        }

        public static T Multiplied<T>(this T r, float n) where T : IResources
        {
            var result = (T) r.Clone();

            for (var i = 0; i < r.ResourceTypesNumber; i++)
            {
                result[i] *= n;
            }

            return result;
        }

        public static bool Equals<T>(this T r1, T r2) where T : IResources
        {
            for (var i = 0; i < r1.ResourceTypesNumber; i++)
            {
                if (r1[i] != r2[i])
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Enough<T>(this T resources, T price) where T : IResources
        {
            return resources.Substracted(price).ResourcesArray.All(r => r >= 0);
        }

        public static float GetEnoughCoefficient<T>(this T resources, T price) where T : IResources
        {
            var result = 1F;

            for (var i = 0; i < resources.ResourceTypesNumber; i++)
            {
                if (price[i] <= 0) continue;

                var current = resources[i] / price[i];

                if (current < result)
                {
                    result = current;
                }
            }

            return result;
        }

        public static bool IsZero<T>(this T resources) where T : IResources 
            => resources?.ResourcesArray.All(r => r == 0) ?? true;
    }
}