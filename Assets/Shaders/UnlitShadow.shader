Shader "Unlit/UnlitShadow"
{
    Properties{
        _Color("Tint", Color) = (0, 0, 0, 1)
        _MainTex("Texture", 2D) = "white" {}
        [HDR] _Emission("Emission", color) = (0,0,0)

        _Ramp("Toon Ramp", 2D) = "white" {}
    }
        SubShader{
        //the material is completely non-transparent and is rendered at the same time as the other opaque geometry
        Tags{ "RenderType" = "Opaque" "Queue" = "Geometry"}

        CGPROGRAM

        //the shader is a surface shader, meaning that it will be extended by unity in the background to have fancy lighting and other features
        //our surface shader function is called surf and we use our custom lighting model
        //fullforwardshadows makes sure unity adds the shadow passes the shader might need
        #pragma surface surf Custom fullforwardshadows
        #pragma target 3.0

        sampler2D _MainTex;
        fixed4 _Color;
        half3 _Emission;

        sampler2D _Ramp;

        //our lighting function. Will be called once per light
        float4 LightingCustom(SurfaceOutput s, float3 lightDir, float atten) {
            //how much does the normal point towards the light?
            float towardsLight = dot(s.Normal, lightDir);
            //remap the value from -1 to 1 to between 0 and 1
            towardsLight = towardsLight * 0.5 + 0.5;

            //read from toon ramp
            float3 lightIntensity = tex2D(_Ramp, towardsLight).rgb;

            //combine the color
            float4 col;
            //intensity we calculated previously, diffuse color, light falloff and shadowcasting, color of the light
            col.rgb = lightIntensity * s.Albedo * atten * _LightColor0.rgb;
            //in case we want to make the shader transparent in the future - irrelevant right now
            col.a = s.Alpha;

            return col;
        }

        //input struct which is automatically filled by unity
        struct Input {
            float2 uv_MainTex;
        };

        //the surface shader function which sets parameters the lighting function then uses
        void surf(Input i, inout SurfaceOutput o) {
            //sample and tint albedo texture
            fixed4 col = tex2D(_MainTex, i.uv_MainTex);
            col *= _Color;
            o.Albedo = col.rgb;

            //o.Emission = _Emission;
        }
        ENDCG

    }

        Subshader
        {
            Tags
            {
                "RenderType" = ""
                "Queue" = "Geometry"
                "PerformanceChecks" = "False"
            }

            Pass
            {
                Name "Diffuse"
                Tags{ "LightMode" = "ForwardBase" }


                CGPROGRAM
                    #pragma multi_compile_fwdadd_fullshadows addshadow
                    #pragma vertex Vert
                    #pragma fragment Frag
                    #include "UnityCG.cginc"
                    #include "AutoLight.cginc"

                    uniform half4 _Color;
                    uniform sampler2D _MainTex;
                    uniform half4 _MainTex_ST;

                    struct V2f {
                        half4 pos : SV_POSITION;
                        half2 uv : TEXCOORD0;
                        SHADOW_COORDS(1)
                    };

                    V2f Vert(appdata_base v)
                    {
                        V2f o;
                        o.pos = UnityObjectToClipPos(v.vertex);
                        o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);

                        TRANSFER_SHADOW(o);

                        return o;
                    }

                    half4 Frag(V2f i) : SV_Target
                    {
                        half4 tex = tex2D(_MainTex, i.uv) * _Color;

                        return tex * SHADOW_ATTENUATION(i);
                    }


                ENDCG
            }

            Pass
            {
                Name "ShadowCaster"
                Tags
                {
                    "LightMode" = "ShadowCaster"
                    "IgnoreProjector" = "True"
                }

                ZWrite On

                CGPROGRAM
                    #pragma target 3.0

                    #pragma shader_feature _ALPHAPREMULTIPLY_ON
                    #pragma multi_compile_shadowcaster
                    #pragma multi_compile_fwdadd_fullshadows
                    #pragma vertex vertShadowCaster
                    #pragma fragment fragShadowCaster

                    #include "UnityStandardShadow.cginc"

                ENDCG
            }
        }
        FallBack "Standard"
}
