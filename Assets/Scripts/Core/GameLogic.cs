using System;
using System.Collections;
using System.Collections.Generic;
using JJFramework.Runtime.Attribute;
using JJFramework.Runtime.Extension;
using UnityEngine;
using UnityEngine.UI;

namespace GameRuntime.Core.Play
{
    public class GameLogic : ExMonoBehaviour
    {
        [ComponentPath] protected Text txtScore;
        [ComponentPath] protected Player objPlayer;

        protected override void Awake()
        {
            base.Awake();
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
