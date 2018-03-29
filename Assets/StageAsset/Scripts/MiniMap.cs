﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour {

    public Sprite unknownIcon, monsterIcon, eventIcon, bossIcon, storeIcon;
    public int minmapSize = 200;
    List<Map.Rect> roomList;
    Texture2D texture;
    new RawImage renderer;
    int size;
    public void GetRoomList()
    {
        roomList = RoomManager.Getinstance().GetRoomList();
    } // Mapmanager로부터 방 데이터 로드

    public void DrawMinimap()
    {
        renderer.texture = texture;
        for (int i = 0; i < roomList.Count; i++)
        {
            for (int x = roomList[i].x * size; x < roomList[i].x * size + roomList[i].width * size; x++)
            {
                for (int y = roomList[i].y * size; y < roomList[i].y * size + roomList[i].height * size; y++)
                {
                    if (roomList[i].isRoom && 
                        (x == roomList[i].x * size || x == roomList[i].x * size + roomList[i].width * size - 1 ||
                        y == roomList[i].y * size || y == roomList[i].y * size + roomList[i].height * size - 1))
                    {
                        texture.SetPixel(x, y, Color.black);
                    }
                    else
                    {
                        texture.SetPixel(x, y, Color.grey);
                    }
                }
            }
        }
        DrawIcon(roomList[0]);
        for (int i = 0; i < minmapSize; i++)
        {
            for (int j = 0; j < minmapSize; j++)
            {
                if (i == 0 || i == minmapSize - 1 || 
                    j == 0 || j == minmapSize - 1)
                    texture.SetPixel(i, j, Color.black);
            }
        }
        texture.filterMode = FilterMode.Point;
        texture.Apply();
    } // 미니맵 그리는 함수

    void DrawIcon(Map.Rect _rect)
    {
        if (!_rect.isRoom)
            return;
        Sprite sprite;
        switch (_rect.eRoomType)
        {
            default:
            case RoomType.MONSTER:
                sprite = monsterIcon;
                break;
            case RoomType.BOSS:
                sprite = bossIcon;
                break;
            case RoomType.EVENT:
                sprite = eventIcon;
                break;
            case RoomType.STORE:
                sprite = storeIcon;
                break;
        }
        int x = _rect.x * size + _rect.width * size / 2;
        int y = _rect.y * size + _rect.height * size / 2;
        Rect textureRect = sprite.textureRect;
        float width = textureRect.width;
        float height = textureRect.height;
        x -= (int)width / 2;
        y -= (int)height / 2;
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                texture.SetPixel(x + i, y + j, sprite.texture.GetPixel((int)textureRect.x + i, (int)textureRect.y + j));
            }
        }
    } // 방 타입에 따른 미니맵 아이콘 표시

    #region UnityFunc
    private void Awake()
    {
        texture = new Texture2D(minmapSize, minmapSize); // 미니맵 픽셀
        size = minmapSize / 10; // 미니맵 사이즈
        renderer = GetComponent<RawImage>(); 
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            GetRoomList();
            DrawMinimap();
        }
    }
    #endregion
}
