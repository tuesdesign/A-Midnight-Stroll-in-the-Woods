using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

namespace Broccoli.Utils
{
    public class MeshJob {
        #region Vars
        public int batchSize = 4;
        public List<Vector4> offsetScales = new List<Vector4> ();
        public List<Quaternion> rotations = new List<Quaternion> ();
        public List<float> bendings = new List<float> ();
        public List<int> starts = new List<int> ();
        public List<int> lengths = new List<int> ();
        private List<Vector3> vertices = new List<Vector3> ();
        private List<Vector3> normals = new List<Vector3> ();
        private List<Vector4> tangents = new List<Vector4> ();
        private Mesh targetMesh = null;
        #endregion

        #region Job
		/// <summary>
		/// Job structure to process branch skins.
		/// </summary>
		struct MeshJobImpl : IJobParallelFor {
			#region Input
			/// <summary>
			/// Contains the OFFSET (x, y, z) and SCALE (w) for the mesh segment.
			/// </summary>
			public NativeArray<Vector4> offsetScale;
			/// <summary>
			/// Contains the ORIENTATION for the mesh segment.
			/// </summary>
			public NativeArray<Quaternion> orientation;
			/// <summary>
			/// Contains the BENDING for the mesh segment.
			/// </summary>
			public NativeArray<float> bending;
			/// <summary>
            /// START for the submesh vertices.
            /// </summary>
            public NativeArray<int> start;
            /// <summary>
            /// LENGTH of the vertices for the submesh
            /// </summary>
            public NativeArray<int> length;
			#endregion

			#region Mesh Input
			/// <summary>
			/// Vertices for the input mesh.
			/// </summary>
			[NativeDisableParallelForRestriction]
			public NativeArray<Vector3> vertices;
			/// <summary>
			/// Normals for the input mesh.
			/// </summary>
			[NativeDisableParallelForRestriction]
			public NativeArray<Vector3> normals;
			/// <summary>
			/// Tangents for the input mesh.
			/// </summary>
			[NativeDisableParallelForRestriction]
			public NativeArray<Vector4> tangents;
			#endregion

			#region Job Methods
			/// <summary>
			/// Executes one per sprout.
			/// </summary>
			/// <param name="i"></param>
			public void Execute (int i) {
				Vector3 spOffset = (Vector3)offsetScale [i];
				Quaternion spOrientation = orientation [i];
				float spScale = offsetScale [i].w;
                float spBending = bending [i];
				int vertexStart = start [i];
				int vertexEnd = start [i] + length [i];

				// Apply the transformations.
				ApplyBend (vertexStart, vertexEnd, spBending);
				ApplyScale (vertexStart, vertexEnd, spScale);
				ApplyRotation (vertexStart, vertexEnd, spOrientation);
				ApplyTranslation (vertexStart, vertexEnd, spOffset);

                /*
				// Add the values to the output mesh vars.
				for (int j = vertexStart; j < vertexEnd; j++) {
					outputVertices [j] = _vertices [k];
					outputNormals [j] = _normals [k];
					outputTangents [j] = _tangents [k];
				}
                */
			}
			public void ApplyBend (int vertexStart, int vertexEnd, float bending) {
				Vector3 gravityForward = Vector3.forward;
				Vector3 gravityUp = Vector3.up;
				if (bending < 0) {
					gravityForward *= -1;
					gravityUp *= -1;
					bending *= -1;
				}
				Quaternion gravityQuaternion = Quaternion.LookRotation (gravityUp * -1, gravityForward);
				Quaternion bendQuaternion;
				float radialStrength;
				for (int i = 0; i < vertices.Length; i++) {
					radialStrength = bending * vertices[i].magnitude / 1f;
					bendQuaternion = Quaternion.Slerp (Quaternion.identity, gravityQuaternion, radialStrength);
					vertices [i] = bendQuaternion * vertices [i];
					normals [i] = bendQuaternion * normals [i];
					tangents [i] = bendQuaternion * tangents [i];
				}
			}
			public void ApplyScale (int vertexStart, int vertexEnd, float scale) {
				for (int i = vertexStart; i < vertexEnd; i++) {
					vertices [i] = vertices [i] * scale;
				}
			}
			public void ApplyRotation (int vertexStart, int vertexEnd, Quaternion orientation) {
				for (int i = vertexStart; i < vertexEnd; i++) {
					vertices [i] = orientation * vertices [i];
					normals [i] = orientation * normals [i];
					tangents [i] = orientation * tangents [i];
				}
			}
			public void ApplyTranslation (int vertexStart, int vertexEnd, Vector3 offset) {
				for (int i = vertexStart; i < vertexEnd; i++) {
					vertices [i] = vertices [i] + offset;
				}
			}
			#endregion
		}
		#endregion

