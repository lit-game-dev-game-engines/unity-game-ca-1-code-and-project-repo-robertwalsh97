using UnityEngine;

// When the player pauses the game, a sphere is created around them
// In Unity you cannot see polygons from the inside of an object (for optimisation purposes) so it must be flipped inside out
public class FlipNormals : MonoBehaviour
{
	// Use this for initialization
    // Reverting vertices from clockwise to counter-clockwise causes the mesh to render on the opposite side
    // The normal is responsible for determining if vertices are ordered clockwise or counter-clockwise
	void Start ()
    {
        // Get the sphere mesh and create a Vector3 array to store all its normals
        Mesh mesh = this.GetComponent<MeshFilter>().mesh;
        Vector3[] normals = mesh.normals;

        // Loop through normals one by one and invert them
        for(int i = 0; i < normals.Length; i++)
        {
            normals[i] = -1 * normals[i]; // Making the normal value negative flips the normal 180 degrees
        }

        // Set the mesh normals array to the new inverted normals array
        mesh.normals = normals;

        // Turn the triangles from clockwise to anticlockwise order
        // The first loop gets a triangle from the array of triangles, the second gets that triangles vertices
        for(int i = 0; i <mesh.subMeshCount; i++)
        {
            int[] tris = mesh.GetTriangles(i);

            for(int j = 0; j < tris.Length; j+=3)
            {
                // Swap order of vertices. Only the first and last vertex needs to be swapped, the second always stays in the same position
                int temp = tris[j];
                tris[j] = tris[j + 1];
                tris[j + 1] = temp;
            }

            // Put the new triangles into the mesh
            mesh.SetTriangles(tris, i);
        }
	}
}
