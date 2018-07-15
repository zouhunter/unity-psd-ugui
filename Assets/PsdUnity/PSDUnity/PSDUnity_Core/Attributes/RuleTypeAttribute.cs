using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System;
namespace PSDUnity
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class RuleTypeAttribute : Attribute
    {
        public int id;
        public string key;
        public RuleTypeAttribute(int id, string key)
        {
            this.id = id;
            this.key = key;
        }
    }
}