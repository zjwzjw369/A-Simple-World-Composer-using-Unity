using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HarvenSin : MonoBehaviour{
    public static double maxLatitude = 85.05112878;
    public static double minLatitude = -85.05112878;
    public static double maxLongitude = 180.0;
    public static double minLongitude = -180.0;
    /// <summary>
    /// use for bing map
    /// </summary>
    /// <param name="mapPixel"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public static GetTerrain.Mappixel clipPixel(GetTerrain.Mappixel mapPixel,double zoom){
        double mapSize = 256 * Math.Pow(2, zoom);
        if (mapPixel.x > mapSize - 1) { mapPixel.x -= mapSize - 1; }
        else if (mapPixel.x < 0) { mapPixel.x = mapSize - 1 - mapPixel.x; }

        if (mapPixel.y > mapSize - 1) { mapPixel.y -= mapSize - 1; }
        else if (mapPixel.y < 0) { mapPixel.y = mapSize - 1 - mapPixel.y; }

        return mapPixel;
    }
    /// <summary>
    /// use for bing map
    /// </summary>
    /// <param name="latlong"></param>
    /// <returns></returns>
    public static GetTerrain.Latlong clipLatlong(GetTerrain.Latlong latlong) {
        if (latlong.lati > maxLatitude) { latlong.lati -= (maxLatitude * 2); }
        else if (latlong.lati < minLatitude) { latlong.lati += (maxLatitude * 2); }
        if (latlong.longti > 180) { latlong.longti -= 360; }
        else if (latlong.longti < -180) { latlong.longti += 360; }
        return latlong;
    }
    /// <summary>
    /// use for bing map
    /// </summary>
    /// <param name="latlong"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public static GetTerrain.Mappixel latlongToPixel(GetTerrain.Latlong latlong, double zoom){
        latlong = clipLatlong(latlong);
        double pi = 3.14159265358979323846264338327950288419716939937510;
        double x = (latlong.longti + 180.0) / 360.0;
        double sinLatitude = Math.Sin(latlong.lati * pi / 180.0);
        double y = 0.5 - Math.Log((1.0 + sinLatitude) / (1.0 - sinLatitude)) / (4.0 * pi);
        x *= 256.0 * Math.Pow(2.0, zoom);
        y *= 256.0 * Math.Pow(2.0, zoom);
        GetTerrain.Mappixel mapPixel = new GetTerrain.Mappixel();
        mapPixel.x = x;
        mapPixel.y = y;
        return mapPixel;
    }
    /// <summary>
    /// use for bing map
    /// </summary>
    /// <param name="mapPixel"></param>
    /// <param name="zoom"></param>
    /// <returns></returns>
    public static GetTerrain.Latlong pixelToLatlong(GetTerrain.Mappixel mapPixel,double zoom) {
        mapPixel = clipPixel(mapPixel, zoom);
        double pi = 3.14159265358979323846264338327950288419716939937510;
        double mapSize = 256.0 * Math.Pow(2.0, zoom);
        double x = (mapPixel.x / mapSize) - 0.5;
        double y = 0.5 - (mapPixel.y / mapSize);
        GetTerrain.Latlong latlong = new GetTerrain.Latlong();
        latlong.lati = 90.0 - 360.0 * Math.Atan(Math.Exp(-y * 2.0 * pi)) / pi;
        latlong.longti = 360.0 * x;
        return latlong;
    }

    public static double calcLatlongAreaResolution(GetTerrain.Latlong latlong ,double zoom) {
        double pi= 3.14159265358979323846264338327950288419716939937510;
        double map_resolution = 156543.04 * Math.Cos(latlong.lati * (pi / 180)) / (Math.Pow(2, zoom));
        return map_resolution;
    }
    public static GetTerrain.Latlong pixelToLatlong(Vector2 offset,GetTerrain.Latlong latlongCenter, double zoom) {
        double pi = 3.14159265358979323846264338327950288419716939937510;
        double mapSize = 256.0 * Math.Pow(2.0, zoom);
        GetTerrain.Mappixel mapPixelCenter = latlongToPixel(latlongCenter, zoom);
        GetTerrain.Mappixel mapPixel = new GetTerrain.Mappixel();
        mapPixel.x = mapPixelCenter.x + offset.x;
        mapPixel.y = mapPixelCenter.y + offset.y;
        double x = (mapPixel.x / mapSize) - 0.5;
        double y = 0.5 - (mapPixel.y / mapSize);
        GetTerrain.Latlong latlong = new GetTerrain.Latlong();
        latlong.lati = 90 - 360 * Math.Atan(Math.Exp(-y * 2 * pi)) / pi;
        latlong.longti = 360 * x;
        latlong = clipLatlong(latlong);
        return latlong;
    }

    public static double haverSin(double theta) {
        double v = Math.Sin(theta / 2);
        return v * v;
    }
    static double EARTH_RADIUS = 6371.0;//km 地球半径平均值

    public static double ConvertDegreesToRadians(double degrees) {
        return degrees * Math.PI / 180;
    }

    public static double ConvertRadiansToDegrees(double Radians)
    {
        return Radians * 180 / Math.PI;
    }

    public static double Distance(double lat1,double lon1,double lat2,double lon2) {
        lat1 = ConvertDegreesToRadians(lat1);
        lon1 = ConvertDegreesToRadians(lon1);
        lat2 = ConvertDegreesToRadians(lat2);
        lon2 = ConvertDegreesToRadians(lon2);
        double vLon = Math.Abs(lon1 - lon2);
        double vLat = Math.Abs(lat1 - lat2);
        double h = haverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * haverSin(vLon);
        double distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));
        return distance;
    }

    public static double Distance(GetTerrain.Latlong lt,GetTerrain.Latlong rb)
    {
        double lat1 = ConvertDegreesToRadians(lt.lati);
        double lon1 = ConvertDegreesToRadians(lt.longti);
        double lat2 = ConvertDegreesToRadians(rb.lati);
        double lon2 = ConvertDegreesToRadians(rb.longti);
        double vLon = Math.Abs(lon1 - lon2);
        double vLat = Math.Abs(lat1 - lat2);
        double h = haverSin(vLat) + Math.Cos(lat1) * Math.Cos(lat2) * haverSin(vLon);
        double distance = 2 * EARTH_RADIUS * Math.Asin(Math.Sqrt(h));
        return distance;
    }

    public static double DisToLat(double lat,double distance) {
        return distance / Math.Cos(ConvertDegreesToRadians(lat))/111.319;
    }

}
