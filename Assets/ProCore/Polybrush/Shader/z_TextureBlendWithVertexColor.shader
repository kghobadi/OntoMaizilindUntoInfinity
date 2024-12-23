// define Z_TEXTURE_CHANNELS 4
// define Z_MESH_ATTRIBUTES 
// Important!  This is a generated file, any changes will be overwritten
// when the _SfSrc suffixed version of this shader is modified.




Shader "Polybrush/Texture Blend With Vertex Color" {

    Properties {

        _Metallic ("Metallic", Range(0, 1)) = 0

        _Gloss ("Gloss", Range(0, 1)) = 0.8

        _Base ("Base", 2D) = "white" {}

        _Texture2 ("Texture 2", 2D) = "white" {}

        _Texture3 ("Texture 3", 2D) = "white" {}

        _Texture4 ("Texture 4", 2D) = "white" {}

        

    }

    SubShader {

        Tags {

            "RenderType"="Opaque"

        }

        Pass {

            Name "FORWARD"

            Tags {

                "LightMode"="ForwardBase"

            }

            

            

            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            #define UNITY_PASS_FORWARDBASE

            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )

            #define _GLOSSYENV 1

            #include "UnityCG.cginc"

            #include "AutoLight.cginc"

            #include "Lighting.cginc"

            #include "UnityPBSLighting.cginc"

            #include "UnityStandardBRDF.cginc"

            #pragma multi_compile_fwdbase_fullshadows

            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

            #pragma multi_compile_fog

            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 

            #pragma target 3.0

            uniform float _Metallic;

            uniform float _Gloss;

            uniform sampler2D _Base; uniform float4 _Base_ST;

            uniform sampler2D _Texture2; uniform float4 _Texture2_ST;

            uniform sampler2D _Texture3; uniform float4 _Texture3_ST;

            uniform sampler2D _Texture4; uniform float4 _Texture4_ST;

            

            struct VertexInput {

                float4 vertex : POSITION;

                float3 normal : NORMAL;

                float4 tangent : TANGENT;

                float4 texcoord0 : TEXCOORD0;

                float4 texcoord1 : TEXCOORD1;

                float4 texcoord2 : TEXCOORD2;

                float4 vertexColor : COLOR;

            };

            struct VertexOutput {

                float4 pos : SV_POSITION;

                float4 uv0 : TEXCOORD0;

                float4 uv1 : TEXCOORD1;

                float4 uv2 : TEXCOORD2;

                float4 posWorld : TEXCOORD3;

                float3 normalDir : TEXCOORD4;

                float3 tangentDir : TEXCOORD5;

                float3 bitangentDir : TEXCOORD6;

                float4 vertexColor : COLOR;

                LIGHTING_COORDS(7,8)

                UNITY_FOG_COORDS(9)

                #if defined(LIGHTMAP_ON) || defined(UNITY_SHOULD_SAMPLE_SH)

                    float4 ambientOrLightmapUV : TEXCOORD10;

                #endif

            };

            VertexOutput vert (VertexInput v) {

                VertexOutput o = (VertexOutput)0;

                o.uv0 = v.texcoord0;

                o.uv1 = v.texcoord1;

                o.uv2 = v.texcoord2;

                o.vertexColor = v.vertexColor;

                #ifdef LIGHTMAP_ON

                    o.ambientOrLightmapUV.xy = v.texcoord1.xy * unity_LightmapST.xy + unity_LightmapST.zw;

                    o.ambientOrLightmapUV.zw = 0;

                #elif UNITY_SHOULD_SAMPLE_SH

                #endif

                #ifdef DYNAMICLIGHTMAP_ON

                    o.ambientOrLightmapUV.zw = v.texcoord2.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;

                #endif

                o.normalDir = UnityObjectToWorldNormal(v.normal);

                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );

                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);

                o.posWorld = mul(unity_ObjectToWorld, v.vertex);

                float3 lightColor = _LightColor0.rgb;

                o.pos = UnityObjectToClipPos(v.vertex );

                UNITY_TRANSFER_FOG(o,o.pos);

                TRANSFER_VERTEX_TO_FRAGMENT(o)

                return o;

            }

            float4 frag(VertexOutput i) : COLOR {

                i.normalDir = normalize(i.normalDir);

                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);

                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

                float3 normalDirection = i.normalDir;

                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );

                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);

                float3 lightColor = _LightColor0.rgb;

                float3 halfDirection = normalize(viewDirection+lightDirection);

////// Lighting:

                float attenuation = LIGHT_ATTENUATION(i);

                float3 attenColor = attenuation * _LightColor0.xyz;

                float Pi = 3.141592654;

                float InvPi = 0.31830988618;

