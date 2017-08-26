using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using LitJson;

public class GetTerrain : MonoBehaviour {
    public static Latlong latlongLT = new Latlong();
    public static Latlong latlongRB = new Latlong();
    public static string bingKey;
    /// <summary>
    /// latLong是含有lati和longti，经纬度值的类，注意bingmap中bounding是South Latitude, West Longitude, North Latitude, East Longitude，而我们系统需求方要求是左上以及右下
    /// </summary>
    public class Latlong {
        public Latlong(){}
        public Latlong(double lat,double longgti) {
            lati = lat;
            longti = longgti;
        }
        public double lati {
            set;get;
        }
        public double longti{
            set;get;
        }
    }
    /// <summary>
    /// Mappixel存放转成像素地图后的位置
    /// </summary>
    public class Mappixel
    {
        public Mappixel() { }
        public Mappixel(double x, double y)
        {
            this.x = x;
            this.y = y;
        }
        public double x{
            set; get;
        }
        public double y{
            set; get;
        }
    }

    /// <summary>
    /// 创建适用于地形的Splat对象
    /// </summary>
    /// <param name="纹理"></param>
    /// <param name="纹理块大小"></param>
    /// <param name="偏移"></param>
    /// <returns></returns>
    public SplatPrototype CreateSplatPrototype(Texture2D tmpTexture, Vector2 tmpTileSize, Vector2 tmpOffset)
    {
        SplatPrototype outSplatPrototype = new SplatPrototype();
        outSplatPrototype.texture = tmpTexture;
        outSplatPrototype.tileOffset = tmpOffset;
        outSplatPrototype.tileSize = tmpTileSize;
        return outSplatPrototype;
    }


    public IEnumerator CreateTerrainZoom(int zoom) {
        Terrain t1 = GetComponent<Terrain>();
        t1.basemapDistance = 10000;
        Latlong RBLl = HarvenSin.pixelToLatlong(new Vector2(512 * 16, 512 * 16), latlongLT, zoom);
        ElevationData evData = new ElevationData();
        //string ElvURL = "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds="+ RBLl.lati+","+latlongLT.longti+","+latlongLT.lati+","+ RBLl.longti+"&rows=32&cols=32&heights=ellipsoid&key="+bingKey;
        //yield return StartCoroutine(evData.GetDataFormUrl(ElvURL));
        yield return StartCoroutine(evData.GetElv(latlongLT, zoom));
        float[,] heightmap = evData.GetHeightMap();
        //float[,] heightmap = evData.GetHeightMap(32, 32);
        t1.terrainData.heightmapResolution = evData.size;
        t1.terrainData.SetHeights(0, 0, heightmap);
        //yield return StartCoroutine(GetImageFormTile(latlongLT,16,16,19));
        yield return StartCoroutine(ImageDataP.GetImageFormUrl(latlongLT, 16, 16, zoom));
        SplatPrototype[] sp = new SplatPrototype[1];
        t1.terrainData.size = new Vector3((float)HarvenSin.Distance(latlongLT.lati, latlongLT.longti, latlongLT.lati, RBLl.longti) * 1000, 10000f, (float)HarvenSin.Distance(RBLl.lati, latlongLT.longti, latlongLT.lati, latlongLT.longti) * 1000f);
        sp[0] = CreateSplatPrototype(imageCombined, new Vector2(t1.terrainData.size.x, t1.terrainData.size.z), new Vector2(0f, 0f));
        File.WriteAllBytes("Assets/TerrainTexture/test.jpg", imageCombined.EncodeToJPG());//output Texture
        t1.terrainData.splatPrototypes = sp;
    }
    public static Texture2D imageCombined;
    /*
    public IEnumerator GetImageFormTile(Latlong latlong, int tilex, int tiley, double zoom){
        Mappixel startMapP = HarvenSin.latlongToPixel(latlong, zoom);
        startMapP.x += 256;
        startMapP.y += 256;
        Texture2D Img = new Texture2D(tilex*512,tiley*512);
        for (int i = 0; i < tilex; ++i){
            for (int j = 0; j < tiley; ++j){
                Mappixel mpTmp = new Mappixel(startMapP.x + 512 * i, startMapP.y + 512 * j);
                Latlong ll =HarvenSin.pixelToLatlong(mpTmp, zoom);
                string url = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/"+ll.lati+","+ll.longti+"/"+zoom+ "?&mapSize=512,544&key="+bingKey;
                //string url = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/47.619048,-122.35384/15?mapSize=500,500&pp=47.620495,-122.34931;21;AA&pp=47.619385,-122.351485;;AB&pp=47.616295,-122.3556;22&key=BingMapsKey"+bingKey;
                //Debug.Log(url);
                ImageDataTile imageDataTile = new ImageDataTile();
                yield return StartCoroutine(imageDataTile.GetImagetileFormUrl(url));
                Color[] color = imageDataTile.getPixels();
                Img.SetPixels(i * 512, 512 * (tiley - 1 - j), 512, 512, color);
                Debug.Log(i+" "+j);
            }
        }
        Img.Apply();
        imageCombined = Img;
    }
    /*
    /// <summary>
    /// 用于获取图片数据
    /// </summary>
    public class ImageDataTile {
        
        public IEnumerator GetImagetileFormUrl(string url) {
            WWW www = new WWW(url);
            yield return www;
            imagetile = www.texture;
            //Debug.Log(www.texture);
            www.Dispose();
        }
        /// <summary>
        /// 获取512*512的图片，imagetile中存放的应该是512*544大小的图片
        /// </summary>
        /// <returns>返回值为一维Color数组</returns>
        public Color[] getPixels() {
            return imagetile.GetPixels(0, 16, 512, 512);
        }
        Texture2D imagetile;
        bool downloaded = false;
    }*/
    public class ImageDataP {
        public static IEnumerator GetImageFormUrl(Latlong latlong, int tilex, int tiley, double zoom) {
            Mappixel startMapP = HarvenSin.latlongToPixel(latlong, zoom);
            startMapP.x += 256;
            startMapP.y += 256;
            Texture2D Img = new Texture2D(tilex * 512, tiley * 512);
            WWW[] www = new WWW[16 * 16];
            for (int i = 0; i < tilex; ++i){
                for (int j = 0; j < tiley; ++j){
                    Mappixel mpTmp = new Mappixel(startMapP.x + 512 * i, startMapP.y + 512 * j);
                    Latlong ll = HarvenSin.pixelToLatlong(mpTmp, zoom);
                    string url = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Aerial/" + ll.lati + "," + ll.longti + "/" + zoom + "?&mapSize=512,544&key=" + bingKey;
                    //string url = "http://dev.virtualearth.net/REST/v1/Imagery/Map/Road/47.619048,-122.35384/15?mapSize=500,500&pp=47.620495,-122.34931;21;AA&pp=47.619385,-122.351485;;AB&pp=47.616295,-122.3556;22&key=BingMapsKey"+bingKey;
                    //Debug.Log(url);
                    www[i * 16 + j] = new WWW(url);
                    
                }
                for (int j = 0; j < 16; j++) {
                    yield return www[i * 16 + j];
                    Color[] color = www[i * 16 + j].texture.GetPixels(0, 16, 512, 512);
                    Img.SetPixels(i * 512, 512 * (tiley - 1 - j), 512, 512, color);
                    www[i * 16 + j].Dispose();
                    Debug.Log(i + " " + j);
                }
            }
            Img.Apply();
            imageCombined = Img;
        }
        

    }

