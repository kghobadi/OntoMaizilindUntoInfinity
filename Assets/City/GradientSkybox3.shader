// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/GradientSkybox3"
{
	Properties
	{
		_Vector1("Vector 1", Vector) = (500,500,0,0)
		_Vector0("Vector 0", Vector) = (5000,5000,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow nofog 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float2 _Vector0;
		uniform float2 _Vector1;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			Gradient gradient1 = NewGradient( 0, 5, 2, float4( 0.9, 0.07941177, 0, 0.1499962 ), float4( 0.09149998, 0.2125711, 0.61, 0.4983749 ), float4( 1, 0.7532815, 0, 0.5179217 ), float4( 1, 0.7532815, 0, 0.5624323 ), float4( 0.09019608, 0.2117647, 0.6117647, 0.5808957 ), 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 panner27 = ( 1.0 * _Time.y * _Vector1 + float2( 0,0 ));
			float2 uv_TexCoord28 = i.uv_texcoord * _Vector0 + panner27;
			float simplePerlin2D29 = snoise( uv_TexCoord28 );
			float noise30 = simplePerlin2D29;
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 temp_output_3_0 = SampleGradient( gradient1, ( ( noise30 * 0.5 ) + ase_vertex3Pos.y ) );
			o.Emission = temp_output_3_0.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
2;658;1906;1013;1836.897;111.5767;1.836597;True;False
Node;AmplifyShaderEditor.Vector2Node;25;-1596.749,-260.5724;Inherit;False;Property;_Vector1;Vector 1;0;0;Create;True;0;0;0;False;0;False;500,500;0,-50;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;27;-1370.991,-248.5303;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;26;-1347.106,-392.0523;Inherit;False;Property;_Vector0;Vector 0;1;0;Create;True;0;0;0;False;0;False;5000,5000;500,500;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;28;-1115.157,-319.4533;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;100,500;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;29;-826.989,-251.3504;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;30;-531.074,-155.5972;Inherit;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-295.8293,-55.00731;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;2;-1131.2,218.9;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;1;-1127.8,116.9;Inherit;False;0;5;2;0.9,0.07941177,0,0.1499962;0.09149998,0.2125711,0.61,0.4983749;1,0.7532815,0,0.5179217;1,0.7532815,0,0.5624323;0.09019608,0.2117647,0.6117647,0.5808957;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;36;-181.8293,43.79266;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GradientNode;32;145.7607,-164.439;Inherit;False;0;4;2;0,0.116883,0.5,0;1,0.09411766,0,0.4107881;0.9490196,0.8693196,0,0.7122759;1,0.09411766,0,1;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.LerpOp;41;665.2767,390.1837;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;43;-1117.625,709.0253;Inherit;False;0;2;2;1,1,1,0;0,0,0,0.2877241;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;3;32.29984,277.1999;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;39;-1144.394,459.9092;Inherit;False;0;2;2;0.9,0.07941177,0,0;0.09149998,0.2125711,0.61,0.4983749;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;38;36.14479,493.8585;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;42;68.48824,737.4004;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;24;-1140.118,22.653;Inherit;False;0;2;2;0.8773585,0.08007637,0,0;1,0.7532815,0,0.4983749;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.GradientSampleNode;33;330.794,-13.24274;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1228.193,192.2657;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Julian/GradientSkybox3;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;27;2;25;0
WireConnection;28;0;26;0
WireConnection;28;1;27;0
WireConnection;29;0;28;0
WireConnection;30;0;29;0
WireConnection;37;0;30;0
WireConnection;36;0;37;0
WireConnection;36;1;2;2
WireConnection;41;0;3;0
WireConnection;41;1;38;0
WireConnection;41;2;42;0
WireConnection;3;0;1;0
WireConnection;3;1;36;0
WireConnection;38;0;39;0
WireConnection;38;1;2;2
WireConnection;42;0;43;0
WireConnection;42;1;2;2
WireConnection;33;0;32;0
WireConnection;33;1;30;0
WireConnection;0;2;3;0
ASEEND*/
//CHKSM=C685FD704BBCED585E8921FFEF1C269534F12819