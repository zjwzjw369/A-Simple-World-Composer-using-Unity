using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIControl : MonoBehaviour {
    public Text LTlat;
    public Text LTlong;
    public Text RBlat;
    public Text RBllong;
    public Text width;
    public Text height;
    public Text BingKey;
    public static bool CreatTerrain = false;
    public void OnEditEnd(string message) {
        if (message=="LTlat") {
            GetTerrain.latlongLT.lati = double.Parse(LTlat.text);
            Debug.Log(GetTerrain.latlongLT.lati);
        }
        if (message == "LTlong") {
            GetTerrain.latlongLT.longti = double.Parse(LTlong.text);
        }
        if (message == "RBlat") {
            GetTerrain.latlongRB.lati = double.Parse(RBlat.text);

        }
        if (message == "RBllong") {
            GetTerrain.latlongRB.longti = double.Parse(RBllong.text);
        }
        if (message== "BingKey") {
            GetTerrain.bingKey = BingKey.text;
            Debug.Log(GetTerrain.bingKey);
        }
    }
    public void OnClick(string message) {
        if (message=="distance") {
            double widthD = HarvenSin.Distance(GetTerrain.latlongLT.lati, GetTerrain.latlongLT.longti, GetTerrain.latlongLT.lati, GetTerrain.latlongRB.longti);
            double heightD= HarvenSin.Distance(GetTerrain.latlongLT.lati, GetTerrain.latlongLT.longti, GetTerrain.latlongRB.lati, GetTerrain.latlongLT.longti);
            width.text = widthD.ToString("0.000");
            height.text = heightD.ToString("0.000");
        }
        if (message=="CreateTerrain") {
            CreatTerrain = true;
        }
    }
}
