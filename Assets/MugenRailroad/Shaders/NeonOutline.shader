Shader "MugenRailroad/NeonOutlineUnlit"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
        _Progress ("Wireframe Progress", Range(0,1)) = 0

        _GlowRamp ("Glow Ramp (Texture)", 2D) = "white" {}
        [PerRendererData]_Glow("Glow", Range(0,1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Lighting Off
        Cull Back
        ZWrite On
        Fog { Mode Off }

        // --- Pass 1: Outline ---
        Pass
        {
            Name "OUTLINE"
            Cull Front
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _Progress;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                float3 worldNormal = UnityObjectToWorldNormal(v.normal);
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float distance = length(_WorldSpaceCameraPos - worldPos);
                float outlineScale = saturate(distance / 5.0);
                float3 offset = worldNormal * _OutlineWidth * outlineScale;

                float4 pos = v.vertex;
                pos.xyz += mul(unity_WorldToObject, float4(offset, 0)).xyz;
                o.pos = UnityObjectToClipPos(pos);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                if (i.uv.x > _Progress)
                    discard;
                return _OutlineColor;
            }
            ENDCG
        }

        // --- Pass 2: Main Body without lighting ---
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            sampler2D _GlowRamp;
            fixed4 _Color;
            float _Glow;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 viewDir : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                o.viewDir = normalize(_WorldSpaceCameraPos - worldPos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.uv) * _Color;

                // Emisión simulada sin luz
                float facing = saturate(dot(normalize(i.viewDir), normalize(i.normal)));
                fixed4 glow = tex2D(_GlowRamp, float2(facing, 0.5)) * _Glow;

                return baseCol + glow;
            }
            ENDCG
        }
    }

    FallBack Off
}
