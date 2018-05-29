﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
class GameData
{
    int hp;
    int hungry;
    int m_floor;
    int m_coin;
    Player.PlayerType m_playerType;

    public GameData()
    {
        m_floor = 1;
        m_coin = 0;
        m_playerType = Player.PlayerType.SOCCER;
    }
    #region getter
    public int GetFloor() { return m_floor; }
    public int GetCoin() { return m_coin; }
    public Player.PlayerType GetPlayerType() { return m_playerType; }
    #endregion
    #region setter
    public void SetFloor() { m_floor++; }
    public void SetCoin(int _coin) { m_coin = _coin; }
    #endregion

}