/**
 * Copyright (c) 2015 Allan Bishop http://www.allanbishop.com
 *
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 *
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 **/

using UnityEngine;
using System.Collections;

public class DebugDrawPhysics2D : MonoBehaviour 
{

	private const int JOINT_CIRCLE_SEGMENTS = 20;
	private const int CIRCLE_COLLIDER_SEGMENTS = 40;

	private Material lineMaterial;
	private BoxCollider2D[] boxColliders2D;
	private PolygonCollider2D[] polygonColliders2D;
	private CircleCollider2D[] circleColliders2D;
	private EdgeCollider2D[] edgeColliders2D;
	private AnchoredJoint2D[] anchoredJoints2D;
	private Vector3[][] boxPointList;
	private Vector3[][] circlePointList;
	private Vector3[][] polygonPointList;
	private Vector3[][] edgePointList;
	private Vector3[][] anchoredJointPointList;

	public Color circleColour = Color.white;
	public Color polygonColour = Color.white;
	public Color boxColour = Color.white;
	public Color edgeColour = Color.white;
	public Color jointColour = Color.yellow;
	
	void Start () 
	{
		CreateLineMaterial();
		boxColliders2D = (BoxCollider2D[])Resources.FindObjectsOfTypeAll(typeof(BoxCollider2D));
		polygonColliders2D = (PolygonCollider2D[])Resources.FindObjectsOfTypeAll(typeof(PolygonCollider2D));
		circleColliders2D = (CircleCollider2D[])Resources.FindObjectsOfTypeAll (typeof(CircleCollider2D));
		anchoredJoints2D = (AnchoredJoint2D[])Resources.FindObjectsOfTypeAll (typeof(AnchoredJoint2D));
		edgeColliders2D = (EdgeCollider2D[])Resources.FindObjectsOfTypeAll (typeof(EdgeCollider2D));
	}

	void Update () 
	{
		boxPointList = new Vector3[boxColliders2D.Length][];
		for(int i = 0; i< boxColliders2D.Length;i++)
		{
			BoxCollider2D collider = boxColliders2D[i];
			Vector3[] boundPoints = GetBoxPoints(collider);
			boxPointList[i] = boundPoints;
		}

		circlePointList = new Vector3[circleColliders2D.Length][];

		for(int i = 0; i< circleColliders2D.Length;i++)
		{
			CircleCollider2D collider = circleColliders2D[i];
			Vector3[] circlePoints = GetCircleColliderPoints(collider,CIRCLE_COLLIDER_SEGMENTS);
			circlePointList[i] = circlePoints;
		}

		polygonPointList = new Vector3[polygonColliders2D.Length][];
		
		for(int i = 0; i< polygonColliders2D.Length;i++)
		{
			PolygonCollider2D collider = polygonColliders2D[i];
			Vector3[] polygonPoints = GetPolygonPoints(collider);
			polygonPointList[i] = polygonPoints;
		}
		
		edgePointList = new Vector3[edgeColliders2D.Length][];
		
		for(int i = 0; i< edgeColliders2D.Length;i++)
		{
			EdgeCollider2D collider = edgeColliders2D[i];
			Vector3[] edgePoints = GetEdgePoints(collider);
			edgePointList[i] = edgePoints;
		}
		
		anchoredJointPointList = new Vector3[anchoredJoints2D.Length][];
		
		for(int i = 0; i< anchoredJoints2D.Length;i++)
		{
			AnchoredJoint2D anchoredJoint = anchoredJoints2D[i];
			Vector3[] anchoredJointPoints = GetAnchoredJointPoints(anchoredJoint);
			anchoredJointPointList[i] = anchoredJointPoints;
		}
	}

	Vector3[] GetPolygonPoints (PolygonCollider2D collider)
	{
		Vector3[] points = new Vector3[collider.points.Length + 1];
		for(int i = 0; i< collider.points.Length;i++)
		{
			Vector2 p = collider.points[i];
			Vector3 point = collider.transform.TransformPoint(p.x+collider.offset.x,p.y+collider.offset.y,0);
			points[i] = point;
		}

		points [collider.points.Length] = points [0];

		return points;
	}
	
