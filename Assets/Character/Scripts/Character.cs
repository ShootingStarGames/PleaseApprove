﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CharacterInfo
{
    public enum OwnerType
    {
        Player, Enemy, Pet, Object
    }
    public enum State
    {
        NOTSPAWNED, DIE, ALIVE
    }

}

struct RaycasthitEnemy
{
    public int index;
    public float distance;
}

// 무기 매니저를 착용 하고 쓸 수 있는 owner들 (player, character, object)에서 써야 될 함수 변수들에 대한 걸 따로 묶어서 인터페이스화 해서 쓸 예정
// 그래야 character는 palyer, enemy에만 적용 하는건데 무기 착용 object에 대한 처리가 애매해짐.

public abstract class Character : MonoBehaviour
{
    #region Status
    public float moveSpeed;     // Character move Speed
    public float hp; // protected인데 debug용으로 어디서든 접근되게 public으로 했고 현재 hpUI에서 접근

    #endregion
    #region Componets
    [SerializeField]
    protected new SpriteRenderer renderer;
    [SerializeField]
    protected WeaponManager weaponManager;
    [SerializeField]
    protected Transform spriteObjTransform; // sprite 컴포넌트가 붙여있는 object, player에 경우 inspector 창에서 붙여줌
    public CircleCollider2D interactiveCollider2D;
    public Animator animator;
    protected BuffManager buffManager;
    protected Rigidbody2D rgbody;

    #endregion
    #region variables
    // 디버그용 SerializeField
    protected bool isActiveAI;
    protected bool isKnockBack;
    [SerializeField]
    protected Sprite sprite;
    protected CharacterInfo.State pState;

    protected bool isAutoAiming;    // 오토에임 적용 유무

    protected Vector3 directionVector;
    protected float directionDegree;  // 바라보는 각도(총구 방향)

    protected bool isRightDirection;    // character 방향이 우측이냐(true) 아니냐(flase = 좌측)

    /// <summary> owner 좌/우 바라볼 때 spriteObject scale 조절에 쓰일 player scale, 우측 (1, 1, 1), 좌측 : (-1, 1, 1) </summary>
    protected Vector3 scaleVector;
    #endregion

    #region getter
    public bool GetAIAct()
    {
        return isActiveAI;
    }
    public virtual bool GetRightDirection() { return isRightDirection; }
    public virtual Vector3 GetDirVector()
    {
        return directionVector;
    }
    public virtual float GetDirDegree()
    {
        return directionDegree;
    }
    public virtual Vector3 GetPosition() { return transform.position; }
    public virtual WeaponManager GetWeaponManager() { return weaponManager; }
    public BuffManager GetBuffManager() { return buffManager; }
    #endregion
    #region Func
    public bool IsDie()
    {
        if (CharacterInfo.State.DIE == pState)
        {
            return true;
        }
        return false;
    }
    #endregion


    // 0531 모장현 프로토 타입 용
    public virtual void SetHp(float _hp) { hp = _hp; }

    /*--abstract--*/
    protected abstract void Die();
    public abstract float Attacked(TransferBulletInfo info);

    // item Character 대상 효과 적용
    public abstract void ApplyItemEffect(CharacterTargetEffect itemUseEffect);

    /// <summary> 상태 이상 효과 적용 </summary>
    public virtual void ApplyStatusEffect(StatusEffectInfo statusEffectInfo)
    {
        // Enemy랑 Player랑 효과 다르게 받아야 될 게 생길 듯
    }

    public virtual void Nag()
    {

    }

    public virtual void DelayState()
    {

    }
    /**/

    /// <summary>총알 외의 충돌로 인한 공격과 넉백 처리</summary>
    public abstract float Attacked(Vector2 _dir, Vector2 bulletPos, float damage, float knockBack, float criticalChance = 0, bool positionBasedKnockBack = false);

    protected IEnumerator CoroutineAttacked()
    {
        renderer.color = new Color(1, 0, 0);
        yield return YieldInstructionCache.WaitForSeconds(0.1f);
        renderer.color = new Color(1, 1, 1);
    }

    protected IEnumerator KnockBackCheck()
    {

        while (true)
        {
            yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime);
            if (Vector2.zero != rgbody.velocity && rgbody.velocity.magnitude < 1f)
            {
                isKnockBack = false;
            }
        }

        /*yield return YieldInstructionCache.WaitForSeconds(Time.fixedDeltaTime * 25);
        isKnockBack = false;*/
    }
}

