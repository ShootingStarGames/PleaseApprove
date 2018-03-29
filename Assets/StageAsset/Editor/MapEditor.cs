﻿using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapEditor : EditorWindow
{
    int width, height,size;
    string roomName;
    GameObject obj;
    GameObject roomObj;
    ObjectType objectType;
    RoomType roomType;
    Sprite objectSprite;
    RoomSet roomSet;
    [MenuItem("Custom/Map")]

    static public void ShowWindow()
    {
        // 윈도우 생성
        MapEditor window = (MapEditor)EditorWindow.GetWindow(typeof(MapEditor));
    }
 

    void OnGUI()
    {
        BeginWindows();
        obj = Object.FindObjectOfType<Map.MapManager>().gameObject;
        EndWindows();
        roomName = EditorGUILayout.TextField("roomName", roomName);
        width = EditorGUILayout.IntField("width", width);
        height = EditorGUILayout.IntField("height", height);
        roomType = (RoomType)EditorGUILayout.EnumPopup("RoomType", roomType);
        size = EditorGUILayout.IntField("size", size);
        if (GUILayout.Button("Create Room"))
        {
            CreateTilemap();
        }
        if (GUILayout.Button("Remove Room"))
        {
            RemoveTilemap();
        }
        objectType = (ObjectType)EditorGUILayout.EnumPopup("ObjectType", objectType);
        objectSprite = (Sprite)EditorGUILayout.ObjectField("ObjectSprite", objectSprite, typeof(Sprite), allowSceneObjects: true);
        if (GUILayout.Button("Create Object"))
            CreateObject();
        if (GUILayout.Button("Save Roomset"))
            SaveRoomset();
        roomSet = (RoomSet)EditorGUILayout.ObjectField("RoomSet", roomSet, typeof(RoomSet), allowSceneObjects: true);
        if (GUILayout.Button("Load Roomset"))
            LoadRoomset();
    }

    void CreateObject()
    {
        if (roomObj == null)
            return;
        GameObject gameObject = new GameObject();
        gameObject.name = "Object";
        gameObject.transform.parent = roomObj.transform;
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.AddComponent<Light>();

        ObjectData objectData = new ObjectData(Vector3.zero,objectSprite, objectType);
        objectData.LoadObject(gameObject);
    }

    void SaveRoomset()
    {
        if (obj == null || roomObj == null || size == 0 || width == 0 || height == 0)
            return;
        RoomSet roomSet = new RoomSet(width, height, size, roomType);

        foreach (Transform child in roomObj.GetComponentsInChildren<Transform>())
        {
            if (child.GetComponent<CustomObject>() != null)
            {
                CustomObject customObject = child.GetComponent<CustomObject>();
                customObject.SetPosition();
                roomSet.Add(new ObjectData(customObject.position, customObject.sprite, customObject.objectType));
            }
        }
        RoomSetManager.GetInstance().SaveRoomSet(roomName, roomSet);
    }

    void CreateTilemap()
    {
        if (obj == null || size == 0 || width == 0 || height == 0)
            return;
        Tilemap tilemap;

        if(roomObj == null)
        {
            GameObject gameObject = new GameObject();
            roomObj = gameObject;
            roomObj.transform.parent = obj.transform;
            gameObject.name = "Room";
            tilemap = gameObject.AddComponent<Tilemap>();
            gameObject.AddComponent<TilemapRenderer>();
            gameObject.AddComponent<CompositeCollider2D>();
            gameObject.AddComponent<TilemapCollider2D>();
            gameObject.transform.parent = obj.transform;
        }
        else
        {
            tilemap = roomObj.GetComponent<Tilemap>();
        }

        tilemap.ClearAllTiles();
        TileBase[] wall = TileManager.GetInstance().wallTile;

        for (int i = 0; i < width * size; i++)
        {
            for(int j = 0; j < height * size; j++)
            {
                if (i == 0 || j == 0 || i == width * size - 1 || j == height * size - 1)
                {
                    if (i == 0 || j == 0 || i == width * size - 1 || j == height * size - 1)
                    {
                        if (i == 0 & j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[0]);
                        else if (i == 0 && j == height * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[5]);
                        else if (i == width * size - 1 && j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[2]);
                        else if (i == width * size - 1 && j == height * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[7]);
                        else if (i == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[3]);
                        else if (j == 0)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[1]);
                        else if (i == width * size - 1)
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[4]);
                        else
                            tilemap.SetTile(new Vector3Int(i, j, 0), wall[6]);
                    }
                }
            }
        }
    }
 
    void RemoveTilemap()
    {
        if (obj == null || roomObj == null)
            return;
        DestroyImmediate(roomObj);
    }

    void LoadRoomset()
    {
        if (roomSet == null)
            return;
        width = roomSet.width;
        height = roomSet.height;
        size = roomSet.size;
        CreateTilemap();
        for (int i = 0; i < roomSet.objectDatas.Count; i++)
            DataToObject(roomSet.objectDatas[i]);
    }

    void DataToObject(ObjectData _objectData)
    {
        GameObject gameObject = new GameObject();
        gameObject.AddComponent<SpriteRenderer>();
        gameObject.AddComponent<BoxCollider2D>();
        gameObject.AddComponent<Light>();
        _objectData.LoadObject(gameObject);
        gameObject.name = "Object";
        gameObject.transform.position = new Vector3(_objectData.position.x,_objectData.position.y,0);
        gameObject.transform.parent = roomObj.transform;
    }
}