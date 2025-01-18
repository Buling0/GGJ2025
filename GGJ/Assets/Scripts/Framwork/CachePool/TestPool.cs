using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPool : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        //UIManager.GetInstance().ShowPanel("longinPanel", E_UI_Layer.Bot);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //PoolManager.GetInstance().GetObj("Test/Cube");
            ResManager.GetInstance().Load<GameObject>("Test/Cube");
        }

        if (Input.GetMouseButtonDown(1))
        {
            //PoolManager.GetInstance().GetObj("Test/Sphere");
            //ResManager.GetInstance().LoadAsync<GameObject>("Test/Sphere", DoSomething);
            
            //lambda表达式版
            ResManager.GetInstance().LoadAsync<GameObject>("Test/Sphere", (obj) =>
            {
                //资源加载后真正要做的事情
                obj.transform.localScale = Vector3.one * 2;
            });
        }
    }

    /*
    private void DoSomething(GameObject obj)
    {
        //资源加载后真正要做的事情
        obj.transform.localScale = Vector3.one * 2;
    }
    */
}
