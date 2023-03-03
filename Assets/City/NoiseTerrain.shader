// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseTerrain"
{
	Properties
	{
		_Albedo("Albedo", Color) = (1,1,1,1)
		_LightSteps("LightSteps", Float) = 4
		_CircularNoiseTileAndSpeed("CircularNoiseTileAndSpeed", Vector) = (0,10,0,0.1)
		_2NoiseContrast("2NoiseContrast", Range( 1 , 6)) = 1
		_2NoiseStrength("2NoiseStrength", Range( 0 , 1)) = 1
		_RemapValues("RemapValues", Vector) = (0,1,0,1.2)
		_NoiseScale("NoiseScale", Float) = 1
		_MainTexture("MainTexture", 2D) = "white" {}
		_3DNoiseTime("3DNoiseTime", Float) = 0.2
		_NoiseStrength("NoiseStrength", Range( 0 , 1)) = 1
		_MaximumHeight1("MaximumHeight", Float) = 380
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#pragma surface surf StandardCustomLighting keepalpha noambient nolightmap  nodynlightmap nodirlightmap nometa noforwardadd 
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

		uniform half4 _Albedo;
		uniform half _NoiseStrength;
		uniform half _3DNoiseTime;
		uniform half _NoiseScale;
		uniform half _2NoiseContrast;
		sampler2D _Sampler60107;
		uniform half4 _CircularNoiseTileAndSpeed;
		uniform half _2NoiseStrength;
		uniform sampler2D _MainTexture;
		uniform half4 _MainTexture_ST;
		uniform half _LightSteps;
		uniform half4 _RemapValues;
		uniform half _MaximumHeight1;


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

		struct Gradient
		{
			int type;
			int colorsLength;
			int alphasLength;
			float4 colors[8];
			float2 alphas[8];
		};


		Gradient NewGradient(int type, int colorsLength, int alphasLength, 
		float4 colors0, float4 colors1, float4 colors2, float4 colors3, float4 colors4, float4 colors5, float4 colors6, float4 colors7,
		float2 alphas0, float2 alphas1, float2 alphas2, float2 alphas3, float2 alphas4, float2 alphas5, float2 alphas6, float2 alphas7)
		{
			Gradient g;
			g.type = type;
			g.colorsLength = colorsLength;
			g.alphasLength = alphasLength;
			g.colors[ 0 ] = colors0;
			g.colors[ 1 ] = colors1;
			g.colors[ 2 ] = colors2;
			g.colors[ 3 ] = colors3;
			g.colors[ 4 ] = colors4;
			g.colors[ 5 ] = colors5;
			g.colors[ 6 ] = colors6;
			g.colors[ 7 ] = colors7;
			g.alphas[ 0 ] = alphas0;
			g.alphas[ 1 ] = alphas1;
			g.alphas[ 2 ] = alphas2;
			g.alphas[ 3 ] = alphas3;
			g.alphas[ 4 ] = alphas4;
			g.alphas[ 5 ] = alphas5;
			g.alphas[ 6 ] = alphas6;
			g.alphas[ 7 ] = alphas7;
			return g;
		}


		float4 SampleGradient( Gradient gradient, float time )
		{
			float3 color = gradient.colors[0].rgb;
			UNITY_UNROLL
			for (int c = 1; c < 8; c++)
			{
			float colorPos = saturate((time - gradient.colors[c-1].w) / ( 0.00001 + (gradient.colors[c].w - gradient.colors[c-1].w)) * step(c, (float)gradient.colorsLength-1));
			color = lerp(color, gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), gradient.type));
			}
			#ifndef UNITY_COLORSPACE_GAMMA
			color = half3(GammaToLinearSpaceExact(color.r), GammaToLinearSpaceExact(color.g), GammaToLinearSpaceExact(color.b));
			#endif
			float alpha = gradient.alphas[0].x;
			UNITY_UNROLL
			for (int a = 1; a < 8; a++)
			{
			float alphaPos = saturate((time - gradient.alphas[a-1].y) / ( 0.00001 + (gradient.alphas[a].y - gradient.alphas[a-1].y)) * step(a, (float)gradient.alphasLength-1));
			alpha = lerp(alpha, gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), gradient.type));
			}
			return float4(color, alpha);
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
			half mulTime175 = _Time.y * _3DNoiseTime;
			float3 ase_worldPos = i.worldPos;
			half simplePerlin3D2 = snoise( ( ( half3(0,2,0) * mulTime175 ) + ( half3(1,1,1) * ase_worldPos ) )*_NoiseScale );
			half2 temp_output_1_0_g36 = float2( 1,1 );
			half2 appendResult10_g36 = (half2(( (temp_output_1_0_g36).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g36).y )));
			half2 temp_output_11_0_g36 = float2( 0,0 );
			half2 panner18_g36 = ( ( (temp_output_11_0_g36).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g36 = ( ( _Time.y * (temp_output_11_0_g36).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g36 = (half2((panner18_g36).x , (panner19_g36).y));
			half2 appendResult109 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult110 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g36 = appendResult110;
			half2 appendResult205 = (half2(ase_worldPos.x , ase_worldPos.z));
			half2 temp_output_31_0_g36 = ( appendResult205 - float2( 1,1 ) );
			half2 appendResult39_g36 = (half2(frac( ( atan2( (temp_output_31_0_g36).x , (temp_output_31_0_g36).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g36 )));
			half2 panner54_g36 = ( ( (temp_output_47_0_g36).x * _Time.y ) * float2( 1,0 ) + appendResult39_g36);
			half2 panner55_g36 = ( ( _Time.y * (temp_output_47_0_g36).y ) * float2( 0,1 ) + appendResult39_g36);
			half2 appendResult58_g36 = (half2((panner54_g36).x , (panner55_g36).y));
			half simplePerlin3D23 = snoise( half3( ( ( (tex2D( _Sampler60107, ( appendResult10_g36 + appendResult24_g36 ) )).rg * 1.0 ) + ( appendResult109 * appendResult58_g36 ) ) ,  0.0 ) );
			half4 temp_cast_1 = (simplePerlin3D23).xxxx;
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			half4 temp_output_203_0 = ( ( simplePerlin3D2 + ( CalculateContrast(_2NoiseContrast,temp_cast_1) * _2NoiseStrength ) ) - tex2D( _MainTexture, uv_MainTexture ) );
			half3 ase_worldNormal = i.worldNormal;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_3 = (saturate( dotResult79 )).xxxx;
			half lightSteps77 = _LightSteps;
			half temp_output_3_0_g38 = lightSteps77;
			half4 temp_cast_4 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g37 = lightSteps77;
			half4 temp_output_87_0 = ( ( trunc( ( temp_cast_3 * temp_output_3_0_g38 ) ) / temp_output_3_0_g38 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_4 * temp_output_3_0_g37 ) ) / temp_output_3_0_g37 ) ) );
			half4 lightLerp129 = temp_output_87_0;
			half4 temp_cast_5 = (_RemapValues.x).xxxx;
			half4 temp_cast_6 = (_RemapValues.y).xxxx;
			half4 temp_cast_7 = (_RemapValues.z).xxxx;
			half4 temp_cast_8 = (_RemapValues.w).xxxx;
			half4 blendOpSrc146 = temp_output_203_0;
			half4 blendOpDest146 = (temp_cast_7 + (lightLerp129 - temp_cast_5) * (temp_cast_8 - temp_cast_7) / (temp_cast_6 - temp_cast_5));
			half4 lerpResult127 = lerp( CalculateContrast(_NoiseStrength,( ase_lightColor * temp_output_203_0 )) , ase_lightColor , ( saturate( ( blendOpSrc146 + blendOpDest146 ) )));
			Gradient gradient215 = NewGradient( 0, 2, 2, float4( 0.5, 0.09999999, 0.09999999, 0 ), float4( 1, 1, 1, 0.8 ), 0, 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			c.rgb = ( _Albedo * lerpResult127 * SampleGradient( gradient215, ( distance( ase_worldPos.y , _MaximumHeight1 ) / _MaximumHeight1 ) ) ).rgb;
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
-77;571;1906;971;1075.777;323.1134;1.809757;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-1671.033,1112.296;Inherit;False;2621.701;1098.188;Comment;25;99;97;96;95;94;93;92;89;88;87;86;85;84;83;82;81;79;78;77;76;75;74;73;129;157;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-418.6519,2076.66;Inherit;False;Property;_LightSteps;LightSteps;1;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;108;-2031.823,418.6461;Inherit;False;Property;_CircularNoiseTileAndSpeed;CircularNoiseTileAndSpeed;2;0;Create;True;0;0;0;False;0;False;0,10,0,0.1;0,0.12,0,-4;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;204;-1937.011,206.3642;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;176;-1674.1,-110.9249;Inherit;False;Property;_3DNoiseTime;3DNoiseTime;8;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;205;-1738.341,239.9241;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-1756.148,425.8065;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;-1745.408,541.5659;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-1643.835,1510.404;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-228.6818,2079.907;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;76;-1638.559,1347.393;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DotProductOpNode;79;-1348.575,1452.623;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1173.663,2005.921;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;178;-1470.012,138.5269;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector3Node;173;-1540.235,-260.5894;Inherit;False;Constant;_NoiseDirection;NoiseDirection;14;0;Create;True;0;0;0;False;0;False;0,2,0;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;107;-1555.417,319.0578;Inherit;False;RadialUVDistortion;-1;;36;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60107;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;0,1;False;47;FLOAT2;0,0.1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleTimeNode;175;-1508.239,-105.7242;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;177;-1456.183,-19.74344;Inherit;False;Constant;_3DNoiseScale;3DNoiseScale;14;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightAttenuation;74;-1401.082,1888.376;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-1120.138,273.9732;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-1268.886,-207.8575;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LightColorNode;84;-1160.811,1697.599;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;81;-1130.091,1438.294;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;82;-1034.718,1886.406;Inherit;False;PosterizeSteps;-1;;37;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1187.7,187.3486;Inherit;False;Property;_2NoiseContrast;2NoiseContrast;3;0;Create;True;0;0;0;False;0;False;1;6;1;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1118.93,1308.028;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;-1250.402,42.16244;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;112;-885.513,154.908;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;180;-1016.354,-140.6131;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-995.5376,-8.636336;Inherit;False;Property;_NoiseScale;NoiseScale;6;0;Create;True;0;0;0;False;0;False;1;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-829.3293,1680.33;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.FunctionNode;85;-922.9891,1419.601;Inherit;False;PosterizeSteps;-1;;38;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-857.3407,420.5849;Inherit;False;Property;_2NoiseStrength;2NoiseStrength;4;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-499.8978,1574.891;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-632.96,166.2483;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-808.0872,-193.2803;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-134.7174,1865.859;Inherit;False;lightLerp;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-291.1029,129.9831;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;168;-302.5768,-190.0771;Inherit;True;Property;_MainTexture;MainTexture;7;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;165;374.1145,494.7108;Inherit;False;Property;_RemapValues;RemapValues;5;0;Create;True;0;0;0;False;0;False;0,1,0,1.2;0,1,0,1.3;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;130;370.0045,269.5224;Inherit;True;129;lightLerp;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;203;15.87819,124.0302;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;163;487.7005,-169.6332;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;211;577.2672,-253.0941;Inherit;False;Property;_MaximumHeight1;MaximumHeight;10;0;Create;True;0;0;0;False;0;False;380;450;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;210;567.8325,-483.3914;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;208;605.9888,209.3771;Inherit;False;Property;_NoiseStrength;NoiseStrength;9;0;Create;True;0;0;0;False;0;False;1;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;672.501,-25.45712;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;160;618.723,391.4684;Inherit;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DistanceOpNode;212;760.6365,-378.1925;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;146;881.6108,264.8625;Inherit;True;LinearDodge;True;3;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT;1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;209;899.3914,127.4385;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;54;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;213;915.225,-365.3451;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;215;915.8618,-463.7813;Inherit;False;0;2;2;0.5,0.09999999,0.09999999,0;1,1,1,0.8;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.ColorNode;31;1238.289,-522.626;Inherit;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.6226415,0.6226415,0.6226415,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;214;1105.348,-303.9713;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;127;1141.528,-35.26228;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;896.6109,36.00267;Inherit;False;99;light;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.TruncOpNode;183;442.1727,-36.5502;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;557.8245,1618.214;Inherit;False;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;95;63.83377,1445.485;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;97;231.0608,1418.249;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;1477.894,-146.9422;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;92;-160.4678,1597.445;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-146.3008,1242.522;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;311.8076,-39.0279;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;93;-491.1676,1171.182;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;89;-645.9431,1423.855;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightAttenuation;88;-788.6608,1211.702;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;187;88.75759,-89.71177;Inherit;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;94;-455.1952,1391.657;Inherit;False;PosterizeSteps;-1;;39;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;275.312,1244.182;Inherit;False;lightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2066.999,-340.4758;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseTerrain;False;False;False;False;True;False;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;205;0;204;1
WireConnection;205;1;204;3
WireConnection;109;0;108;1
WireConnection;109;1;108;2
WireConnection;110;0;108;3
WireConnection;110;1;108;4
WireConnection;77;0;73;0
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;107;68;109;0
WireConnection;107;47;110;0
WireConnection;107;29;205;0
WireConnection;175;0;176;0
WireConnection;23;0;107;0
WireConnection;174;0;173;0
WireConnection;174;1;175;0
WireConnection;81;0;79;0
WireConnection;82;1;74;0
WireConnection;82;3;78;0
WireConnection;179;0;177;0
WireConnection;179;1;178;0
WireConnection;112;1;23;0
WireConnection;112;0;113;0
WireConnection;180;0;174;0
WireConnection;180;1;179;0
WireConnection;86;0;84;2
WireConnection;86;1;82;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;114;0;112;0
WireConnection;114;1;115;0
WireConnection;2;0;180;0
WireConnection;2;1;181;0
WireConnection;129;0;87;0
WireConnection;26;0;2;0
WireConnection;26;1;114;0
WireConnection;203;0;26;0
WireConnection;203;1;168;0
WireConnection;128;0;163;0
WireConnection;128;1;203;0
WireConnection;160;0;130;0
WireConnection;160;1;165;1
WireConnection;160;2;165;2
WireConnection;160;3;165;3
WireConnection;160;4;165;4
WireConnection;212;0;210;2
WireConnection;212;1;211;0
WireConnection;146;0;203;0
WireConnection;146;1;160;0
WireConnection;209;1;128;0
WireConnection;209;0;208;0
WireConnection;213;0;212;0
WireConnection;213;1;211;0
WireConnection;214;0;215;0
WireConnection;214;1;213;0
WireConnection;127;0;209;0
WireConnection;127;1;163;0
WireConnection;127;2;146;0
WireConnection;183;0;186;0
WireConnection;99;0;97;0
WireConnection;95;0;92;0
WireConnection;97;0;96;0
WireConnection;97;2;95;0
WireConnection;162;0;31;0
WireConnection;162;1;127;0
WireConnection;162;2;214;0
WireConnection;92;0;87;0
WireConnection;96;0;93;0
WireConnection;96;1;94;0
WireConnection;186;0;203;0
WireConnection;186;1;187;0
WireConnection;94;1;88;0
WireConnection;94;3;89;0
WireConnection;0;13;162;0
ASEEND*/
//CHKSM=6181AE32B7B7A0592F468F9F5137C4567E0B08F4