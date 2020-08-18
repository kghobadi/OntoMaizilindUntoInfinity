// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "halftone"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_effectStrength("effectStrength", Range( 0 , 1)) = 0
		_frequency("frequency", Float) = 20
		_paper_like("paper_like", Range( 0 , 1)) = 1
		_greyscale("greyscale", Range( 0 , 1)) = 0
		_colorStrength("colorStrength", Range( 0 , 2)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		

		Pass
		{
			Name "Unlit"
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"
			

			struct appdata
			{
				float4 vertex : POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				float4 ase_texcoord : TEXCOORD0;
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 ase_texcoord : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _colorStrength;
			uniform float _frequency;
			uniform float _paper_like;
			uniform float _greyscale;
			uniform float _effectStrength;
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
			
			
			v2f vert ( appdata v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);

				o.ase_texcoord.xy = v.ase_texcoord.xy;
				
				//setting value to unused interpolator channels and avoid initialization warnings
				o.ase_texcoord.zw = 0;
				
				v.vertex.xyz +=  float3(0,0,0) ;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID(i);
				fixed4 finalColor;
				float2 uv_MainTex = i.ase_texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				float4 tex2DNode36 = tex2D( _MainTex, uv_MainTex );
				float4 temp_output_64_0 = ( 1.0 - tex2DNode36 );
				float4 break65 = temp_output_64_0;
				float3 appendResult66 = (float3(break65.r , break65.g , break65.b));
				float temp_output_67_0 = min( break65.r , min( break65.g , break65.b ) );
				float3 temp_cast_0 = (temp_output_67_0).xxx;
				float4 appendResult70 = (float4(( appendResult66 - temp_cast_0 ) , temp_output_67_0));
				float4 break85 = appendResult70;
				float clampResult22_g46 = clamp( _colorStrength , 0.0 , 2.0 );
				float2 temp_cast_3 = (1.0).xx;
				float simplePerlin2D43 = snoise( ( i.ase_texcoord.xy * 200.0 ) );
				float simplePerlin2D49 = snoise( ( i.ase_texcoord.xy * 400.0 ) );
				float simplePerlin2D53 = snoise( ( i.ase_texcoord.xy * 800.0 ) );
				float noise74 = ( ( ( simplePerlin2D43 * 0.1 ) + ( simplePerlin2D49 * 0.05 ) + ( simplePerlin2D53 * 0.025 ) ) * _paper_like );
				float temp_output_3_0_g47 = ( ( ( sqrt( break85.x ) * clampResult22_g46 ) - ( length( ( ( frac( ( mul( float3( i.ase_texcoord.xy ,  0.0 ), float3x3(0.966,-0.259,0,0.259,0.966,0,0,0,1) ).xy * _frequency ) ) * 2.0 ) - temp_cast_3 ) ) + noise74 ) ) - 0.0 );
				float c80 = saturate( ( temp_output_3_0_g47 / fwidth( temp_output_3_0_g47 ) ) );
				float clampResult22_g44 = clamp( _colorStrength , 0.0 , 2.0 );
				float2 temp_cast_6 = (1.0).xx;
				float temp_output_3_0_g45 = ( ( ( sqrt( break85.y ) * clampResult22_g44 ) - ( length( ( ( frac( ( mul( float3( i.ase_texcoord.xy ,  0.0 ), float3x3(0.966,0.259,0,-0.259,0.966,0,0,0,1) ).xy * _frequency ) ) * 2.0 ) - temp_cast_6 ) ) + noise74 ) ) - 0.0 );
				float m81 = saturate( ( temp_output_3_0_g45 / fwidth( temp_output_3_0_g45 ) ) );
				float clampResult22_g40 = clamp( _colorStrength , 0.0 , 2.0 );
				float2 temp_cast_9 = (1.0).xx;
				float temp_output_3_0_g41 = ( ( ( sqrt( break85.z ) * clampResult22_g40 ) - ( length( ( ( frac( ( mul( float3( i.ase_texcoord.xy ,  0.0 ), float3x3(-1,0,0,1,-1,0,0,0,0) ).xy * _frequency ) ) * 2.0 ) - temp_cast_9 ) ) + noise74 ) ) - 0.0 );
				float y82 = saturate( ( temp_output_3_0_g41 / fwidth( temp_output_3_0_g41 ) ) );
				float3 appendResult180 = (float3(c80 , m81 , y82));
				float temp_output_58_0 = ( noise74 + 0.1 );
				float3 temp_cast_10 = (temp_output_58_0).xxx;
				float clampResult22_g48 = clamp( _colorStrength , 0.0 , 2.0 );
				float2 temp_cast_13 = (1.0).xx;
				float temp_output_3_0_g49 = ( ( ( sqrt( break85.w ) * clampResult22_g48 ) - ( length( ( ( frac( ( mul( float3( i.ase_texcoord.xy ,  0.0 ), float3x3(0.707,-0.707,0,0.707,0.707,0,0,0,1) ).xy * _frequency ) ) * 2.0 ) - temp_cast_13 ) ) + noise74 ) ) - 0.0 );
				float k83 = saturate( ( temp_output_3_0_g49 / fwidth( temp_output_3_0_g49 ) ) );
				float3 lerpResult21 = lerp( ( 1.0 - ( ( appendResult180 * 0.9 ) + noise74 ) ) , temp_cast_10 , ( ( k83 * 0.85 ) + ( noise74 * 0.3 ) ));
				float grayscale199 = Luminance(temp_output_64_0.rgb);
				float clampResult22_g50 = clamp( _colorStrength , 0.0 , 2.0 );
				float2 temp_cast_17 = (1.0).xx;
				float temp_output_3_0_g51 = ( ( ( sqrt( grayscale199 ) * clampResult22_g50 ) - ( length( ( ( frac( ( mul( float3( i.ase_texcoord.xy ,  0.0 ), float3x3(0.707,-0.707,0,0.707,0.707,0,0,0,1) ).xy * _frequency ) ) * 2.0 ) - temp_cast_17 ) ) + noise74 ) ) - 0.0 );
				float greyscale210 = saturate( ( temp_output_3_0_g51 / fwidth( temp_output_3_0_g51 ) ) );
				float lerpResult211 = lerp( ( ( noise74 * 0.5 ) + 0.98 ) , temp_output_58_0 , greyscale210);
				float3 temp_cast_18 = (lerpResult211).xxx;
				float3 lerpResult213 = lerp( lerpResult21 , temp_cast_18 , _greyscale);
				float4 lerpResult222 = lerp( tex2DNode36 , float4( lerpResult213 , 0.0 ) , _effectStrength);
				
				
				finalColor = lerpResult222;
				return finalColor;
			}
			ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=15900
48;88;1332;581;4439.071;1093.773;2.858351;True;True
Node;AmplifyShaderEditor.CommentaryNode;55;-3207.798,165.4881;Float;False;1480.895;604.3972;for the irregular edges;17;74;56;46;57;50;44;54;53;49;43;42;48;52;41;51;47;26;;0.03397562,0.9852941,0,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;179;-2992.716,-1607.125;Float;False;2653.941;1610.518;cmyk_texture_dots;10;3;89;110;178;125;160;36;72;215;221;;0.7426471,0.7354164,0.4805363,1;0;0
Node;AmplifyShaderEditor.TexCoordVertexDataNode;26;-3194.264,318.1959;Float;False;0;2;0;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;51;-2922.719,572.9664;Float;False;Constant;_Float4;Float 4;2;0;Create;True;0;0;False;0;800;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-3050.29,-591.4992;Float;True;Property;_MainTex;MainTex;0;0;Create;True;0;0;False;0;e24b2c680edaa90458d31f11544d79ca;e24b2c680edaa90458d31f11544d79ca;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;47;-2905.962,443.4932;Float;False;Constant;_Float3;Float 3;2;0;Create;True;0;0;False;0;400;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;72;-2689.574,-894.4492;Float;False;1277.523;363.6057;convert to cmyk;8;85;70;69;66;67;68;65;64;;0.516782,0.7573529,0.4566393,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;41;-2902.727,334.3499;Float;False;Constant;_Float2;Float 2;2;0;Create;True;0;0;False;0;200;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;64;-2676.874,-688.2678;Float;False;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2743.199,383.978;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;52;-2740.154,539.3456;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;42;-2755.196,250.463;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BreakToComponentsNode;65;-2506.17,-811.1367;Float;False;COLOR;1;0;COLOR;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.NoiseGeneratorNode;43;-2603.017,246.3319;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;53;-2609.3,501.7034;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;49;-2604.729,378.323;Float;False;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;54;-2415.2,540.2886;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.025;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMinOpNode;68;-2295.659,-647.6253;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-2408.917,284.916;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;50;-2410.628,416.9081;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.05;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;66;-2232.843,-821.778;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMinOpNode;67;-2132.443,-681.6551;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;46;-2222.107,391.345;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-2241.439,576.1694;Float;False;Property;_paper_like;paper_like;3;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;69;-2009.061,-811.9085;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;56;-2072.458,401.116;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;178;-1322.347,-760.7445;Float;False;930.0436;322.4638;y;3;82;195;196;;0.9862069,1,0,1;0;0
Node;AmplifyShaderEditor.DynamicAppendNode;70;-1833.845,-814.0059;Float;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;74;-1930.133,332.025;Float;False;noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;160;-1330.691,-1139.653;Float;False;896.7295;359.9452;m;3;81;194;159;;1,0,0.4344826,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;125;-1329.502,-1480.442;Float;False;793.0884;316.9917;c;3;116;193;80;;0,0.6689653,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;3;-1627.496,-512.7419;Float;False;Property;_frequency;frequency;2;0;Create;True;0;0;False;0;20;77.96;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Matrix3X3Node;196;-1285.178,-686.2616;Float;False;Constant;_Matrix3;Matrix 3;0;0;Create;True;0;0;False;0;-1,0,0,1,-1,0,0,0,0;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.Matrix3X3Node;159;-1308.361,-1082.467;Float;False;Constant;_Matrix2;Matrix 2;0;0;Create;True;0;0;False;0;0.966,0.259,0,-0.259,0.966,0,0,0,1;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.GetLocalVarNode;110;-1641.14,-974.3969;Float;False;74;noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BreakToComponentsNode;85;-1672.613,-771.0631;Float;True;FLOAT4;1;0;FLOAT4;0,0,0,0;False;16;FLOAT;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4;FLOAT;5;FLOAT;6;FLOAT;7;FLOAT;8;FLOAT;9;FLOAT;10;FLOAT;11;FLOAT;12;FLOAT;13;FLOAT;14;FLOAT;15
Node;AmplifyShaderEditor.Matrix3X3Node;116;-1298.621,-1433.221;Float;False;Constant;_Matrix1;Matrix 1;0;0;Create;True;0;0;False;0;0.966,-0.259,0,0.259,0.966,0,0,0,1;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.RangedFloatNode;221;-2070.791,-510.9526;Float;False;Property;_colorStrength;colorStrength;5;0;Create;True;0;0;False;0;1;1;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;194;-958.9248,-981.8837;Float;False;CreateHalftoneDots;-1;;44;048136905c997409990aca2254cdd604;0;5;18;FLOAT;0;False;17;FLOAT;0;False;16;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;15;FLOAT;0;False;21;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;193;-1034.884,-1347.21;Float;False;CreateHalftoneDots;-1;;46;048136905c997409990aca2254cdd604;0;5;18;FLOAT;0;False;17;FLOAT;0;False;16;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;15;FLOAT;0;False;21;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;89;-1360.219,-393.7718;Float;False;896.2274;324.4742;k;3;31;83;197;;0,0,0,1;0;0
Node;AmplifyShaderEditor.FunctionNode;195;-991.712,-658.2675;Float;False;CreateHalftoneDots;-1;;40;048136905c997409990aca2254cdd604;0;5;18;FLOAT;0;False;17;FLOAT;0;False;16;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;15;FLOAT;0;False;21;FLOAT;1.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;81;-652.7639,-1028.386;Float;True;m;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;80;-739.2541,-1394.865;Float;True;c;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;82;-607.8272,-691.2432;Float;True;y;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Matrix3X3Node;31;-1343.124,-331.2185;Float;False;Constant;_Matrix0;Matrix 0;0;0;Create;True;0;0;False;0;0.707,-0.707,0,0.707,0.707,0,0,0,1;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.GetLocalVarNode;182;-1637.516,133.9382;Float;False;81;m;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;-1638.516,209.9382;Float;False;82;y;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;181;-1641.516,57.93827;Float;False;80;c;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;197;-1036.167,-330.2426;Float;False;CreateHalftoneDots;-1;;48;048136905c997409990aca2254cdd604;0;5;18;FLOAT;0;False;17;FLOAT;0;False;16;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;15;FLOAT;0;False;21;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;215;-2419.866,-384.4935;Float;False;870.454;330.2505;greyscale;4;210;209;208;199;;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-1458.516,251.9384;Float;False;Constant;_Float14;Float 14;6;0;Create;True;0;0;False;0;0.9;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCGrayscale;199;-2361.238,-337.9234;Float;False;0;1;0;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;83;-671.7988,-360.4155;Float;True;k;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Matrix3X3Node;208;-2382.567,-261.6359;Float;False;Constant;_Matrix4;Matrix 4;0;0;Create;True;0;0;False;0;0.707,-0.707,0,0.707,0.707,0,0,0,1;0;1;FLOAT3x3;0
Node;AmplifyShaderEditor.DynamicAppendNode;180;-1455.228,109.8004;Float;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;188;-1293.075,309.0297;Float;False;83;k;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;-1229.794,69.60838;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;84;-1470.407,478.1714;Float;False;74;noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;209;-2042.401,-259.8043;Float;False;CreateHalftoneDots;-1;;50;048136905c997409990aca2254cdd604;0;5;18;FLOAT;0;False;17;FLOAT;0;False;16;FLOAT3x3;0,0,0,0,0,1,0,0,0,0,1,0,0,0,0,1;False;15;FLOAT;0;False;21;FLOAT;2;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;207;-1148.059,833.9817;Float;False;415.5033;276.5253;white;2;202;201;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;-1051.646,103.6731;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;189;-1060.89,320.5431;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.85;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;-1098.059,883.9817;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;191;-1141.482,439.514;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0.3;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;210;-1747.749,-300.1306;Float;True;greyscale;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;59;-1053.022,568.7383;Float;False;260;241;black;1;58;;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;192;-926.9995,19.18163;Float;False;194;243;rgb screen;1;187;;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;190;-884.3527,401.1364;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-1011.022,599.7382;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;212;-647.8746,988.8265;Float;False;210;greyscale;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;187;-896.9462,54.12388;Float;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;202;-938.2358,885.0075;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0.98;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;21;-649.3352,302.7248;Float;True;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;214;-581.4566,167.4517;Float;False;Property;_greyscale;greyscale;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;211;-446.4059,672.232;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;223;-358.2693,266.478;Float;False;Property;_effectStrength;effectStrength;1;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;213;-244.9365,421.7118;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;222;-44.26935,314.478;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;25;144.8403,381.2842;Float;False;True;2;Float;ASEMaterialInspector;0;1;halftone;0770190933193b94aaa3065e307002fa;0;0;Unlit;2;True;0;1;False;-1;0;False;-1;0;1;False;-1;0;False;-1;True;0;False;-1;0;False;-1;True;0;False;-1;True;True;True;True;True;0;False;-1;True;False;255;False;-1;255;False;-1;255;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;True;1;False;-1;True;3;False;-1;True;True;0;False;-1;0;False;-1;True;1;RenderType=Opaque=RenderType;True;2;0;False;False;False;False;False;False;False;False;False;False;0;;0;0;Standard;0;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;0
WireConnection;64;0;36;0
WireConnection;48;0;26;0
WireConnection;48;1;47;0
WireConnection;52;0;26;0
WireConnection;52;1;51;0
WireConnection;42;0;26;0
WireConnection;42;1;41;0
WireConnection;65;0;64;0
WireConnection;43;0;42;0
WireConnection;53;0;52;0
WireConnection;49;0;48;0
WireConnection;54;0;53;0
WireConnection;68;0;65;1
WireConnection;68;1;65;2
WireConnection;44;0;43;0
WireConnection;50;0;49;0
WireConnection;66;0;65;0
WireConnection;66;1;65;1
WireConnection;66;2;65;2
WireConnection;67;0;65;0
WireConnection;67;1;68;0
WireConnection;46;0;44;0
WireConnection;46;1;50;0
WireConnection;46;2;54;0
WireConnection;69;0;66;0
WireConnection;69;1;67;0
WireConnection;56;0;46;0
WireConnection;56;1;57;0
WireConnection;70;0;69;0
WireConnection;70;3;67;0
WireConnection;74;0;56;0
WireConnection;85;0;70;0
WireConnection;194;18;3;0
WireConnection;194;17;85;1
WireConnection;194;16;159;0
WireConnection;194;15;110;0
WireConnection;194;21;221;0
WireConnection;193;18;3;0
WireConnection;193;17;85;0
WireConnection;193;16;116;0
WireConnection;193;15;110;0
WireConnection;193;21;221;0
WireConnection;195;18;3;0
WireConnection;195;17;85;2
WireConnection;195;16;196;0
WireConnection;195;15;110;0
WireConnection;195;21;221;0
WireConnection;81;0;194;0
WireConnection;80;0;193;0
WireConnection;82;0;195;0
WireConnection;197;18;3;0
WireConnection;197;17;85;3
WireConnection;197;16;31;0
WireConnection;197;15;110;0
WireConnection;197;21;221;0
WireConnection;199;0;64;0
WireConnection;83;0;197;0
WireConnection;180;0;181;0
WireConnection;180;1;182;0
WireConnection;180;2;183;0
WireConnection;184;0;180;0
WireConnection;184;1;185;0
WireConnection;209;18;3;0
WireConnection;209;17;199;0
WireConnection;209;16;208;0
WireConnection;209;15;110;0
WireConnection;209;21;221;0
WireConnection;186;0;184;0
WireConnection;186;1;84;0
WireConnection;189;0;188;0
WireConnection;201;0;84;0
WireConnection;191;0;84;0
WireConnection;210;0;209;0
WireConnection;190;0;189;0
WireConnection;190;1;191;0
WireConnection;58;0;84;0
WireConnection;187;0;186;0
WireConnection;202;0;201;0
WireConnection;21;0;187;0
WireConnection;21;1;58;0
WireConnection;21;2;190;0
WireConnection;211;0;202;0
WireConnection;211;1;58;0
WireConnection;211;2;212;0
WireConnection;213;0;21;0
WireConnection;213;1;211;0
WireConnection;213;2;214;0
WireConnection;222;0;36;0
WireConnection;222;1;213;0
WireConnection;222;2;223;0
WireConnection;25;0;222;0
ASEEND*/
//CHKSM=42220DA40D6A3FD2EE87EDEE3117BB16BC291E3E