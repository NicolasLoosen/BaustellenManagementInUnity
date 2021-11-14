/*
 * Runtime OBJ import, because Unity only does it for us in editor. Yay for reinventing the wheel!
 * Usage: ObjectLoader.Load(string directory_path, string filename), returns the root object.
 * Usually called from IfcImporter.RuntimeImport().
 *
 * v Geometric vertices
 * vt Texture vertices
 * vn Vertex normals
 * f Face
 * mtllib Related .mtl file
 * usemtl Name of the material that should be used
 */

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using System.Threading;
using System.Linq;

public class ObjectLoader : MonoBehaviour {

    public struct ObjData {
        public string name;
        public string mtllib;
        public List<string> usemtl;
        public List<Vector3> v;
        public List<Vector3> vn;
        public List<Vector2> vt;
        public List<List<int[]>> f;
    }

    public struct MtlData {
        public List<string> name;
        public Dictionary<string, Color> kd;
    }
    
    private static ObjData NewObjData() {
        ObjData obj = new ObjData ();
        obj.usemtl = new List<string> ();
        obj.v = new List<Vector3> ();
        obj.vn = new List<Vector3> ();
        obj.vt = new List<Vector2> ();
        obj.f = new List<List<int[]>> ();
        return obj;
    }

    public static List<ObjData> ReadObjData (string path) {
        string name = "";
        string mtllib = "";
        int vertex_count = 0;

        ObjData obj = NewObjData();
        List<ObjData> objs = new List<ObjData>();
        
        string[] lines = File.ReadAllLines (path);


        foreach (string line in lines) {
            if (line == "" || line.StartsWith ("#"))
                continue;
            string[] token = line.Split (' ');
            //Useful debugging snippet - exclude lines we're not interested in, print the rest
            /*string[] excludes = new string[]{"v", "vn", "s"};
            if (!excludes.Contains(token[0])){
                UnityEngine.Debug.Log(line);
            }*/

            switch (token [0]) {
            case ("o"):
                name = token [1];
                obj.name = name;
                break;
            case ("mtllib"):
                mtllib = token [1];
                obj.mtllib = mtllib;
                break;
            case ("usemtl"):
                obj.usemtl.Add (token [1]);
                obj.f.Add (new List<int[]> ());
                break;
            case ("v"):
                obj.v.Add (new Vector3 (
                    float.Parse (token [1]),
                    float.Parse (token [2]),
                    float.Parse (token [3])));
                break;
            case ("vn"):
                obj.vn.Add (new Vector3 (
                    float.Parse (token [1]),
                    float.Parse (token [2]),
                    float.Parse (token [3])));
                break;
            case ("vt"):
                obj.vt.Add (new Vector3 (
                    float.Parse (token [2]),
                    float.Parse (token [1])));
                break;
            case ("g"):
                //A new groupname means a new part of the building - let's store the previous one and start a new mesh
                objs.Add(obj);
                vertex_count += obj.v.Count;
                obj = NewObjData();
                obj.mtllib = mtllib;
                obj.name = token [1];
                break;
            case ("f"):
                for (int i = 1; i < 4; i += 1) {
                    int[] triplet = Array.ConvertAll (token [i].Split ('/'), x => {
                        if (String.IsNullOrEmpty (x))
                            return 0;
                        return int.Parse (x) - vertex_count;
                    });
                    obj.f [obj.f.Count - 1].Add (triplet);
                }
                break;
            }
        }
        objs.Add(obj);
        //Root object that shouldn't contain anything of value
        objs.RemoveAt(0);
        return objs;
    }

