// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseCity"
{
	Properties
	{
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveDirection("WaveDirection", Vector) = (1,0,0,0)
		_WaveTile("WaveTile", Float) = 1
		_WaveSize("WaveSize", Vector) = (1,1,0,0)
		_PublicColor("Public Color", Color) = (1,1,1,1)
		[KeywordEnum(AlignOnX,AlignOnZ,VerticalHorizontal,VerticalVertical,TextureCoordinates)] _NoiseAlignment("NoiseAlignment", Float) = 0
		[Toggle]_UseLight("UseLight", Float) = 1
		_LightSteps("LightSteps", Float) = 4
		_CircularNoiseTileAndSpeed("CircularNoiseTileAndSpeed", Vector) = (0,10,0,0.1)
		_2NoiseContrast("2NoiseContrast", Range( 1 , 6)) = 1
		_2NoiseStrength("2NoiseStrength", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
		#pragma shader_feature_local _NOISEALIGNMENT_ALIGNONX _NOISEALIGNMENT_ALIGNONZ _NOISEALIGNMENT_VERTICALHORIZONTAL _NOISEALIGNMENT_VERTICALVERTICAL _NOISEALIGNMENT_TEXTURECOORDINATES
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			half3 worldNormal;
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
		uniform float _WaveSpeed;
		uniform float2 _WaveDirection;
		uniform float2 _WaveSize;
		uniform float _WaveTile;
		uniform half _2NoiseContrast;
		sampler2D _Sampler60107;
		uniform half4 _CircularNoiseTileAndSpeed;
		uniform half _2NoiseStrength;
		uniform half4 _PublicColor;
		uniform half _LightSteps;


		float3 mod3D289( float3 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 mod3D289( float4 x ) { return x - floor( x / 289.0 ) * 289.0; }

		float4 permute( float4 x ) { return mod3D289( ( x * 34.0 + 1.0 ) * x ); }

		float4 taylorInvSqrt( float4 r ) { return 1.79284291400159 - r * 0.85373472095314; }

		float snoise( float3 v )
		{
			const float2 C = float2( 1.0 / 6.0, 1.0 / 3.0 );
			float3 i = floor( v + dot( v, C.yyy ) );
			float3 x0 = v - i + dot( i, C.xxx );
			float3 g = step( x0.yzx, x0.xyz );
			float3 l = 1.0 - g;
			float3 i1 = min( g.xyz, l.zxy );
			float3 i2 = max( g.xyz, l.zxy );
			float3 x1 = x0 - i1 + C.xxx;
			float3 x2 = x0 - i2 + C.yyy;
			float3 x3 = x0 - 0.5;
			i = mod3D289( i);
			float4 p = permute( permute( permute( i.z + float4( 0.0, i1.z, i2.z, 1.0 ) ) + i.y + float4( 0.0, i1.y, i2.y, 1.0 ) ) + i.x + float4( 0.0, i1.x, i2.x, 1.0 ) );
			float4 j = p - 49.0 * floor( p / 49.0 );  // mod(p,7*7)
			float4 x_ = floor( j / 7.0 );
			float4 y_ = floor( j - 7.0 * x_ );  // mod(j,N)
			float4 x = ( x_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 y = ( y_ * 2.0 + 0.5 ) / 7.0 - 1.0;
			float4 h = 1.0 - abs( x ) - abs( y );
			float4 b0 = float4( x.xy, y.xy );
			float4 b1 = float4( x.zw, y.zw );
			float4 s0 = floor( b0 ) * 2.0 + 1.0;
			float4 s1 = floor( b1 ) * 2.0 + 1.0;
			float4 sh = -step( h, 0.0 );
			float4 a0 = b0.xzyw + s0.xzyw * sh.xxyy;
			float4 a1 = b1.xzyw + s1.xzyw * sh.zzww;
			float3 g0 = float3( a0.xy, h.x );
			float3 g1 = float3( a0.zw, h.y );
			float3 g2 = float3( a1.xy, h.z );
			float3 g3 = float3( a1.zw, h.w );
			float4 norm = taylorInvSqrt( float4( dot( g0, g0 ), dot( g1, g1 ), dot( g2, g2 ), dot( g3, g3 ) ) );
			g0 *= norm.x;
			g1 *= norm.y;
			g2 *= norm.z;
			g3 *= norm.w;
			float4 m = max( 0.6 - float4( dot( x0, x0 ), dot( x1, x1 ), dot( x2, x2 ), dot( x3, x3 ) ), 0.0 );
			m = m* m;
			m = m* m;
			float4 px = float4( dot( x0, g0 ), dot( x1, g1 ), dot( x2, g2 ), dot( x3, g3 ) );
			return 42.0 * dot( m, px);
		}


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
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float3 ase_worldPos = i.worldPos;
			half2 appendResult29 = (half2(ase_worldPos.z , ase_worldPos.x));
			half2 appendResult9 = (half2(ase_worldPos.x , ase_worldPos.z));
			half2 appendResult38 = (half2(ase_worldPos.y , ase_worldPos.z));
			half2 appendResult39 = (half2(ase_worldPos.z , ase_worldPos.y));
			#if defined(_NOISEALIGNMENT_ALIGNONX)
				half2 staticSwitch34 = appendResult29;
			#elif defined(_NOISEALIGNMENT_ALIGNONZ)
				half2 staticSwitch34 = appendResult9;
			#elif defined(_NOISEALIGNMENT_VERTICALHORIZONTAL)
				half2 staticSwitch34 = appendResult38;
			#elif defined(_NOISEALIGNMENT_VERTICALVERTICAL)
				half2 staticSwitch34 = appendResult39;
			#elif defined(_NOISEALIGNMENT_TEXTURECOORDINATES)
				half2 staticSwitch34 = i.uv_texcoord;
			#else
				half2 staticSwitch34 = appendResult29;
			#endif
			float2 WorldSpaceTile10 = staticSwitch34;
			float2 WaveTileUV20 = ( ( WorldSpaceTile10 * _WaveSize ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20);
			half simplePerlin3D2 = snoise( half3( panner3 ,  0.0 ) );
			half2 temp_output_1_0_g30 = float2( 1,1 );
			half2 appendResult10_g30 = (half2(( (temp_output_1_0_g30).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g30).y )));
			half2 temp_output_11_0_g30 = float2( 0,0 );
			half2 panner18_g30 = ( ( (temp_output_11_0_g30).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g30 = ( ( _Time.y * (temp_output_11_0_g30).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g30 = (half2((panner18_g30).x , (panner19_g30).y));
			half2 appendResult109 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult110 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g30 = appendResult110;
			float2 uv_TexCoord78_g30 = i.uv_texcoord * float2( 2,2 );
			half2 temp_output_31_0_g30 = ( uv_TexCoord78_g30 - float2( 1,1 ) );
			half2 appendResult39_g30 = (half2(frac( ( atan2( (temp_output_31_0_g30).x , (temp_output_31_0_g30).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g30 )));
			half2 panner54_g30 = ( ( (temp_output_47_0_g30).x * _Time.y ) * float2( 1,0 ) + appendResult39_g30);
			half2 panner55_g30 = ( ( _Time.y * (temp_output_47_0_g30).y ) * float2( 0,1 ) + appendResult39_g30);
			half2 appendResult58_g30 = (half2((panner54_g30).x , (panner55_g30).y));
			half simplePerlin3D23 = snoise( half3( ( ( (tex2D( _Sampler60107, ( appendResult10_g30 + appendResult24_g30 ) )).rg * 1.0 ) + ( appendResult109 * appendResult58_g30 ) ) ,  0.0 ) );
			half4 temp_cast_7 = (simplePerlin3D23).xxxx;
			half4 temp_output_26_0 = ( simplePerlin3D2 + ( CalculateContrast(_2NoiseContrast,temp_cast_7) * _2NoiseStrength ) );
			half4 temp_output_32_0 = ( temp_output_26_0 * _PublicColor );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half4 temp_cast_8 = (( ase_lightAtten * 5 )).xxxx;
			half lightSteps77 = _LightSteps;
			half temp_output_3_0_g31 = lightSteps77;
			half3 ase_worldNormal = i.worldNormal;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_10 = (saturate( dotResult79 )).xxxx;
			half temp_output_3_0_g28 = lightSteps77;
			half4 temp_cast_11 = (( ase_lightAtten * 5 )).xxxx;
			half temp_output_3_0_g27 = lightSteps77;
			half4 lerpResult97 = lerp( ( ase_lightColor * ( trunc( ( temp_cast_8 * temp_output_3_0_g31 ) ) / temp_output_3_0_g31 ) ) , float4( 0,0,0,0 ) , saturate( ( 1.0 - CalculateContrast(4.0,( ( trunc( ( temp_cast_10 * temp_output_3_0_g28 ) ) / temp_output_3_0_g28 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_11 * temp_output_3_0_g27 ) ) / temp_output_3_0_g27 ) ) )) ) ));
			half4 light99 = lerpResult97;
			c.rgb = ( (( _UseLight )?( 1.0 ):( 0.0 )) == 0.0 ? temp_output_32_0 : ( temp_output_32_0 * light99 ) ).rgb;
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
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float3 ase_worldPos = i.worldPos;
			half2 appendResult29 = (half2(ase_worldPos.z , ase_worldPos.x));
			half2 appendResult9 = (half2(ase_worldPos.x , ase_worldPos.z));
			half2 appendResult38 = (half2(ase_worldPos.y , ase_worldPos.z));
			half2 appendResult39 = (half2(ase_worldPos.z , ase_worldPos.y));
			#if defined(_NOISEALIGNMENT_ALIGNONX)
				half2 staticSwitch34 = appendResult29;
			#elif defined(_NOISEALIGNMENT_ALIGNONZ)
				half2 staticSwitch34 = appendResult9;
			#elif defined(_NOISEALIGNMENT_VERTICALHORIZONTAL)
				half2 staticSwitch34 = appendResult38;
			#elif defined(_NOISEALIGNMENT_VERTICALVERTICAL)
				half2 staticSwitch34 = appendResult39;
			#elif defined(_NOISEALIGNMENT_TEXTURECOORDINATES)
				half2 staticSwitch34 = i.uv_texcoord;
			#else
				half2 staticSwitch34 = appendResult29;
			#endif
			float2 WorldSpaceTile10 = staticSwitch34;
			float2 WaveTileUV20 = ( ( WorldSpaceTile10 * _WaveSize ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20);
			half simplePerlin3D2 = snoise( half3( panner3 ,  0.0 ) );
			half2 temp_output_1_0_g30 = float2( 1,1 );
			half2 appendResult10_g30 = (half2(( (temp_output_1_0_g30).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g30).y )));
			half2 temp_output_11_0_g30 = float2( 0,0 );
			half2 panner18_g30 = ( ( (temp_output_11_0_g30).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g30 = ( ( _Time.y * (temp_output_11_0_g30).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g30 = (half2((panner18_g30).x , (panner19_g30).y));
			half2 appendResult109 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult110 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g30 = appendResult110;
			float2 uv_TexCoord78_g30 = i.uv_texcoord * float2( 2,2 );
			half2 temp_output_31_0_g30 = ( uv_TexCoord78_g30 - float2( 1,1 ) );
			half2 appendResult39_g30 = (half2(frac( ( atan2( (temp_output_31_0_g30).x , (temp_output_31_0_g30).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g30 )));
			half2 panner54_g30 = ( ( (temp_output_47_0_g30).x * _Time.y ) * float2( 1,0 ) + appendResult39_g30);
			half2 panner55_g30 = ( ( _Time.y * (temp_output_47_0_g30).y ) * float2( 0,1 ) + appendResult39_g30);
			half2 appendResult58_g30 = (half2((panner54_g30).x , (panner55_g30).y));
			half simplePerlin3D23 = snoise( half3( ( ( (tex2D( _Sampler60107, ( appendResult10_g30 + appendResult24_g30 ) )).rg * 1.0 ) + ( appendResult109 * appendResult58_g30 ) ) ,  0.0 ) );
			half4 temp_cast_2 = (simplePerlin3D23).xxxx;
			half4 temp_output_26_0 = ( simplePerlin3D2 + ( CalculateContrast(_2NoiseContrast,temp_cast_2) * _2NoiseStrength ) );
			half4 temp_output_32_0 = ( temp_output_26_0 * _PublicColor );
			half4 temp_output_101_0 = ( (( _UseLight )?( 1.0 ):( 0.0 )) == 1.0 ? float4( 0,0,0,0 ) : temp_output_32_0 );
			o.Albedo = temp_output_101_0.rgb;
			o.Emission = temp_output_101_0.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma exclude_renderers xboxseries playstation switch nomrt 
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
			#pragma target 4.6
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
7;78;1906;941;1519.34;-144.1814;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;11;-1653.051,-1125.017;Inherit;False;871.1336;510.3396;;10;10;30;9;29;8;28;34;38;39;40;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;72;-2509.615,1108.915;Inherit;False;2621.701;1098.188;Comment;27;99;98;97;96;95;94;93;92;91;90;89;88;87;86;85;84;83;82;81;80;79;78;77;76;75;74;73;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-1607.151,-940.017;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-1602.423,-1084.21;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-1632.98,-763.6873;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;29;-1363.624,-1075.31;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-1355.632,-842.5486;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1368.022,-959.0023;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-1257.234,2073.278;Inherit;False;Property;_LightSteps;LightSteps;10;0;Create;True;0;0;0;False;0;False;4;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1358.833,-729.885;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-2363.396,1531.149;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-1067.264,2076.526;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;76;-2358.12,1368.138;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.StaticSwitch;34;-1074.662,-843.066;Inherit;False;Property;_NoiseAlignment;NoiseAlignment;7;0;Create;True;0;0;0;False;0;False;0;0;1;True;;KeywordEnum;5;AlignOnX;AlignOnZ;VerticalHorizontal;VerticalVertical;TextureCoordinates;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LightAttenuation;74;-2402.64,1977.494;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-2175.221,2095.039;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-974.3055,-986.2117;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;80;-2177.73,1969.78;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;79;-2068.136,1473.368;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1748.793,-413.3993;Inherit;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;82;-2036.275,1975.524;Inherit;False;PosterizeSteps;-1;;27;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-2028.281,1274.087;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;81;-1849.652,1459.039;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;14;-1678.819,-224.9958;Float;False;Property;_WaveSize;WaveSize;3;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1698.793,-363.3991;Inherit;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LightColorNode;84;-2303.32,1716.242;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.Vector4Node;108;-1772.684,789.2684;Inherit;False;Property;_CircularNoiseTileAndSpeed;CircularNoiseTileAndSpeed;11;0;Create;True;0;0;0;False;0;False;0,10,0,0.1;0,70,0,0.01;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-2029.1,1714.389;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1440.537,-316.3137;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1326.441,-162.1133;Float;False;Property;_WaveTile;WaveTile;2;0;Create;True;0;0;0;False;0;False;1;20;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;85;-1774.438,1210.346;Inherit;False;PosterizeSteps;-1;;28;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightAttenuation;88;-1538.703,1385.584;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1160.749,-330.1563;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-1658.564,1559.017;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;-1486.269,912.1882;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-1497.009,796.4288;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;90;-1413.611,1572.503;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;107;-1296.278,689.6802;Inherit;True;RadialUVDistortion;-1;;30;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60107;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;0,1;False;47;FLOAT2;0,0.1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-1333.917,1487.378;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-968.5618,-335.771;Float;False;WaveTileUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ScaleNode;91;-1312.662,1416.196;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1723.689,330.3844;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1699.776,424.8315;Float;False;Property;_WaveSpeed;WaveSpeed;0;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-1556.096,213.0983;Float;False;Property;_WaveDirection;WaveDirection;1;0;Create;True;0;0;0;False;0;False;1,0;0,2;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;92;-889.1454,1577.055;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;93;-1454.537,1197.857;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1435.207,341.1361;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-1601.667,29.13445;Inherit;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;94;-1171.207,1421.94;Inherit;False;PosterizeSteps;-1;;31;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-860.9994,644.5955;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-974.212,321.5337;Inherit;False;Property;_2NoiseContrast;2NoiseContrast;12;0;Create;True;0;0;0;False;0;False;1;1.5;1;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;112;-718.9637,405.1914;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-812.3401,915.1813;Inherit;False;Property;_2NoiseStrength;2NoiseStrength;13;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;-774.7485,1442.104;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-1002.502,1197.296;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.PannerNode;3;-1003.783,110.4484;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-539.3401,640.1813;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;97;-607.5214,1414.868;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-703.5278,60.64958;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-375.8132,249.4037;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;31;-172.3586,-199.2358;Inherit;False;Property;_PublicColor;Public Color;6;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.4716981,0.2645209,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;-80.47341,1449.383;Inherit;True;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-83.39205,95.6242;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;161.0376,262.7021;Inherit;False;99;light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;103;207.2838,-118.8793;Inherit;False;Property;_UseLight;UseLight;8;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;354.0376,175.7021;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;102;526.7419,-20.00131;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;27;-340.1995,530.8367;Inherit;True;Property;_Transparency;Transparency;4;0;Create;True;0;0;0;False;0;False;-1;9f754b83aaea3344daf1c26cfd803969;9f754b83aaea3344daf1c26cfd803969;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;41;95.77649,423.5668;Inherit;True;ColorBurn;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1348.045,130.8476;Inherit;False;2;2;0;FLOAT2;0.1,0;False;1;FLOAT2;0.1,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;98;-311.0148,1523.219;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.Compare;101;526.0558,-221.0471;Inherit;False;0;4;0;FLOAT;0;False;1;FLOAT;1;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-290.8394,743.6924;Inherit;False;Property;_BlendStrength;BlendStrength;9;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;30;-1200.635,-987.1863;Inherit;False;Property;_AlignOnWorldX;AlignOnWorldX;5;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;24;-995.5617,402.9817;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;823.0146,-156.2;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseCity;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;29;0;28;3
WireConnection;29;1;28;1
WireConnection;38;0;8;2
WireConnection;38;1;8;3
WireConnection;9;0;8;1
WireConnection;9;1;8;3
WireConnection;39;0;8;3
WireConnection;39;1;8;2
WireConnection;77;0;73;0
WireConnection;34;1;29;0
WireConnection;34;0;9;0
WireConnection;34;2;38;0
WireConnection;34;3;39;0
WireConnection;34;4;40;0
WireConnection;10;0;34;0
WireConnection;80;0;74;0
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;82;1;80;0
WireConnection;82;3;78;0
WireConnection;81;0;79;0
WireConnection;86;0;84;2
WireConnection;86;1;82;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;110;0;108;3
WireConnection;110;1;108;4
WireConnection;109;0;108;1
WireConnection;109;1;108;2
WireConnection;90;1;87;0
WireConnection;107;68;109;0
WireConnection;107;47;110;0
WireConnection;20;0;15;0
WireConnection;91;0;88;0
WireConnection;92;0;90;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;94;1;91;0
WireConnection;94;3;89;0
WireConnection;23;0;107;0
WireConnection;112;1;23;0
WireConnection;112;0;113;0
WireConnection;95;0;92;0
WireConnection;96;0;93;0
WireConnection;96;1;94;0
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;114;0;112;0
WireConnection;114;1;115;0
WireConnection;97;0;96;0
WireConnection;97;2;95;0
WireConnection;2;0;3;0
WireConnection;26;0;2;0
WireConnection;26;1;114;0
WireConnection;99;0;97;0
WireConnection;32;0;26;0
WireConnection;32;1;31;0
WireConnection;105;0;32;0
WireConnection;105;1;104;0
WireConnection;102;0;103;0
WireConnection;102;2;32;0
WireConnection;102;3;105;0
WireConnection;41;0;26;0
WireConnection;41;1;27;0
WireConnection;41;2;43;0
WireConnection;25;0;22;0
WireConnection;98;0;97;0
WireConnection;101;0;103;0
WireConnection;101;3;32;0
WireConnection;30;0;9;0
WireConnection;30;1;29;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;0;0;101;0
WireConnection;0;2;101;0
WireConnection;0;13;102;0
ASEEND*/
//CHKSM=6FE540FA04CFA0BC6171ACA75BD61AB3B44329E9