///////// Gloss:

                float gloss = _Gloss;

                float specPow = exp2( gloss * 10.0+1.0);

/////// GI Data:

                UnityLight light;

                #ifdef LIGHTMAP_OFF

                    light.color = lightColor;

                    light.dir = lightDirection;

                    light.ndotl = LambertTerm (normalDirection, light.dir);

                #else

                    light.color = half3(0.f, 0.f, 0.f);

                    light.ndotl = 0.0f;

                    light.dir = half3(0.f, 0.f, 0.f);

                #endif

                UnityGIInput d;

                d.light = light;

                d.worldPos = i.posWorld.xyz;

                d.worldViewDir = viewDirection;

                d.atten = attenuation;

                #if defined(LIGHTMAP_ON) || defined(DYNAMICLIGHTMAP_ON)

                    d.ambient = 0;

                    d.lightmapUV = i.ambientOrLightmapUV;

                #else

                    d.ambient = i.ambientOrLightmapUV;

                #endif

                d.boxMax[0] = unity_SpecCube0_BoxMax;

                d.boxMin[0] = unity_SpecCube0_BoxMin;

                d.probePosition[0] = unity_SpecCube0_ProbePosition;

                d.probeHDR[0] = unity_SpecCube0_HDR;

                d.boxMax[1] = unity_SpecCube1_BoxMax;

                d.boxMin[1] = unity_SpecCube1_BoxMin;

                d.probePosition[1] = unity_SpecCube1_ProbePosition;

                d.probeHDR[1] = unity_SpecCube1_HDR;

                Unity_GlossyEnvironmentData ugls_en_data;

                ugls_en_data.roughness = 1.0 - gloss;

                ugls_en_data.reflUVW = viewReflectDirection;

                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );

                lightDirection = gi.light.dir;

                lightColor = gi.light.color;

////// Specular:

                float NdotL = max(0, dot( normalDirection, lightDirection ));

                float LdotH = max(0.0,dot(lightDirection, halfDirection));

                float4 node_8334 = normalize(float4(i.uv2.r,i.uv2.g,i.uv2.b,i.uv2.a));

                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));

                float4 _Texture2_var = tex2D(_Texture2,TRANSFORM_TEX(i.uv0, _Texture2));

                float4 _Texture3_var = tex2D(_Texture3,TRANSFORM_TEX(i.uv0, _Texture3));

                float4 _Texture4_var = tex2D(_Texture4,TRANSFORM_TEX(i.uv0, _Texture4));

                float3 diffuseColor = ((lerp( lerp( lerp( lerp( _Base_var.rgb, _Base_var.rgb, node_8334.r ), _Texture2_var.rgb, node_8334.g ), _Texture3_var.rgb, node_8334.b ), _Texture4_var.rgb, node_8334.a ))*i.vertexColor.rgb); // Need this for specular when using metallic

                float specularMonochrome;

                float3 specularColor;

                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, _Metallic, specularColor, specularMonochrome );

                specularMonochrome = 1-specularMonochrome;

                float NdotV = max(0.0,dot( normalDirection, viewDirection ));

                float NdotH = max(0.0,dot( normalDirection, halfDirection ));

                float VdotH = max(0.0,dot( viewDirection, halfDirection ));

                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );

                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));

                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );

                float3 directSpecular = 1 * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);

                half grazingTerm = saturate( gloss + specularMonochrome );

                float3 indirectSpecular = (gi.indirect.specular);

                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);

                float3 specular = (directSpecular + indirectSpecular);

/////// Diffuse:

                NdotL = max(0.0,dot( normalDirection, lightDirection ));

                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);

                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;

                float3 indirectDiffuse = float3(0,0,0);

                indirectDiffuse += gi.indirect.diffuse;

                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;

