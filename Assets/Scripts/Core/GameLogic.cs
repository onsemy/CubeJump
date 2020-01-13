using System;
using System.Collections;
using System.Collections.Generic;
using JJFramework.Runtime;
using JJFramework.Runtime.Attribute;
using JJFramework.Runtime.Extension;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace GameRuntime.Core.Play
{
    public class GameLogic : ExMonoSingleton<GameLogic>
    {
        [ComponentPath, SerializeField] protected Button btnStart;
        [ComponentPath, SerializeField] protected Text txtTime;
        [ComponentPath, SerializeField] protected Player objPlayer;
        [ComponentPath, SerializeField] protected BoxCollider objCube;
        [ComponentPath, SerializeField] protected BoxCollider objRegenArea;

        [SerializeField] private FloatReactiveProperty _time = new FloatReactiveProperty(0f);

        public BoolReactiveProperty IsStart { get; } = new BoolReactiveProperty(false);

        protected override void Awake()
        {
            base.Awake();

            IsStart.Subscribe(x => btnStart.SetActive(!x));
            IsStart.Value = false;
            
            txtTime.text = string.Empty;

            objCube.UpdateAsObservable()
                .Select(x => objCube.transform)
                .Where(t => IsStart.Value)
                .Subscribe(t => t.Translate(-Time.deltaTime * 5f, 0f, 0f));

            objCube.OnTriggerEnterAsObservable()
                .Where(c => c.name == nameof(objPlayer))
                .Subscribe(c => IsStart.Value = false);
            
            objRegenArea.OnTriggerEnterAsObservable()
                .Where(c => c.name == nameof(objCube))
                .Subscribe(c => objCube.transform.localPosition = new Vector3(4f, -1.5f, 0f));

            Observable.EveryUpdate()
                .Where(x => IsStart.Value)
                .Where(x => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                .Subscribe(_ => objPlayer.Jump());

            btnStart.OnClickAsObservable()
                .Where(x => IsStart.Value == false)
                .Subscribe(_ =>
                {
                    objCube.transform.localPosition = new Vector3(4f, -1.5f, 0f);                   
                    _time.Value = 0f;
                    IsStart.Value = true;
                });

            Observable.EveryUpdate()
                .Where(x => IsStart.Value)
                .Subscribe(_ => _time.Value += Time.deltaTime);

            _time.SubscribeToText(txtTime, x => $"Time: {x:0.0}");
        }
    }
}
