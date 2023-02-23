// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseBuilding"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 0.01
		_Detail("Detail", 2D) = "white" {}
		_Albedo("Albedo", Color) = (1,1,1,1)
		_LightSteps("LightSteps", Float) = 4
		_CircularNoiseTileAndSpeed("CircularNoiseTileAndSpeed", Vector) = (0,10,0,0.1)
		_2NoiseContrast("2NoiseContrast", Range( 1 , 6)) = 1
		_2NoiseStrength("2NoiseStrength", Range( 0 , 1)) = 1
		_RemapValues("RemapValues", Vector) = (0,1,0,1.2)
		_MainTexture("MainTexture", 2D) = "white" {}
		_NoiseScale("NoiseScale", Float) = 10
		_3DNoiseTime("3DNoiseTime", Float) = 0.2
		_DetailTiling("DetailTiling", Vector) = (1,1,0,0)
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
			float3 worldPos;
			float2 uv_texcoord;
			half3 worldNormal;
			INTERNAL_DATA
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
		uniform half _2NoiseContrast;
		sampler2D _Sampler60107;
		uniform half4 _CircularNoiseTileAndSpeed;
		uniform half _2NoiseStrength;
		sampler2D _Detail;
		uniform half2 _DetailTiling;
		uniform half _LightSteps;
		uniform half4 _RemapValues;
		uniform sampler2D _MainTexture;
		uniform half4 _MainTexture_ST;


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

		inline float4 TriplanarSampling183( sampler2D topTexMap, float3 worldPos, float3 worldNormal, float falloff, float2 tiling, float3 normalScale, float3 index )
		{
			float3 projNormal = ( pow( abs( worldNormal ), falloff ) );
			projNormal /= ( projNormal.x + projNormal.y + projNormal.z ) + 0.00001;
			float3 nsign = sign( worldNormal );
			half4 xNorm; half4 yNorm; half4 zNorm;
			xNorm = tex2D( topTexMap, tiling * worldPos.zy * float2(  nsign.x, 1.0 ) );
			yNorm = tex2D( topTexMap, tiling * worldPos.xz * float2(  nsign.y, 1.0 ) );
			zNorm = tex2D( topTexMap, tiling * worldPos.xy * float2( -nsign.z, 1.0 ) );
			return xNorm * projNormal.x + yNorm * projNormal.y + zNorm * projNormal.z;
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
			half2 temp_output_1_0_g34 = float2( 1,1 );
			half2 appendResult10_g34 = (half2(( (temp_output_1_0_g34).x * i.uv_texcoord.x ) , ( i.uv_texcoord.y * (temp_output_1_0_g34).y )));
			half2 temp_output_11_0_g34 = float2( 0,0 );
			half2 panner18_g34 = ( ( (temp_output_11_0_g34).x * _Time.y ) * float2( 1,0 ) + i.uv_texcoord);
			half2 panner19_g34 = ( ( _Time.y * (temp_output_11_0_g34).y ) * float2( 0,1 ) + i.uv_texcoord);
			half2 appendResult24_g34 = (half2((panner18_g34).x , (panner19_g34).y));
			half2 appendResult109 = (half2(_CircularNoiseTileAndSpeed.x , _CircularNoiseTileAndSpeed.y));
			half2 appendResult110 = (half2(_CircularNoiseTileAndSpeed.z , _CircularNoiseTileAndSpeed.w));
			half2 temp_output_47_0_g34 = appendResult110;
			half2 appendResult188 = (half2(ase_worldPos.x , ase_worldPos.z));
			half2 temp_output_31_0_g34 = ( appendResult188 - float2( 1,1 ) );
			half2 appendResult39_g34 = (half2(frac( ( atan2( (temp_output_31_0_g34).x , (temp_output_31_0_g34).y ) / 6.28318548202515 ) ) , length( temp_output_31_0_g34 )));
			half2 panner54_g34 = ( ( (temp_output_47_0_g34).x * _Time.y ) * float2( 1,0 ) + appendResult39_g34);
			half2 panner55_g34 = ( ( _Time.y * (temp_output_47_0_g34).y ) * float2( 0,1 ) + appendResult39_g34);
			half2 appendResult58_g34 = (half2((panner54_g34).x , (panner55_g34).y));
			half simplePerlin3D23 = snoise( half3( ( ( (tex2D( _Sampler60107, ( appendResult10_g34 + appendResult24_g34 ) )).rg * 1.0 ) + ( appendResult109 * appendResult58_g34 ) ) ,  0.0 ) );
			half4 temp_cast_1 = (simplePerlin3D23).xxxx;
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float4 triplanar183 = TriplanarSampling183( _Detail, ase_worldPos, ase_worldNormal, 1.0, _DetailTiling, 1.0, 0 );
			half4 temp_output_179_0 = ( ( simplePerlin3D2 + ( CalculateContrast(_2NoiseContrast,temp_cast_1) * _2NoiseStrength ) ) - triplanar183 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_worldNormal , ase_worldlightDir );
			half4 temp_cast_3 = (saturate( dotResult79 )).xxxx;
			half lightSteps77 = _LightSteps;
			half temp_output_3_0_g40 = lightSteps77;
			half4 temp_cast_4 = (ase_lightAtten).xxxx;
			half temp_output_3_0_g39 = lightSteps77;
			half4 temp_output_87_0 = ( ( trunc( ( temp_cast_3 * temp_output_3_0_g40 ) ) / temp_output_3_0_g40 ) * ( ase_lightColor.a * ( trunc( ( temp_cast_4 * temp_output_3_0_g39 ) ) / temp_output_3_0_g39 ) ) );
			half4 lightLerp129 = temp_output_87_0;
			half4 temp_cast_5 = (_RemapValues.x).xxxx;
			half4 temp_cast_6 = (_RemapValues.y).xxxx;
			half4 temp_cast_7 = (_RemapValues.z).xxxx;
			half4 temp_cast_8 = (_RemapValues.w).xxxx;
			half4 temp_output_160_0 = (temp_cast_7 + (lightLerp129 - temp_cast_5) * (temp_cast_8 - temp_cast_7) / (temp_cast_6 - temp_cast_5));
			half4 blendOpSrc146 = temp_output_160_0;
			half4 blendOpDest146 = temp_output_179_0;
			half4 lerpResult127 = lerp( ( ase_lightColor * temp_output_179_0 ) , ase_lightColor , ( saturate( ( blendOpSrc146 + blendOpDest146 ) )));
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			c.rgb = ( _Albedo * lerpResult127 * tex2D( _MainTexture, uv_MainTexture ) ).rgb;
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
			o.Normal = float3(0,0,1);
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
288;725;1906;941;2065.146;125.227;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;72;-1671.033,1112.296;Inherit;False;2621.701;1098.188;Comment;25;99;97;96;95;94;93;92;89;88;87;86;85;84;83;82;81;79;78;77;76;75;74;73;129;157;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-418.6519,2076.66;Inherit;False;Property;_LightSteps;LightSteps;6;0;Create;True;0;0;0;False;0;False;4;4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;186;-2153.057,555.5624;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector4Node;108;-1927.264,672.5759;Inherit;False;Property;_CircularNoiseTileAndSpeed;CircularNoiseTileAndSpeed;7;0;Create;True;0;0;0;False;0;False;0,10,0,0.1;0,0.05,0,12;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;76;-1638.559,1347.393;Inherit;False;False;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-228.6818,2079.907;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-1643.835,1510.404;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1173.663,2005.921;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;109;-1651.589,679.7363;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;169;-1688.242,130.5841;Inherit;False;Property;_3DNoiseTime;3DNoiseTime;15;0;Create;True;0;0;0;False;0;False;0.2;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;79;-1348.575,1452.623;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;110;-1640.849,795.4957;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LightAttenuation;74;-1401.082,1888.376;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;188;-1897.736,528.0278;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;82;-1034.718,1886.406;Inherit;False;PosterizeSteps;-1;;39;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.WorldPosInputsNode;171;-1484.154,380.0359;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SaturateNode;81;-1130.091,1438.294;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;170;-1554.377,-19.08035;Inherit;False;Constant;_NoiseDirection;NoiseDirection;14;0;Create;True;0;0;0;False;0;False;0.3,2,0.3;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.LightColorNode;84;-1160.811,1697.599;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.Vector3Node;173;-1470.325,221.7656;Inherit;False;Constant;_3DNoiseScale;3DNoiseScale;14;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;107;-1450.858,572.9877;Inherit;False;RadialUVDistortion;-1;;34;051d65e7699b41a4c800363fd0e822b2;0;7;60;SAMPLER2D;_Sampler60107;False;1;FLOAT2;1,1;False;11;FLOAT2;0,0;False;65;FLOAT;1;False;68;FLOAT2;5,5;False;47;FLOAT2;0,0.1;False;29;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1118.93,1308.028;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;172;-1522.381,135.7848;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-829.3293,1680.33;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;175;-1264.544,283.6714;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;174;-1283.028,33.65154;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;113;-1083.141,441.2784;Inherit;False;Property;_2NoiseContrast;2NoiseContrast;8;0;Create;True;0;0;0;False;0;False;1;2;1;6;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;85;-922.9891,1419.601;Inherit;False;PosterizeSteps;-1;;40;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-1015.579,527.9031;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-499.8978,1574.891;Inherit;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;176;-1030.495,100.8959;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;115;-752.7813,674.5148;Inherit;False;Property;_2NoiseStrength;2NoiseStrength;9;0;Create;True;0;0;0;False;0;False;1;0.8;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;112;-780.9536,408.8379;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-1009.679,232.8727;Inherit;False;Property;_NoiseScale;NoiseScale;13;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-703.5278,60.64958;Inherit;True;Simplex3D;False;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;114;-528.4006,420.1782;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.Vector2Node;185;-654.7039,-311.697;Inherit;False;Property;_DetailTiling;DetailTiling;16;0;Create;True;0;0;0;False;0;False;1,1;0.04,0.04;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-134.7174,1865.859;Inherit;False;lightLerp;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector4Node;165;130.8353,603.9285;Inherit;False;Property;_RemapValues;RemapValues;11;0;Create;True;0;0;0;False;0;False;0,1,0,1.2;0,0.6,0,1.6;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;130;125.0563,391.5569;Inherit;True;129;lightLerp;1;0;OBJECT;;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-291.1029,129.9831;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TriplanarNode;183;-444.348,-338.8212;Inherit;True;Spherical;World;False;Detail;_Detail;white;0;None;Mid Texture 0;_MidTexture0;white;-1;None;Bot Texture 0;_BotTexture0;white;-1;None;Triplanar Sampler;Tangent;10;0;SAMPLER2D;;False;5;FLOAT;1;False;1;SAMPLER2D;;False;6;FLOAT;0;False;2;SAMPLER2D;;False;7;FLOAT;0;False;9;FLOAT3;0,0,0;False;8;FLOAT;1;False;3;FLOAT2;1,1;False;4;FLOAT;1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;163;360.961,-222.7281;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.TFHCRemapNode;160;362.5339,472.0701;Inherit;True;5;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;2;FLOAT4;1,1,1,1;False;3;FLOAT4;0,0,0,0;False;4;FLOAT4;1,1,1,1;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;179;-39.3955,88.82133;Inherit;True;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;146;685.9683,382.7909;Inherit;True;LinearDodge;True;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;550.1737,-26.43597;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1748.793,-413.3993;Inherit;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;127;846.0188,41.74914;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;31;845.8781,-357.0184;Inherit;False;Property;_Albedo;Albedo;4;0;Create;True;0;0;0;False;0;False;1,1,1,1;0.4,0.253125,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;11;-1653.051,-1125.017;Inherit;False;871.1336;510.3396;;10;10;30;9;29;8;28;34;38;39;40;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.SamplerNode;168;764.2002,-178.9301;Inherit;True;Property;_MainTexture;MainTexture;12;0;Create;True;0;0;0;False;0;False;-1;None;ed07a62e2614afc4c9dbb7fbf9e542b0;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;162;1157.894,-143.7422;Inherit;True;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;29;-1363.624,-1075.31;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1368.022,-959.0023;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;557.8245,1618.214;Inherit;False;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1698.793,-363.3991;Inherit;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;38;-1355.632,-842.5486;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.OneMinusNode;92;-160.4678,1597.445;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ColorNode;126;1095.531,-492.1125;Inherit;False;Property;_EmissionColor;EmissionColor;10;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StaticSwitch;34;-1074.662,-843.066;Inherit;False;Property;_NoiseAlignment;NoiseAlignment;5;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;5;AlignOnX;AlignOnZ;VerticalHorizontal;VerticalVertical;TextureCoordinates;Create;True;True;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;28;-1602.423,-1084.21;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;14;-1678.819,-224.9958;Float;False;Property;_WaveSize;WaveSize;2;0;Create;True;0;0;0;False;0;False;1,1;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TruncOpNode;181;354.9127,-14.67989;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;95;63.83377,1445.485;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1160.749,-330.1563;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LightColorNode;93;-491.1676,1171.182;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-1607.151,-940.017;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;40;-1632.98,-763.6873;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;104;614.581,214.4938;Inherit;False;157;lightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;180;230.1183,3.466379;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.LightAttenuation;88;-788.6608,1211.702;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-645.9431,1423.855;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-146.3008,1242.522;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;202.512,1232.482;Inherit;False;lightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;189;706.7968,631.0675;Inherit;True;HardMix;True;3;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-968.5618,-335.771;Float;False;WaveTileUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.FunctionNode;94;-455.1952,1391.657;Inherit;False;PosterizeSteps;-1;;41;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;97;231.0608,1418.249;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;39;-1358.833,-729.885;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1440.537,-316.3137;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ToggleSwitchNode;30;-1200.635,-987.1863;Inherit;False;Property;_AlignOnWorldX;AlignOnWorldX;3;0;Create;True;0;0;0;False;0;False;1;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1326.441,-162.1133;Float;False;Property;_WaveTile;WaveTile;1;0;Create;True;0;0;0;False;0;False;1;10000;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;178;-326.4331,-131.951;Inherit;True;Property;_DetailTexture;DetailTexture;14;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;182;72.08622,-67.10966;Inherit;False;Constant;_Float0;Float 0;14;0;Create;True;0;0;0;False;0;False;2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-974.3055,-986.2117;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1460.999,-306.0758;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseBuilding;False;False;False;False;True;False;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;0.01;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;77;0;73;0
WireConnection;109;0;108;1
WireConnection;109;1;108;2
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;110;0;108;3
WireConnection;110;1;108;4
WireConnection;188;0;186;1
WireConnection;188;1;186;3
WireConnection;82;1;74;0
WireConnection;82;3;78;0
WireConnection;81;0;79;0
WireConnection;107;68;109;0
WireConnection;107;47;110;0
WireConnection;107;29;188;0
WireConnection;172;0;169;0
WireConnection;86;0;84;2
WireConnection;86;1;82;0
WireConnection;175;0;173;0
WireConnection;175;1;171;0
WireConnection;174;0;170;0
WireConnection;174;1;172;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;23;0;107;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;176;0;174;0
WireConnection;176;1;175;0
WireConnection;112;1;23;0
WireConnection;112;0;113;0
WireConnection;2;0;176;0
WireConnection;2;1;177;0
WireConnection;114;0;112;0
WireConnection;114;1;115;0
WireConnection;129;0;87;0
WireConnection;26;0;2;0
WireConnection;26;1;114;0
WireConnection;183;3;185;0
WireConnection;160;0;130;0
WireConnection;160;1;165;1
WireConnection;160;2;165;2
WireConnection;160;3;165;3
WireConnection;160;4;165;4
WireConnection;179;0;26;0
WireConnection;179;1;183;0
WireConnection;146;0;160;0
WireConnection;146;1;179;0
WireConnection;128;0;163;0
WireConnection;128;1;179;0
WireConnection;127;0;128;0
WireConnection;127;1;163;0
WireConnection;127;2;146;0
WireConnection;162;0;31;0
WireConnection;162;1;127;0
WireConnection;162;2;168;0
WireConnection;29;0;28;3
WireConnection;29;1;28;1
WireConnection;9;0;8;1
WireConnection;9;1;8;3
WireConnection;99;0;97;0
WireConnection;38;0;8;2
WireConnection;38;1;8;3
WireConnection;92;0;87;0
WireConnection;34;1;29;0
WireConnection;34;0;9;0
WireConnection;34;2;38;0
WireConnection;34;3;39;0
WireConnection;34;4;40;0
WireConnection;181;0;180;0
WireConnection;95;0;92;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;180;0;179;0
WireConnection;180;1;182;0
WireConnection;96;0;93;0
WireConnection;96;1;94;0
WireConnection;157;0;96;0
WireConnection;189;0;160;0
WireConnection;189;1;179;0
WireConnection;20;0;15;0
WireConnection;94;1;88;0
WireConnection;94;3;89;0
WireConnection;97;0;96;0
WireConnection;97;2;92;0
WireConnection;39;0;8;3
WireConnection;39;1;8;2
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;30;0;9;0
WireConnection;30;1;29;0
WireConnection;10;0;34;0
WireConnection;0;13;162;0
ASEEND*/
//CHKSM=A57F1D3563CB60B83F3D9AE493E94DF9FC4F2482