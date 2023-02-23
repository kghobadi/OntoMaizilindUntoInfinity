// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseLandscape"
{
	Properties
	{
		_Albedo("Albedo", Color) = (1,1,1,1)
		_LightSteps("LightSteps", Float) = 4
		_RemapValues("RemapValues", Vector) = (0,1,0,1.2)
		_NoiseScale("NoiseScale", Float) = 10
		_3DNoiseTime("3DNoiseTime", Float) = 0.2
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 4.6
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf StandardCustomLighting keepalpha exclude_path:deferred noambient nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
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

		uniform half4 _Albedo;
		uniform half _3DNoiseTime;
		uniform half _NoiseScale;
		uniform half _LightSteps;
		uniform half4 _RemapValues;


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
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half mulTime172 = _Time.y * _3DNoiseTime;
			float3 ase_worldPos = i.worldPos;
			half simplePerlin3D2 = snoise( ( ( half3(0.3,2,0.3) * mulTime172 ) + ( half3(1,1,1) * ase_worldPos ) )*_NoiseScale );
			half3 ase_worldNormal = i.worldNormal;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_0 = (saturate( dotResult79 )).xxxx;
			half lightSteps77 = _LightSteps;
			half temp_output_3_0_g41 = lightSteps77;
			half4 temp_cast_1 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g39 = lightSteps77;
			half4 temp_output_87_0 = ( ( trunc( ( temp_cast_0 * temp_output_3_0_g41 ) ) / temp_output_3_0_g41 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_1 * temp_output_3_0_g39 ) ) / temp_output_3_0_g39 ) ) );
			half4 lightLerp129 = temp_output_87_0;
			half4 temp_cast_2 = (_RemapValues.x).xxxx;
			half4 temp_cast_3 = (_RemapValues.y).xxxx;
			half4 temp_cast_4 = (_RemapValues.z).xxxx;
			half4 temp_cast_5 = (_RemapValues.w).xxxx;
			half4 temp_output_160_0 = (temp_cast_4 + (lightLerp129 - temp_cast_2) * (temp_cast_5 - temp_cast_4) / (temp_cast_3 - temp_cast_2));
			half4 temp_cast_6 = (simplePerlin3D2).xxxx;
			half4 blendOpSrc146 = temp_output_160_0;
			half4 blendOpDest146 = temp_cast_6;
			half4 lerpResult127 = lerp( ( ase_lightColor * simplePerlin3D2 ) , ase_lightColor , ( saturate( ( blendOpSrc146 + blendOpDest146 ) )));
			c.rgb = ( _Albedo * lerpResult127 ).rgb;
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
41;165;1906;929;294.7623;241.6383;1.143032;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-1671.033,1112.296;Inherit;False;2621.701;1098.188;Comment;25;99;97;96;95;94;93;92;89;88;87;86;85;84;83;82;81;79;78;77;76;75;74;73;129;157;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-418.6519,2076.66;Inherit;False;Property;_LightSteps;LightSteps;1;0;Create;True;0;0;0;False;0;False;4;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;76;-1638.559,1347.393;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-228.6818,2079.907;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-1643.835,1510.404;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightAttenuation;74;-1401.082,1888.376;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1173.663,2005.921;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;79;-1348.575,1452.623;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;84;-1160.811,1697.599;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;81;-1130.091,1438.294;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;82;-1034.718,1886.406;Inherit;False;PosterizeSteps;-1;;39;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1118.93,1308.028;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;-1362.295,149.7575;Inherit;False;Property;_3DNoiseTime;3DNoiseTime;7;0;Create;True;0;0;0;False;0;False;0.2;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;85;-922.9891,1419.601;Inherit;False;PosterizeSteps;-1;;41;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-829.3293,1680.33;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-499.8978,1574.891;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;172;-1196.434,154.9582;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;170;-1228.43,0.09300089;Inherit;False;Constant;_NoiseDirection;NoiseDirection;14;0;Create;True;0;0;0;False;0;False;0.3,2,0.3;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;171;-1158.207,399.2092;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;173;-1144.378,240.9389;Inherit;False;Constant;_3DNoiseScale;3DNoiseScale;14;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-134.7174,1865.859;Inherit;False;lightLerp;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-957.0809,52.82489;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-938.5969,302.8447;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.Vector4Node;165;130.8353,603.9285;Inherit;False;Property;_RemapValues;RemapValues;3;0;Create;True;0;0;0;False;0;False;0,1,0,1.2;0,1,0,1.6;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-704.5479,120.0693;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-683.7319,252.0461;Inherit;False;Property;_NoiseScale;NoiseScale;5;0;Create;True;0;0;0;False;0;False;10;0.15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;125.0563,391.5569;Inherit;True;129;lightLerp;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-377.5808,79.82293;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;160;362.5339,472.0701;Inherit;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightColorNode;163;360.961,-222.7281;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.BlendOpsNode;146;685.9683,382.7909;Inherit;True;LinearDodge;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;550.1737,-26.43597;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;127;846.0188,41.74914;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;31;845.8781,-357.0184;Inherit;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.5849056,0.3265723,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;104;614.581,214.4938;Inherit;False;157;lightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;168;764.2002,-178.9301;Inherit;True;Property;_MainTexture;MainTexture;4;0;Create;True;0;0;0;False;0;False;-1;None;f560965f4b074b246abc3394bd364eb1;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;557.8245,1618.214;Inherit;False;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;92;-160.4678,1597.445;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightColorNode;93;-491.1676,1171.182;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.TruncOpNode;181;354.9127,-14.67989;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;97;231.0608,1418.249;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;95;63.83377,1445.485;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-146.3008,1242.522;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;189;706.7968,631.0675;Inherit;True;HardMix;True;3;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LightAttenuation;88;-788.6608,1211.702;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;1157.894,-143.7422;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;182;72.08622,-67.10966;Inherit;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;178;-388.1568,-130.808;Inherit;True;Property;_DetailTexture;DetailTexture;6;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;89;-645.9431,1423.855;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;94;-455.1952,1391.657;Inherit;False;PosterizeSteps;-1;;42;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;230.1183,3.466379;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;126;1095.531,-492.1125;Inherit;False;Property;_EmissionColor;EmissionColor;2;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;202.512,1232.482;Inherit;False;lightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1460.999,-306.0758;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseLandscape;False;False;False;False;True;False;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;73;0
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;81;0;79;0
WireConnection;82;1;74;0
WireConnection;82;3;78;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;86;0;84;2
WireConnection;86;1;82;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;172;0;169;0
WireConnection;129;0;87;0
WireConnection;174;0;170;0
WireConnection;174;1;172;0
WireConnection;175;0;173;0
WireConnection;175;1;171;0
WireConnection;176;0;174;0
WireConnection;176;1;175;0
WireConnection;2;0;176;0
WireConnection;2;1;177;0
WireConnection;160;0;130;0
WireConnection;160;1;165;1
WireConnection;160;2;165;2
WireConnection;160;3;165;3
WireConnection;160;4;165;4
WireConnection;146;0;160;0
WireConnection;146;1;2;0
WireConnection;128;0;163;0
WireConnection;128;1;2;0
WireConnection;127;0;128;0
WireConnection;127;1;163;0
WireConnection;127;2;146;0
WireConnection;99;0;97;0
WireConnection;92;0;87;0
WireConnection;181;0;180;0
WireConnection;97;0;96;0
WireConnection;97;2;92;0
WireConnection;95;0;92;0
WireConnection;96;0;93;0
WireConnection;96;1;94;0
WireConnection;189;0;160;0
WireConnection;189;1;2;0
WireConnection;162;0;31;0
WireConnection;162;1;127;0
WireConnection;94;1;88;0
WireConnection;94;3;89;0
WireConnection;180;1;182;0
WireConnection;157;0;96;0
WireConnection;0;13;162;0
ASEEND*/
//CHKSM=AFF833F8BA8E6EC1CF11A523BAD0965E45652282