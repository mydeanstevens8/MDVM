Shader "Outline/NewOutlineShader"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineWidth ("Outline Width", Float) = 0.01
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }
        LOD 100

        // To allow the object to render entirely on top of the outline.
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front  // We will hide front-facing faces to allow the object to render at the front.

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2g
            {
                UNITY_FOG_COORDS(1)
                float4 normal : NORMAL;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            struct g2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                fixed4 color : COLOR;
            };

            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;

            v2g vert (appdata v)
            {
                v2g o;
                o.vertex = v.vertex;
                o.normal = v.normal;
                o.color = _OutlineColor;

                return o;
            }

            // Apply for each triangle
            [maxvertexcount(3)]
            void geom(triangle v2g input[3], inout TriangleStream<g2f> triStream)
            {
                g2f o;

                // The normal of our geometry is given as below:
                // (Clockwise winding order)
                float3 geomWorldNormal = mul(unity_ObjectToWorld, normalize(cross(input[1].vertex - input[0].vertex, input[2].vertex - input[0].vertex)));

                float4 objectCenterInWorld = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));

                float3 geomWorldCenterPoint = mul(unity_ObjectToWorld, (input[0].vertex + input[1].vertex + input[2].vertex) / 3);

                for (int i = 0; i < 3; i++)
                {
                    v2g v = input[i];

                    float4 vertWorld = mul(unity_ObjectToWorld, v.vertex);
                    float4 vOldVert = vertWorld;

                    float calcDistFrom = distance(_WorldSpaceCameraPos, vertWorld);

                    float calcOutWidth = _OutlineWidth * calcDistFrom;

                    // Push out the normals by the outline width given.
                    float4 vNewVert = vOldVert;
                    vNewVert.xyz += normalize(v.normal) * calcOutWidth;

                    o.vertex = UnityWorldToClipPos(vNewVert);
                    UNITY_TRANSFER_FOG(o, o.vertex);

                    o.normal = UnityObjectToWorldNormal(v.normal);

                    o.color = v.color;
                    triStream.Append(o);
                }
                triStream.RestartStrip();
            }

            fixed4 frag (g2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = i.color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }

        // UsePass "Standard/FORWARD"
    }
}
