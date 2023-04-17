// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/CourtyardFloor"
{
	Properties
	{
		_Color1("Color1", Color) = (0,0,0,0)
		_Color0("Color 0", Color) = (1,1,1,0)
		_CircularNoiseTileAndSpeed("CircularNoiseTileAndSpeed", Vector) = (0,10,0,0.1)
		[NoScaleOffset]_Texture1("Texture1", 2D) = "white" {}
		_2NoiseContrast("2NoiseContrast", Range( 1 , 6)) = 1
		_Tex1TilingOffset("Tex1Tiling&Offset", Vector) = (0.5,0.5,1,1)
		_NoiseScale("NoiseScale", Float) = 1
		_MaskSoftness("MaskSoftness", Range( 0 , 1)) = 0
		_2NoiseStrength("2NoiseStrength", Range( 0 , 1)) = 1
		_MaskRange("MaskRange", Range( 0.4 , 1)) = 0.5
		_3DNoiseTime("3DNoiseTime", Float) = 0.2
		_MaskRangeMin("MaskRangeMin", Range( 0.1 , 1)) = 0.4
		_LightSteps("LightSteps", Float) = 4
		[HDR]_AmbientColor("AmbientColor", Color) = (0,0,0,0)
		_BackgroundColor("BackgroundColor", Color) = (0.08677466,0.2332488,0.4716981,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityCG.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha 
		struct Input
		{
			half3 worldNormal;
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

		uniform half4 _BackgroundColor;
		uniform half _LightSteps;
		uniform half _2NoiseContrast;
		sampler2D _Sampler60213;
		uniform half4 _CircularNoiseTileAndSpeed;
		uniform half _2NoiseStrength;
		uniform half _3DNoiseTime;
		uniform half _NoiseScale;
		uniform half4 _Color1;
		uniform sampler2D _Texture1;
		uniform half4 _Tex1TilingOffset;
		uniform half _MaskRangeMin;
		uniform half _MaskRange;
		uniform half _MaskSoftness;
		uniform half4 _Color0;
		uniform half4 _AmbientColor;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

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
			half3 ase_worldNormal = i.worldNormal;
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult6 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_0 = (saturate( dotResult6 )).xxxx;
			half lightSteps171 = _LightSteps;
			half temp_output_3_0_g23 = lightSteps171;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half4 temp_cast_1 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g22 = lightSteps171;
			half4 temp_output_26_0 = CalculateContrast(4.0,( ( trunc( ( temp_cast_0 * temp_output_3_0_g23 ) ) / temp_output_3_0_g23 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_1 * temp_output_3_0_g22 ) ) / temp_output_3_0_g22 ) ) ));
			half4 lightIntensity253 = temp_output_26_0;
			half4 lerpResult256 = lerp( float4( 0,0,0,0 ) , _BackgroundColor , ( lightIntensity253 + float4( 0.3,0.3,0.3,0 ) ));
			half2 temp_output_1_0_g40 = float2( 1,1 );
			half2 appendResult10_g40 = (half2(( (temp_output_1_0_g40).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g40).y )));
			half2 temp_output_11_0_g40 = float2( 0,0 );
			half2 panner18_g40 = ( ( (temp_output_11_0_g40).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g40 = ( ( _Time.y * (temp_output_11_0_g40).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g40 = (half2((panner18_g40).x , (panner19_g40).y));
			half2 appendResult211 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult212 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g40 = appendResult212;
			float2 uv_TexCoord78_g40 = i.uv_texcoord * float2( 2,2 );
			half2 temp_output_31_0_g40 = ( uv_TexCoord78_g40 - float2( 1,1 ) );
			half2 appendResult39_g40 = (half2(frac( ( atan2( (temp_output_31_0_g40).x , (temp_output_31_0_g40).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g40 )));
			half2 panner54_g40 = ( ( (temp_output_47_0_g40).x * _Time.y ) * float2( 1,0 ) + appendResult39_g40);
			half2 panner55_g40 = ( ( _Time.y * (temp_output_47_0_g40).y ) * float2( 0,1 ) + appendResult39_g40);
			half2 appendResult58_g40 = (half2((panner54_g40).x , (panner55_g40).y));
			half simplePerlin3D214 = snoise( half3( ( ( (tex2D( _Sampler60213, ( appendResult10_g40 + appendResult24_g40 ) )).rg * 1.0 ) + ( appendResult211 * appendResult58_g40 ) ) ,  0.0 ) );
			half4 temp_cast_4 = (simplePerlin3D214).xxxx;
			half mulTime243 = _Time.y * _3DNoiseTime;
			half simplePerlin3D249 = snoise( ( ( half3(0,2,0) * mulTime243 ) + ( half3(1,1,1) * ase_worldPos ) )*_NoiseScale );
			simplePerlin3D249 = simplePerlin3D249*0.5 + 0.5;
			half4 temp_cast_5 = (simplePerlin3D249).xxxx;
			half4 blendOpSrc250 = ( CalculateContrast(_2NoiseContrast,temp_cast_4) * _2NoiseStrength );
			half4 blendOpDest250 = temp_cast_5;
			half2 appendResult90 = (half2(_Tex1TilingOffset.x , _Tex1TilingOffset.y));
			half2 appendResult91 = (half2(_Tex1TilingOffset.z , _Tex1TilingOffset.w));
			float2 uv_TexCoord51 = i.uv_texcoord * appendResult90 + ( _Time.x * appendResult91 );
			half lerpResult150 = lerp( _MaskRangeMin , _MaskRange , _SinTime.w);
			half temp_output_97_0 = saturate( ( 1.0 - ( ( distance( _Color1.rgb , ( tex2D( _Texture1, uv_TexCoord51 ) * _Color1 ).rgb ) - lerpResult150 ) / max( _MaskSoftness , 1E-05 ) ) ) );
			half mainTex85 = temp_output_97_0;
			half4 temp_cast_8 = (mainTex85).xxxx;
			half4 lerpResult219 = lerp( ( saturate( (( blendOpSrc250 > 0.5 ) ? ( blendOpDest250 / max( ( 1.0 - blendOpSrc250 ) * 2.0 ,0.00001) ) : ( 1.0 - ( ( ( 1.0 - blendOpDest250 ) * 0.5 ) / max( blendOpSrc250,0.00001) ) ) ) )) , temp_cast_8 , mainTex85);
			half4 temp_cast_9 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g39 = lightSteps171;
			half4 lerpResult29 = lerp( ( ase_lightColor * ( trunc( ( temp_cast_9 * temp_output_3_0_g39 ) ) / temp_output_3_0_g39 ) ) , float4( 0,0,0,0 ) , saturate( ( 1.0 - temp_output_26_0 ) ));
			half4 light73 = lerpResult29;
			half4 lerpResult205 = lerp( _AmbientColor , light73 , light73);
			half4 lerpResult251 = lerp( lerpResult256 , ( lerpResult219 * _Color0 * lerpResult205 ) , lerpResult219);
			c.rgb = lerpResult251.rgb;
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
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
64;276;1906;977;650.9095;6.551734;1.418148;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-2922.789,1564.146;Inherit;False;2621.701;1098.188;Comment;29;73;35;29;37;34;36;33;31;26;9;7;8;14;13;6;5;4;170;171;182;186;189;191;192;193;194;195;196;253;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;170;-1670.408,2528.509;Inherit;False;Property;_LightSteps;LightSteps;13;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;5;-2771.294,1823.369;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;4;-2776.57,1986.38;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;171;-1480.438,2531.757;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-2481.31,1928.599;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;14;-2815.813,2432.725;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;195;-2588.395,2550.27;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;193;-2449.448,2430.755;Inherit;False;PosterizeSteps;-1;;22;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;194;-2441.455,1729.318;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;89;-1101.271,-1345.145;Inherit;False;Property;_Tex1TilingOffset;Tex1Tiling&Offset;6;0;Create;True;0;0;0;False;0;False;0.5,0.5,1,1;3,3,0.2,-0.2;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;13;-2716.494,2171.473;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;8;-2262.826,1914.27;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;182;-2187.612,1665.577;Inherit;False;PosterizeSteps;-1;;23;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;91;-881.4902,-1363.255;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-2442.274,2169.62;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TimeNode;52;-1081.506,-1512.152;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-808.8126,-1493.265;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;90;-858.058,-1274.832;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;208;-1526.339,730.723;Inherit;False;Property;_CircularNoiseTileAndSpeed;CircularNoiseTileAndSpeed;3;0;Create;True;0;0;0;False;0;False;0,10,0,0.1;15,30,0,0.03;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-2071.738,2014.248;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;212;-1239.924,853.6428;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;240;-1008.266,1056.714;Inherit;False;Property;_3DNoiseTime;3DNoiseTime;11;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;61;-659.2667,-1676.458;Inherit;True;Property;_Texture1;Texture1;4;1;[NoScaleOffset];Create;True;0;0;0;False;0;False;None;c88ea5f1ed114804cb0b473751cf3676;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.DynamicAppendNode;211;-1250.664,737.8834;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;51;-647.0625,-1439.051;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightAttenuation;36;-1951.877,1840.815;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;26;-1826.785,2027.734;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;4;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;196;-1747.091,1942.609;Inherit;False;171;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;244;-790.3488,1147.896;Inherit;False;Constant;_3DNoiseScale;3DNoiseScale;14;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SinTimeNode;147;-29.17243,-1711.5;Inherit;True;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;103;84.4082,-1496.501;Inherit;False;Property;_MaskRange;MaskRange;10;0;Create;True;0;0;0;False;0;False;0.5;0.9;0.4;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;243;-842.4048,1061.915;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;33;-1302.319,2032.286;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;46;-430.3256,-1494.227;Inherit;True;Property;_texture1;texture1;0;0;Create;True;0;0;0;False;0;False;61;None;e49c251a37bfe8f4b97a7bcff572613c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;213;-1049.933,631.1348;Inherit;False;RadialUVDistortion;-1;;40;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60213;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;0,1;False;47;FLOAT2;0,0.1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;53;-362.3328,-1247.502;Inherit;False;Property;_Color1;Color1;0;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;241;-874.4008,907.0498;Inherit;False;Constant;_NoiseDirection;NoiseDirection;14;0;Create;True;0;0;0;False;0;False;0,2,0;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;242;-804.1777,1306.166;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightColorNode;31;-1867.711,1653.088;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;189;-1584.381,1877.171;Inherit;False;PosterizeSteps;-1;;39;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;161;-46.17177,-1815.854;Inherit;False;Property;_MaskRangeMin;MaskRangeMin;12;0;Create;True;0;0;0;False;0;False;0.4;0.7;0.1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;245;-603.0518,959.7817;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;150;250.8275,-1732.5;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;1;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;214;-614.6536,586.0502;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;34;-1187.922,1897.335;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;264;166.9142,-1353.434;Inherit;False;Property;_MaskSoftness;MaskSoftness;8;0;Create;True;0;0;0;False;0;False;0;0.1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1415.676,1652.527;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;218;-682.2156,499.4255;Inherit;False;Property;_2NoiseContrast;2NoiseContrast;5;0;Create;True;0;0;0;False;0;False;1;1;1;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;246;-584.5677,1209.802;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-117.5646,-1382.593;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;248;-350.5198,1027.026;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;216;-373.0287,490.985;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;29;-1020.695,1870.099;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;215;-376.8558,768.6619;Inherit;False;Property;_2NoiseStrength;2NoiseStrength;9;0;Create;True;0;0;0;False;0;False;1;0.728;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;247;-329.7034,1159.003;Inherit;False;Property;_NoiseScale;NoiseScale;7;0;Create;True;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;97;461.6801,-1510.629;Inherit;True;Color Mask;-1;;41;eec747d987850564c95bde0e5a6d1867;0;4;1;FLOAT3;0,0,0;False;3;FLOAT3;1,1,1;False;4;FLOAT;0.7;False;5;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;249;-139.6266,906.0729;Inherit;True;Simplex3D;True;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;73;-493.6472,1904.614;Inherit;True;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;217;-124.4754,521.3252;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;253;-1538.586,2196.124;Inherit;False;lightIntensity;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;85;1207.623,-1535.734;Inherit;False;mainTex;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;250;135.2615,762.0068;Inherit;True;VividLight;True;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;191.2534,357.7871;Inherit;True;85;mainTex;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;254;409.9532,838.5161;Inherit;False;253;lightIntensity;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;220.5354,-162.1644;Inherit;True;73;light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;207;90.03642,56.714;Inherit;False;Property;_AmbientColor;AmbientColor;14;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;219;559.006,272.436;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;202;682.5833,493.9648;Inherit;False;Property;_Color0;Color 0;2;0;Create;True;0;0;0;False;0;False;1,1,1,0;0.754717,0.5312355,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;263;629.6416,805.8137;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.3,0.3,0.3,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;252;433.5801,610.3703;Inherit;False;Property;_BackgroundColor;BackgroundColor;15;0;Create;True;0;0;0;False;0;False;0.08677466,0.2332488,0.4716981,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;205;547.3909,-26.71809;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;256;913.9145,648.7151;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0.1792453,0.1792453,0.1792453,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;229;905.2738,111.6418;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;191;-1725.836,1871.427;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;206;324.2932,38.45381;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.ColorNode;144;567.0901,-1262.239;Inherit;False;Property;_EmissionColor;EmissionColor;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;788.4028,-1332.154;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;13.02,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;186;-734.852,1774.004;Inherit;False;PosterizeSteps;-1;;42;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;35;-697.9462,2065.378;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;251;1093.165,366.9732;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScaleNode;192;-2590.904,2425.011;Inherit;False;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;102;1055.559,-1233.553;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1327.717,4.150301;Half;False;True;-1;2;ASEMaterialInspector;0;0;CustomLighting;Julian/CourtyardFloor;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.05;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;171;0;170;0
WireConnection;6;0;5;0
WireConnection;6;1;4;0
WireConnection;193;1;14;0
WireConnection;193;3;195;0
WireConnection;8;0;6;0
WireConnection;182;1;8;0
WireConnection;182;3;194;0
WireConnection;91;0;89;3
WireConnection;91;1;89;4
WireConnection;7;0;13;2
WireConnection;7;1;193;0
WireConnection;55;0;52;1
WireConnection;55;1;91;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;9;0;182;0
WireConnection;9;1;7;0
WireConnection;212;0;208;3
WireConnection;212;1;208;4
WireConnection;211;0;208;1
WireConnection;211;1;208;2
WireConnection;51;0;90;0
WireConnection;51;1;55;0
WireConnection;26;1;9;0
WireConnection;243;0;240;0
WireConnection;33;0;26;0
WireConnection;46;0;61;0
WireConnection;46;1;51;0
WireConnection;213;68;211;0
WireConnection;213;47;212;0
WireConnection;189;1;36;0
WireConnection;189;3;196;0
WireConnection;245;0;241;0
WireConnection;245;1;243;0
WireConnection;150;0;161;0
WireConnection;150;1;103;0
WireConnection;150;2;147;4
WireConnection;214;0;213;0
WireConnection;34;0;33;0
WireConnection;37;0;31;0
WireConnection;37;1;189;0
WireConnection;246;0;244;0
WireConnection;246;1;242;0
WireConnection;58;0;46;0
WireConnection;58;1;53;0
WireConnection;248;0;245;0
WireConnection;248;1;246;0
WireConnection;216;1;214;0
WireConnection;216;0;218;0
WireConnection;29;0;37;0
WireConnection;29;2;34;0
WireConnection;97;1;58;0
WireConnection;97;3;53;0
WireConnection;97;4;150;0
WireConnection;97;5;264;0
WireConnection;249;0;248;0
WireConnection;249;1;247;0
WireConnection;73;0;29;0
WireConnection;217;0;216;0
WireConnection;217;1;215;0
WireConnection;253;0;26;0
WireConnection;85;0;97;0
WireConnection;250;0;217;0
WireConnection;250;1;249;0
WireConnection;219;0;250;0
WireConnection;219;1;86;0
WireConnection;219;2;86;0
WireConnection;263;0;254;0
WireConnection;205;0;207;0
WireConnection;205;1;74;0
WireConnection;205;2;74;0
WireConnection;256;1;252;0
WireConnection;256;2;263;0
WireConnection;229;0;219;0
WireConnection;229;1;202;0
WireConnection;229;2;205;0
WireConnection;191;0;36;0
WireConnection;98;0;97;0
WireConnection;98;1;144;0
WireConnection;186;1;29;0
WireConnection;35;0;29;0
WireConnection;251;0;256;0
WireConnection;251;1;229;0
WireConnection;251;2;219;0
WireConnection;192;0;14;0
WireConnection;102;0;98;0
WireConnection;0;13;251;0
ASEEND*/
//CHKSM=F2860B41A4699578FA47DF14F338194AC2C1E9D6