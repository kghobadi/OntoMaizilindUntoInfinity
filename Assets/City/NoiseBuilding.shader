// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseBuilding"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.01
		_LightSteps("LightSteps", Float) = 4
		_CircularNoiseTileAndSpeed("CircularNoiseTileAndSpeed", Vector) = (0,10,0,0.1)
		_2NoiseContrast("2NoiseContrast", Range( 0 , 6)) = 1
		_2NoiseStrength("2NoiseStrength", Range( 0 , 1)) = 1
		_RemapValues("RemapValues", Vector) = (0,1,0,1.2)
		_MainTexture("MainTexture", 2D) = "white" {}
		_NoiseScale("NoiseScale", Float) = 10
		_DetailTexture("DetailTexture", 2D) = "white" {}
		_3DNoiseTime("3DNoiseTime", Float) = 0.2
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
		struct Input {
			half filler;
		};
		half4 _ASEOutlineColor;
		half _ASEOutlineWidth;
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			v.vertex.xyz += ( v.normal * _ASEOutlineWidth );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			o.Emission = _ASEOutlineColor.rgb;
			o.Alpha = 1;
		}
		ENDCG
		

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
			float2 uv_texcoord;
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

		uniform sampler2D _MainTexture;
		uniform half4 _MainTexture_ST;
		uniform half _3DNoiseTime;
		uniform half _NoiseScale;
		uniform half _2NoiseContrast;
		sampler2D _Sampler60107;
		uniform half4 _CircularNoiseTileAndSpeed;
		uniform half _2NoiseStrength;
		uniform sampler2D _DetailTexture;
		uniform half4 _DetailTexture_ST;
		uniform half _LightSteps;
		uniform half4 _RemapValues;


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


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
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
			Gradient gradient195 = NewGradient( 1, 3, 2, float4( 0.8000001, 0.509804, 0, 0.3300069 ), float4( 0.6, 0.3854015, 0, 0.6599985 ), float4( 0.4, 0.253125, 0, 1 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			half3 objToWorld198 = mul( unity_ObjectToWorld, float4( float3( 0,0,0 ), 1 ) ).xyz;
			half2 appendResult194 = (half2(objToWorld198.x , objToWorld198.z));
			half dotResult4_g48 = dot( appendResult194 , half2( 12.9898,78.233 ) );
			half lerpResult10_g48 = lerp( 0.0 , 1.0 , frac( ( sin( dotResult4_g48 ) * 43758.55 ) ));
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			half mulTime172 = _Time.y * _3DNoiseTime;
			float3 ase_worldPos = i.worldPos;
			half simplePerlin3D2 = snoise( ( ( half3(0.3,2,0.3) * mulTime172 ) + ase_worldPos )*_NoiseScale );
			half2 temp_output_1_0_g46 = float2( 1,1 );
			half2 appendResult10_g46 = (half2(( (temp_output_1_0_g46).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g46).y )));
			half2 temp_output_11_0_g46 = float2( 0,0 );
			half2 panner18_g46 = ( ( (temp_output_11_0_g46).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g46 = ( ( _Time.y * (temp_output_11_0_g46).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g46 = (half2((panner18_g46).x , (panner19_g46).y));
			half2 appendResult109 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult110 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g46 = appendResult110;
			half2 appendResult188 = (half2(ase_worldPos.x , ase_worldPos.z));
			half2 temp_output_31_0_g46 = ( appendResult188 - float2( 1,1 ) );
			half2 appendResult39_g46 = (half2(frac( ( atan2( (temp_output_31_0_g46).x , (temp_output_31_0_g46).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g46 )));
			half2 panner54_g46 = ( ( (temp_output_47_0_g46).x * _Time.y ) * float2( 1,0 ) + appendResult39_g46);
			half2 panner55_g46 = ( ( _Time.y * (temp_output_47_0_g46).y ) * float2( 0,1 ) + appendResult39_g46);
			half2 appendResult58_g46 = (half2((panner54_g46).x , (panner55_g46).y));
			half simplePerlin2D23 = snoise( ( ( (tex2D( _Sampler60107, ( appendResult10_g46 + appendResult24_g46 ) )).rg * 1.0 ) + ( appendResult109 * appendResult58_g46 ) ) );
			half4 temp_cast_0 = (simplePerlin2D23).xxxx;
			float2 uv_DetailTexture = i.uv_texcoord * _DetailTexture_ST.xy + _DetailTexture_ST.zw;
			half4 temp_output_179_0 = ( ( simplePerlin3D2 + ( CalculateContrast(_2NoiseContrast,temp_cast_0) * _2NoiseStrength ) ) - tex2D( _DetailTexture, uv_DetailTexture ) );
			half3 ase_worldNormal = i.worldNormal;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_1 = (saturate( dotResult79 )).xxxx;
			half lightSteps77 = _LightSteps;
			half temp_output_3_0_g47 = lightSteps77;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half4 temp_cast_2 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g45 = lightSteps77;
			half4 lightLerp129 = ( ( trunc( ( temp_cast_1 * temp_output_3_0_g47 ) ) / temp_output_3_0_g47 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_2 * temp_output_3_0_g45 ) ) / temp_output_3_0_g45 ) ) );
			half4 temp_cast_3 = (_RemapValues.x).xxxx;
			half4 temp_cast_4 = (_RemapValues.y).xxxx;
			half4 temp_cast_5 = (_RemapValues.z).xxxx;
			half4 temp_cast_6 = (_RemapValues.w).xxxx;
			half4 blendOpSrc146 = (temp_cast_5 + (lightLerp129 - temp_cast_3) * (temp_cast_6 - temp_cast_5) / (temp_cast_4 - temp_cast_3));
			half4 blendOpDest146 = temp_output_179_0;
			half4 lerpResult190 = lerp( temp_output_179_0 , float4( 1,1,1,0 ) , ( saturate( ( blendOpSrc146 + blendOpDest146 ) )));
			c.rgb = ( SampleGradient( gradient195, lerpResult10_g48 ) * tex2D( _MainTexture, uv_MainTexture ) * lerpResult190 * ase_lightColor ).rgb;
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
195;562;1906;899;928.4617;720.8687;1.510638;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-1671.033,1112.296;Inherit;False;2621.701;1098.188;Comment;15;87;86;85;84;83;82;81;79;78;77;76;75;74;73;129;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-418.6519,2076.66;Inherit;False;Property;_LightSteps;LightSteps;1;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;186;-1634.472,429.3529;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector4Node;108;-1641.47,601.7261;Inherit;False;Property;_CircularNoiseTileAndSpeed;CircularNoiseTileAndSpeed;2;0;Create;True;0;0;0;False;0;False;0,10,0,0.1;0,0.05,0,12;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;76;-1638.559,1347.393;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-228.6818,2079.907;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-1643.835,1510.404;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1173.663,2005.921;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;-1355.055,724.6459;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LightAttenuation;74;-1401.082,1888.376;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;169;-1374.988,11.27485;Inherit;False;Property;_3DNoiseTime;3DNoiseTime;9;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-1365.795,608.8865;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;188;-1409.842,451.8933;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DotProductOpNode;79;-1348.575,1452.623;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1118.93,1308.028;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;82;-1034.718,1886.406;Inherit;False;PosterizeSteps;-1;;45;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;81;-1130.091,1438.294;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;172;-1209.127,16.47555;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;107;-1153.757,576.4424;Inherit;False;RadialUVDistortion;-1;;46;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60107;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;5,5;False;47;FLOAT2;0,0.1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;170;-1241.123,-138.3896;Inherit;False;Constant;_NoiseDirection;NoiseDirection;14;0;Create;True;0;0;0;False;0;False;0.3,2,0.3;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightColorNode;84;-1160.811,1697.599;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.FunctionNode;85;-922.9891,1419.601;Inherit;True;PosterizeSteps;-1;;47;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-829.3293,1680.33;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-786.0399,444.733;Inherit;False;Property;_2NoiseContrast;2NoiseContrast;3;0;Create;True;0;0;0;False;0;False;1;1;0;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-969.7739,-85.65772;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WorldPosInputsNode;171;-1024.698,94.51849;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-718.4778,531.3578;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-696.4246,113.5635;Inherit;False;Property;_NoiseScale;NoiseScale;7;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-717.2408,-18.41335;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;1,1,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;112;-483.8524,412.2925;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-455.6801,677.9695;Inherit;False;Property;_2NoiseStrength;2NoiseStrength;4;0;Create;True;0;0;0;False;0;False;1;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-499.8978,1574.891;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-197.3556,1624.531;Inherit;True;lightLerp;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-406.4266,64.10424;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-231.2995,423.6328;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TransformPositionNode;198;105.3728,-578.8005;Inherit;False;Object;World;False;Fast;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;130;120.0563,367.5569;Inherit;True;129;lightLerp;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;165;135.8353,564.9285;Inherit;False;Property;_RemapValues;RemapValues;5;0;Create;True;0;0;0;False;0;False;0,1,0,1.2;0,0.6,0,1.6;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;178;-60.332,-142.4963;Inherit;True;Property;_DetailTexture;DetailTexture;8;0;Create;True;0;0;0;False;0;False;-1;None;59578a8d90747c2489d0ebc3f66ca605;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;26;5.99819,133.4378;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;179;257.7056,92.27599;Inherit;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;160;353.5339,355.0701;Inherit;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DynamicAppendNode;194;357.9934,-559.5229;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GradientNode;195;705.1998,-627.6682;Inherit;False;1;3;2;0.8000001,0.509804,0,0.3300069;0.6,0.3854015,0,0.6599985;0.4,0.253125,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.BlendOpsNode;146;632.6125,298.4711;Inherit;True;LinearDodge;True;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;191;542.6517,-536.1615;Inherit;False;Random Range;-1;;48;7b754edb8aebbfb4a9ace907af661cfc;0;3;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;196;910.5768,-552.2765;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;168;764.2002,-178.9301;Inherit;True;Property;_MainTexture;MainTexture;6;0;Create;True;0;0;0;False;0;False;-1;None;ed07a62e2614afc4c9dbb7fbf9e542b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;190;914.5134,166.179;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;163;909.7609,26.87189;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;1181.329,78.40897;Inherit;True;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;31;845.8781,-357.0184;Inherit;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.8,0.5108911,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1492.199,-107.1758;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseBuilding;False;False;False;False;True;False;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;73;0
WireConnection;110;0;108;3
WireConnection;110;1;108;4
WireConnection;109;0;108;1
WireConnection;109;1;108;2
WireConnection;188;0;186;1
WireConnection;188;1;186;3
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;82;1;74;0
WireConnection;82;3;78;0
WireConnection;81;0;79;0
WireConnection;172;0;169;0
WireConnection;107;68;109;0
WireConnection;107;47;110;0
WireConnection;107;29;188;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;86;0;84;2
WireConnection;86;1;82;0
WireConnection;174;0;170;0
WireConnection;174;1;172;0
WireConnection;23;0;107;0
WireConnection;176;0;174;0
WireConnection;176;1;171;0
WireConnection;112;1;23;0
WireConnection;112;0;113;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;129;0;87;0
WireConnection;2;0;176;0
WireConnection;2;1;177;0
WireConnection;114;0;112;0
WireConnection;114;1;115;0
WireConnection;26;0;2;0
WireConnection;26;1;114;0
WireConnection;179;0;26;0
WireConnection;179;1;178;0
WireConnection;160;0;130;0
WireConnection;160;1;165;1
WireConnection;160;2;165;2
WireConnection;160;3;165;3
WireConnection;160;4;165;4
WireConnection;194;0;198;1
WireConnection;194;1;198;3
WireConnection;146;0;160;0
WireConnection;146;1;179;0
WireConnection;191;1;194;0
WireConnection;196;0;195;0
WireConnection;196;1;191;0
WireConnection;190;0;179;0
WireConnection;190;2;146;0
WireConnection;189;0;196;0
WireConnection;189;1;168;0
WireConnection;189;2;190;0
WireConnection;189;3;163;0
WireConnection;0;13;189;0
ASEEND*/
//CHKSM=0172B4E1A7E433C166E5D4C7DABBFC5F788913A7