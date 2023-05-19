// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/AirplaneLandscape"
{
	Properties
	{
		_TessPhongStrength( "Phong Tess Strength", Range( 0, 1 ) ) = 0.5
		_MainTex("MainTex", 2D) = "white" {}
		_SpeedX("SpeedX", Float) = 0
		_SpeedY("SpeedY", Float) = -0.5
		_TesselationMinMaxDistance("TesselationMinMaxDistance", Vector) = (0,0,0,0)
		_TesselationFactor("TesselationFactor", Range( 0 , 60)) = 0
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
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc tessellate:tessFunction tessphong:_TessPhongStrength 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float _SpeedX;
		uniform float _SpeedY;
		uniform float2 _TesselationMinMaxDistance;
		uniform float _TesselationFactor;
		uniform float _TessPhongStrength;

		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			return UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _TesselationMinMaxDistance.x,_TesselationMinMaxDistance.y,_TesselationFactor);
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float2 appendResult11 = (float2(_SpeedX , ( _Time.y * _SpeedY )));
			float2 uv_TexCoord2 = v.texcoord.xy + appendResult11;
			float2 panner3 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TexCoord2);
			float4 tex2DNode1 = tex2Dlod( _MainTex, float4( panner3, 0, 0.0) );
			float grayscale18 = Luminance(( tex2DNode1 * float4( 1,1,1,0 ) ).rgb);
			float4 appendResult16 = (float4(0.0 , grayscale18 , 0.0 , 0.0));
			v.vertex.xyz += appendResult16.xyz;
			v.vertex.w = 1;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 appendResult11 = (float2(_SpeedX , ( _Time.y * _SpeedY )));
			float2 uv_TexCoord2 = i.uv_texcoord + appendResult11;
			float2 panner3 = ( 1.0 * _Time.y * float2( 0,0 ) + uv_TexCoord2);
			float4 tex2DNode1 = tex2D( _MainTex, panner3 );
			o.Emission = tex2DNode1.rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
7;78;1906;941;1363.289;156.5298;1;True;True
Node;AmplifyShaderEditor.SimpleTimeNode;10;-1906.385,60.5926;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-1887,262.5;Inherit;False;Property;_SpeedY;SpeedY;2;0;Create;True;0;0;0;False;0;False;-0.5;-0.05;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1705.185,117.9926;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;-0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;5;-1717,-19.5;Inherit;False;Property;_SpeedX;SpeedX;1;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;11;-1559.285,44.2926;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;2;-1367,-15.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;3;-1102,28.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-857,28.5;Inherit;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;0;False;0;False;-1;960678684904e6c408b6ee38279985eb;960678684904e6c408b6ee38279985eb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-522.6144,166.9045;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TFHCGrayscale;18;-321.2891,183.4702;Inherit;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;15;-689.2891,405.4702;Inherit;False;Property;_TesselationFactor;TesselationFactor;4;0;Create;True;0;0;0;False;0;False;0;60;0;60;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;14;-681.2891,485.4702;Inherit;False;Property;_TesselationMinMaxDistance;TesselationMinMaxDistance;3;0;Create;True;0;0;0;False;0;False;0,0;200,900;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;9;-787.2853,278.2926;Inherit;False;Constant;_Float0;Float 0;2;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;16;-146.2891,184.4702;Inherit;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;13;-394.2891,422.4702;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;38,-35;Float;False;True;-1;6;ASEMaterialInspector;0;0;Standard;Julian/AirplaneLandscape;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;True;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;12;0;10;0
WireConnection;12;1;6;0
WireConnection;11;0;5;0
WireConnection;11;1;12;0
WireConnection;2;1;11;0
WireConnection;3;0;2;0
WireConnection;1;1;3;0
WireConnection;8;0;1;0
WireConnection;18;0;8;0
WireConnection;16;1;18;0
WireConnection;13;0;15;0
WireConnection;13;1;14;1
WireConnection;13;2;14;2
WireConnection;0;2;1;0
WireConnection;0;11;16;0
WireConnection;0;14;13;0
ASEEND*/
//CHKSM=F016D230014B6355FD3AFA2205F31E56F431BDD0