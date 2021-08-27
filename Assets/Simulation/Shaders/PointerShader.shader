/* * *
Copyright (c) 2021 Dean Stevens and affiliates.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files ("MDVM"), to deal in
MDVM without restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
MDVM, and to permit persons to whom MDVM is furnished to do so, subject to
the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of MDVM.

MDVM IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR
A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL DEAN STEVENS,
OTHER AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH MDVM OR THE USE OR OTHER DEALINGS IN MDVM.

If you reside in Australia, the above disclaimer does not affect any consumer
guarantees automatically given to you under the Competition and Consumer Act
2010 (Commonwealth) and the Australian Consumer Law.
 * * */

Shader "Unlit/PointerShader"
{
    /*
     * Render pipeline revision:
     * Application -> Vertex Shader -> Geometry Shader -> <tesselation> -> Domain Shader -> <rasterization> -> Fragment Shader -> Post-process -> GPU Output
     * 
     * This is the first time that I have ever programmed a shader! You know, I always learn something new every day. (Dean Stevens, 15/06/2021)
     */

    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Main Color", Color) = (0.5,0.5,0.5,0.5)
    }
    SubShader
    {
        Tags 
        { 
            // Allows transparency
            "Queue"="Transparent" 
            "RenderType"="Transparent" 
        }
        LOD 100

        Pass
        {
            // Render options
            Blend SrcAlpha OneMinusSrcAlpha    // Allows transparency.
            Cull Back    // Backface culling.

            // Execute on the GPU
            CGPROGRAM

            // Two shader processes
            #pragma vertex vert
            #pragma fragment frag

            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            // Application data transferred over for the vertex shader.
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            // Results from vertex shading for the fragment (pixel) shader.
            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            // Shader global parameters
            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;

            // Vertex shader: define the color of the vertex.
            v2f vert (appdata v)
            {
                v2f o;

                // Define UV texture positions using our vertex.
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);

                // Define our color of the vertex here for the fragment shader
                o.color = _Color;
                return o;
            }

            // Fragment (pixel) shader
            // Allow the shader program to rasterize the vertices and triangles as normal, and then
            // apply our fragment shading (pixels). Use the z-buffer to allow transparency effects.
            fixed4 frag (v2f i) : SV_Target
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
}
