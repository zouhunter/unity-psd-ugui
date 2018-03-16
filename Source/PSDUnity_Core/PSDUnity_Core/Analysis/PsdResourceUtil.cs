using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace PSDUnity
{
    public static class PsdResourceUtil
    {
        public static RuleObject DefultRuleObj()
        {
            return ScriptableObject.CreateInstance<RuleObject>();
        }
    }
}