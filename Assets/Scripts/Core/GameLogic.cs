using System;
using System.Collections;
using System.Collections.Generic;
using JJFramework.Runtime;
using JJFramework.Runtime.Attribute;
using JJFramework.Runtime.Extension;
using UniRx;
using UniRx.Diagnostics;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameRuntime.Core.Play
{
    public class GameLogic : ExMonoSingleton<GameLogic>
    {
        [ComponentPath, SerializeField] protected Button btnStart;
        [ComponentPath, SerializeField] protected Text txtTime;
        [ComponentPath, SerializeField] protected Text txtSpeed;
        [ComponentPath, SerializeField] protected Player objPlayer;
        [ComponentPath, SerializeField] protected BoxCollider objCube;
        [ComponentPath, SerializeField] protected BoxCollider objRegenArea;

        [SerializeField] private FloatReactiveProperty _time = new FloatReactiveProperty(0f);
        [SerializeField] private FloatReactiveProperty _speedFactor = new FloatReactiveProperty(5f);
        [SerializeField] private int _randSeed = 2001132211;

        public BoolReactiveProperty IsStart { get; } = new BoolReactiveProperty(false);

        protected override void Awake()
        {
            base.Awake();
            
            UnityEngine.Random.InitState(_randSeed);

            IsStart.Subscribe(x => btnStart.SetActive(!x));
            IsStart.Value = false;
            
            txtTime.text = string.Empty;
            txtSpeed.text = string.Empty;

            objCube.UpdateAsObservable()
                .Select(x => objCube.transform)
                .Where(t => IsStart.Value)
                .Subscribe(t =>
                {
                    t.Translate(-Time.deltaTime * _speedFactor.Value, 0f, 0f);
                    if (t.localPosition.x < -5f)
                    {
                        t.localPosition = new Vector3(4f, -1.5f, 0f);
                        _speedFactor.Value = UnityEngine.Random.Range(4f, 8f);
                        Debug.Log($"[GameLogic] Current SpeedFactor: {_speedFactor.Value}");
                    }
                });

            objCube.OnTriggerEnterAsObservable()
                .Where(c => c.name == nameof(objPlayer))
                .Subscribe(c => IsStart.Value = false);

            Observable.EveryUpdate()
                .Where(x => IsStart.Value)
                .Where(x => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
                .Subscribe(_ => objPlayer.Jump());

            btnStart.OnClickAsObservable()
                .Where(x => IsStart.Value == false)
                .Subscribe(_ =>
                {
                    UnityEngine.Random.InitState(_randSeed);
                    _speedFactor.Value = 5f;
                    objCube.transform.localPosition = new Vector3(4f, -1.5f, 0f);                   
                    _time.Value = 0f;
                    IsStart.Value = true;
                });

            Observable.EveryUpdate()
                .Where(x => IsStart.Value)
                .Subscribe(_ => _time.Value += Time.deltaTime);

            _time.SubscribeToText(txtTime, x => $"Time: {x:0.000}");
            _speedFactor.SubscribeToText(txtSpeed, x => $"Speed: {x:0.000}");
        }
    }
}
