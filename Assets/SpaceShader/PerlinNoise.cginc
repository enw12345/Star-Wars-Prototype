#ifndef PERLIN_NOISE
#define PERLIN_NOISE


float smootherstep(float a, float b, float r)
{
    r = clamp(r, 0.0, 1.0);
    r = r * r * r * (r * (6.0 * r - 15.0) + 10.0);
    return lerp(a, b, r);
}

float perlin_2d(float2 p, sampler2D tNoise)
{
    float tNoiseSize = 512;

    float2 p0 = floor(p);
    float2 p1 = p0 + float2(1, 0);
    float2 p2 = p0 + float2(1, 1);
    float2 p3 = p0 + float2(0, 1);
    float2 d0 = tex2D(tNoise, p0 / tNoiseSize).xy;
    float2 d1 = tex2D(tNoise, p1 / tNoiseSize).xy;
    float2 d2 = tex2D(tNoise, p2 / tNoiseSize).xy;
    float2 d3 = tex2D(tNoise, p3 / tNoiseSize).xy;
    d0 = 2.0 * d0 - 1.0;
    d1 = 2.0 * d1 - 1.0;
    d2 = 2.0 * d2 - 1.0;
    d3 = 2.0 * d3 - 1.0;
    float2 p0p = p - p0;
    float2 p1p = p - p1;
    float2 p2p = p - p2;
    float2 p3p = p - p3;
    float dp0 = dot(d0, p0p);
    float dp1 = dot(d1, p1p);
    float dp2 = dot(d2, p2p);
    float dp3 = dot(d3, p3p);
    float fx = p.x - p0.x;
    float fy = p.y - p0.y;
    float m01 = smootherstep(dp0, dp1, fx);
    float m32 = smootherstep(dp3, dp2, fx);
    float m01m32 = smootherstep(m01, m32, fy);
    return m01m32;
}

float normalNoise(float2 p, sampler2D tNoise)
{
    return perlin_2d(p, tNoise) * 0.5 + 0.5;
}

#endif
