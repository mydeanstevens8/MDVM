Shader "Custom/OutlineOnlyShader"
{
	Properties
	{
		_Color("Highlight Color", Color) = (1,1,1,0)

		_OutlineColor("Outline Color", Color) = (0,0,0,1)
		_OutlineWidth("Outline width", Range(0.0, 0.1)) = .005

		[HideInInspector] _Mode("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend("__src", Float) = 1.0
		[HideInInspector] _DstBlend("__dst", Float) = 0.0
		[HideInInspector] _ZWrite("__zw", Float) = 1.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" }

		// Pass 1: Outline!
		// Taken from the ToyShaderWithOutline provided by Oculus.
		Pass
		{
			Name "OUTLINE"
			// To allow the object to render entirely on top of the outline.
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Cull Front  // We will hide front-facing faces to allow the object to render at the front.

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			// Application data
			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// Vertex shading data
			struct v2f
			{
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;

				UNITY_VERTEX_OUTPUT_STEREO
			};

			// Parameters for outlining
			uniform float _OutlineWidth;
			uniform float4 _OutlineColor;
			uniform float4x4 _ObjectToWorldFixed;

			// VERTEX SHADER: Change the position of vertices.
			// Pushes the verts out a little from the object center.
			// Lets us give an outline to objects that all have normals facing away from the center.
			// If we can't assume that, we need to tweak the math of this shader.
			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_OUTPUT(v2f, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				// MTF TODO 
				// 1. Fix batching so that it actually occurs.
				// 2. See if batching causes problems,
				// if it does fix this line by adding that component that sets it.
				//float4 objectCenterWorld = mul(_ObjectToWorldFixed, float4(0.0, 0.0, 0.0, 1.0));
				float4 objectCenterWorld = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
				float4 vertWorld = mul(unity_ObjectToWorld, v.vertex);

				float3 offsetDir = vertWorld.xyz - objectCenterWorld.xyz;
				offsetDir = normalize(offsetDir) * _OutlineWidth;

				o.pos = UnityWorldToClipPos(vertWorld + offsetDir);

				o.color = _OutlineColor;
				return o;
			}

			// FRAGMENT SHADER
			fixed4 frag(v2f i) : SV_Target
			{
				// Just draw the _OutlineColor from the vert pass above.
				return i.color;
			}
			ENDCG
		}

		// Pass 2: Draw Transparent overlay
		Pass 
		{
			Name "OUTLINESHADE"

			Blend SrcAlpha OneMinusSrcAlpha    // Allows transparency.
			Cull Back    // Backface culling.

			CGPROGRAM
			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			// Shader global parameters
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;

			// Application data
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : NORMAL;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			// Vertex shading data
			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 pos : SV_POSITION;
				fixed4 color : COLOR;

				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert(appdata v)
			{
				v2f o;

				// Define UV texture positions using our vertex.
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				UNITY_TRANSFER_FOG(o, o.pos);

				// Define our color of the vertex here for the fragment shader
				o.color = _Color;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// sample the texture and also apply transparency and color effects
				fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color * 2.0;

				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG

		}

	}

	FallBack "Diffuse"
}
