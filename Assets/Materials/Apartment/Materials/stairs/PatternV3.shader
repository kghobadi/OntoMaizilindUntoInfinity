// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/PatternV3"
{
	Properties
	{
		_Color2("Color1", Color) = (0,0,0,0)
		_Color3("Color2", Color) = (0,0,0,0)
		[NoScaleOffset]_Texture2("Texture1", 2D) = "white" {}
		_Tex1TilingOffset1("Tex1Tiling&Offset", Vector) = (0.5,0.5,1,1)
		_Tex2TilingOffset1("Tex2Tiling&Offset", Vector) = (0.5,0.5,1,1)
		_MaskRange1("MaskRange", Range( 0.1 , 1)) = 0.5
		[Toggle]_UseLight("UseLight", Float) = 1
		[NoScaleOffset]_Texture3("Texture2", 2D) = "white" {}
		_LightStrength("LightStrength", Range( 0 , 10)) = 4
		_LightSteps("LightSteps", Float) = 4
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			half3 worldNormal;
			float3 worldPos;
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

		uniform half _UseLight;
		uniform half4 _Color2;
		uniform sampler2D _Texture2;
		uniform half4 _Tex1TilingOffset1;
		uniform sampler2D _Texture3;
		uniform half4 _Tex2TilingOffset1;
		uniform half _MaskRange1;
		uniform half4 _Color3;
		uniform half _LightStrength;
		uniform half _LightSteps;


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
			half2 appendResult200 = (half2(_Tex1TilingOffset1.x , _Tex1TilingOffset1.y));
			half2 appendResult193 = (half2(_Tex1TilingOffset1.z , _Tex1TilingOffset1.w));
			float2 uv_TexCoord202 = i.uv_texcoord * appendResult200 + ( _Time.x * appendResult193 );
			half2 appendResult199 = (half2(_Tex2TilingOffset1.x , _Tex2TilingOffset1.y));
			half2 appendResult194 = (half2(_Tex2TilingOffset1.z , _Tex2TilingOffset1.w));
			float2 uv_TexCoord201 = i.uv_texcoord * appendResult199 + ( _Time.x * appendResult194 );
			half4 color220 = IsGammaSpace() ? half4(1,1,1,0) : half4(1,1,1,0);
			half4 temp_output_212_0 = ( ( tex2D( _Texture2, uv_TexCoord202 ) * _Color2 ) + ( tex2D( _Texture3, uv_TexCoord201 ) * color220 ) );
			half maskRange219 = _MaskRange1;
			half4 mainTex85 = ( ( saturate( ( 1.0 - ( ( distance( _Color2.rgb , temp_output_212_0.rgb ) - maskRange219 ) / max( 0.0 , 1E-05 ) ) ) ) * _Color2 ) + ( saturate( ( 1.0 - ( ( distance( color220.rgb , temp_output_212_0.rgb ) - maskRange219 ) / max( 0.0 , 1E-05 ) ) ) ) * _Color3 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult6 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_9 = (( ( ase_lightColor.a * ase_lightAtten ) * saturate( dotResult6 ) )).xxxx;
			half4 lerpResult29 = lerp( ( ase_lightColor * ase_lightAtten ) , float4( 0,0,0,0 ) , saturate( ( 1.0 - ( trunc( ( CalculateContrast(_LightStrength,temp_cast_9) * _LightSteps ) ) / _LightSteps ) ) ));
			half4 light73 = lerpResult29;
			half4 temp_output_49_0 = ( mainTex85 * light73 );
			c.rgb = ( (( _UseLight )?( 1.0 ):( 0.0 )) == 0.0 ? float4( 0,0,0,0 ) : temp_output_49_0 ).rgb;
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
			half2 appendResult200 = (half2(_Tex1TilingOffset1.x , _Tex1TilingOffset1.y));
			half2 appendResult193 = (half2(_Tex1TilingOffset1.z , _Tex1TilingOffset1.w));
			float2 uv_TexCoord202 = i.uv_texcoord * appendResult200 + ( _Time.x * appendResult193 );
			half2 appendResult199 = (half2(_Tex2TilingOffset1.x , _Tex2TilingOffset1.y));
			half2 appendResult194 = (half2(_Tex2TilingOffset1.z , _Tex2TilingOffset1.w));
			float2 uv_TexCoord201 = i.uv_texcoord * appendResult199 + ( _Time.x * appendResult194 );
			half4 color220 = IsGammaSpace() ? half4(1,1,1,0) : half4(1,1,1,0);
			half4 temp_output_212_0 = ( ( tex2D( _Texture2, uv_TexCoord202 ) * _Color2 ) + ( tex2D( _Texture3, uv_TexCoord201 ) * color220 ) );
			half maskRange219 = _MaskRange1;
			half4 mainTex85 = ( ( saturate( ( 1.0 - ( ( distance( _Color2.rgb , temp_output_212_0.rgb ) - maskRange219 ) / max( 0.0 , 1E-05 ) ) ) ) * _Color2 ) + ( saturate( ( 1.0 - ( ( distance( color220.rgb , temp_output_212_0.rgb ) - maskRange219 ) / max( 0.0 , 1E-05 ) ) ) ) * _Color3 ) );
			o.Emission = ( (( _UseLight )?( 1.0 ):( 0.0 )) == 1.0 ? float4( 0,0,0,0 ) : mainTex85 ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

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
0;0;1920;1019;1287.34;2123.905;1.745213;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-2927.886,1092.672;Inherit;False;2621.701;1098.188;Comment;31;73;35;29;37;34;36;33;31;25;26;9;7;8;14;13;6;5;4;166;167;168;169;170;171;172;173;174;175;176;177;178;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldNormalVector;5;-2776.391,1351.895;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-2776.138,1515.033;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;6;-2486.407,1457.125;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;13;-2652.998,1696.291;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;14;-2599.801,2002.014;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;191;-1330.766,-1433.556;Inherit;False;Property;_Tex1TilingOffset1;Tex1Tiling&Offset;4;0;Create;True;0;0;0;False;0;False;0.5,0.5,1,1;0.15,0.15,-0.02,-0.05;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;192;-1322.912,-1020.891;Inherit;False;Property;_Tex2TilingOffset1;Tex2Tiling&Offset;5;0;Create;True;0;0;0;False;0;False;0.5,0.5,1,1;0.5,0.5,0.1,0.05;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;193;-1110.985,-1451.666;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TimeNode;195;-1329.246,-1191.549;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;196;-1311.001,-1600.563;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;194;-1079.701,-1041.12;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2337.766,1686.494;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;8;-2267.923,1442.796;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;198;-1038.307,-1581.676;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2076.835,1542.774;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;-2031.685,1902.684;Inherit;False;Property;_LightStrength;LightStrength;10;0;Create;True;0;0;0;False;0;False;4;10;0;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;197;-1049.746,-1158.592;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;199;-1081.699,-950.5774;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;200;-1087.553,-1363.243;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;26;-1866.562,1578.954;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.TexturePropertyNode;204;-888.7614,-1764.869;Inherit;True;Property;_Texture2;Texture1;3;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;ac27f71af49b7684fb60eb16ea254730;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TexturePropertyNode;203;-890.0441,-1322.214;Inherit;True;Property;_Texture3;Texture2;8;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;e49c251a37bfe8f4b97a7bcff572613c;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.TextureCoordinatesNode;202;-876.5572,-1527.462;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;168;-1571.622,1872.266;Inherit;False;Property;_LightSteps;LightSteps;11;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;201;-894.8021,-1118.448;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;166;-1626.318,1587.39;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;220;-555.1832,-875.5289;Inherit;False;Constant;_Color0;Color 0;12;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;218;-32.1194,-1660.045;Inherit;False;Property;_MaskRange1;MaskRange;6;0;Create;True;0;0;0;False;0;False;0.5;0.5;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;205;-661.0227,-1163.055;Inherit;True;Property;_texture3;texture2;0;0;Create;True;0;0;0;False;0;False;-1;None;e49c251a37bfe8f4b97a7bcff572613c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;208;-659.8203,-1582.638;Inherit;True;Property;_texture2;texture1;0;0;Create;True;0;0;0;False;0;False;-1;None;e49c251a37bfe8f4b97a7bcff572613c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;206;-591.8275,-1401.226;Inherit;False;Property;_Color2;Color1;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.4490066,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;209;-345.4869,-1051.421;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;219;272.9212,-1640.35;Inherit;False;maskRange;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;210;-347.0593,-1471.004;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TruncOpNode;25;-1505.413,1567.576;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;167;-1345.422,1579.766;Inherit;False;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;212;-58.91263,-1335.222;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;211;-65.63409,-1121.155;Inherit;False;219;maskRange;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;36;-1881.187,1370.775;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;31;-1872.808,1181.614;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;213;267.506,-1412.093;Inherit;True;Color Mask;-1;;10;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.7;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;214;283.1039,-1175.081;Inherit;True;Color Mask;-1;;11;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0.7;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;207;167.0401,-868.1712;Inherit;False;Property;_Color3;Color2;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0.2327224,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;33;-1220.608,1565.584;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1447.491,1241.794;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;34;-1193.019,1425.861;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;216;574.506,-1183.552;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;215;558.9081,-1420.565;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;217;826.0643,-1321.964;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;29;-1025.792,1398.625;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;1207.623,-1535.734;Inherit;False;mainTex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-544.1136,1418.704;Inherit;False;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-343.9222,143.9416;Inherit;False;85;mainTex;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-339.8733,352.6864;Inherit;False;73;light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-83.48105,291.2883;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;114;-139.3424,-50.62039;Inherit;False;Property;_UseLight;UseLight;7;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;125;466.7791,221.5077;Inherit;False;Property;_UseTransparency;UseTransparency;9;0;Create;True;0;0;0;False;0;False;0;True;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TruncOpNode;173;-951.9828,2017.818;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;174;-824.537,1906.472;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;177;-1386.378,1846.629;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;171;-1339.033,1943.045;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;172;-1110.584,1962.419;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;175;-574.5878,1927.05;Inherit;False;lightAttenuation;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;115;202.1157,47.25757;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;176;-2594.358,1918.381;Inherit;False;175;lightAttenuation;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;178;-1843.681,1449.145;Inherit;False;175;lightAttenuation;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Compare;116;201.4297,-153.7882;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;122;338.4126,529.8983;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;123;161.8763,402.485;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;118;211.7038,199.3761;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;170;-1029.524,2116.29;Inherit;False;177;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;35;-770.7067,1509.4;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;765.7167,4.150301;Half;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Julian/PatternV3;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.05;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;193;0;191;3
WireConnection;193;1;191;4
WireConnection;194;0;192;3
WireConnection;194;1;192;4
WireConnection;7;0;13;2
WireConnection;7;1;14;0
WireConnection;8;0;6;0
WireConnection;198;0;196;1
WireConnection;198;1;193;0
WireConnection;9;0;7;0
WireConnection;9;1;8;0
WireConnection;197;0;195;1
WireConnection;197;1;194;0
WireConnection;199;0;192;1
WireConnection;199;1;192;2
WireConnection;200;0;191;1
WireConnection;200;1;191;2
WireConnection;26;1;9;0
WireConnection;26;0;169;0
WireConnection;202;0;200;0
WireConnection;202;1;198;0
WireConnection;201;0;199;0
WireConnection;201;1;197;0
WireConnection;166;0;26;0
WireConnection;166;1;168;0
WireConnection;205;0;203;0
WireConnection;205;1;201;0
WireConnection;208;0;204;0
WireConnection;208;1;202;0
WireConnection;209;0;205;0
WireConnection;209;1;220;0
WireConnection;219;0;218;0
WireConnection;210;0;208;0
WireConnection;210;1;206;0
WireConnection;25;0;166;0
WireConnection;167;0;25;0
WireConnection;167;1;168;0
WireConnection;212;0;210;0
WireConnection;212;1;209;0
WireConnection;213;1;212;0
WireConnection;213;3;206;0
WireConnection;213;4;211;0
WireConnection;214;1;212;0
WireConnection;214;3;220;0
WireConnection;214;4;211;0
WireConnection;33;0;167;0
WireConnection;37;0;31;0
WireConnection;37;1;36;0
WireConnection;34;0;33;0
WireConnection;216;0;214;0
WireConnection;216;1;207;0
WireConnection;215;0;213;0
WireConnection;215;1;206;0
WireConnection;217;0;215;0
WireConnection;217;1;216;0
WireConnection;29;0;37;0
WireConnection;29;2;34;0
WireConnection;85;0;217;0
WireConnection;73;0;29;0
WireConnection;49;0;86;0
WireConnection;49;1;74;0
WireConnection;125;1;122;0
WireConnection;173;0;172;0
WireConnection;174;0;173;0
WireConnection;174;1;170;0
WireConnection;177;0;168;0
WireConnection;172;0;171;0
WireConnection;172;1;170;0
WireConnection;175;0;174;0
WireConnection;115;0;114;0
WireConnection;115;3;49;0
WireConnection;116;0;114;0
WireConnection;116;3;86;0
WireConnection;122;0;123;0
WireConnection;123;0;118;0
WireConnection;118;0;114;0
WireConnection;118;2;86;0
WireConnection;118;3;49;0
WireConnection;35;0;29;0
WireConnection;0;2;116;0
WireConnection;0;13;115;0
ASEEND*/
//CHKSM=8DF3C41D3C3923BE9E2DCB1520B90CA1794CE70A