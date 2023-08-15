Shader "Unlit/SDF"
{
    Properties
    {
         _MainTex ("MainTexture", 2D) = "white" { }
         _Color ("Color", Color) = (1.000000,1.000000,1.000000,0.000000)
         _Threshold ("Threshold", Range(0.000000,1.000000)) = 0.500000
         _Margin ("Margin", Range(0.000000,1.000000)) = 0.100000
         // _NegThreshold ("Neg Threshold", Range(0.000000,1.000000)) = 0.400000
    }
    SubShader
    {
         Tags {
             "Queue"="Transparent" 
             "IgnoreProjector"="True" 
             "RenderType"="Transparent"
         }
         LOD 100
         
         ZWrite Off
         Blend SrcAlpha OneMinusSrcAlpha 

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            float4 _Color;
            float _Threshold;  
            float _Margin;
            // float _NegThreshold;

            v2f vert (appdata v)
            {
                v2f o;
                
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                o.color = _Color * v.color;
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 sdf = tex2D(_MainTex, i.uv);
                
                float lower = _Threshold - _Margin; 
                float upper = _Threshold + _Margin;
                
                if (sdf.r < lower)
                {
                    discard; 
                }
                
                fixed4 output = i.color;
                
                output.a *= smoothstep(lower, upper, sdf.r);
                
                return output;
            }
            ENDCG
        }
    }
}