	Vector3[] GetEdgePoints (EdgeCollider2D collider)
	{
		Vector3[] points = new Vector3[collider.points.Length];
		for(int i = 0; i< collider.points.Length;i++)
		{
			Vector2 p = collider.points[i];
			Vector3 point = collider.transform.TransformPoint(p.x+collider.offset.x,p.y+collider.offset.y,0);
			points[i] = point;
		}

		return points;
	}

	Vector3[] GetAnchoredJointPoints(AnchoredJoint2D joint)
	{
		if (joint.connectedBody == null) 
		{
			return new Vector3[0];
		}
	
		Vector3[] points = new Vector3[2];

		points[0] = joint.gameObject.transform.TransformPoint(joint.anchor.x,joint.anchor.y,0);
		points[1] = joint.connectedBody.transform.TransformPoint(joint.connectedAnchor.x,joint.connectedAnchor.y,0);

		if(points[0]==points[1])
		{
			points = GetCircle(points[0].x,points[0].y,0.1f,JOINT_CIRCLE_SEGMENTS);
		}

		return points;

	}

	Vector3[] GetBoxPoints(BoxCollider2D collider)
	{
		Vector2 scale = collider.size;
		scale*=0.5f;
		Vector3[] points = new Vector3[5];

		points[0] = collider.transform.TransformPoint(new Vector3(-scale.x+collider.offset.x,scale.y+collider.offset.y,0));
		points[1] = collider.transform.TransformPoint(new Vector3(scale.x+collider.offset.x,scale.y+collider.offset.y,0));
		points[2] = collider.transform.TransformPoint(new Vector3(scale.x+collider.offset.x,-scale.y+collider.offset.y,0));
		points[3] = collider.transform.TransformPoint(new Vector3(-scale.x+collider.offset.x,-scale.y+collider.offset.y,0));
		points[4] = points[0];

		return points;
	}


