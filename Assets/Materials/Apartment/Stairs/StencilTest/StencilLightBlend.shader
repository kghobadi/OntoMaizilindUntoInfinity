Shader "StencLightBlend" {
    Properties {
        _Color ("Main Color", Color) = (0.5,0.5,0.5,0)
        [HideInInspector]_StencilLayer("Stencil layer", int) = 1
        [HideInInspector]_StencilLayerPlusOne("Stencil Layer + 1", int ) = 2

        [Header(Gradient)]
        [Toggle(_GRADIENT)]_Toggle ("Enabled", Float) = 0
        [Gradient]_GradientMap("Gradient", 2D) = "white" {}
        [KeywordEnum(World Space, Local Space)] _GradientSpace("Gradient Space", Float) = 1
        _GradientYStartPos_F ("Gradient start Y", Float) = 0
        _GradientHeight_F("Gradient Height", Float) = 1
    }
    SubShader {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "DisableBatching"="True" "StencilVolume" = "True"}

         ZWrite off
       
        CGINCLUDE
        #include "UnityCG.cginc"
            uniform half4 _Color1_F;                 			
            uniform half4 _Color2_F;
            uniform half _GradientYStartPos_F;
            uniform half _GradientHeight_F;
            uniform fixed _GradientSpace;
            uniform fixed _GradientType;
            uniform sampler2D _GradientMap;

            struct appdata {
                float4 vertex : POSITION;
            };
            struct v2f {
                float4 pos : SV_POSITION;
                #if _GRADIENT
                half4 colorFront : COLOR0;
                #endif
            };
            v2f vert(appdata v) {
                v2f o;
                o.pos =  UnityObjectToClipPos(v.vertex);
                #if _GRADIENT
                float4 worldPos = mul(unity_ObjectToWorld, v.vertex);
                float4 GradPos = lerp(worldPos, v.vertex, _GradientSpace);

                half RotatedGrad_F = (GradPos.y - _GradientYStartPos_F);
                half GradientFactor_F = saturate(RotatedGrad_F / -_GradientHeight_F);

                half GradientFactor_radial = saturate(length(GradPos) / _GradientHeight_F);

                GradientFactor_F = lerp(GradientFactor_F, GradientFactor_radial, _GradientType);
                //o.colorFront = lerp(_Color1_F, _Color2_F, GradientFactor_F);
                o.colorFront.x = GradientFactor_F;
                #endif
                return o;
            }
        ENDCG

        Pass {

            Stencil {
                   // writes back faces to a stencil buffer
                Ref [_StencilLayer]
                Comp always
                Pass replace
            }
            colormask 0
            Cull Front
            ZTest greater
            ZWrite off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            half4 frag(v2f i) : SV_Target {
                return half4(0.2,0.2,0,1);
            }
            ENDCG
        }
        Pass {
            Stencil {
                // where front faces match back faces incriment the stencil buffer
                Ref [_StencilLayer]
                Comp Equal
                Pass IncrSat
            }
            colormask 0
            Cull Back
            ZTest less
            ZWrite off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

               half4 frag(v2f i) : SV_Target {
                return half4(0,0.2,0.2,1);
            }
            ENDCG
        }

        Pass
        {
            ColorMask RGB
            Cull Front
            ZTest Always
            Stencil {
                // only renders pixels that made it past the second phase
                Ref [_StencilLayerPlusOne]
                // should really work out how to do ^ programatically
                // can't seem to do any math in the ref line
                Comp equal 
                Pass zero
                Fail zero
            }

            // max blending
            // personally I don't want doubling up of differnt light bands
            // BlendOP Max
            // Blend One One
            //Blend SrcAlpha OneMinusSrcAlpha
            //Blend DstColor Zero // multiply
            Blend DstColor SrcColor // 2x multiplicative
            ZWrite off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma shader_feature _GRADIENT
            


            float4 _Color;
            
            fixed4 frag (v2f i) : SV_Target
            {
                #if _GRADIENT
                return tex2D( _GradientMap, half2(i.colorFront.x, 0.5));
                //return i.colorFront.xxxw;
                #endif
                return _Color;
            }
            ENDCG
        }

        // only used if inside the volume
        Pass {
            Stencil {
                // where front faces match back faces incriment the stencil buffer
                Ref [_StencilLayer]
                Comp Equal
                Pass IncrSat
            }
            colormask 0
            Cull Off
            ZTest off
            ZWrite off
        
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

               half4 frag(v2f i) : SV_Target {
                return half4(0,0.2,0.2,1);
            }
            ENDCG
        }
    } 
}  