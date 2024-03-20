// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/GradientSkybox2"
{
	Properties
	{
		_TopColor("TopColor", Color) = (0.7921569,0,0,1)
		_RemapValues("RemapValues", Vector) = (0,1,0,1)
		_SpeedMountains("SpeedMountains", Float) = 1
		_Float0("Float 0", Float) = 1
		_BackgroundTex("BackgroundTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Off
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow nofog 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform float4 _TopColor;
		uniform float4 _RemapValues;
		uniform sampler2D _BackgroundTex;
		uniform float _SpeedMountains;
		uniform float4 _BackgroundTex_ST;
		uniform float _Float0;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float temp_output_24_0 = (_RemapValues.z + (ase_vertex3Pos.y - _RemapValues.x) * (_RemapValues.w - _RemapValues.z) / (_RemapValues.y - _RemapValues.x));
			float4 lerpResult27 = lerp( _TopColor , _TopColor , saturate( temp_output_24_0 ));
			Gradient gradient63 = NewGradient( 0, 3, 2, float4( 0.1801745, 0.1967491, 0.6293655, 0 ), float4( 0.8679245, 0.08126056, 0, 0.2095369 ), float4( 1, 0.7305143, 0, 0.8278019 ), 0, 0, 0, 0, 0, float2( 1, 0 ), float2( 1, 1 ), 0, 0, 0, 0, 0, 0 );
			float2 appendResult48 = (float2(_SpeedMountains , 0.0));
			float2 uv_BackgroundTex = i.uv_texcoord * _BackgroundTex_ST.xy + _BackgroundTex_ST.zw;
			float2 panner45 = ( 1.0 * _Time.y * appendResult48 + uv_BackgroundTex);
			float4 tex2DNode37 = tex2D( _BackgroundTex, panner45 );
			float2 appendResult54 = (float2(_Float0 , 0.0));
			float2 panner56 = ( 1.0 * _Time.y * appendResult54 + uv_BackgroundTex);
			float3 ase_worldPos = i.worldPos;
			float4 lerpResult39 = lerp( tex2DNode37 , tex2D( _BackgroundTex, panner56 ) , ceil( saturate( ase_worldPos.x ) ));
			float temp_output_3_0_g44 = 4.0;
			float4 lerpResult58 = lerp( lerpResult27 , SampleGradient( gradient63, ( trunc( ( lerpResult39 * temp_output_3_0_g44 ) ) / temp_output_3_0_g44 ).x ) , lerpResult39.a);
			o.Emission = lerpResult58.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
7;571;1505;448;2073.405;-393.226;1.271549;True;False
Node;AmplifyShaderEditor.RangedFloatNode;60;-750.4221,1231.619;Inherit;False;Property;_Float0;Float 0;6;0;Create;True;0;0;0;False;0;False;1;-0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-746.9558,1126.843;Inherit;False;Property;_SpeedMountains;SpeedMountains;5;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;54;-450.2206,1204.42;Inherit;False;FLOAT2;4;0;FLOAT;-1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;40;368.4279,908.7095;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TextureCoordinatesNode;46;-841.0343,946.1827;Inherit;False;0;51;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;48;-434.7409,1045.303;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;51;-591.0941,768.586;Inherit;True;Property;_BackgroundTex;BackgroundTex;7;0;Create;True;0;0;0;False;0;False;None;9e88b336bd16b1e4b99de75f486126c1;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.PannerNode;56;-284.623,1141.062;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SaturateNode;44;565.8583,862.7906;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;45;-316.6626,909.9452;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;59;-108.4191,1022.963;Inherit;True;Property;_TextureSample0;Texture Sample 0;4;0;Create;True;0;0;0;False;0;False;-1;None;cd7946e1eb09a2c41bc707dd4b3545fb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;37;-113.2345,725.4949;Inherit;True;Property;_BackgroundTex1;BackgroundTex1;4;0;Create;True;0;0;0;False;0;False;-1;None;cd7946e1eb09a2c41bc707dd4b3545fb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;35;-1622.909,607.0806;Inherit;False;Property;_RemapValues;RemapValues;3;0;Create;True;0;0;0;False;0;False;0,1,0,1;-0.92,0,0,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PosVertexDataNode;2;-1562.286,221.4465;Inherit;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CeilOpNode;43;696.8438,781.7583;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;39;826.2928,680.4715;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCRemapNode;24;-1411.065,414.0445;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;64;699.3624,1259.632;Inherit;False;PosterizeSteps;-1;;44;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;4;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GradientNode;63;707.9412,1093.18;Inherit;False;0;3;2;0.1801745,0.1967491,0.6293655,0;0.8679245,0.08126056,0,0.2095369;1,0.7305143,0,0.8278019;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SaturateNode;30;-1024.104,499.0835;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;29;-1162.227,832.9615;Inherit;False;Property;_TopColor;TopColor;2;0;Create;True;0;0;0;False;0;False;0.7921569,0,0,1;1,0.09411766,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BreakToComponentsNode;61;1098.566,653.7338;Inherit;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.LerpOp;27;-713.1882,562.2451;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientSampleNode;65;984.1172,1218.472;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;28;-1176.158,627.1995;Inherit;False;Property;_BottomColor;BottomColor;1;0;Create;True;0;0;0;False;0;False;1,0.5607843,0,1;0,0.2333653,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;11;-114.502,-147.9029;Inherit;True;2;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;6;-149.8493,100.1691;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5.51;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScreenPosInputsNode;19;300.1986,219.7278;Float;False;0;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientSampleNode;3;-908.2001,292.4;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;20;409.5045,66.47961;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-1697.588,513.0607;Inherit;False;Property;_HeightThreshold;HeightThreshold;0;0;Create;True;0;0;0;False;0;False;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GradientSampleNode;12;905.5981,-121.4017;Inherit;True;2;0;OBJECT;;False;1;FLOAT;0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;62;353.839,1267.935;Inherit;True;Property;_MainTexture;MainTexture;4;0;Create;True;0;0;0;False;0;False;-1;None;1927b8fe9e82b304da74f1c6e88f7195;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GradientNode;31;-1145.675,128.0174;Inherit;False;0;3;2;0,0.2352941,1,0.1576715;0.7411765,0,0,0.4367285;1,0.5607843,0,0.867216;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.TFHCGrayscale;9;-564.3267,-155.2902;Inherit;True;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;7;-417.0267,94.81023;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;33;-1214.8,471.0802;Inherit;False;PosterizeSteps;-1;;45;a5af708c0f358a1409563877edfc2aac;0;2;1;FLOAT4;0,0,0,0;False;3;FLOAT;3;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;21;706.2159,123.0573;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;15;353.5122,-100.4897;Inherit;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.GradientNode;26;-1121.709,43.78396;Inherit;False;0;2;2;1,0.5615012,0,0.1799954;0.7924528,0,0,0.9000076;1,0;1,1;0;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;509.9831,-92.03656;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT2;1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;13;-909.9239,114.6798;Inherit;False;gradient;-1;True;1;0;OBJECT;;False;1;OBJECT;0
Node;AmplifyShaderEditor.GetLocalVarNode;14;353.1149,-309.6836;Inherit;False;13;gradient;1;0;OBJECT;;False;1;OBJECT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;23;669.3063,-103.927;Inherit;False;2;0;COLOR;0,0,0,0;False;1;COLOR;0.5,0.5,0.5,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;38;214.8579,536.751;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;58;1108.246,399.1373;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;36;-372.065,729.9607;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleContrastOpNode;10;-383.6267,-155.2901;Inherit;True;2;1;COLOR;0,0,0,0;False;0;FLOAT;3;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;8;82.17323,-133.59;Inherit;True;Overlay;True;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1506.589,167.906;Float;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Julian/GradientSkybox2;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;54;0;60;0
WireConnection;48;0;49;0
WireConnection;56;0;46;0
WireConnection;56;2;54;0
WireConnection;44;0;40;1
WireConnection;45;0;46;0
WireConnection;45;2;48;0
WireConnection;59;0;51;0
WireConnection;59;1;56;0
WireConnection;37;0;51;0
WireConnection;37;1;45;0
WireConnection;43;0;44;0
WireConnection;39;0;37;0
WireConnection;39;1;59;0
WireConnection;39;2;43;0
WireConnection;24;0;2;2
WireConnection;24;1;35;1
WireConnection;24;2;35;2
WireConnection;24;3;35;3
WireConnection;24;4;35;4
WireConnection;64;1;39;0
WireConnection;30;0;24;0
WireConnection;61;0;39;0
WireConnection;27;0;29;0
WireConnection;27;1;29;0
WireConnection;27;2;30;0
WireConnection;65;0;63;0
WireConnection;65;1;64;0
WireConnection;11;0;10;0
WireConnection;6;0;7;0
WireConnection;3;0;31;0
WireConnection;3;1;33;0
WireConnection;12;0;14;0
WireConnection;12;1;23;0
WireConnection;9;0;27;0
WireConnection;33;1;24;0
WireConnection;21;1;19;0
WireConnection;15;0;8;0
WireConnection;16;0;15;0
WireConnection;16;1;21;0
WireConnection;13;0;26;0
WireConnection;23;0;16;0
WireConnection;38;0;27;0
WireConnection;38;1;37;0
WireConnection;38;2;37;4
WireConnection;58;0;27;0
WireConnection;58;1;65;0
WireConnection;58;2;61;3
WireConnection;10;1;9;0
WireConnection;8;0;6;0
WireConnection;8;1;11;0
WireConnection;0;2;58;0
ASEEND*/
//CHKSM=61DCFD9E5F2F8288A897DF7550DA625CD19B32A7