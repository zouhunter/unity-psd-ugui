using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
namespace PSDUnity.Data
{
    public class RuleItem
    {
        public int id;
        public string key;
        public string fieldName;

        public RuleItem(int id, string key, string fieldName)
        {
            this.id = id;
            this.key = key;
            this.fieldName = fieldName;
        }
    }
}