/// Final Color:

                float3 finalColor = diffuse + specular;

                fixed4 finalRGBA = fixed4(finalColor,1);

                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;

            }

            ENDCG

        }

        Pass {

            Name "FORWARD_DELTA"

            Tags {

                "LightMode"="ForwardAdd"

            }

            Blend One One

            

            

            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            #define UNITY_PASS_FORWARDADD

            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )

            #define _GLOSSYENV 1

            #include "UnityCG.cginc"

            #include "AutoLight.cginc"

            #include "Lighting.cginc"

            #include "UnityPBSLighting.cginc"

            #include "UnityStandardBRDF.cginc"

            #pragma multi_compile_fwdadd_fullshadows

            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

            #pragma multi_compile_fog

            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 

            #pragma target 3.0

            uniform float _Metallic;

            uniform float _Gloss;

            uniform sampler2D _Base; uniform float4 _Base_ST;

            uniform sampler2D _Texture2; uniform float4 _Texture2_ST;

            uniform sampler2D _Texture3; uniform float4 _Texture3_ST;

            uniform sampler2D _Texture4; uniform float4 _Texture4_ST;

            

            struct VertexInput {

                float4 vertex : POSITION;

                float3 normal : NORMAL;

                float4 tangent : TANGENT;

                float4 texcoord0 : TEXCOORD0;

                float4 texcoord1 : TEXCOORD1;

                float4 texcoord2 : TEXCOORD2;

                float4 vertexColor : COLOR;

            };

            struct VertexOutput {

                float4 pos : SV_POSITION;

                float4 uv0 : TEXCOORD0;

                float4 uv1 : TEXCOORD1;

                float4 uv2 : TEXCOORD2;

                float4 posWorld : TEXCOORD3;

                float3 normalDir : TEXCOORD4;

                float3 tangentDir : TEXCOORD5;

                float3 bitangentDir : TEXCOORD6;

                float4 vertexColor : COLOR;

                LIGHTING_COORDS(7,8)

                UNITY_FOG_COORDS(9)

            };

            VertexOutput vert (VertexInput v) {

                VertexOutput o = (VertexOutput)0;

                o.uv0 = v.texcoord0;

                o.uv1 = v.texcoord1;

                o.uv2 = v.texcoord2;

                o.vertexColor = v.vertexColor;

                o.normalDir = UnityObjectToWorldNormal(v.normal);

                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );

                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);

                o.posWorld = mul(unity_ObjectToWorld, v.vertex);

                float3 lightColor = _LightColor0.rgb;

                o.pos = UnityObjectToClipPos(v.vertex );

                UNITY_TRANSFER_FOG(o,o.pos);

                TRANSFER_VERTEX_TO_FRAGMENT(o)

                return o;

            }

            float4 frag(VertexOutput i) : COLOR {

                i.normalDir = normalize(i.normalDir);

                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);

                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

                float3 normalDirection = i.normalDir;

                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));

                float3 lightColor = _LightColor0.rgb;

                float3 halfDirection = normalize(viewDirection+lightDirection);

////// Lighting:

                float attenuation = LIGHT_ATTENUATION(i);

                float3 attenColor = attenuation * _LightColor0.xyz;

                float Pi = 3.141592654;

                float InvPi = 0.31830988618;

///////// Gloss:

                float gloss = _Gloss;

                float specPow = exp2( gloss * 10.0+1.0);

////// Specular:

                float NdotL = max(0, dot( normalDirection, lightDirection ));

                float LdotH = max(0.0,dot(lightDirection, halfDirection));

                float4 node_8334 = normalize(float4(i.uv2.r,i.uv2.g,i.uv2.b,i.uv2.a));

                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));

                float4 _Texture2_var = tex2D(_Texture2,TRANSFORM_TEX(i.uv0, _Texture2));

                float4 _Texture3_var = tex2D(_Texture3,TRANSFORM_TEX(i.uv0, _Texture3));

                float4 _Texture4_var = tex2D(_Texture4,TRANSFORM_TEX(i.uv0, _Texture4));

                float3 diffuseColor = ((lerp( lerp( lerp( lerp( _Base_var.rgb, _Base_var.rgb, node_8334.r ), _Texture2_var.rgb, node_8334.g ), _Texture3_var.rgb, node_8334.b ), _Texture4_var.rgb, node_8334.a ))*i.vertexColor.rgb); // Need this for specular when using metallic

                float specularMonochrome;

                float3 specularColor;

                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, _Metallic, specularColor, specularMonochrome );

                specularMonochrome = 1-specularMonochrome;

                float NdotV = max(0.0,dot( normalDirection, viewDirection ));

                float NdotH = max(0.0,dot( normalDirection, halfDirection ));

                float VdotH = max(0.0,dot( viewDirection, halfDirection ));

                float visTerm = SmithBeckmannVisibilityTerm( NdotL, NdotV, 1.0-gloss );

                float normTerm = max(0.0, NDFBlinnPhongNormalizedTerm(NdotH, RoughnessToSpecPower(1.0-gloss)));

                float specularPBL = max(0, (NdotL*visTerm*normTerm) * (UNITY_PI / 4) );

                float3 directSpecular = attenColor * pow(max(0,dot(halfDirection,normalDirection)),specPow)*specularPBL*lightColor*FresnelTerm(specularColor, LdotH);

                float3 specular = directSpecular;

