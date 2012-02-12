// Include all shared types
#include "Includes.inc"

//
//
// Input/Output
//
//

	struct VertexIn {
		float4	position	:	POSITION;
	};
	
	struct VertexOut {
		float4  position		   :	POSITION;
	};

//
//
// Vertex Shaders
//
//

	VertexOut SolidColorVertexShader(VertexIn input)
	{
		VertexOut output;
		
		// Create world, view, and projection matrix
		float4x4 worldViewProjMatrix = mul(mul(worldMatrix, viewMatrix), projectionMatrix);
		
		// Compute output position in projection space
		output.position = mul(input.position, worldViewProjMatrix);
		
		return output;
	}
	
//
//
// Pixel Shader
//
//

	float4 SolidColorPixelShader(VertexOut input) : COLOR
	{	
		return materialDiffuseColor;
	}
	
//
//
// Techniques
//
//

	technique SolidColor
	{
		pass Pass0
		{
			VertexShader = compile vs_2_0 SolidColorVertexShader();
			PixelShader = compile ps_2_0 SolidColorPixelShader();
			
			CullMode = CCW;
		}
	}
	
	
	