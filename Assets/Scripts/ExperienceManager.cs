using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceManager : MonoBehaviour
{
    private int _dropRate = 1;
    private int _expRate = 1;

    public int GainExperience(int baseExp)
    {
        return baseExp * _expRate;
    }

    public void Init(DropAndExpRate rates)
    {
        _dropRate = rates.dropRate;
        _expRate = rates.expRate;

        Debug.Log($"Exp:{_expRate}, Drop: {_dropRate}");
    }

    [Serializable]
    public struct DropAndExpRate
    {
        public int expRate;
        public int dropRate;
    }
}
