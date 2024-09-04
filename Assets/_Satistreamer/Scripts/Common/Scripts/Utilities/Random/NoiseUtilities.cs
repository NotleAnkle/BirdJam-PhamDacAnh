using System.Collections;
using Unity.Burst;
using Unity.Mathematics;
using UnityEngine;

namespace Utilities
{
    [BurstCompile(FloatPrecision = FloatPrecision.High)]
    public static class NoiseUtilities
    {

        //--------------------------------------------
        //
        // Description : Array and textureless GLSL 2D simplex noise function.
        //      Author : Ian McEwan, Ashima Arts.
        //  Maintainer : stegu
        //     Lastmod : 20110822 (ijm)
        //     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
        //               Distributed under the MIT License. See LICENSE file.
        //               https://github.com/ashima/webgl-noise
        //               https://github.com/stegu/webgl-noise
        // 

        
        static float4 mod289(float4 x)
        {
            return x - math.floor(x * (1.0f / 289.0f)) * 289.0f;
        }

        
        static float3 mod289(float3 x)
        {
            return x - math.floor(x * (1.0f / 289.0f)) * 289.0f;
        }

        
        static float2 mod289(float2 x)
        {
            return x - math.floor(x * (1.0f / 289.0f)) * 289.0f;
        }

        
        static float4 permute(float4 x)
        {
            return mod289(((x * 34.0f) + 10.0f) * x);
        }
        
        static float3 permute(float3 x)
        {
            return mod289(((x * 34.0f) + 10.0f) * x);
        }

        
        public static float snoise(float2 v)
        {
            float4 C = new float4(0.211324865405187f,  // (3.0-sqrt(3.0))/6.0
                                0.366025403784439f,  // 0.5*(sqrt(3.0)-1.0)
                                -0.577350269189626f,  // -1.0 + 2.0 * C.x
                                0.024390243902439f); // 1.0 / 41.0
                                                     // First corner
            float2 i = math.floor(v + math.dot(v, C.yy));
            float2 x0 = v - i + math.dot(i, C.xx);

            // Other corners
            float2 i1;
            //i1.x = step( x0.ySpeed, x0.x ); // x0.x > x0.ySpeed ? 1.0 : 0.0
            //i1.ySpeed = 1.0 - i1.x;
            i1 = (x0.x > x0.y) ? new float2(1.0f, 0.0f) : new float2(0.0f, 1.0f);
            // x0 = x0 - 0.0 + 0.0 * C.xx ;
            // x1 = x0 - i1 + 1.0 * C.xx ;
            // x2 = x0 - 1.0 + 2.0 * C.xx ;
            float4 x12 = x0.xyxy + C.xxzz;
            x12.xy -= i1;

            // Permutations
            i = mod289(i); // Avoid truncation effects in permutation
            float3 p = permute(permute(i.y + new float3(0.0f, i1.y, 1.0f))
                + i.x + new float3(0.0f, i1.x, 1.0f));

            float3 m = math.max(0.5f - new float3(math.dot(x0, x0), math.dot(x12.xy, x12.xy), math.dot(x12.zw, x12.zw)), 0.0f);
            m = m * m;
            m = m * m;

            // Gradients: 41 points uniformly over a line, mapped onto a diamond.
            // The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

            float3 x = 2.0f * math.frac(p * C.www) - 1.0f;
            float3 h = math.abs((x) - 0.5f);
            float3 ox = math.floor(x + 0.5f);
            float3 a0 = x - ox;

            // Normalise gradients implicitly by scaling m
            // Approximation of: m *= inversesqrt( a0*a0 + h*h );
            m *= 1.79284291400159f - 0.85373472095314f * (a0 * a0 + h * h);

            // Compute final noise value at P
            float3 g = float3.zero;
            g.x = a0.x * x0.x + h.x * x0.y;
            g.yz = a0.yz * x12.xz + h.yz * x12.yw;
            return 130.0f * math.dot(m, g);
        }
        //--------------------------------------------

