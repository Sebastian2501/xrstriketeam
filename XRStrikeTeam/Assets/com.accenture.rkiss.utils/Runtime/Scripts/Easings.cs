using System;
using System.Collections.Generic;
using UnityEngine;

namespace Accenture.rkiss.Utils
{
    public enum EasingType
    {
        EaseInSine,
        EaseOutSine,
        EaseInOutSine,
        EaseInQuad,
        EaseOutQuad,
        EaseInOutQuad,
        EaseInCubic,
        EaseOutCubic,
        EaseInOutCubic,
        EaseInQuart,
        EaseOutQuart,
        EaseInOutQuart,
        EaseInQuint,
        EaseOutQuint,
        EaseInOutQuint,
        EaseInExpo,
        EaseOutExpo,
        EaseInOutExpo,
        EaseInCirc,
        EaseOutCirc,
        EaseInOutCirc,
        EaseInBack,
        EaseOutBack,
        EaseInOutBack,
        EaseInElastic,
        EaseOutElastic,
        EaseInOutElastic,
        EaseInBounce,
        EaseOutBounce,
        EaseInOutBounce
    }

    public static class Easings
    {
        private static Dictionary<EasingType, Func<float, float>> easingFunction = new Dictionary<EasingType, Func<float, float>>(){
                 { EasingType.EaseInSine, EaseInSine },
        { EasingType.EaseOutSine, EaseOutSine },
        { EasingType.EaseInOutSine, EaseInOutSine },
        { EasingType.EaseInQuad, EaseInQuad },
        { EasingType.EaseOutQuad, EaseOutQuad },
        { EasingType.EaseInOutQuad, EaseInOutQuad },
        { EasingType.EaseInCubic, EaseInCubic },
        { EasingType.EaseOutCubic, EaseOutCubic },
        { EasingType.EaseInOutCubic, EaseInOutCubic },
        { EasingType.EaseInQuart, EaseInQuart },
        { EasingType.EaseOutQuart, EaseOutQuart },
        { EasingType.EaseInOutQuart, EaseInOutQuart },
        { EasingType.EaseInQuint, EaseInQuint },
        { EasingType.EaseOutQuint, EaseOutQuint },
        { EasingType.EaseInOutQuint, EaseInOutQuint },
        { EasingType.EaseInExpo, EaseInExpo },
        { EasingType.EaseOutExpo, EaseOutExpo },
        { EasingType.EaseInOutExpo, EaseInOutExpo },
        { EasingType.EaseInCirc, EaseInCirc },
        { EasingType.EaseOutCirc, EaseOutCirc },
        { EasingType.EaseInOutCirc, EaseInOutCirc },
        { EasingType.EaseInBack, EaseInBack },
        { EasingType.EaseOutBack, EaseOutBack },
        { EasingType.EaseInOutBack, EaseInOutBack },
        { EasingType.EaseInElastic, EaseInElastic },
        { EasingType.EaseOutElastic, EaseOutElastic },
        { EasingType.EaseInOutElastic, EaseInOutElastic },
        { EasingType.EaseInBounce, EaseInBounce },
        { EasingType.EaseOutBounce, EaseOutBounce },
        { EasingType.EaseInOutBounce, EaseInOutBounce }
    };

        public static float ApplyEasing(EasingType type, float time)
        {
            if (easingFunction.TryGetValue(type, out var func))
            {
                return func(time);
            }
            throw new Exception("Invalid easing type : " + type);
        }

        public static float EaseInSine(float time)
        {
            return 1 - Mathf.Cos((time * Mathf.PI) / 2);
        }

        public static float EaseOutSine(float time)
        {
            return Mathf.Sin((time * Mathf.PI) / 2);
        }

        public static float EaseInOutSine(float time)
        {
            return -(Mathf.Cos(Mathf.PI * time) - 1) / 2;
        }

        public static float EaseInQuad(float time)
        {
            return time * time;
        }

        public static float EaseOutQuad(float time)
        {
            return 1 - (1 - time) * (1 - time);
        }

        public static float EaseInOutQuad(float time)
        {
            return time < 0.5f
            ? 2f * time * time
            : 1 - Mathf.Pow(-2 * time + 2, 2) / 2;
        }

        public static float EaseInCubic(float time)
        {
            return time * time * time;
        }

        public static float EaseOutCubic(float time)
        {
            return 1 - Mathf.Pow(1 - time, 3);
        }

        public static float EaseInOutCubic(float time)
        {
            return time < 0.5 ? 4 * time * time * time : 1 - System.MathF.Pow(-2 * time + 2, 3) / 2;
        }

