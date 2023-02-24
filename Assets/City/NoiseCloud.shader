// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Julian/NoiseCloud"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.135
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveDirection("WaveDirection", Vector) = (1,0,0,0)
		_NoiseScale("NoiseScale", Float) = 10
		_NoiseDirection("NoiseDirection", Vector) = (1,1,1,0)
		_TimeScaleNoise("TimeScaleNoise", Float) = 1
		_3DNoisceScale("3DNoisceScale", Vector) = (1,1,1,0)
		_TransparencyGradient("TransparencyGradient", 2D) = "white" {}
		[HDR]_Color("Color", Color) = (1,1,1,1)
		_WaveTiling("WaveTiling", Vector) = (0,0,0,0)
		_WaveScale("WaveScale", Float) = 1
		_WaveStrength("WaveStrength", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow noambient nofog nometa noforwardadd 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform float _WaveSpeed;
		uniform float2 _WaveDirection;
		uniform half2 _WaveTiling;
		uniform half _WaveScale;
		uniform half _WaveStrength;
		uniform half3 _NoiseDirection;
		uniform half _TimeScaleNoise;
		uniform half3 _3DNoisceScale;
		uniform half _NoiseScale;
		uniform half4 _Color;
		uniform sampler2D _TransparencyGradient;
		uniform half4 _TransparencyGradient_ST;
		uniform float _Cutoff = 0.135;


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


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			half mulTime250 = _Time.y * _WaveSpeed;
			float3 ase_worldPos = i.worldPos;
			half3 appendResult269 = (half3(ase_worldPos.x , ase_worldPos.z , 0.0));
			float2 uv_TexCoord248 = i.uv_texcoord * ( half3( _WaveTiling ,  0.0 ) * appendResult269 ).xy;
			half2 panner253 = ( mulTime250 * _WaveDirection + uv_TexCoord248);
			half simplePerlin2D254 = snoise( panner253*_WaveScale );
			half mulTime231 = _Time.y * _TimeScaleNoise;
			half simplePerlin3D119 = snoise( ( ( _NoiseDirection * mulTime231 ) + ( _3DNoisceScale * ase_worldPos ) )*_NoiseScale );
			o.Emission = ( ( ( simplePerlin2D254 * _WaveStrength ) + simplePerlin3D119 ) * _Color ).rgb;
			o.Alpha = 1;
			half temp_output_262_0 = (0.5 + (simplePerlin3D119 - 0.0) * (1.0 - 0.5) / (1.0 - 0.0));
			float2 uv_TransparencyGradient = i.uv_texcoord * _TransparencyGradient_ST.xy + _TransparencyGradient_ST.zw;
			half4 tex2DNode120 = tex2D( _TransparencyGradient, uv_TransparencyGradient );
			half blendOpSrc265 = temp_output_262_0;
			half blendOpDest265 = ( tex2DNode120.a * i.vertexColor.a );
			clip( ( saturate( (( blendOpSrc265 > 0.5 ) ? ( blendOpDest265 / max( ( 1.0 - blendOpSrc265 ) * 2.0 ,0.00001) ) : ( 1.0 - ( ( ( 1.0 - blendOpDest265 ) * 0.5 ) / max( blendOpSrc265,0.00001) ) ) ) )) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18912
120;620;1628;623;-1085.087;746.5431;1.9;True;True
Node;AmplifyShaderEditor.WorldPosInputsNode;268;1075.653,-1127.052;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;256;1246.874,-944.6548;Inherit;False;Property;_WaveTiling;WaveTiling;9;0;Create;True;0;0;0;False;0;False;0,0;0.2,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.DynamicAppendNode;269;1311.082,-1115.638;Inherit;False;FLOAT3;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;251;1378.633,-650.0377;Float;False;Property;_WaveSpeed;WaveSpeed;1;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;267;1429.967,-932.8452;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;1,1,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;232;1292.557,-316.2724;Inherit;False;Property;_TimeScaleNoise;TimeScaleNoise;5;0;Create;True;0;0;0;False;0;False;1;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;234;1519.917,-232.602;Inherit;False;Property;_3DNoisceScale;3DNoisceScale;6;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.Vector2Node;255;1356.25,-791.6214;Float;False;Property;_WaveDirection;WaveDirection;2;0;Create;True;0;0;0;False;0;False;1,0;1,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleTimeNode;250;1587.088,-705.0889;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;248;1594.603,-926.7928;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.75,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;226;1525.894,-78.75717;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleTimeNode;231;1524.451,-315.1599;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;230;1519.669,-470.8026;Inherit;False;Property;_NoiseDirection;NoiseDirection;4;0;Create;True;0;0;0;False;0;False;1,1,1;1,1,1;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;233;1721.948,-381.6336;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;235;1723.687,-191.497;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;258;1804.114,-633.8912;Inherit;False;Property;_WaveScale;WaveScale;10;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;253;1847.038,-869.3438;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;264;2083.967,-613.249;Inherit;False;Property;_WaveStrength;WaveStrength;11;0;Create;True;0;0;0;False;0;False;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;128;1903.344,-258.0131;Inherit;False;Property;_NoiseScale;NoiseScale;3;0;Create;True;0;0;0;False;0;False;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;236;1909.992,-384.4979;Inherit;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;254;2056.327,-897.817;Inherit;True;Simplex2D;False;False;2;0;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;271;2108.567,72.05494;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;120;2016.237,-144.6261;Inherit;True;Property;_TransparencyGradient;TransparencyGradient;7;0;Create;True;0;0;0;False;0;False;-1;None;08db3f1093106384b803a7effc00eb84;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;119;2088.128,-393.2674;Inherit;True;Simplex3D;False;True;2;0;FLOAT3;1,1,1;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;263;2271.967,-705.249;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;262;2414.567,-346.249;Inherit;True;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0.5;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;270;2415.367,5.75493;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;259;2392.268,-875.1498;Inherit;False;Property;_Color;Color;8;1;[HDR];Create;True;0;0;0;False;0;False;1,1,1,1;1.866667,0.8498246,0.1866667,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;257;2409.921,-617.1431;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;260;2684.438,-651.3824;Inherit;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendOpsNode;123;2754.422,-321.6609;Inherit;True;HardMix;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.BlendOpsNode;265;2769.967,-37.84902;Inherit;True;VividLight;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;3147.031,-651.9002;Half;False;True;-1;2;ASEMaterialInspector;0;0;Unlit;Julian/NoiseCloud;False;False;False;False;True;False;False;False;False;True;True;True;False;False;True;False;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.135;True;False;0;True;TransparentCutout;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;269;0;268;1
WireConnection;269;1;268;3
WireConnection;267;0;256;0
WireConnection;267;1;269;0
WireConnection;250;0;251;0
WireConnection;248;0;267;0
WireConnection;231;0;232;0
WireConnection;233;0;230;0
WireConnection;233;1;231;0
WireConnection;235;0;234;0
WireConnection;235;1;226;0
WireConnection;253;0;248;0
WireConnection;253;2;255;0
WireConnection;253;1;250;0
WireConnection;236;0;233;0
WireConnection;236;1;235;0
WireConnection;254;0;253;0
WireConnection;254;1;258;0
WireConnection;119;0;236;0
WireConnection;119;1;128;0
WireConnection;263;0;254;0
WireConnection;263;1;264;0
WireConnection;262;0;119;0
WireConnection;270;0;120;4
WireConnection;270;1;271;4
WireConnection;257;0;263;0
WireConnection;257;1;119;0
WireConnection;260;0;257;0
WireConnection;260;1;259;0
WireConnection;123;0;262;0
WireConnection;123;1;120;4
WireConnection;265;0;262;0
WireConnection;265;1;270;0
WireConnection;0;2;260;0
WireConnection;0;10;265;0
ASEEND*/
//CHKSM=B4154426705979D78321EB0F9FAF88836272A695