	Vector3[] GetCircle(float x, float y, float radius, int segments)
	{
		float segmentSize = 360f / segments;
		Vector3[] circlePoints = new Vector3[segments+1];
		
		for(int i = 0; i< segments;i++)
		{
			Vector3 p = new Vector3(Mathf.Cos(Mathf.Deg2Rad*(i*segmentSize))*radius+x,Mathf.Sin(Mathf.Deg2Rad*(i*segmentSize))*radius+y);
			circlePoints[i] = p;
		}

		circlePoints [segments] = circlePoints [0];
		return circlePoints;
	}

	
	Vector3[] GetCircleColliderPoints(CircleCollider2D collider, int segments)
	{
		float radius = collider.radius;
		float angle = collider.transform.rotation.z;
		float segmentSize = 360f / segments;
		Vector3[] circlePoints = new Vector3[segments+3];

		//drawing the angle line
		circlePoints [0] = collider.transform.TransformPoint (new Vector3 (collider.offset.x, collider.offset.y));
		circlePoints [1] = collider.transform.TransformPoint(new Vector3(Mathf.Cos(Mathf.Deg2Rad*angle)*radius+collider.offset.x,Mathf.Sin(Mathf.Deg2Rad*angle)*radius+collider.offset.y));

		for(int i = 0; i< segments;i++)
		{
			Vector3 p = collider.transform.TransformPoint(new Vector3(Mathf.Cos(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.x,Mathf.Sin(Mathf.Deg2Rad*(i*segmentSize+angle))*radius+collider.offset.y));
			circlePoints[i+2] = p;
		}

		circlePoints [segments+2] = circlePoints [1];
		return circlePoints;
	}

	void OnDrawGizmos() 
	{
		if (!Application.isPlaying) 
		{
			boxColliders2D = (BoxCollider2D[])Resources.FindObjectsOfTypeAll (typeof(BoxCollider2D));
			boxPointList = new Vector3[boxColliders2D.Length][];

			circleColliders2D = (CircleCollider2D[])Resources.FindObjectsOfTypeAll (typeof(CircleCollider2D));
			circlePointList = new Vector3[circleColliders2D.Length][];

			polygonColliders2D = (PolygonCollider2D[])Resources.FindObjectsOfTypeAll (typeof(PolygonCollider2D));
			polygonPointList = new Vector3[polygonColliders2D.Length][];
		
			edgeColliders2D = (EdgeCollider2D[])Resources.FindObjectsOfTypeAll (typeof(EdgeCollider2D));
			edgePointList = new Vector3[edgeColliders2D.Length][];

			anchoredJoints2D = (AnchoredJoint2D[])Resources.FindObjectsOfTypeAll (typeof(AnchoredJoint2D));
			anchoredJointPointList = new Vector3[anchoredJoints2D.Length][];

			for (int i = 0; i< boxColliders2D.Length; i++) {
				BoxCollider2D collider = boxColliders2D [i];
				Vector3[] boundPoints = GetBoxPoints (collider);
				boxPointList [i] = boundPoints;
			}

			for (int i = 0; i< circleColliders2D.Length; i++) {
				CircleCollider2D collider = circleColliders2D [i];
				Vector3[] circlePoints = GetCircleColliderPoints (collider, 40);
				circlePointList [i] = circlePoints;
			}

			for (int i = 0; i< polygonColliders2D.Length; i++) {
				PolygonCollider2D collider = polygonColliders2D [i];
				Vector3[] polygonPoints = GetPolygonPoints (collider);
				polygonPointList [i] = polygonPoints;
			}
		
			for (int i = 0; i< edgeColliders2D.Length; i++) {
				EdgeCollider2D collider = edgeColliders2D [i];
				Vector3[] edgePoints = GetEdgePoints (collider);
				edgePointList [i] = edgePoints;
			}

			for (int i = 0; i< anchoredJoints2D.Length; i++) {
				AnchoredJoint2D anchoredJoint = anchoredJoints2D [i];
				Vector3[] anchoredJointPoints = GetAnchoredJointPoints (anchoredJoint);
				anchoredJointPointList [i] = anchoredJointPoints;
			}
	
			DrawBox2DGizmo (boxPointList);
			DrawBox2DGizmo (circlePointList);
			DrawBox2DGizmo (polygonPointList);
			DrawBox2DGizmo (edgePointList);
			DrawBox2DGizmo (anchoredJointPointList);
		}
	}

	void DrawBox2DGizmo(Vector3[][] colliderPoints)
	{
		for(int i = 0; i < colliderPoints.Length;i++)
		{
			Vector3[] points = colliderPoints[i];
			
			for(int k = 1; k < points.Length;k++)
			{
				Vector3 p1 = points[k-1];
				Vector3 p2 = points[k];
				Gizmos.DrawLine(p1, p2);
			}
		}
	}

	void OnPostRender()
	{
		RenderColliders (polygonPointList, polygonColour);
		RenderColliders (boxPointList, boxColour);
		RenderColliders (circlePointList, circleColour);
		RenderColliders (edgePointList, edgeColour);
		RenderColliders (anchoredJointPointList, jointColour);
	}

	void RenderColliders(Vector3[][] colliderPoints, Color colour)
	{
		for(int i = 0; i < colliderPoints.Length;i++)
		{
			Vector3[] points = colliderPoints[i];
			
			GL.Begin(GL.LINES);
			GL.Color(colour);
			
			for(int k = 1; k < points.Length;k++)
			{
				Vector3 p1 = points[k-1];
				GL.Vertex3(p1.x,p1.y,p1.z);
				
				Vector3 p2 = points[k];
				GL.Vertex3(p2.x,p2.y,p2.z);
			}
			
			GL.End();
		}
	}

	void CreateLineMaterial() 
	{
		if( lineMaterial == null )
		{ 
			lineMaterial = new Material(
				@"Shader ""Lines/Colored Blended"" 
				{
     				SubShader 
					{
	         			Tags { ""RenderType""=""Opaque"" }
	        			Pass 
						{
				             ZWrite On
				             ZTest LEqual
				             Cull Off
				             Fog { Mode Off }
				             BindChannels 
							 {
	                 			Bind ""vertex"", vertex Bind ""color"", color
	             			 }
	         			}
     				}
 				}");
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;    
		}
	}
}
