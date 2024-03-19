using UnityEngine;

public static class MeshGenerator2
{
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve _heightCurve, int levelOfDetail)
    {
        AnimationCurve heightCurve = new AnimationCurve(_heightCurve.keys);
        //Get width and height of height map
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        //Center the mesh
        float topLextX = (width - 1) / -2f;
        float topLextZ = (height - 1) / 2f;

        //Figure out how much to simplify the mesh.
        int meshSimplificationIncrement = (levelOfDetail == 0)?1:levelOfDetail * 2;
        
        //determine how many verticies to use per line with the MSI.
        int verticiesPerLine = (width - 1) / meshSimplificationIncrement + 1;
        
        //Creates an instance of a class we created to easily manage and manipulate meshes. 
        MeshData meshData = new MeshData(verticiesPerLine, verticiesPerLine);
        
        //which vertex we working with?
        int vertexIndex = 0;
        
        //From 0 to our new resolution.
        for (int y = 0; y < height; y += meshSimplificationIncrement)
        {
            for (int x = 0; x < width; x += meshSimplificationIncrement)
            {
                //For this particular vertex. Create a Vector three using our current location and the height map,
                meshData.verticies[vertexIndex] = new Vector3(topLextX + x, heightCurve.Evaluate(heightMap[x, y]) * heightMultiplier, topLextZ - y);
                
                //Convert pixels on the mesh to U,V coordinates. The division makes it somewhere between 1 and 0
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);
                
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + verticiesPerLine + 1, vertexIndex + verticiesPerLine);
                    meshData.AddTriangle(vertexIndex + verticiesPerLine + 1, vertexIndex, vertexIndex + 1);
                }
                vertexIndex++;
            }
        }
        return meshData;
    }
}

public class MeshData
{
    public Vector3[] verticies;
    public int[] triangles;
    public Vector2[] uvs;
    private int triangleIndex;
    public MeshData(int meshWidth, int meshHeight)
    {
        verticies = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshHeight * meshWidth];
        triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateBounds();
        return mesh;
    }
}
