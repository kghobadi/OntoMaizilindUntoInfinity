// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/DesertFinal"
{
	Properties
	{
		_TessValue( "Max Tessellation", Range( 1, 32 ) ) = 16
		_TessMin( "Tess Min Distance", Float ) = 20
		_TessMax( "Tess Max Distance", Float ) = 500
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveTile("WaveTile", Float) = 1
		_WaveSize("WaveSize", Vector) = (1,1,0,0)
		_Texture("Texture", 2D) = "white" {}
		_MainTex("MainTex", 2D) = "white" {}
		_WaveUp("WaveUp", Vector) = (0,1,0,0)
		[Toggle]_VertexOffset("VertexOffset", Float) = 1
		_OffsetStrength("OffsetStrength", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
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
		#pragma exclude_renderers xboxseries playstation switch nomrt 
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		uniform half _VertexOffset;
		uniform float3 _WaveUp;
		uniform float _WaveSpeed;
		uniform float2 _WaveSize;
		uniform float _WaveTile;
		uniform half _OffsetStrength;
		uniform sampler2D _Texture;
		uniform half4 _Texture_ST;
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;
		uniform float _TessValue;
		uniform float _TessMin;
		uniform float _TessMax;
		uniform float _TessPhongStrength;


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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TessMin, _TessMax, _TessValue );
		}

		void vertexDataFunc( inout appdata_full v )
		{
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half4 appendResult9 = (half4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * half4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			half simplePerlin2D2 = snoise( panner3 );
			half3 lerpResult28 = lerp( ( _WaveUp * simplePerlin2D2 * _OffsetStrength ) , float3( 0,0,0 ) , float3( 0,0,0 ));
			float2 uv_Texture = v.texcoord * _Texture_ST.xy + _Texture_ST.zw;
			half4 tex2DNode27 = tex2Dlod( _Texture, float4( uv_Texture, 0, 0.0) );
			v.vertex.xyz += (( _VertexOffset )?( ( half4( lerpResult28 , 0.0 ) * tex2DNode27 ) ):( float4( 0,0,0,0 ) )).rgb;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_Texture = i.uv_texcoord * _Texture_ST.xy + _Texture_ST.zw;
			half4 tex2DNode27 = tex2D( _Texture, uv_Texture );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half temp_output_43_0 = saturate( tex2D( _MainTex, uv_MainTex ).a );
			o.Albedo = ( tex2DNode27 * temp_output_43_0 ).rgb;
			half temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = i.worldPos;
			half4 appendResult9 = (half4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * half4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			half2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			half simplePerlin2D2 = snoise( panner3 );
			half2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0,0,0 ) ).xy);
			half simplePerlin2D23 = snoise( panner24 );
			half3 temp_cast_4 = (( ( simplePerlin2D2 + simplePerlin2D23 ) * temp_output_43_0 )).xxx;
			o.Emission = temp_cast_4;
			half temp_output_84_0 = 0.0;
			o.Metallic = temp_output_84_0;
			o.Smoothness = temp_output_84_0;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
91;659;1436;716;1019.981;456.8798;1.3198;True;False
Node;AmplifyShaderEditor.CommentaryNode;11;-3420.327,-866.2303;Inherit;False;784.8464;240.4145;;3;8;9;10;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-3370.327,-816.2303;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;9;-3134.898,-804.8158;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-2889.481,-773.4252;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;21;-3374.442,-478.1145;Inherit;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;14;-3304.468,-289.7111;Float;False;Property;_WaveSize;WaveSize;7;0;Create;True;0;0;0;False;0;False;1,1;0.012,0.0008;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;12;-3324.442,-428.1144;Inherit;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-3066.186,-381.029;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-2952.09,-226.8287;Float;False;Property;_WaveTile;WaveTile;6;0;Create;True;0;0;0;False;0;False;1;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-2786.398,-394.8716;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1722.208,783.2477;Float;False;Property;_WaveSpeed;WaveSpeed;5;0;Create;True;0;0;0;False;0;False;0;0.3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-2594.211,-400.4863;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-1756.015,670.4256;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-1457.639,699.5522;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-1710.158,458.4843;Float;False;Constant;_WaveDirection;WaveDirection;0;0;Create;True;0;0;0;False;0;False;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;22;-1823.473,196.9357;Inherit;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;3;-1223.66,351.5422;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-923.4055,301.7433;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;18;-911.3203,87.81867;Float;False;Property;_WaveUp;WaveUp;10;0;Create;True;0;0;0;False;0;False;0,1,0;0,0,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-1553.222,347.0647;Inherit;False;2;2;0;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-890.4261,-48.25148;Inherit;False;Property;_OffsetStrength;OffsetStrength;15;0;Create;True;0;0;0;False;0;False;1;3;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;24;-1156.675,681.9511;Inherit;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-661.2646,210.2238;Inherit;True;3;3;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-864.958,642.8242;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;82;-163.7614,-100.1862;Inherit;True;Property;_MainTex;MainTex;9;0;Create;True;0;0;0;False;0;False;-1;None;95db204b361bff84fad34f7ee896c606;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;27;-272.9919,-369.5023;Inherit;True;Property;_Texture;Texture;8;0;Create;True;0;0;0;False;0;False;-1;None;652146cb96608124eaeffc53c6dceeb2;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;28;-299.5483,152.4299;Inherit;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;43;130.5363,-62.73254;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;85;220.7348,151.7819;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-482.9021,541.9799;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;45;337.8529,-60.43475;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;38;-520.6956,25.04919;Inherit;False;Property;_PosMinMax;PosMinMax;11;0;Create;True;0;0;0;False;0;False;0,0;6,9;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;17;52.73405,359.0613;Float;False;Property;_Tessellation;Tessellation;13;0;Create;True;0;0;0;False;0;False;8;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;34;-499.8788,-97.04441;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;-20;False;2;FLOAT;20;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;36;2.506336,-180.2765;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;351.4258,-194.9295;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.ToggleSwitchNode;30;412.4266,118.9265;Inherit;True;Property;_VertexOffset;VertexOffset;14;0;Create;True;0;0;0;False;0;False;1;True;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;83;15.71086,-504.7127;Inherit;False;Property;_Color;Color;12;1;[HDR];Create;True;0;0;0;False;0;False;1,0.534086,0.03301889,1;4,2.133333,0.1254902,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;32;-704.6385,-122.6724;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;84;479.3555,-309.7345;Inherit;False;Constant;_Float0;Float 0;12;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;40;-342.8957,-80.55077;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;-100;False;2;FLOAT;100;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;667.8282,-194.2907;Half;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Julian/DesertFinal;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;14;d3d9;d3d11_9x;d3d11;glcore;gles;gles3;metal;vulkan;xbox360;xboxone;ps4;psp2;n3ds;wiiu;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;0;16;20;500;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;0;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
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
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;2;0;3;0
WireConnection;25;0;22;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;19;0;18;0
WireConnection;19;1;2;0
WireConnection;19;2;31;0
WireConnection;23;0;24;0
WireConnection;28;0;19;0
WireConnection;43;0;82;4
WireConnection;85;0;28;0
WireConnection;85;1;27;0
WireConnection;26;0;2;0
WireConnection;26;1;23;0
WireConnection;45;0;26;0
WireConnection;45;1;43;0
WireConnection;34;0;32;2
WireConnection;42;0;27;0
WireConnection;42;1;43;0
WireConnection;30;1;85;0
WireConnection;40;0;34;0
WireConnection;40;1;38;1
WireConnection;40;2;38;2
WireConnection;0;0;42;0
WireConnection;0;2;45;0
WireConnection;0;3;84;0
WireConnection;0;4;84;0
WireConnection;0;11;30;0
ASEEND*/
//CHKSM=70D9521EC6D56D461452E3C2AC8041C7A2D9701E