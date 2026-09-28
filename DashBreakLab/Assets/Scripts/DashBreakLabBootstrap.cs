using UnityEngine;

public class DashBreakLabBootstrap : MonoBehaviour
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Boot(){ if(FindAnyObjectByType<DashBreakLabBootstrap>()==null)new GameObject("Dash Break Lab").AddComponent<DashBreakLabBootstrap>(); }

    void Start()
    {
        if(GameObject.Find("Arena Floor")!=null)return;
        var floor=GameObject.CreatePrimitive(PrimitiveType.Cube); floor.name="Arena Floor"; floor.transform.position=new Vector3(0,-.3f,0); floor.transform.localScale=new Vector3(32,.3f,25); floor.GetComponent<Renderer>().material=Mat(new Color(.07f,.1f,.16f));
        var cam=new GameObject("Main Camera").AddComponent<Camera>(); cam.tag="MainCamera"; cam.transform.position=new Vector3(0,18,-13); cam.transform.rotation=Quaternion.Euler(52,0,0); cam.gameObject.AddComponent<AudioListener>();
        var light=new GameObject("Directional Light").AddComponent<Light>(); light.type=LightType.Directional; light.intensity=1.4f; light.transform.rotation=Quaternion.Euler(50,-30,0);
        var player=GameObject.CreatePrimitive(PrimitiveType.Capsule); player.name="Player — Dash with Space"; player.transform.position=new Vector3(0,1,-8); player.GetComponent<Renderer>().material=Mat(new Color(.1f,.8f,1f)); player.AddComponent<Rigidbody>(); player.AddComponent<DashController>();
        for(int i=0;i<6;i++){var b=GameObject.CreatePrimitive(PrimitiveType.Cube); b.name="Breakable Target"; b.layer=6; b.transform.position=new Vector3((i%2==0?-.8f:.8f),.75f,-3+i*1.8f); b.transform.localScale=new Vector3(1.25f,1.5f,1.25f); b.GetComponent<Renderer>().material=Mat(new Color(1f,.3f,.08f)); b.AddComponent<Breakable>();}
        var wall=GameObject.CreatePrimitive(PrimitiveType.Cube); wall.name="Solid Dash Stopper"; wall.layer=6; wall.transform.position=new Vector3(0,1,9); wall.transform.localScale=new Vector3(5,2,1); wall.GetComponent<Renderer>().material=Mat(new Color(.18f,.27f,.38f));
        for(float x=-15;x<=15;x+=30){var w=GameObject.CreatePrimitive(PrimitiveType.Cube);w.name="Solid Arena Wall";w.layer=6;w.transform.position=new Vector3(x,1,0);w.transform.localScale=new Vector3(1,2,24);w.GetComponent<Renderer>().material=Mat(new Color(.18f,.27f,.38f));}
    }
    Material Mat(Color c){var m=new Material(Shader.Find("Universal Render Pipeline/Lit")??Shader.Find("Standard"));m.color=c;return m;}
}