        //--------------------------------------------
        //
        // GLSL textureless classic 2D noise "cnoise",
        // with an RSL-style periodic variant "pnoise".
        // Author:  Stefan Gustavson (stefan.gustavson@liu.se)
        // Version: 2011-08-22
        //
        // Many thanks to Ian McEwan of Ashima Arts for the
        // ideas for permutation and gradient selection.
        //
        // Copyright (c) 2011 Stefan Gustavson. All rights reserved.
        // Distributed under the MIT license. See LICENSE file.
        // https://github.com/stegu/webgl-noise
        //



        
        static float4 taylorInvSqrt(float4 r)
        {
            return 1.79284291400159f - 0.85373472095314f * r;
        }

        
        static float2 fade(float2 t)
        {
            return t * t * t * (t * (t * 6.0f - 15.0f) + 10.0f);
        }

        // Classic Perlin noise
        
        public static float cnoise(float2 P)
        {
            float4 Pi = math.floor(P.xyxy) + new float4(0.0f, 0.0f, 1.0f, 1.0f);
            float4 Pf = math.frac(P.xyxy) - new float4(0.0f, 0.0f, 1.0f, 1.0f);
            Pi = mod289(Pi); // To avoid truncation effects in permutation
            float4 ix = Pi.xzxz;
            float4 iy = Pi.yyww;
            float4 fx = Pf.xzxz;
            float4 fy = Pf.yyww;

            float4 i = permute(permute(ix) + iy);

            float4 gx = math.frac(i * (1.0f / 41.0f)) * 2.0f - 1.0f;
            float4 gy = math.abs((gx) - 0.5f);
            float4 tx = math.floor(gx + 0.5f);
            gx = gx - tx;

            float2 g00 = new float2(gx.x, gy.x);
            float2 g10 = new float2(gx.y, gy.y);
            float2 g01 = new float2(gx.z, gy.z);
            float2 g11 = new float2(gx.w, gy.w);

            float4 norm = taylorInvSqrt(new float4(math.dot(g00, g00), math.dot(g01, g01), math.dot(g10, g10), math.dot(g11, g11)));
            g00 *= norm.x;
            g01 *= norm.y;
            g10 *= norm.z;
            g11 *= norm.w;

            float n00 = math.dot(g00, new float2(fx.x, fy.x));
            float n10 = math.dot(g10, new float2(fx.y, fy.y));
            float n01 = math.dot(g01, new float2(fx.z, fy.z));
            float n11 = math.dot(g11, new float2(fx.w, fy.w));

            float2 fade_xy = fade(Pf.xy);
            float2 n_x = math.lerp(new float2(n00, n01), new float2(n10, n11), fade_xy.x);
            float n_xy = math.lerp(n_x.x, n_x.y, fade_xy.y);
            return 2.3f * n_xy;
        }

        // Classic Perlin noise, periodic variant
        
        public static float pnoise(float2 P, float2 rep)
        {
            float4 Pi = math.floor(P.xyxy) + new float4(0.0f, 0.0f, 1.0f, 1.0f);
            float4 Pf = math.frac(P.xyxy) - new float4(0.0f, 0.0f, 1.0f, 1.0f);
            Pi = math.fmod(Pi, rep.xyxy); // To create noise with explicit period
            Pi = mod289(Pi);        // To avoid truncation effects in permutation
            float4 ix = Pi.xzxz;
            float4 iy = Pi.yyww;
            float4 fx = Pf.xzxz;
            float4 fy = Pf.yyww;

            float4 i = permute(permute(ix) + iy);

            float4 gx = math.frac(i * (1.0f / 41.0f)) * 2.0f - 1.0f;
            float4 gy = math.abs((gx) - 0.5f);
            float4 tx = math.floor(gx + 0.5f);
            gx = gx - tx;

            float2 g00 = new float2(gx.x, gy.x);
            float2 g10 = new float2(gx.y, gy.y);
            float2 g01 = new float2(gx.z, gy.z);
            float2 g11 = new float2(gx.w, gy.w);

            float4 norm = taylorInvSqrt(new float4(math.dot(g00, g00), math.dot(g01, g01), math.dot(g10, g10), math.dot(g11, g11)));
            g00 *= norm.x;
            g01 *= norm.y;
            g10 *= norm.z;
            g11 *= norm.w;

            float n00 = math.dot(g00, new float2(fx.x, fy.x));
            float n10 = math.dot(g10, new float2(fx.y, fy.y));
            float n01 = math.dot(g01, new float2(fx.z, fy.z));
            float n11 = math.dot(g11, new float2(fx.w, fy.w));

            float2 fade_xy = fade(Pf.xy);
            float2 n_x = math.lerp(new float2(n00, n01), new float2(n10, n11), fade_xy.x);
            float n_xy = math.lerp(n_x.x, n_x.y, fade_xy.y);
            return 2.3f * n_xy;
        }
        //--------------------------------------------

