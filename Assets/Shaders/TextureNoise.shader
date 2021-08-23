// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TextureNoise"
{
	Properties
	{
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveTile("WaveTile", Float) = 1
		_WaveSize("WaveSize", Vector) = (1,1,0,0)
		_TessellationValue("TessellationValue", Float) = 8
		_WaveUp("WaveUp", Vector) = (0,1,0,0)
		_EmissionColor("EmissionColor", Color) = (0,0,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma exclude_renderers switch nomrt 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
		};

		uniform float3 _WaveUp;
		uniform float _WaveSpeed;
		uniform float2 _WaveSize;
		uniform float _WaveTile;
		uniform float4 _EmissionColor;
		uniform float _TessellationValue;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_2 = (_TessellationValue).xxxx;
			return temp_cast_2;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult9 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * float4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			float2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			float simplePerlin2D2 = snoise( panner3 );
			v.vertex.xyz += ( _WaveUp * simplePerlin2D2 );
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			Gradient gradient31 = NewGradient( 0, 3, 2, float4( 0.2923638, 0.5641437, 0.8490566, 0 ), float4( 0.2862745, 0.7490196, 1, 0.1368887 ), float4( 1, 1, 1, 0.9979095 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = i.worldPos;
			float4 appendResult9 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * float4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			float2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			float simplePerlin2D2 = snoise( panner3 );
			float2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0,0,0 ) ).xy);
			float simplePerlin2D23 = snoise( panner24 );
			float temp_output_26_0 = ( simplePerlin2D2 + simplePerlin2D23 );
			float4 temp_output_32_0 = SampleGradient( gradient31, temp_output_26_0 );
			o.Albedo = temp_output_32_0.rgb;
			o.Emission = ( _EmissionColor * SampleGradient( gradient31, temp_output_26_0 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
65;815;1620;845;1378.736;136.7807;1.609531;True;True
Node;AmplifyShaderEditor.CommentaryNode;11;-1654.555,-760.2157;Inherit;False;784.8464;240.4145;;3;8;9;10;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-1604.555,-710.2157;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;9;-1369.126,-698.8011;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-1123.709,-667.4105;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;21;-1748.793,-413.3993;Inherit;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;14;-1678.819,-224.9958;Float;False;Property;_WaveSize;WaveSize;2;0;Create;True;0;0;0;False;0;False;1,1;0.8,40;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;12;-1698.793,-363.3991;Inherit;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1326.441,-162.1133;Float;False;Property;_WaveTile;WaveTile;1;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-1440.537,-316.3137;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-1160.749,-330.1563;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-968.5618,-335.771;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1502.331,542.1536;Float;False;Property;_WaveSpeed;WaveSpeed;0;0;Create;True;0;0;0;False;0;False;0;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1536.138,429.3317;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-1601.667,29.13445;Inherit;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;4;-1490.281,217.3905;Float;False;Constant;_WaveDirection;WaveDirection;0;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1237.762,458.4583;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1333.345,105.9709;Inherit;False;2;2;0;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;3;-1003.783,110.4484;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;24;-936.7976,440.8572;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-703.5278,60.64958;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-645.0803,401.7303;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-357.8737,307.2095;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;31;-357.2331,429.759;Inherit;False;0;3;2;0.2923638,0.5641437,0.8490566,0;0.2862745,0.7490196,1,0.1368887;1,1,1,0.9979095;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.ColorNode;33;174.8699,570.3279;Inherit;False;Property;_EmissionColor;EmissionColor;6;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.4,0.4,0.4,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector3Node;18;-691.4426,-153.2751;Float;False;Property;_WaveUp;WaveUp;5;0;Create;True;0;0;0;False;0;False;0,1,0;0.1,0.1,0.05;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.GradientSampleNode;32;-125.4689,317.7309;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;-398.1746,706.3618;Inherit;False;Property;_Color1;Color1;7;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;39;-93.74615,723.9598;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;42;-191.0982,934.8366;Inherit;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;41;-416.3895,882.0094;Inherit;False;Property;_Color2;Color2;8;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.2901961,0.5882353,0.8705883,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-441.387,-30.86996;Inherit;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;30;-116.7483,148.4306;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;223.4137,362.7632;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;27;-418.6918,-272.4796;Inherit;True;Property;_Texture;Texture;3;0;Create;True;0;0;0;False;0;False;-1;None;095c95436574dbe4785a26fb3b273719;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;42.76249,415.0244;Float;False;Property;_TessellationValue;TessellationValue;4;0;Create;True;0;0;0;False;0;False;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;383.756,-196.1326;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;TextureNoise;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;8;1
WireConnection;9;1;8;3
WireConnection;10;0;9;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;20;0;15;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;25;0;22;0
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;2;0;3;0
WireConnection;23;0;24;0
WireConnection;26;0;2;0
WireConnection;26;1;23;0
WireConnection;32;0;31;0
WireConnection;32;1;26;0
WireConnection;39;0;40;0
WireConnection;39;1;41;0
WireConnection;39;2;42;0
WireConnection;19;0;18;0
WireConnection;19;1;2;0
WireConnection;30;0;26;0
WireConnection;30;1;27;0
WireConnection;34;0;33;0
WireConnection;34;1;32;0
WireConnection;0;0;32;0
WireConnection;0;2;34;0
WireConnection;0;11;19;0
WireConnection;0;14;17;0
ASEEND*/
//CHKSM=75FBEE03C148513A465222092AD491398BC4351B