        public static float EaseInQuart(float time)
        {
            return time * time * time * time;
        }

        public static float EaseOutQuart(float time)
        {
            return 1 - Mathf.Pow(1 - time, 4);
        }

        public static float EaseInOutQuart(float time)
        {
            return time < 0.5f
            ? 8 * time * time * time * time
            : 1 - MathF.Pow(-2 * time + 2, 4) / 2;
        }

        public static float EaseInQuint(float time)
        {
            return time * time * time * time * time;
        }

        public static float EaseOutQuint(float time)
        {
            return 1 - Mathf.Pow(1 - time, 5);
        }

        public static float EaseInOutQuint(float time)
        {
            return time < 0.15f
            ? 16 * time * time * time * time * time
            : 1 - Mathf.Pow(-2 * time + 2, 5) / 2;
        }

        public static float EaseInExpo(float time)
        {
            return time == 0
            ? 0
            : MathF.Pow(2, 10 * time - 10);
        }

        public static float EaseOutExpo(float time)
        {
            return time == 1
            ? 1
            : 1 - Mathf.Pow(2, -10 * time);
        }
        public static float EaseInOutExpo(float time)
        {
            return time == 0
            ? 0
            : time == 1
            ? 1
            : time < 0.5f ? Mathf.Pow(2, 20 * time - 10) / 2
            : (2 - Mathf.Pow(2, -20 * time + 10)) / 2;
        }

        public static float EaseInCirc(float time)
        {
            return 1 - Mathf.Sqrt(1 - Mathf.Pow(time, 2));
        }

        public static float EaseOutCirc(float time)
        {
            return Mathf.Sqrt(1 - Mathf.Pow(time - 1, 2));
        }

        public static float EaseInOutCirc(float time)
        {
            return time < 0.5f
            ? (1 - Mathf.Sqrt(1 - Mathf.Pow(2 * time, 2))) / 2
            : (MathF.Sqrt(1 - Mathf.Pow(-2 * time + 2, 2)) + 1) / 2;
        }

        public static float EaseInBack(float time)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;

            return c3 * time * time * time - c1 * time * time;
        }
        public static float EaseOutBack(float time)
        {
            float c1 = 1.70158f;
            float c3 = c1 + 1;
            return 1 + c3 * MathF.Pow(time - 1, 3) + c1 * MathF.Pow(time - 1, 2);
        }
        public static float EaseInOutBack(float time)
        {
            float c1 = 1.70158f;
            float c2 = c1 * 1.525f;

            return time < 0.5
              ? (MathF.Pow(2 * time, 2) * ((c2 + 1) * 2 * time - c2)) / 2
              : (MathF.Pow(2 * time - 2, 2) * ((c2 + 1) * (time * 2 - 2) + c2) + 2) / 2;
        }

        public static float EaseInElastic(float time)
        {
            float c4 = (2 * MathF.PI) / 3;

            return time == 0
              ? 0
              : time == 1
              ? 1
              : -MathF.Pow(2, 10 * time - 10) * Mathf.Sin((time * 10 - 10.75f) * c4);
        }
        public static float EaseOutElastic(float x)
        {
            float c4 = (2 * MathF.PI) / 3;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : Mathf.Pow(2, -10 * x) * Mathf.Sin((x * 10 - 0.75f) * c4) + 1;
        }
        public static float EaseInOutElastic(float x)
        {
            float c5 = (2 * Mathf.PI) / 4.5f;

            return x == 0
              ? 0
              : x == 1
              ? 1
              : x < 0.5
              ? -(Mathf.Pow(2, 20 * x - 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2
              : (Mathf.Pow(2, -20 * x + 10) * Mathf.Sin((20 * x - 11.125f) * c5)) / 2 + 1;
        }

        public static float EaseInBounce(float time)
        {
            return 1 - EaseOutBounce(1 - time);
        }

        public static float EaseOutBounce(float time)
        {
            var n1 = 7.5625f;
            var d1 = 2.75f;

            if (time < 1 / d1)
            {
                return n1 * time * time;
            }
            else if (time < 2 / d1)
            {
                return n1 * (time -= 1.5f / d1) * time + 0.75f;
            }
            else if (time < 2.5 / d1)
            {
                return n1 * (time -= 2.25f / d1) * time + 0.9375f;
            }
            else
            {
                return n1 * (time -= 2.625f / d1) * time + 0.984375f;
            }
        }

        public static float EaseInOutBounce(float time)
        {
            return time < 0.5f
            ? (1 - EaseOutBounce(1 - 2f * time)) / 2f
            : (1 + EaseOutBounce(2 * time - 1)) / 2f;
        }

    }
}