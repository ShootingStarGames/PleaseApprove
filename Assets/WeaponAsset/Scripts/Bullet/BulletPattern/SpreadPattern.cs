﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WeaponAsset;
using CharacterInfo;

// 일정 부채꼴 범위로 퍼져나가는 총알로 따로 각도 계산 없이 퍼질 부채꼴 범위, 총알 갯수만 있으면 되고
// 패시브 아이템 중 샷건류 퍼져나가는 총알 수 늘리는거 적용 하려고 따로 만듬(multi pattern 으로 해도 되는데 그냥 따로 만듬)
public class SpreadPattern : BulletPattern
{
    private SpreadPatternInfo info;
    private float sectorAngle;
    private int bulletCount;

    public SpreadPattern(SpreadPatternInfo patternInfo, int executionCount, float delay, bool isFixedOwnerDir, bool isFixedOwnerPos, CharacterInfo.OwnerType ownerType)
    {
        info = patternInfo;
        this.executionCount = executionCount;
        this.delay = delay;
        this.isFixedOwnerDir = isFixedOwnerDir;
        this.isFixedOwnerPos = isFixedOwnerPos;
        this.ownerType = ownerType;
    }
    public override BulletPattern Clone()
    {
        return new SpreadPattern(info, executionCount, delay, isFixedOwnerDir, isFixedOwnerPos, ownerType);
    }

    public override void CreateBullet(float damageIncreaseRate)
    {
        ApplyWeaponBuff();
        float initAngle = sectorAngle / 2;
        float deltaAngle = sectorAngle / (bulletCount - 1);

        for (int i = 0; i < bulletCount; i++)
        {
            if (PatternCallType.WEAPON == patternCallType)
            {
                if (AttackType.RANGED == weapon.GetAttackType())
                {
                    if (weapon.HasCostForAttack())
                        weapon.UseAmmo();
                    else break;
                }
            }
            if (info.ignoreOwnerDir)
            {
                dirDegree = 0;
            }
            else
            {
                dirDegree = ownerDirDegree();
            }

            OwnerDash(info.dashInfo, info.canDash);

            AutoSelectBulletInfo(info.bulletInfo, info.diffProbsBulletInfoList);

            createdObj = ObjectPoolManager.Instance.CreateBullet();
            createdObj.GetComponent<Bullet>().Init(bulletInfo, ownerBuff, ownerType,
                weapon.GetMuzzlePos() + GetadditionalPos(info.ignoreOwnerDir, info.addDirVecMagnitude, info.additionalVerticalPos),
                info.angleAxis + dirDegree - initAngle + deltaAngle * i + Random.Range(-info.randomAngle, info.randomAngle) * accuracyIncrement,
                transferBulletInfo, info.childBulletCommonProperty.timeForOriginalShape);
            CreateChildBullets(info.childBulletInfoList, info.childBulletCommonProperty);
        }
    }

    public override void Init(Weapon weapon)
    {
        base.Init(weapon);
        if (null != info.diffProbsBulletInfoList && info.diffProbsBulletInfoList.Length > 0)
        {
            randomSelectProbTotal = 0;
            for (int i = 0; i < info.diffProbsBulletInfoList.Length; i++)
            {
                info.diffProbsBulletInfoList[i].bulletInfo.Init();
                randomSelectProbTotal += info.diffProbsBulletInfoList[i].probRatio;
            }
        }
        else
            info.bulletInfo.Init();
        for (int i = 0; i < info.childBulletInfoList.Count; i++)
        {
            info.childBulletInfoList[i].bulletInfo.Init();
        }
    }

    public override void Init(BuffManager ownerBuff, OwnerType ownerType, TransferBulletInfo transferBulletInfo, DelGetDirDegree dirDegree,
        DelGetPosition dirVec, DelGetPosition pos, float addDirVecMagnitude = 0)
    {
        info.bulletInfo.Init();
        base.Init(ownerBuff, ownerType, transferBulletInfo, dirDegree, dirVec, pos, addDirVecMagnitude);
    }

    public override void StartAttack(float damageIncreaseRate, CharacterInfo.OwnerType ownerType)
    {
        this.ownerType = ownerType;
        CreateBullet(damageIncreaseRate);
    }

    public override void StopAttack()
    {
    }

    public override void ApplyWeaponBuff()
    {
        base.ApplyWeaponBuff();
        // 정확도
        sectorAngle = info.sectorAngle * accuracyIncrement;
        // 샷건 발사 수 증가
        bulletCount = info.bulletCount + effectInfo.bulletCountIncrement;
    }

    protected override void IncreaseAdditionalAngle()
    {
        additionalAngle += info.rotatedAnglePerExecution;
    }
}