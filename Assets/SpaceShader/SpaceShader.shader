Shader "Unlit/SpaceShader"
{
    Properties
    {
        [Header(Point Star Variables)]_StarDensity("Star Density", Range(0,0.1)) = 0.05
        _StarBrightness("Star Brightness", Float) = 0.1
        _CellSize ("Cell Size", Vector) = (1,1,1,0)

        [Header(Nebula Properties)]_PerlinNoiseTexture("Noise Texture", 2D) = "white"{}
        _NebulaColor("Nebula Color", Color) = (.25, .5, .5, 1)
        _NebulaColor1("Nebula Color 1", Color) = (.25, .5, .5, 1)
        _NebulaColor2("Nebula Color 2", Color) = (.25, .5, .5, 1)
        _NebulaColor3("Nebula Color 3", Color) = (.25, .5, .5, 1)
        _NebulaColor4("Nebula Color 4", Color) = (.25, .5, .5, 1)

        _NebulaOffset("Nebula Offset", Float) = .5
        _NebulaOffset1("Nebula Offset 1", Float) = .5
        _NebulaOffset2("Nebula Offset 2", Float) = .5
        _NebulaOffset3("Nebula Offset 3", Float) = .5
        _NebulaOffset4("Nebula Offset 4", Float) = .5

        _NebulaScale("Nebula Scale", Float) = .5
        _NebulaDensity("Nebula Density", Range(0, .2)) = .15
        _NebulaFalloff("Nebula Falloff", Range(1, 5)) = 5

        [Header(Star Properties)]_CoreColor("Star Color", Color) = (.25, .25, .25, 1)
        _HaloColor("Halo Color", Color) = (.25, .25, .25, 1)
        _HaloFalloff("Halo Falloff", Range(0,5)) = 2
        _StarRadius("StarRadius", Range(0,.5)) = .3
        _StarScale("Star Scale", Range(0,1)) = 1
        _StarCenter("Star Center", Vector) = (.5, .5, 0, 0)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "WhiteNoise.cginc"
            #include "PerlinNoise.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD;
                float4 vertex : SV_POSITION;
            };

            //Point Stars
            float _StarDensity;
            float _StarBrightness;
            float3 _CellSize;

            sampler2D _PerlinNoiseTexture;
            float4 _PerlinNoiseTexture_ST;

            //Nebula
            float4 _NebulaColor;
            float4 _NebulaColor1;
            float4 _NebulaColor2;
            float4 _NebulaColor3;
            float4 _NebulaColor4;

            float _NebulaOffset;
            float _NebulaOffset1;
            float _NebulaOffset2;
            float _NebulaOffset3;
            float _NebulaOffset4;

            float _NebulaScale;
            float _NebulaDensity;
            float _NebulaFalloff;

            //Sun and Stars
            float4 _CoreColor;
            float4 _HaloColor;
            float _HaloFalloff;
            float _StarRadius;
            float _StarScale;
            float4 _StarCenter;

            float noise(float2 p, float nebulaOffset)
            {
                p += nebulaOffset;

                int steps = 5;
                float scale = pow(2.0, float(steps));
                float displace = 0.0;
                for(int i = 0; i < steps; i++)
                {
                    scale *= 0.5;
                    displace = normalNoise(p * scale + displace, _PerlinNoiseTexture);
                }

                return tex2D(_PerlinNoiseTexture, p + displace).x;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _PerlinNoiseTexture); 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 randVar = floor(float3(i.vertex.x, i.vertex.y, .32)/_CellSize);

                float3 random = rand3dTo3dBW(randVar);
                float4 randomCol = float4(random.xyz, 1);  
                
                float n = noise(i.uv.xy * _NebulaScale * 1.0, _NebulaOffset);
                n = pow(n + _NebulaDensity, _NebulaFalloff);
                
                float n1 = noise(i.uv.xy * _NebulaScale * 1.0, _NebulaOffset1);
                n1 = pow(n1 + _NebulaDensity, _NebulaFalloff);

                float n2 = noise(i.uv.xy * _NebulaScale * 1.0, _NebulaOffset2);
                n2 = pow(n2 + _NebulaDensity, _NebulaFalloff);

                float n3 = noise(i.uv.xy * _NebulaScale * 1.0, _NebulaOffset3);
                n3 = pow(n3 + _NebulaDensity, _NebulaFalloff);

                float n4 = noise(i.uv.xy * _NebulaScale * 1.0, _NebulaOffset4);
                n4 = pow(n4 + _NebulaDensity, _NebulaFalloff);

                float4 col = step(randomCol, float4(_StarDensity, _StarDensity, _StarDensity, _StarDensity)) * _StarBrightness * rand3dTo1dBW(random);

                float d = length(i.uv.xy - _StarCenter.xy) / _StarScale;
                if( d <= _StarRadius)
                {
                    return col = _CoreColor;
                }

                float e = 1.0 - exp(-(d - _StarRadius) * _HaloFalloff);
                float3 rgb = lerp(_CoreColor, _HaloColor, e);
                rgb = lerp(rgb, float3(0,0,0), e);

                fixed4 nebulaColor = float4(lerp(col, _NebulaColor, n));
                fixed4 nebulaColor1 = float4(lerp(nebulaColor, _NebulaColor1, n1));
                fixed4 nebulaColor2 = float4(lerp(nebulaColor1, _NebulaColor2, n2));
                fixed4 nebulaColor3 = float4(lerp(nebulaColor2, _NebulaColor3, n3));
                fixed4 nebulaColor4 = float4(lerp(nebulaColor3, _NebulaColor4, n4));

                fixed4 finalColor = nebulaColor4 + float4(rgb,1);
                return finalColor;
            }
            ENDCG
        }
    }
}
