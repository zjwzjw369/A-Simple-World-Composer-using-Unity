using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class FaddingMessage : MonoBehaviour
{
    Text t;
    float DURATION = 0;
    void Start(){
        t = GetComponent<Text>();
    }
    // Update is called once per frame
    void Update(){
        if ( DURATION*Time.deltaTime>1.0f){
            Destroy(gameObject);
        }
        DURATION++;
        if (DURATION * Time.deltaTime > 0.5f) {
            Color newColor = t.color;
            float proportion = (DURATION * Time.deltaTime / 1.0f);
            newColor.a = Mathf.Lerp(1, 0, proportion);
            t.color = newColor;
        }
    }
}