/////// Diffuse:

                NdotL = max(0.0,dot( normalDirection, lightDirection ));

                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);

                float3 directDiffuse = ((1 +(fd90 - 1)*pow((1.00001-NdotL), 5)) * (1 + (fd90 - 1)*pow((1.00001-NdotV), 5)) * NdotL) * attenColor;

                float3 diffuse = directDiffuse * diffuseColor;

/// Final Color:

                float3 finalColor = diffuse + specular;

                fixed4 finalRGBA = fixed4(finalColor * 1,0);

                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;

            }

            ENDCG

        }

        Pass {

            Name "Meta"

            Tags {

                "LightMode"="Meta"

            }

            Cull Off

            

            CGPROGRAM

            #pragma vertex vert

            #pragma fragment frag

            #define UNITY_PASS_META 1

            #define SHOULD_SAMPLE_SH ( defined (LIGHTMAP_OFF) && defined(DYNAMICLIGHTMAP_OFF) )

            #define _GLOSSYENV 1

            #include "UnityCG.cginc"

            #include "Lighting.cginc"

            #include "UnityPBSLighting.cginc"

            #include "UnityStandardBRDF.cginc"

            #include "UnityMetaPass.cginc"

            #pragma fragmentoption ARB_precision_hint_fastest

            #pragma multi_compile_shadowcaster

            #pragma multi_compile LIGHTMAP_OFF LIGHTMAP_ON

            #pragma multi_compile DIRLIGHTMAP_OFF DIRLIGHTMAP_COMBINED DIRLIGHTMAP_SEPARATE

            #pragma multi_compile DYNAMICLIGHTMAP_OFF DYNAMICLIGHTMAP_ON

            #pragma multi_compile_fog

            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 

            #pragma target 3.0

            uniform float _Metallic;

            uniform float _Gloss;

            uniform sampler2D _Base; uniform float4 _Base_ST;

            uniform sampler2D _Texture2; uniform float4 _Texture2_ST;

            uniform sampler2D _Texture3; uniform float4 _Texture3_ST;

            uniform sampler2D _Texture4; uniform float4 _Texture4_ST;

            

            struct VertexInput {

                float4 vertex : POSITION;

                float4 texcoord0 : TEXCOORD0;

                float4 texcoord1 : TEXCOORD1;

                float4 texcoord2 : TEXCOORD2;

                float4 vertexColor : COLOR;

            };

            struct VertexOutput {

                float4 pos : SV_POSITION;

                float4 uv0 : TEXCOORD0;

                float4 uv1 : TEXCOORD1;

                float4 uv2 : TEXCOORD2;

                float4 posWorld : TEXCOORD3;

                float4 vertexColor : COLOR;

            };

            VertexOutput vert (VertexInput v) {

                VertexOutput o = (VertexOutput)0;

                o.uv0 = v.texcoord0;

                o.uv1 = v.texcoord1;

                o.uv2 = v.texcoord2;

                o.vertexColor = v.vertexColor;

                o.posWorld = mul(unity_ObjectToWorld, v.vertex);

                o.pos = UnityMetaVertexPosition(v.vertex, v.texcoord1.xy, v.texcoord2.xy, unity_LightmapST, unity_DynamicLightmapST );

                return o;

            }

            float4 frag(VertexOutput i) : SV_Target {

                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);

                UnityMetaInput o;

                UNITY_INITIALIZE_OUTPUT( UnityMetaInput, o );

                

                o.Emission = 0;

                

                float4 node_8334 = normalize(float4(i.uv2.r,i.uv2.g,i.uv2.b,i.uv2.a));

                float4 _Base_var = tex2D(_Base,TRANSFORM_TEX(i.uv0, _Base));

                float4 _Texture2_var = tex2D(_Texture2,TRANSFORM_TEX(i.uv0, _Texture2));

                float4 _Texture3_var = tex2D(_Texture3,TRANSFORM_TEX(i.uv0, _Texture3));

                float4 _Texture4_var = tex2D(_Texture4,TRANSFORM_TEX(i.uv0, _Texture4));

                float3 diffColor = ((lerp( lerp( lerp( lerp( _Base_var.rgb, _Base_var.rgb, node_8334.r ), _Texture2_var.rgb, node_8334.g ), _Texture3_var.rgb, node_8334.b ), _Texture4_var.rgb, node_8334.a ))*i.vertexColor.rgb);

                float specularMonochrome;

                float3 specColor;

                diffColor = DiffuseAndSpecularFromMetallic( diffColor, _Metallic, specColor, specularMonochrome );

                float roughness = 1.0 - _Gloss;

                o.Albedo = diffColor + specColor * roughness * roughness * 0.5;

                

                return UnityMetaFragment( o );

            }

            ENDCG

        }

    }

    FallBack "Standard"

    CustomEditor "z_BlendMaterialInspector"

}

