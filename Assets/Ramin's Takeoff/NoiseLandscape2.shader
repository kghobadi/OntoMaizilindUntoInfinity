// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseLandscape2"
{
	Properties
	{
		_ASEOutlineColor( "Outline Color", Color ) = (0,0,0,0)
		_ASEOutlineWidth( "Outline Width", Float ) = 1.5
		_Albedo("Albedo", Color) = (1,1,1,1)
		_RemapValues("RemapValues", Vector) = (0,1,0,1.2)
		_MainTexture("MainTexture", 2D) = "white" {}
		_NoiseScale("NoiseScale", Float) = 10
		_NoiseSpeed("NoiseSpeed", Vector) = (500,500,0,0)
		_Vector1("Vector 1", Vector) = (500,500,0,0)
		_NoiseTiling("NoiseTiling", Vector) = (500,500,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		
		
		
		
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
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
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

		uniform half4 _Albedo;
		uniform half2 _Vector1;
		uniform sampler2D _MainTexture;
		uniform half4 _MainTexture_ST;
		uniform half2 _NoiseTiling;
		uniform half2 _NoiseSpeed;
		uniform half _NoiseScale;
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
			c.rgb = 0;
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
			Gradient gradient218 = NewGradient( 0, 3, 2, float4( 0.1801745, 0.1967491, 0.6293655, 0 ), float4( 0.8679245, 0.08126056, 0, 0.2095369 ), float4( 1, 0.7305143, 0, 0.8278019 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			half2 panner298 = ( 1.0 * _Time.y * _Vector1 + float2( 0,0 ));
			float2 uv_TexCoord300 = i.uv_texcoord * half2( 5000,5000 ) + panner298;
			half simplePerlin2D301 = snoise( uv_TexCoord300 );
			half noise302 = simplePerlin2D301;
			float2 uv_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			half4 tex2DNode168 = tex2D( _MainTexture, uv_MainTexture );
			half temp_output_3_0_g44 = 4.0;
			half4 temp_output_201_0 = ( trunc( ( ( ( noise302 * 0.25 ) + tex2DNode168 ) * temp_output_3_0_g44 ) ) / temp_output_3_0_g44 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half2 panner224 = ( 1.0 * _Time.y * _NoiseSpeed + float2( 0,0 ));
			float2 uv_TexCoord211 = i.uv_texcoord * _NoiseTiling + panner224;
			half simplePerlin2D2 = snoise( uv_TexCoord211*_NoiseScale );
			half temp_output_223_0 = (-5.0 + (( saturate( ( noise302 * 1.0 ) ) + simplePerlin2D2 ) - -1.0) * (1.0 - -5.0) / (1.0 - -1.0));
			half3 ase_worldNormal = i.worldNormal;
			half3 ase_normWorldNormal = normalize( ase_worldNormal );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult79 = dot( ase_normWorldNormal , ase_worldlightDir );
			half temp_output_81_0 = saturate( dotResult79 );
			half lightLerp129 = ( temp_output_81_0 * ( ase_lightColor.a * 1 ) );
			half blendOpSrc146 = (_RemapValues.z + (lightLerp129 - _RemapValues.x) * (_RemapValues.w - _RemapValues.z) / (_RemapValues.y - _RemapValues.x));
			half blendOpDest146 = temp_output_223_0;
			half4 lerpResult127 = lerp( ( ase_lightColor * temp_output_223_0 ) , ase_lightColor , ( saturate( ( blendOpSrc146 + blendOpDest146 ) )));
			half4 temp_output_214_0 = ceil( lerpResult127 );
			half4 lerpResult215 = lerp( _Albedo , SampleGradient( gradient218, temp_output_201_0.x ) , ( saturate( temp_output_214_0 ) + float4( 0.2735849,0.2735849,0.2735849,0 ) ));
			o.Emission = lerpResult215.rgb;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
157;341;1906;781;779.7228;638.7689;1;True;False
Node;AmplifyShaderEditor.Vector2Node;297;-1330.515,-1006.35;Inherit;False;Property;_Vector1;Vector 1;7;0;Create;True;0;0;0;False;0;False;500,500;1,1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;299;-1080.872,-1137.83;Inherit;False;Constant;_Vector0;Vector 0;9;0;Create;True;0;0;0;False;0;False;5000,5000;5000,5000;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;298;-1104.757,-994.3079;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;72;-1605.506,1249.536;Inherit;False;2621.701;1098.188;Comment;25;99;97;96;95;94;93;92;89;88;87;86;85;84;83;82;81;79;78;77;76;75;74;73;129;157;LIGHT;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;300;-848.9232,-1065.231;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;100,500;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;301;-560.7549,-997.128;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldNormalVector;76;-1573.032,1484.634;Inherit;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;75;-1578.308,1647.643;Inherit;False;False;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;302;-139.9293,-1013.583;Inherit;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;228;-833.5276,-42.26625;Inherit;False;Property;_NoiseSpeed;NoiseSpeed;6;0;Create;True;0;0;0;False;0;False;500,500;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;74;-1335.555,2025.617;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;79;-1283.048,1589.863;Inherit;True;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;84;-1128.884,1807.64;Inherit;True;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;304;-209.4769,-242.026;Inherit;False;302;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;86;-763.8029,1817.571;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;227;-583.8847,-173.745;Inherit;False;Property;_NoiseTiling;NoiseTiling;8;0;Create;True;0;0;0;False;0;False;500,500;100,20;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SaturateNode;81;-1064.564,1575.534;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;224;-609.0825,-31.5367;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;307;-46.40652,-222.4616;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-434.3713,1712.132;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;177;-294.6146,205.352;Inherit;False;Property;_NoiseScale;NoiseScale;4;0;Create;True;0;0;0;False;0;False;10;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;211;-351.9355,-101.1471;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;100,500;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;308;81.27716,-177.7689;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-104.9342,33.12886;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;129;-69.19092,2003.099;Inherit;False;lightLerp;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;306;136.3652,-100.4117;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;130;54.88553,351.0738;Inherit;True;129;lightLerp;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector4Node;165;60.66453,563.4454;Inherit;False;Property;_RemapValues;RemapValues;2;0;Create;True;0;0;0;False;0;False;0,1,0,1.2;0.85,1,-3.37,0.7;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TFHCRemapNode;160;282.9171,385.7061;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;223;210.1863,39.0783;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.LightColorNode;163;427.1152,-121.9622;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;128;496.1183,25.81413;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;291;400.1284,-1042.045;Inherit;False;302;noise;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;146;590.1583,369.2965;Inherit;True;LinearDodge;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;168;633.9548,-761.6508;Inherit;True;Property;_MainTexture;MainTexture;3;0;Create;True;0;0;0;False;0;False;-1;None;1927b8fe9e82b304da74f1c6e88f7195;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;127;824.0451,49.54914;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;293;675.688,-1021.941;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.25;False;1;FLOAT;0
Node;AmplifyShaderEditor.CeilOpNode;214;1078.237,225.0477;Inherit;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;292;898.9426,-994.696;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;201;1087.378,-718.053;Inherit;False;PosterizeSteps;-1;;44;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;4;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;216;1301.918,-4.329153;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;218;1144.057,-914.4052;Inherit;False;0;3;2;0.1801745,0.1967491,0.6293655,0;0.8679245,0.08126056,0,0.2095369;1,0.7305143,0,0.8278019;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.ColorNode;31;765.0396,-212.2769;Inherit;False;Property;_Albedo;Albedo;0;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0.1820198,0.7735849,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;226;1462.296,-130.9343;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0.2735849,0.2735849,0.2735849,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;217;1420.233,-789.1135;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;232;784.5953,-325.862;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;238;-649.2445,120.8493;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GetLocalVarNode;83;-1053.403,1445.269;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;235;1298.273,-339.4263;Inherit;False;PosterizeSteps;-1;;50;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;4;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;157;268.0385,1369.723;Inherit;False;lightColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;89;-580.4166,1561.095;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldPosInputsNode;230;334.2029,-429.9083;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.FunctionNode;94;-389.6687,1528.896;Inherit;False;PosterizeSteps;-1;;51;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GradientNode;234;691.6901,-526.1586;Inherit;False;0;2;2;0,0.2352941,1,0.1000076;1,0.09411766,0,0.9000076;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.LightAttenuation;88;-723.1343,1348.943;Inherit;True;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;95;129.3603,1582.725;Inherit;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GradientSampleNode;233;962.6476,-372.1244;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;82;-969.1915,2023.646;Inherit;False;PosterizeSteps;-1;;52;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-80.77431,1379.762;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;196;981.5721,-564.1363;Inherit;False;Property;_MainTexIntensity;MainTexIntensity;5;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;236;387.2726,-512.4263;Inherit;False;Property;_MinHeight;MinHeight;10;0;Create;True;0;0;0;False;0;False;28.4;90.7;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;195;1296.261,-652.0348;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;215;1522.14,-292.7464;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;240;-416.537,114.2498;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;99;623.351,1755.454;Inherit;False;light;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LightColorNode;93;-425.6411,1308.423;Inherit;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;92;-94.94134,1734.685;Inherit;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;77;-163.1553,2217.147;Inherit;False;lightSteps;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;104;651.9298,279.4539;Inherit;False;157;lightColor;1;0;OBJECT;;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;249;1119.708,-819.2201;Inherit;False;tex;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.FunctionNode;85;-857.4626,1556.842;Inherit;False;PosterizeSteps;-1;;53;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.LerpOp;97;296.5873,1555.489;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;73;-353.1254,2213.901;Inherit;False;Property;_LightSteps;LightSteps;1;0;Create;True;0;0;0;False;0;False;4;120;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;78;-1108.136,2143.162;Inherit;False;77;lightSteps;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;237;513.0717,-400.8665;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;213;1524.456,145.6596;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;229;446.6375,-213.6109;Inherit;False;Property;_MaximumHeight;MaximumHeight;9;0;Create;True;0;0;0;False;0;False;380;80.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DistanceOpNode;231;630.0069,-338.7095;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1827.484,-281.799;Half;False;True;-1;6;ASEMaterialInspector;0;0;CustomLighting;Julian/NoiseLandscape2;False;False;False;False;True;False;True;True;True;False;True;True;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;True;1.5;0,0,0,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;298;2;297;0
WireConnection;300;0;299;0
WireConnection;300;1;298;0
WireConnection;301;0;300;0
WireConnection;302;0;301;0
WireConnection;79;0;76;0
WireConnection;79;1;75;0
WireConnection;86;0;84;2
WireConnection;86;1;74;0
WireConnection;81;0;79;0
WireConnection;224;2;228;0
WireConnection;307;0;304;0
WireConnection;87;0;81;0
WireConnection;87;1;86;0
WireConnection;211;0;227;0
WireConnection;211;1;224;0
WireConnection;308;0;307;0
WireConnection;2;0;211;0
WireConnection;2;1;177;0
WireConnection;129;0;87;0
WireConnection;306;0;308;0
WireConnection;306;1;2;0
WireConnection;160;0;130;0
WireConnection;160;1;165;1
WireConnection;160;2;165;2
WireConnection;160;3;165;3
WireConnection;160;4;165;4
WireConnection;223;0;306;0
WireConnection;128;0;163;0
WireConnection;128;1;223;0
WireConnection;146;0;160;0
WireConnection;146;1;223;0
WireConnection;127;0;128;0
WireConnection;127;1;163;0
WireConnection;127;2;146;0
WireConnection;293;0;291;0
WireConnection;214;0;127;0
WireConnection;292;0;293;0
WireConnection;292;1;168;0
WireConnection;201;1;292;0
WireConnection;216;0;214;0
WireConnection;226;0;216;0
WireConnection;217;0;218;0
WireConnection;217;1;201;0
WireConnection;232;0;231;0
WireConnection;232;1;229;0
WireConnection;235;1;233;0
WireConnection;94;1;88;0
WireConnection;94;3;89;0
WireConnection;95;0;92;0
WireConnection;233;0;234;0
WireConnection;233;1;232;0
WireConnection;82;1;74;0
WireConnection;82;3;78;0
WireConnection;96;0;93;0
WireConnection;96;1;88;0
WireConnection;195;0;201;0
WireConnection;195;1;196;0
WireConnection;215;0;31;0
WireConnection;215;1;217;0
WireConnection;215;2;226;0
WireConnection;240;0;238;1
WireConnection;240;1;238;3
WireConnection;77;0;73;0
WireConnection;249;0;168;0
WireConnection;85;1;81;0
WireConnection;85;3;83;0
WireConnection;97;0;96;0
WireConnection;97;2;92;0
WireConnection;237;0;236;0
WireConnection;237;1;230;2
WireConnection;213;2;214;0
WireConnection;231;0;237;0
WireConnection;231;1;229;0
WireConnection;0;2;215;0
ASEEND*/
//CHKSM=FC2133A8897317D68F3A388EEB985563CD60E2C5