    /// <summary>
    /// 用于获取高程数据
    /// </summary>
    public class ElevationData{
        public IEnumerator GetElv(Latlong latlong,int zoom) {
            size = 32;
            size = zoom >= 18 ? 32 : (int) (Math.Pow(2.0, (18 - zoom)) * 32.0);
            int tileX = size / 32;
            int tileY = size / 32;
            Latlong RBLl = HarvenSin.pixelToLatlong(new Vector2(512 * 16, 512 * 16), latlongLT, zoom);
            if (zoom >= 18){
                string ElvURL = "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds=" + RBLl.lati + "," + latlongLT.longti + "," + latlongLT.lati + "," + RBLl.longti + "&rows=32&cols=32&heights=ellipsoid&key=" + bingKey;
                WWW www = new WWW(ElvURL);
                yield return www;
                JsonData EData = JsonMapper.ToObject(www.text);
                zoomLevel = int.Parse(EData["resourceSets"][0]["resources"][0]["zoomLevel"].ToString());
                Debug.Log(EData["resourceSets"][0]["resources"][0]["zoomLevel"]);
                ElevaList.Clear();
                //Update ElevaList to WWW
                foreach (JsonData e in EData["resourceSets"][0]["resources"][0]["elevations"]){
                    float eF = float.Parse(e.ToString());
                    ElevaList.Add(eF);
                }
                www.Dispose();
            }else {
                Latlong LB = new Latlong(RBLl.lati, latlongLT.longti);
                int pixelDis = 8192 / tileX;
                WWW[] www = new WWW[tileX * tileY];
                ElevaList.Clear();
                for (int i=0;i<tileX;++i) {
                    for (int j=0;j<tileY;++j) {
                        Latlong tileLB = HarvenSin.pixelToLatlong(new Vector2(pixelDis * j, pixelDis * -i), LB, zoom);
                        Latlong tileRT = HarvenSin.pixelToLatlong(new Vector2(pixelDis, -pixelDis), tileLB, zoom);
                        string ElvURL = "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds=" + tileLB.lati + "," + tileLB.longti + "," + tileRT.lati + "," + tileRT.longti + "&rows=32&cols=32&heights=ellipsoid&key=" + bingKey;
                        //Debug.Log(ElvURL);
                        www[i * tileX + j] = new WWW(ElvURL);
                    }
                    for (int j = 0; j < tileY; ++j){
                        yield return www[i * tileX + j]; 
                        JsonData EData = JsonMapper.ToObject(www[i * tileX + j].text);
                        zoomLevel = int.Parse(EData["resourceSets"][0]["resources"][0]["zoomLevel"].ToString());
                        Debug.Log("elv"+i+" "+j);
                        //Update ElevaList to WWW
                        foreach (JsonData e in EData["resourceSets"][0]["resources"][0]["elevations"]){
                            float eF = float.Parse(e.ToString());
                            ElevaList.Add(eF);
                        }
                        www[i * tileX + j].Dispose();
                    }
                }

            }
        }
		public IEnumerator GetDataFormUrl(string url){
            //get ElevationData From WWW 
            downloaded = false;
            WWW www = new WWW(url);
            yield return www;
            ElevaList.Clear ();
			JsonData EData = JsonMapper.ToObject (www.text);
			zoomLevel=int.Parse(EData["resourceSets"][0]["resources"][0]["zoomLevel"].ToString());
			Debug.Log (EData["resourceSets"][0]["resources"][0]["zoomLevel"]);
			//Update ElevaList to WWW
			foreach(JsonData e in EData["resourceSets"][0]["resources"][0]["elevations"] ){
				float eF = float.Parse(e.ToString());
				ElevaList.Add (eF);
            }
            downloaded = true;
            www.Dispose();
        }
        public float[,] GetHeightMap(int width,int height) {
            float[,] heightmap = new float[height,width];
            int index = 0;
            for (int i=0;i<height;i++) {
                for (int j=0;j<width;j++) {
                    heightmap[i, j] = ElevaList[index++]/6000;
                   // Debug.Log(heightmap[i, j]);
                }
            }
            return heightmap;
        }
        public float[,] GetHeightMap() {
            if (zoomLevel >= 18) {
                return GetHeightMap(32, 32);
            }
            else {
                float[,] heightmap = new float[size, size];
                int tileX, tileY;
                tileX = tileY = size / 32;
                int index = 0;
                for (int i = 0; i < tileX; ++i) {
                    for (int j = 0; j < tileY; j++) {
                        for (int ii = 0; ii < 32; ii++) {
                            for (int jj = 0; jj < 32; jj++) {
                                heightmap[i * 32 + ii, j * 32 + jj] = ElevaList[index++] / 6000;
                            }
                        }
                    }
                }
                return heightmap;
            }
        }
		public int zoomLevel;
        private List<float> ElevaList=new List<float>();
        public bool downloaded = false;
        public bool built = false;
        public int size;
    }
	//public string url = "http://dev.virtualearth.net/REST/v1/Elevation/Bounds?bounds=30.19,120.19,30.22,120.23&rows=32&cols=32&heights=ellipsoid&key=AhIyn_mdJvi8GCZdHKPKXChklbhjT6tGxogKfQ2meVNGILUL8zCX30dicIPH2hUB";
	void Start(){
        //latlongLT.lati = 30.22;
        //latlongLT.longti = 120.19;
        latlongLT.lati = 45.6721;
        latlongLT.longti = 6.5158;
        latlongRB.lati = 45.6720;
        latlongRB.longti = 6.5159;
        
        //StartCoroutine(CreateTerrainZoom19());
    }
    void Update() {
        if (UIControl.CreatTerrain) {
            if (latlongLT.lati < latlongRB.lati || latlongLT.longti > latlongRB.longti){
                Debug.Log("非法输入");//illegal input
                UIControl.CreatTerrain = false;
            }else {
                int zoom = 19;
                for ( zoom = 19; zoom >= 12; --zoom){
                    Mappixel LT = HarvenSin.latlongToPixel(latlongLT, zoom);
                    Mappixel RB = HarvenSin.latlongToPixel(latlongRB, zoom);
                    double x = RB.y - LT.y;
                    double y = RB.x - LT.x;
                    if (x <= 8192 && y <= 8192){
                        StartCoroutine(CreateTerrainZoom(zoom));
                        UIControl.CreatTerrain = false;
                        break;
                    }
                }
                Debug.Log(zoom);
                if (zoom==11) {
                    Debug.Log("选择区域过大");//oversize
                }
            }
        }
    }
}

