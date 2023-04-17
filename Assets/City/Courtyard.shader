// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/Courtyard"
{
	Properties
	{
		_Color1("Color1", Color) = (0,0,0,0)
		_Color0("Color 0", Color) = (1,1,1,0)
		_Color2("Color2", Color) = (0,0,0,0)
		[NoScaleOffset]_Texture1("Texture1", 2D) = "white" {}
		_Tex1TilingOffset("Tex1Tiling&Offset", Vector) = (0.5,0.5,1,1)
		_MaskSoftness("MaskSoftness", Range( 0 , 1)) = 0
		_MaskRange("MaskRange", Range( 0.4 , 1)) = 0.5
		_MaskRangeMin("MaskRangeMin", Range( 0.1 , 1)) = 0.4
		[Toggle]_Use2Textures("Use2Textures", Float) = 0
		_LightSteps("LightSteps", Float) = 4
		_Vector0("Vector 0", Vector) = (0.5,0.5,1,1)
		[NoScaleOffset]_Texture0("Texture 0", 2D) = "white" {}
		_BackgroundColor("BackgroundColor", Color) = (0.08677466,0.2332488,0.4716981,0)
		[HDR]_AmbientColor("AmbientColor", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldNormal;
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float4 _BackgroundColor;
		uniform float _LightSteps;
		uniform float4 _Color1;
		uniform sampler2D _Texture1;
		uniform float4 _Tex1TilingOffset;
		uniform float _Use2Textures;
		uniform sampler2D _Texture0;
		uniform float4 _Vector0;
		uniform float4 _Color2;
		uniform float _MaskRangeMin;
		uniform float _MaskRange;
		uniform float _MaskSoftness;
		uniform float4 _AmbientColor;
		uniform float4 _Color0;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult6 = dot( ase_worldNormal , ase_worldlightDir );
			float4 temp_cast_0 = (saturate( dotResult6 )).xxxx;
			float lightSteps171 = _LightSteps;
			float temp_output_3_0_g23 = lightSteps171;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_cast_1 = (ase_lightAtten).xxxx;
			float temp_output_3_0_g22 = lightSteps171;
			float4 temp_output_26_0 = CalculateContrast(4.0,( ( trunc( ( temp_cast_0 * temp_output_3_0_g23 ) ) / temp_output_3_0_g23 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_1 * temp_output_3_0_g22 ) ) / temp_output_3_0_g22 ) ) ));
			float4 lightIntensity213 = temp_output_26_0;
			float4 lerpResult211 = lerp( float4( 0,0,0,0 ) , _BackgroundColor , ( lightIntensity213 + float4( 0.3,0.3,0.3,0 ) ));
			float2 appendResult90 = (float2(_Tex1TilingOffset.x , _Tex1TilingOffset.y));
			float2 appendResult91 = (float2(_Tex1TilingOffset.z , _Tex1TilingOffset.w));
			float2 uv_TexCoord51 = i.uv_texcoord * appendResult90 + ( _Time.x * appendResult91 );
			float2 appendResult130 = (float2(_Vector0.x , _Vector0.y));
			float2 appendResult128 = (float2(_Vector0.z , _Vector0.w));
			float2 uv_TexCoord132 = i.uv_texcoord * appendResult130 + ( _Time.x * appendResult128 );
			float lerpResult150 = lerp( _MaskRangeMin , _MaskRange , _SinTime.w);
			float temp_output_97_0 = saturate( ( 1.0 - ( ( distance( _Color1.rgb , ( ( tex2D( _Texture1, uv_TexCoord51 ) * _Color1 ) + (( _Use2Textures )?( ( tex2D( _Texture0, uv_TexCoord132 ) * _Color2 ) ):( float4( 0,0,0,0 ) )) ).rgb ) - lerpResult150 ) / max( _MaskSoftness , 1E-05 ) ) ) );
			float mainTex85 = temp_output_97_0;
			float4 temp_cast_5 = (ase_lightAtten).xxxx;
			float temp_output_3_0_g24 = lightSteps171;
			float4 lerpResult29 = lerp( ( ase_lightColor * ( trunc( ( temp_cast_5 * temp_output_3_0_g24 ) ) / temp_output_3_0_g24 ) ) , float4( 0,0,0,0 ) , saturate( ( 1.0 - temp_output_26_0 ) ));
			float4 light73 = lerpResult29;
			float4 lerpResult205 = lerp( _AmbientColor , light73 , light73);
			float4 lerpResult212 = lerp( lerpResult211 , ( mainTex85 * lerpResult205 * _Color0 ) , ceil( mainTex85 ));
			c.rgb = lerpResult212.rgb;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 worldNormal : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
