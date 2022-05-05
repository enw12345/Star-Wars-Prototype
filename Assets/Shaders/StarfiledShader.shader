Shader "Unlit/StarfiledShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
        LOD 100
//        Blend SrcAlpha OneMinusSrcAlpha
        //ZWrite off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Assets/SpaceShader/WhiteNoise.cginc"
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _StarFieldDensity;
            float _StarFieldBrightness;
            float3 _StarFieldCellSize;
            
            
            fixed4 frag (v2f_img i) : SV_Target
            {
                // i.pos.x += rand1dTo1d(_Time.y) * 10;
                // i.pos.y += _Time.y * 100;// * rand1dTo1d(_Time.y) * 50;
                // i.pos.z += _Time.y * 100;
            float3 randVar = floor(float3(i.pos.x, i.pos.y, 0)/_StarFieldCellSize);
            float3 random = rand3dTo3dBW(randVar);
            float4 randomCol = float4(random.xyz, 1);
            fixed4 starCol = step(randomCol, float4(_StarFieldDensity, _StarFieldDensity, _StarFieldDensity, _StarFieldDensity))
                * _StarFieldBrightness * rand3dTo1dBW(random);
            fixed4 col = tex2D(_MainTex, i.uv) + starCol;
                return col;
            }
            
            ENDCG
        }
    }
}
