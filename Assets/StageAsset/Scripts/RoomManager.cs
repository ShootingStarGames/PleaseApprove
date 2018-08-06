﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Map;

public class RoomManager : MonoBehaviourSingleton<RoomManager> {

    List<Map.Rect> roomList;
    Map.Rect currentRoom;

    private int monsterNum = 0;

    public void InitRoomList()
    {
        roomList = MapManager.Instance.GetMap().GetList(out currentRoom);
        //for(int i = 0;i<roomList.Count;i++)
        //{
        //    DisalbeObject(roomList[i]);
        //}
    } // 룸리스트 받아오기

    public int GetGage()
    {
        return currentRoom.gage;
    }

    public void GetCurrentRoomBound(out Vector3 leftDown,out Vector3 rightTop)
    {
        leftDown = currentRoom.areaLeftDown;
        rightTop = currentRoom.areaRightTop;
    }

    void DoorActive() 
    {
        if (currentRoom.doorObjects != null)
        {
            for (int j = 0; j < currentRoom.doorObjects.Count; j++)
            {
                currentRoom.doorObjects[j].GetComponent<CustomObject>().Active();
            }
        }
    }//작동 가능여부 turn

    void ObjectSetAvailable() 
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                currentRoom.customObjects[j].GetComponent<CustomObject>().SetAvailable();
            }
        }
    } // 작동 가능여부 turn

    void EnableObjects()
    {
        if (!currentRoom.isRoom)
            return;
        for (int i = 0; i < currentRoom.customObjects.Length; i++)
        {
            if (currentRoom.customObjects[i].GetComponent<Portal>() == null)
            {
                currentRoom.customObjects[i].SetActive(true);
            }
        }
    }

    void DisalbeObject(Map.Rect _room)
    {
        if (_room.customObjects == null)
            return;
        for (int j = 0; j < _room.customObjects.Length; j++)
            _room.customObjects[j].SetActive(false);
    }

    void ClearRoom()
    {
        MiniMap.Instance.HideMiniMap();
        DoorActive();
        ObjectSetAvailable();
        FindCurrentRoom();
        UnityContext.GetClock().RemoveAllTimer();
        //TODO : 무기만듬

        Item item = ObjectPoolManager.Instance.CreateUsableItem(UsableItemType.CLOTHING);
        ItemManager.Instance.CallItemBox(currentRoom.GetAvailableArea(), item);
        ItemManager.Instance.CollectItem();
        if (currentRoom.eRoomType == RoomType.BOSS)
        {
            for (int i = 0; i < currentRoom.customObjects.Length; i++)
            {
                if (currentRoom.customObjects[i].GetComponent<Portal>() != null)
                {
                    currentRoom.customObjects[i].SetActive(true);
                }
            }
        }
        NeignborDraw(currentRoom);
    }

    void SpawnMonster()
    {
        if (currentRoom.customObjects != null)
        {
            for (int j = 0; j < currentRoom.customObjects.Length; j++)
            {
                if (currentRoom.gage <= 0)
                    return;
                if (currentRoom.customObjects[j].GetComponent<Spawner>() != null)
                {
                    currentRoom.customObjects[j].GetComponent<Spawner>().Active();
                    break;
                }
            }
        }
    } // 몬스터 소환

    public Vector3 Spawned()
    {
        currentRoom.gage--;
        monsterNum++;
        return currentRoom.GetAvailableArea();
    }

    public void DieMonster()
    {
        monsterNum--;
        if (monsterNum == 0)
        {
            if (currentRoom.gage == 0)
                ClearRoom();
            else
                SpawnMonster();
        }
    } // 몬스터 사망

    public List<Map.Rect> GetRoomList()
    {
        return roomList;
    } // 룸리스트

    public Vector3 RoomStartPoint()
    {
        return MapManager.Instance.GetMap().GetStartPosition();
    }

    public void FindCurrentRoom()
    {
        StartCoroutine("FindRoomCoroutine");
    }

    IEnumerator FindRoomCoroutine() // 현재 방 찾기 코루틴
    {
        while (true)
        {
            if (!currentRoom.isClear)
            {
                MapManager.Instance.GetMap().RemoveFog(currentRoom);

                currentRoom.isClear = true;
                if (currentRoom.eRoomType == RoomType.BOSS)
                {
                    InitRoom();
                    //EnableObjects();
                    InitBossRoom();
                    break;
                }
                else
                {
                    if (currentRoom.gage > 0)
                    {
                        InitRoom();
                        //EnableObjects();
                        SpawnMonster();
                        break;
                    }
                }
                if (currentRoom.isRoom)
                {
                    MiniMap.Instance.ClearRoom(currentRoom);
                    //EnableObjects();
                }
            }
            else
                currentRoom = GetCurrentRect(PlayerManager.Instance.GetPlayerPosition());

            yield return YieldInstructionCache.WaitForSeconds(0.05f);
        }
    }

    void NeignborDraw(Map.Rect room)
    {
        if (!room.isRoom)
            return;
        MiniMap.Instance.ClearRoom(room);

        for (int i = 0; i < room.linkedEdgeRect.Count; i++)
        {
            if(room.linkedEdgeRect[i].isRoom && !room.linkedEdgeRect[i].isDrawed)
            {
                MiniMap.Instance.ClearRoom(room.linkedEdgeRect[i]);
            }
        }
    }

    void InitRoom()
    {
        MiniMap.Instance.HideMiniMap();
        DoorActive();
        ObjectSetAvailable();
    }

    void InitBossRoom()
    {
        StartCoroutine(CoroutineBoss());
    }

    IEnumerator CoroutineBoss()
    {
        BossInitScene();
        yield return YieldInstructionCache.WaitForSeconds(2);
        BossSceneEnd();
    }

    void BossInitScene()
    {
        UIManager.Instance.TogglePreventObj();
        CutSceneUI.Instance.SetCharacter(EnemyManager.Instance.GetBossSprite());
        CutSceneUI.Instance.ShowCutScene(Vector2.right, Vector2.down, Vector2.up);
    }

    void BossSceneEnd()
    {
        UIManager.Instance.TogglePreventObj();
        CutSceneUI.Instance.Hide();
        EnemyManager.Instance.SpawnBoss(GameDataManager.Instance.GetFloor(), (currentRoom.areaLeftDown + currentRoom.areaRightTop) / 2);
        monsterNum++;
        SpawnMonster();
    }

    public Map.Rect GetCurrentRect(Vector3 _position) // 현재 방 찾기
    {
        for (int i = 0; i < currentRoom.edgeRect.Count; i++)
        {
            if (currentRoom.edgeRect[i].IsContain(_position))
            {
                return currentRoom.edgeRect[i];
            }
        }
        return currentRoom;
    }

}