        #region Processing
		/// <summary>
        /// Clears Job related variables.
        /// </summary>
        public void Clear () {
            offsetScales.Clear ();
            rotations.Clear ();
            bendings.Clear ();
            starts.Clear ();
            lengths.Clear ();
			ClearMesh ();
        }
        /// <summary>
        /// Clears Mesh related variables.
        /// </summary>
        public void ClearMesh () {
            vertices.Clear ();
            normals.Clear ();
            tangents.Clear ();
            targetMesh = null;
        }
        public void SetTargetMesh (Mesh mesh) {
            ClearMesh ();
            targetMesh = mesh;
            vertices.AddRange (mesh.vertices);
            normals.AddRange (mesh.normals);
            tangents.AddRange (mesh.tangents);
        }
        public Mesh GetTargetMesh () {
            return targetMesh;
        }
        public void AddTransform (int vertexStart, int vertexLength, Vector3 offset, float scale, Quaternion rotation, float bending) {
            starts.Add (vertexStart);
            lengths.Add (vertexLength);
            offsetScales.Add (new Vector4 (offset.x, offset.y, offset.z, scale));
            rotations.Add (rotation);
            bendings.Add (bending);
        }
        public void ExecuteJob () {
			// Mark the mesh as dynamic.
			targetMesh.MarkDynamic ();
			// Create the job.
			MeshJobImpl _meshJob = new MeshJobImpl () {
				offsetScale = new NativeArray<Vector4> (offsetScales.ToArray (), Allocator.TempJob),
				orientation = new NativeArray<Quaternion> (rotations.ToArray (), Allocator.TempJob),
				bending = new NativeArray<float> (bendings.ToArray (), Allocator.TempJob),
				start = new NativeArray<int> (starts.ToArray (), Allocator.TempJob),
				length = new NativeArray<int> (lengths.ToArray (), Allocator.TempJob),
				vertices = new NativeArray<Vector3> (vertices.ToArray (), Allocator.TempJob),
				normals = new NativeArray<Vector3> (normals.ToArray (), Allocator.TempJob),
				tangents = new NativeArray<Vector4> (tangents.ToArray (), Allocator.TempJob)
			};
			// Execute the job .
			JobHandle _meshJobHandle = _meshJob.Schedule (offsetScales.Count, batchSize);

			// Complete the job.
			_meshJobHandle.Complete();

			targetMesh.SetVertices (_meshJob.vertices);
			targetMesh.SetNormals (_meshJob.normals);
			targetMesh.SetTangents (_meshJob.tangents);
			targetMesh.UploadMeshData (true);

			// Dispose allocated memory.
			_meshJob.offsetScale.Dispose ();
			_meshJob.orientation.Dispose ();
			_meshJob.bending.Dispose ();
			_meshJob.start.Dispose ();
			_meshJob.length.Dispose ();
			_meshJob.vertices.Dispose ();
			_meshJob.normals.Dispose ();
			_meshJob.tangents.Dispose ();
        }
        #endregion
    }   
}
