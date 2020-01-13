using System.Collections;
using System.Collections.Generic;
using JJFramework.Runtime.Extension;
using UniRx;
using UnityEngine;

namespace GameRuntime.Core.Play
{
    public class Player : ExMonoBehaviour
    {
        [SerializeField] private CharacterController _controller;
        [SerializeField] private float _jumpFactor = 10f;
        [SerializeField] private Vector3 _direction;
        [SerializeField] private float _targetHeight;

        private const float _gravity = 9.8f * 2f;

        protected override void Awake()
        {
            base.Awake();

            _controller = GetComponent<CharacterController>();

            Observable.EveryUpdate().Subscribe(_ =>
            {
                if (_controller.isGrounded == false)
                {
                    _direction.y -= _gravity * Time.deltaTime;
                }

                _controller.Move(_direction * Time.deltaTime); // * Time.deltaTime);
            });
        }

        public void Jump()
        {
            if (_controller.isGrounded)
            {
                _direction.y = _jumpFactor;
            }
            else
            {
                Debug.LogWarning("[Player::Jump] Object is not grounded yet.");
            }
        }
    }
}