    public static MtlData ReadMtlData (string path) {
        MtlData mtl = new MtlData ();
        string[] lines = File.ReadAllLines (path);

        mtl.name = new List<string> ();
        mtl.kd = new Dictionary<string, Color> ();
        string currmtl = "";

        foreach (string line in lines) {
            if (line == "" || line.StartsWith ("#"))
                continue;

            string[] token = line.Split (' ');
            switch (token [0]) {

            case ("newmtl"):
                mtl.name.Add (token [1]);
                currmtl = token[1];
                break;
            case ("Kd"): //Color rgb value
                //If we already have an alpha value, use it
                if (mtl.kd.ContainsKey(currmtl)) {
                    mtl.kd[currmtl] = new Color(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]), mtl.kd[currmtl].a);
                }
                else { //New color, use default alpha of 1.0f
                    mtl.kd[currmtl] = new Color(float.Parse(token[1]), float.Parse(token[2]), float.Parse(token[3]), 1.0f);
                }
                break;
            case ("d"): //Color alpha
                //If we already have rgb values, just update alpha
                if (mtl.kd.ContainsKey(currmtl)) {
                    float r = mtl.kd[currmtl].r;
                    float g = mtl.kd[currmtl].g;
                    float b = mtl.kd[currmtl].b;
                    mtl.kd[currmtl] = new Color(r, g, b, float.Parse(token[1]));
                }
                else { //New color
                    mtl.kd[currmtl] = new Color(0,0,0, float.Parse(token[1]));
                }
                break;
            }
        }
        return mtl;
    }

    public static GameObject Load (string directory_path, string filename) {
        GameObject root_object = ConstructModel (filename, directory_path);
        TreeBuilder.ReconstructTree(root_object);
        return root_object;
    }

    private static GameObject ConstructModel (string filename, string directory_path) {
        //Needed to make file IO use correct floating point separators etc regardless of region
        CultureInfo culture = new CultureInfo("en-US");
        Thread.CurrentThread.CurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        
        GameObject root_object = new GameObject();

        List<ObjData> objs = ReadObjData (directory_path + filename);
        root_object.name = filename.Split ('.')[0];
        //All our materials come from the same .mtl file, so let's just grab the first one
        MtlData mtl = ReadMtlData (directory_path + objs[0].mtllib);
        
        Mesh[] meshes = PopulateMesh(objs);
        
        for (int i = 0; i < objs.Count; i++) {
            Material[] materials = DefineMaterial (objs[i], mtl);
            
            GameObject childObject = new GameObject();
            childObject.transform.parent = root_object.transform;
            MeshFilter mf = childObject.AddComponent<MeshFilter> ();
            MeshRenderer mr = childObject.AddComponent<MeshRenderer> ();
            mf.mesh = meshes[i];
            mr.materials = materials;
            childObject.name = meshes[i].name;
        }
        //Due to different coordinate systems, the entire thing is mirrored. Let's fix it:
        root_object.transform.localScale = new Vector3(-1,1,1);
        return root_object;
    }

    private static Mesh[] PopulateMesh (List<ObjData> objs) {
        Mesh[] meshes = new Mesh[objs.Count];
        for (int k = 0; k < objs.Count; k++) {
            ObjData obj = objs[k];
            List<int[]> triplets = new List<int[]> ();
            List<int> submeshes = new List<int> ();
            for (int i = 0; i < obj.f.Count; i += 1) {
                for (int j = 0; j < obj.f [i].Count; j += 1) {
                    triplets.Add (obj.f [i] [j]);
                }
                submeshes.Add (obj.f [i].Count);
            }
            
            

            Vector3[] vertices = new Vector3[triplets.Count];
            Vector3[] normals = new Vector3[triplets.Count];
            Vector2[] uvs = new Vector2[triplets.Count];

            for (int i = 0; i < triplets.Count; i += 1) {
                vertices [i] = obj.v [triplets [i] [0] - 1];
                normals [i] = obj.vn [triplets [i] [2] - 1];
                if (triplets [i] [1] > 0)
                    uvs [i] = obj.vt [triplets [i] [1] - 1];
            }

            Mesh mesh = new Mesh ();
            mesh.name = obj.name;//groupname;
            mesh.vertices = vertices;
            mesh.normals = normals;
            mesh.uv = uvs;
            mesh.subMeshCount = submeshes.Count;
            int vertex = 0;
            for (int i = 0; i < submeshes.Count; i += 1) {
                int[] triangles = new int[submeshes [i]];
                for (int j = 0; j < submeshes [i]; j += 1) {
                    triangles [j] = vertex;
                    vertex += 1;
                }
                mesh.SetTriangles (triangles, i);
            }
            mesh.RecalculateBounds ();
            mesh.Optimize ();
            meshes[k] = mesh;
        }
    
        return meshes;
    }

    private static Material[] DefineMaterial (ObjData obj, MtlData mtl) {

        Material[] materials = new Material[obj.usemtl.Count];
        Material ifcTransparent = Resources.Load("ifcTransparent") as Material;
        Material ifcOpaque = Resources.Load("ifcOpaque") as Material;

        for (int i = 0; i < obj.usemtl.Count; i += 1) {
            string mtl_name = obj.usemtl [i];

            Texture2D texture = new Texture2D (1, 1);
            Color[] cols = texture.GetPixels();
            Color mtl_color = mtl.kd[mtl_name];
            for (int j = 0; j < cols.Length; ++j)
            {
                cols[j] = mtl_color;
            } 
            texture.SetPixels(cols);
            texture.Apply();
            //materials [i] = new Material (Shader.Find ("Standard"));
            try{
                if (mtl_color.a < 1.0f) {
                    materials [i] = new Material (ifcTransparent);
                }
                else {
                    materials [i] = new Material (ifcOpaque);
                }
            } catch {
                UnityEngine.Debug.LogError("Can't find ifcTransparent and ifcOpaque materials in the Resources folder. Please copy them from IfcImporter/Copy_to_Resources.");
                throw;
            }
            materials [i].name = mtl_name;
            materials [i].color = mtl_color;
            
            //So this works in editor but not on runtime. BUT! If we save the material generated by this, we can use and recolor that on runtime.
            /*if (mtl_color.a < 1.0f) {
                materials [i].SetFloat("_Mode", 3);
         
                materials [i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                materials [i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                materials [i].SetInt("_ZWrite", 0);
                materials [i].DisableKeyword("_ALPHATEST_ON");
                materials [i].EnableKeyword("_ALPHABLEND_ON");
                materials [i].DisableKeyword("_ALPHAPREMULTIPLY_ON");
                materials [i].renderQueue = 3000;
         
         
            }*/
            //materials [i].mainTexture = texture;
        }
        return materials;
    }
}
