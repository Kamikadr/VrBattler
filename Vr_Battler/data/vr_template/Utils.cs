#region Math Variables
#if UNIGINE_DOUBLE
	using Scalar = System.Double;
	using Vec2 = Unigine.dvec2;
	using Vec3 = Unigine.dvec3;
	using Vec4 = Unigine.dvec4;
	using Mat4 = Unigine.dmat4;
#else
using Scalar = System.Single;
using Vec2 = Unigine.vec2;
using Vec3 = Unigine.vec3;
using Vec4 = Unigine.vec4;
using Mat4 = Unigine.mat4;
using WorldBoundBox = Unigine.BoundBox;
using WorldBoundSphere = Unigine.BoundSphere;
using WorldBoundFrustum = Unigine.BoundFrustum;
#endif
#endregion

using Unigine;
using System;

public class Utils
{
	static public vec3 LinearRegression(vec3[] values)
	{
		if (values.Length <= 0)
			return vec3.ZERO;

		float sumOfX = 0;
		vec3 sumOfY = vec3.ZERO;
		float sumOfXSq = 0;
		vec3 sumOfYSq = vec3.ZERO;
		vec3 sumCodeviates = vec3.ZERO;
		float count = values.Length;

		for (int ctr = 0; ctr < count; ctr++)
		{
			float x = ctr;
			vec3 y = values[ctr];
			sumCodeviates += y * x;
			sumOfX += x;
			sumOfY += y;
			sumOfXSq += x * x;
			sumOfYSq += y * y;
		}

		float ssX = sumOfXSq - ((sumOfX * sumOfX) / count);
		vec3 ssY = sumOfYSq - ((sumOfY * sumOfY) / count);
		vec3 sCo = sumCodeviates - ((sumOfY * sumOfX) / count);

		float meanX = sumOfX / count;
		vec3 meanY = sumOfY / count;

		vec3 yintercept = meanY - ((sCo / ssX) * meanX);
		vec3 slope = sCo / ssX;

		return slope * values.Length + yintercept;
	}

	static public void GetAxisAngle(quat rot, out vec3 axis, out float angle)
	{
		float ilength = MathLib.Rsqrt(rot.x * rot.x + rot.y * rot.y + rot.z * rot.z);
		axis = new vec3(rot.x * ilength, rot.y * ilength, rot.z * ilength);
		angle = MathLib.Acos(MathLib.Clamp(rot.w, -1.0f, 1.0f)) * MathLib.RAD2DEG * 2.0f;
		if (angle > 180.0f)
			angle -= 360.0f;

		axis = new vec3(axis.x, axis.y, axis.z);
	}

	static public vec3 GetDirectionX(Node node)
	{
		mat4 mat = new mat4(node.Transform);
		return new vec3(mat.m00, mat.m10, mat.m20); // column3(0)
	}

	static public vec3 GetDirectionY(Node node)
	{
		mat4 mat = new mat4(node.Transform);
		return new vec3(mat.m01, mat.m11, mat.m21); // column3(1)
	}

	static public vec3 GetDirectionZ(Node node)
	{
		mat4 mat = new mat4(node.Transform);
		return new vec3(mat.m02, mat.m12, mat.m22); // column3(2)
	}

	static public vec3 GetDirectionNX(Node node) { return -GetDirectionX(node); }
	static public vec3 GetDirectionNY(Node node) { return -GetDirectionY(node); }
	static public vec3 GetDirectionNZ(Node node) { return -GetDirectionZ(node); }

	static public vec3 GetWorldDirectionX(Node node)
	{
		mat4 mat = new mat4(node.WorldTransform);
		return new vec3(mat.m00, mat.m10, mat.m20); // column3(0)
	}

	static public vec3 GetWorldDirectionY(Node node)
	{
		mat4 mat = new mat4(node.WorldTransform);
		return new vec3(mat.m01, mat.m11, mat.m21); // column3(1)
	}

	static public vec3 GetWorldDirectionZ(Node node)
	{
		mat4 mat = new  mat4(node.WorldTransform);
		return new vec3(mat.m02, mat.m12, mat.m22); // column3(2)
	}

	static public vec3 GetWorldDirectionNX(Node node) { return -GetWorldDirectionX(node); }
	static public vec3 GetWorldDirectionNY(Node node) { return -GetWorldDirectionY(node); }
	static public vec3 GetWorldDirectionNZ(Node node) { return -GetWorldDirectionZ(node); }

	static public vec3 GetDirectionX(mat4 mat)
	{
		return new vec3(mat.m00, mat.m10, mat.m20); // column3(0)
	}

	static public vec3 GetDirectionY(mat4 mat)
	{
		return new vec3(mat.m01, mat.m11, mat.m21); // column3(1)
	}

	static public vec3 GetDirectionZ(mat4 mat)
	{
		return new vec3(mat.m02, mat.m12, mat.m22); // column3(2)
	}

	static public vec3 GetDirectionNX(mat4 mat) { return -GetDirectionX(mat); }
	static public vec3 GetDirectionNY(mat4 mat) { return -GetDirectionY(mat); }
	static public vec3 GetDirectionNZ(mat4 mat) { return -GetDirectionZ(mat); }

	static public vec3 GetWorldDirectionX(mat4 mat)
	{
		return new vec3(mat.m00, mat.m10, mat.m20); // column3(0)
	}

	static public vec3 GetWorldDirectionY(mat4 mat)
	{
		return new vec3(mat.m01, mat.m11, mat.m21); // column3(1)
	}

	static public vec3 GetWorldDirectionZ(mat4 mat)
	{
		return new vec3(mat.m02, mat.m12, mat.m22); // column3(2)
	}

	static public vec3 GetWorldDirectionNX(mat4 mat) { return -GetWorldDirectionX(mat); }
	static public vec3 GetWorldDirectionNY(mat4 mat) { return -GetWorldDirectionY(mat); }
	static public vec3 GetWorldDirectionNZ(mat4 mat) { return -GetWorldDirectionZ(mat); }

	static public void AddLineSegment(ObjectMeshDynamic mesh, vec3 from, vec3 to, vec3 from_right, vec3 to_right, float width)
	{
		mesh.AddTriangleQuads(1);
		vec3 p0 = from - from_right * width * 0.5f; // 0, 0
		vec3 p1 = from + from_right * width * 0.5f;  // 1, 0
		vec3 p2 = to + to_right * width * 0.5f; // 1, 1
		vec3 p3 = to - to_right * width * 0.5f; // 0, 1
		mesh.AddVertex(p0); mesh.AddTexCoord(new vec4(0, 0, 0, 0));
		mesh.AddVertex(p1); mesh.AddTexCoord(new vec4(1, 0, 0, 0));
		mesh.AddVertex(p2); mesh.AddTexCoord(new vec4(1, 1, 0, 0));
		mesh.AddVertex(p3); mesh.AddTexCoord(new vec4(0, 1, 0, 0));
	}

	static public void AddLineSegment(ObjectMeshDynamic mesh, vec3 from, vec3 to, vec3 from_forward, float width)
	{
		vec3 up = vec3.UP;
		vec3 to_forward = (to - from).Normalized;
		vec3 to_right = MathLib.Normalize(MathLib.Cross(to_forward, up));
		vec3 from_right = MathLib.Normalize(MathLib.Cross(from_forward, up));
		AddLineSegment(mesh, from, to, from_right, to_right, width);
	}

	static public void AddLineSegment(ObjectMeshDynamic mesh, vec3 from, vec3 to, float width)
	{
		vec3 from_forward = (to - from).Normalized;
		AddLineSegment(mesh, from, to, from_forward, width);
	}
}
