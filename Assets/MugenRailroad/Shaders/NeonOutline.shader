Shader "MugenRailroad/NeonOutline"
{
    Properties
    {
        _MainTex ("Base (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,1,1,1)
        _OutlineWidth ("Outline Width", Float) = 0.03
        _Progress ("Wireframe Progress", Range(0,1)) = 0

        _GlowRamp ("Glow Ramp (Texture)", 2D) = "white" {}
		[PerRendererData]_Glow("Glow", Range( 0 , 1)) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

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
            uniform float _Glow;
            uniform float _MaxGlow;

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

        // Surface Shader para el cuerpo
        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _GlowRamp;

        fixed4 _Color;
        float _Glow;

        struct Input
        {
            float2 uv_MainTex;
            float3 viewDir; // dirección de la cámara (Unity lo calcula automáticamente)
            float3 worldNormal; // para dot(viewDir, normal)
        };

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseColor = tex2D(_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = baseColor.rgb;
            o.Alpha = baseColor.a;

            // Cálculo del glow usando la rampa
            float facing = saturate(dot(normalize(IN.viewDir), normalize(IN.worldNormal)));
            fixed4 glow = tex2D(_GlowRamp, float2(facing, 0.5)) * _Glow;

            o.Emission = glow.rgb;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
