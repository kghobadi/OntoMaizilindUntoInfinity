// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TrippyClouds"
{
	Properties
	{
		_Length1("Length", Float) = 1000
		_Offset1("Offset", Float) = 0
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 8
		_TessMin( "Tess Min Distance", Float ) = 0
		_TessMax( "Tess Max Distance", Float ) = 1000
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveTile("WaveTile", Float) = 1
		_WaveSize("WaveSize", Vector) = (1,1,0,0)
		_OffsetStrength("OffsetStrength", Float) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf Unlit keepalpha noshadow exclude_path:deferred nofog vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
		};

		uniform float _WaveSpeed;
		uniform float2 _WaveSize;
		uniform float _WaveTile;
		uniform half _OffsetStrength;
		uniform half _Length1;
		uniform half _Offset1;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;


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


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half4 appendResult9 = (half4(ase_worldPos.z , ase_worldPos.x , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * half4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			half simplePerlin2D2 = snoise( panner3 );
			v.vertex.xyz += ( float3(0,1,0) * simplePerlin2D2 * _OffsetStrength );
			v.vertex.w = 1;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient45 = NewGradient( 0, 4, 2, float4( 0, 0.116883, 0.5, 0 ), float4( 1, 0.09411766, 0, 0.4107881 ), float4( 0.9490196, 0.8693196, 0, 0.7122759 ), float4( 1, 0.09411766, 0, 1 ), 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = i.worldPos;
			half4 appendResult9 = (half4(ase_worldPos.z , ase_worldPos.x , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * half4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			half simplePerlin2D2 = snoise( panner3 );
			half2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0,0,0 ) ).xy);
			half simplePerlin2D23 = snoise( panner24 );
			half temp_output_26_0 = ( simplePerlin2D2 + (-0.5 + (simplePerlin2D23 - -1.0) * (1.0 - -0.5) / (1.0 - -1.0)) );
			Gradient gradient7_g1 = NewGradient( 0, 5, 2, float4( 0.9, 0.07941177, 0, 0.1000076 ), float4( 0.09149998, 0.2125711, 0.61, 0.4983749 ), float4( 1, 0.7532815, 0, 0.5179217 ), float4( 1, 0.7532815, 0, 0.5624323 ), float4( 0.09019608, 0.2117647, 0.6117647, 0.5808957 ), 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			half temp_output_15_0_g1 = simplePerlin2D2;
			half4 temp_cast_3 = ((0.0 + (temp_output_15_0_g1 - -1.0) * (1.0 - 0.0) / (1.0 - -1.0))).xxxx;
			float4 ase_vertex4Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float3 ase_viewPos = UnityObjectToViewPos( ase_vertex4Pos );
			float ase_screenDepth = -ase_viewPos.z;
			half cameraDepthFade6_g1 = (( ase_screenDepth -_ProjectionParams.y - _Offset1 ) / _Length1);
			half temp_output_10_0_g1 = saturate( cameraDepthFade6_g1 );
			half4 temp_cast_4 = (temp_output_10_0_g1).xxxx;
			half4 blendOpSrc11_g1 = CalculateContrast(3.0,temp_cast_3);
			half4 blendOpDest11_g1 = temp_cast_4;
			half4 lerpBlendMode11_g1 = lerp(blendOpDest11_g1,( blendOpDest11_g1 / max(blendOpSrc11_g1,0.00001) ),temp_output_10_0_g1);
			half4 lerpResult14_g1 = lerp( SampleGradient( gradient45, temp_output_26_0 ) , SampleGradient( gradient7_g1, temp_output_15_0_g1 ) , round( saturate( ( saturate( lerpBlendMode11_g1 )) ) ));
			o.Emission = lerpResult14_g1.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
1105;258;1906;1001;1813.511;367.7884;2.08329;True;False
Node;AmplifyShaderEditor.CommentaryNode;11;-3420.327,-866.2303;Inherit;False;784.8464;240.4145;;3;8;9;10;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-3370.327,-816.2303;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;9;-3134.898,-804.8158;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;21;-3374.442,-478.1145;Inherit;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-2889.481,-773.4252;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;14;-3304.468,-289.7111;Float;False;Property;_WaveSize;WaveSize;11;0;Create;True;0;0;0;False;0;False;1,1;0.01,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;12;-3324.442,-428.1144;Inherit;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2952.09,-226.8287;Float;False;Property;_WaveTile;WaveTile;10;0;Create;True;0;0;0;False;0;False;1;2.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-3066.186,-381.029;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2786.398,-394.8716;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-2594.211,-400.4863;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1722.208,783.2477;Float;False;Property;_WaveSpeed;WaveSpeed;9;0;Create;True;0;0;0;False;0;False;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-1823.473,196.9357;Inherit;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1756.015,670.4256;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1553.222,347.0647;Inherit;False;2;2;0;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;4;-1710.158,458.4843;Float;False;Constant;_WaveDirection;WaveDirection;0;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1457.639,699.5522;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;-1156.675,681.9511;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-864.958,642.8242;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;3;-1223.66,351.5422;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-923.4055,301.7433;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;46;-596.2647,843.4925;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-1;False;2;FLOAT;1;False;3;FLOAT;-0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;45;-158.2738,655.1994;Inherit;False;0;4;2;0,0.116883,0.5,0;1,0.09411766,0,0.4107881;0.9490196,0.8693196,0,0.7122759;1,0.09411766,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-563.3359,603.9148;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;18;-911.3203,87.81867;Float;False;Constant;_WaveUp;WaveUp;3;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;31;-890.4261,-48.25148;Inherit;False;Property;_OffsetStrength;OffsetStrength;17;0;Create;True;0;0;0;False;0;False;1;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;42;26.75953,806.3957;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;47;627.5974,475.6556;Inherit;False;NoisyFog;1;;1;6788f60f0499f094390af26a250b3515;0;2;1;COLOR;0,0,0,0;False;15;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;17;52.73405,359.0613;Float;False;Property;_Tessellation;Tessellation;16;0;Create;True;0;0;0;False;0;False;8;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;39;-480.7404,741.9954;Inherit;False;Property;_Power;Power;14;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;27;-360.1513,-266.6256;Inherit;True;Property;_Texture;Texture;15;0;Create;True;0;0;0;False;0;False;-1;None;9a4a55d8d2e54394d97426434477cdcf;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;35;-253.3822,202.6606;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;33;-354.1053,-62.96926;Inherit;False;Property;_Color1;Color1;12;0;Create;True;0;0;0;False;0;False;1,1,1,0;1,0.09411766,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;36;-568.3007,36.37174;Inherit;False;Property;_Color2;Color2;13;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0.2330508,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DistanceBasedTessNode;32;-128.8246,466.4838;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleContrastOpNode;40;-365.2405,890.4955;Inherit;False;2;1;COLOR;0,0,0,0;False;0;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;41;-178.4405,723.1954;Inherit;False;0;6;2;0,0.1184211,0.5,0;0,0.2313726,1,0.09439231;0,0.2313726,1,0.2417029;0.01690578,0.2505683,1,0.4107881;0.454902,0.7490196,1,0.7582971;1,0.09411766,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.PowerNode;37;-393.1825,519.0507;Inherit;False;False;2;0;FLOAT;0;False;1;FLOAT;4.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-661.2646,210.2238;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GradientNode;44;-816.132,984.4631;Inherit;False;0;6;2;0.1505882,0.2329412,0.5019608,0;0.3,0.4619608,1,0.09439231;0.3,0.4619608,1,0.2417029;0.3,0.4673309,1,0.4107881;0.75,0.8848921,1,0.7582971;1,0.3658824,0.3,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1228.954,-195.1706;Half;False;True;-1;6;ASEMaterialInspector;0;0;Unlit;TrippyClouds;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Transparent;;Geometry;ForwardOnly;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;8;0;1000;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;False;False;Cylindrical;False;Relative;0;;0;-1;-1;4;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;8;3
WireConnection;9;1;8;1
WireConnection;10;0;9;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;20;0;15;0
WireConnection;25;0;22;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;23;0;24;0
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;2;0;3;0
WireConnection;46;0;23;0
WireConnection;26;0;2;0
WireConnection;26;1;46;0
WireConnection;42;0;45;0
WireConnection;42;1;26;0
WireConnection;47;1;42;0
WireConnection;47;15;2;0
WireConnection;35;0;36;0
WireConnection;35;1;33;0
WireConnection;35;2;26;0
WireConnection;40;1;26;0
WireConnection;40;0;39;0
WireConnection;37;0;26;0
WireConnection;19;0;18;0
WireConnection;19;1;2;0
WireConnection;19;2;31;0
WireConnection;0;2;47;0
WireConnection;0;11;19;0
ASEEND*/
//CHKSM=B9D02C799220DE4065FA66E1BEFFB9552C7F50D1