-57;499;1906;989;1351.266;480.8927;1.837588;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-2927.886,1092.672;Inherit;False;2621.701;1098.188;Comment;29;73;35;29;37;34;36;33;31;26;9;7;8;14;13;6;5;4;170;171;182;186;189;191;192;193;194;195;196;213;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1675.505,2057.035;Inherit;False;Property;_LightSteps;LightSteps;10;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-2781.667,1514.906;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldNormalVector;5;-2776.391,1351.895;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;171;-1485.535,2060.283;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-2486.407,1457.125;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;126;-1082.698,-976.2512;Inherit;False;Property;_Vector0;Vector 0;11;0;Create;True;0;0;0;False;0;False;0.5,0.5,1,1;1,0,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;14;-2820.911,1961.251;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-2593.492,2078.796;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;193;-2454.546,1959.281;Inherit;False;PosterizeSteps;-1;;22;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;127;-1087.032,-1146.91;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;8;-2267.923,1442.796;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;89;-1101.271,-1345.145;Inherit;False;Property;_Tex1TilingOffset;Tex1Tiling&Offset;5;0;Create;True;0;0;0;False;0;False;0.5,0.5,1,1;3,0.986,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;194;-2446.552,1257.844;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;13;-2721.591,1699.999;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;128;-837.4869,-996.4802;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2447.371,1698.146;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-881.4902,-1363.255;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;130;-839.4847,-905.938;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;129;-807.5314,-1113.953;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;52;-1081.506,-1512.152;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;182;-2192.709,1194.103;Inherit;False;PosterizeSteps;-1;;23;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;132;-652.5879,-1073.809;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;90;-858.058,-1274.832;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-808.8126,-1493.265;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;131;-647.8299,-1277.575;Inherit;True;Property;_Texture0;Texture 0;12;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;5c0d01a670cd56949954f39d01862a2d;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2076.835,1542.774;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-647.0625,-1439.051;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;134;-418.8086,-1118.416;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;61;None;e49c251a37bfe8f4b97a7bcff572613c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;61;-659.2667,-1676.458;Inherit;True;Property;_Texture1;Texture1;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;7e4dc6302050dc34fa03aa4a810240e4;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SimpleContrastOpNode;26;-1831.882,1556.26;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;142;-282.0769,-922.0991;Inherit;False;Property;_Color2;Color2;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.1,0.1,0.1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;36;-1956.974,1369.341;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;-1752.188,1471.135;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;53;-362.3328,-1312.815;Inherit;False;Property;_Color1;Color1;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;46;-430.3256,-1494.227;Inherit;True;Property;_texture1;texture1;0;0;Create;True;0;0;0;False;0;False;131;None;e49c251a37bfe8f4b97a7bcff572613c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;189;-1589.478,1405.697;Inherit;False;PosterizeSteps;-1;;24;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightColorNode;31;-1872.808,1181.614;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;143;-6.612587,-1052.215;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;33;-1307.416,1560.812;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-117.5646,-1382.593;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;34;-1193.019,1425.861;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-46.17177,-1815.854;Inherit;False;Property;_MaskRangeMin;MaskRangeMin;8;0;Create;True;0;0;0;False;0;False;0.4;0.48;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;140;107.6322,-1183.926;Inherit;False;Property;_Use2Textures;Use2Textures;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;COLOR;0,0,0,0;False;1;COLOR;1,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1420.773,1181.053;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;103;84.4082,-1496.501;Inherit;False;Property;_MaskRange;MaskRange;7;0;Create;True;0;0;0;False;0;False;0.5;0.7;0.4;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;147;-29.17243,-1711.5;Inherit;True;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;150;250.8275,-1732.5;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;29;-1025.792,1398.625;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;84;321.1738,-1272.162;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;214;164.8539,-1359.435;Inherit;False;Property;_MaskSoftness;MaskSoftness;6;0;Create;True;0;0;0;False;0;False;0;0.2;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;213;-1557.201,1735.831;Inherit;False;lightIntensity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-498.7442,1433.14;Inherit;True;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;97;461.6801,-1510.629;Inherit;True;Color Mask;-1;;25;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;1,1,1;False;4;FLOAT;0.7;False;5;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;207;-475.4485,560.5794;Inherit;False;Property;_AmbientColor;AmbientColor;14;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0.6037736,0.3097896,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;208;4.34058,-164.7808;Inherit;False;213;lightIntensity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-285.9496,332.7011;Inherit;True;73;light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;1207.623,-1535.734;Inherit;False;mainTex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;202;-5.486389,291.3871;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.754717,0.5312355,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;209;224.0291,-197.4832;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.3,0.3,0.3,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;210;27.96756,-392.9265;Inherit;False;Property;_BackgroundColor;BackgroundColor;13;0;Create;True;0;0;0;False;0;False;0.08677466,0.2332488,0.4716981,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;205;-16.09418,526.1473;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;1.004974,43.33966;Inherit;True;85;mainTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;123;450.1562,62.94505;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;211;572.3193,-216.2222;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.1792453,0.1792453,0.1792453,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;281.3975,299.4325;Inherit;True;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;35;-703.0432,1593.904;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;206;-239.1918,591.3192;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;186;-739.949,1302.53;Inherit;False;PosterizeSteps;-1;;26;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;788.4028,-1332.154;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;13.02,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;192;-2598.927,1925.739;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;212;872.1049,133.2638;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;1055.559,-1233.553;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;191;-1730.933,1399.953;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;144;567.0901,-1262.239;Inherit;False;Property;_EmissionColor;EmissionColor;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1246.508,-9.205021;Float;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Julian/Courtyard;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;True;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.05;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;171;0;170;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;193;1;14;0
WireConnection;193;3;195;0
WireConnection;8;0;6;0
WireConnection;128;0;126;3
WireConnection;128;1;126;4
WireConnection;7;0;13;2
WireConnection;7;1;193;0
WireConnection;91;0;89;3
WireConnection;91;1;89;4
WireConnection;130;0;126;1
WireConnection;130;1;126;2
WireConnection;129;0;127;1
WireConnection;129;1;128;0
WireConnection;182;1;8;0
WireConnection;182;3;194;0
WireConnection;132;0;130;0
WireConnection;132;1;129;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;55;0;52;1
WireConnection;55;1;91;0
WireConnection;9;0;182;0
WireConnection;9;1;7;0
WireConnection;51;0;90;0
WireConnection;51;1;55;0
WireConnection;134;0;131;0
WireConnection;134;1;132;0
WireConnection;26;1;9;0
WireConnection;46;0;61;0
WireConnection;46;1;51;0
WireConnection;189;1;36;0
WireConnection;189;3;196;0
WireConnection;143;0;134;0
WireConnection;143;1;142;0
WireConnection;33;0;26;0
WireConnection;58;0;46;0
WireConnection;58;1;53;0
WireConnection;34;0;33;0
WireConnection;140;1;143;0
WireConnection;37;0;31;0
WireConnection;37;1;189;0
WireConnection;150;0;161;0
WireConnection;150;1;103;0
WireConnection;150;2;147;4
WireConnection;29;0;37;0
WireConnection;29;2;34;0
WireConnection;84;0;58;0
WireConnection;84;1;140;0
WireConnection;213;0;26;0
WireConnection;73;0;29;0
WireConnection;97;1;84;0
WireConnection;97;3;53;0
WireConnection;97;4;150;0
WireConnection;97;5;214;0
WireConnection;85;0;97;0
WireConnection;209;0;208;0
WireConnection;205;0;207;0
WireConnection;205;1;74;0
WireConnection;205;2;74;0
WireConnection;123;0;86;0
WireConnection;211;1;210;0
WireConnection;211;2;209;0
WireConnection;49;0;86;0
WireConnection;49;1;205;0
WireConnection;49;2;202;0
WireConnection;35;0;29;0
WireConnection;186;1;29;0
WireConnection;98;0;97;0
WireConnection;98;1;144;0
WireConnection;192;0;14;0
WireConnection;212;0;211;0
WireConnection;212;1;49;0
WireConnection;212;2;123;0
WireConnection;102;0;98;0
WireConnection;191;0;36;0
WireConnection;0;13;212;0
ASEEND*/
//CHKSM=33E184A9192924985FD1730C5CC21BE77A87C6EF