        //--------------------------------------------
        // Cellular noise ("Worley noise") in 2D in GLSL.
        // Copyright (c) Stefan Gustavson 2011-04-19. All rights reserved.
        // This code is released under the conditions of the MIT license.
        // See LICENSE file for details.
        // https://github.com/stegu/webgl-noise

        // Modulo 7 without a division
        
        static float3 mod7(float3 x)
        {
            return x - math.floor(x * (1.0f / 7.0f)) * 7.0f;
        }

        // Cellular noise, returning F1 and F2 in a float2.
        // Standard 3x3 search window for good F1 and F2 values
        
        public static float2 cellular(float2 P)
        {
            const float K = 0.142857142857f; // 1/7
            const float Ko = 0.428571428571f; // 3/7
            const float jitter = 1.0f; // Less gives more regular pattern
            float2 Pi = mod289(math.floor(P));
            float2 Pf = math.frac(P);
            float3 oi = new float3(-1.0f, 0.0f, 1.0f);
            float3 of = new float3(-0.5f, 0.5f, 1.5f);
            float3 px = permute(Pi.x + oi);
            float3 p = permute(px.x + Pi.y + oi); // p11, p12, p13
            float3 ox = math.frac(p * K) - Ko;
            float3 oy = mod7(math.floor(p * K)) * K - Ko;
            float3 dx = Pf.x + 0.5f + jitter * ox;
            float3 dy = Pf.y - of + jitter * oy;
            float3 d1 = dx * dx + dy * dy; // d11, d12 and d13, squared
            p = permute(px.y + Pi.y + oi); // p21, p22, p23
            ox = math.frac(p * K) - Ko;
            oy = mod7(math.floor(p * K)) * K - Ko;
            dx = Pf.x - 0.5f + jitter * ox;
            dy = Pf.y - of + jitter * oy;
            float3 d2 = dx * dx + dy * dy; // d21, d22 and d23, squared
            p = permute(px.z + Pi.y + oi); // p31, p32, p33
            ox = math.frac(p * K) - Ko;
            oy = mod7(math.floor(p * K)) * K - Ko;
            dx = Pf.x - 1.5f + jitter * ox;
            dy = Pf.y - of + jitter * oy;
            float3 d3 = dx * dx + dy * dy; // d31, d32 and d33, squared
                                           // Sort out the two smallest distances (F1, F2)
            float3 d1a = math.min(d1, d2);
            d2 = math.max(d1, d2); // Swap to keep candidates for F2
            d2 = math.min(d2, d3); // neither F1 nor F2 are now in d3
            d1 = math.min(d1a, d2); // F1 is now in d1
            d2 = math.max(d1a, d2); // Swap to keep candidates for F2
            d1.xy = (d1.x < d1.y) ? d1.xy : d1.yx; // Swap if smaller
            d1.xz = (d1.x < d1.z) ? d1.xz : d1.zx; // F1 is in d1.x
            d1.yz = math.min(d1.yz, d2.yz); // F2 is now not in d2.yz
            d1.y = math.min(d1.y, d1.z); // nor in  d1.z
            d1.y = math.min(d1.y, d2.x); // F2 is in d1.ySpeed, we're done.
            return math.sqrt(d1.xy);
        }
        //--------------------------------------------
    }
}