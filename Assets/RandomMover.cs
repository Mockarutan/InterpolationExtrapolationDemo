using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomMover : MonoBehaviour
{
    public int Seed;
    public int Stages;
    public float MaxForce;
    public float MaxDuration;
    public float MaxSpeed;
    public int SendFPS;

    public InputField SeedInput;
    public InputField StagesInput;
    public InputField MaxForceInput;
    public InputField MaxDurationInput;
    public InputField MaxSpeedInput;
    public InputField SendFPSInput;

    public NetworkSimulator NetworkSimulator;

    private Vector2 _StartPosition;

    private int _DurrentState;
    private Vector3 _CurrentDirection;
    private float _CurrentForce;
    private float _CurrentCountdown;

    private float _TimeAccumelator;

    private Transform _Trans;
    private Rigidbody _Body;
    private System.Random _Rand;

    void Start()
    {
        SeedInput.text = Seed.ToString();
        StagesInput.text = Stages.ToString();
        MaxForceInput.text = MaxForce.ToString();
        MaxDurationInput.text = MaxDuration.ToString();
        MaxSpeedInput.text = MaxSpeed.ToString();
        SendFPSInput.text = SendFPS.ToString();

        SeedInput.onValueChanged.AddListener((value) => Seed = int.Parse(value));
        StagesInput.onValueChanged.AddListener((value) => Stages = int.Parse(value));
        MaxForceInput.onValueChanged.AddListener((value) => MaxForce = float.Parse(value));
        MaxDurationInput.onValueChanged.AddListener((value) => MaxDuration = float.Parse(value));
        MaxSpeedInput.onValueChanged.AddListener((value) => MaxSpeed = float.Parse(value));
        SendFPSInput.onValueChanged.AddListener((value) => SendFPS = int.Parse(value));

        _Trans = transform;
        _StartPosition = transform.position;
        _Body = GetComponent<Rigidbody>();
        _Rand = new System.Random(Seed);
        GenerateAllRandomValues();
    }

    void Update()
    {
        _CurrentCountdown -= Time.deltaTime;

        _Body.AddForce(_CurrentDirection * _CurrentForce);
        if (_Body.velocity.magnitude > MaxSpeed)
            _Body.velocity = _Body.velocity.normalized * MaxSpeed;

        if (_CurrentCountdown <= 0)
        {
            _DurrentState++;
            if (_DurrentState >= Stages)
            {
                _DurrentState = 0;
                _Rand = new System.Random(Seed);

                transform.position = _StartPosition;
                _Body.velocity = Vector3.zero;
            }

            GenerateAllRandomValues();
        }

        _TimeAccumelator += Time.deltaTime;
        if (_TimeAccumelator < (1 / SendFPS))
            return;
        else
        {
            _TimeAccumelator -= 1 / SendFPS;
            NetworkSimulator.EnqueuePackage(_Trans.position);
        }
    }

    private void GenerateAllRandomValues()
    {
        var randAngle = ((float)_Rand.NextDouble()) * 360;
        _CurrentDirection = Quaternion.Euler(0, randAngle, 0) * Vector3.forward;
        _CurrentForce = ((float)_Rand.NextDouble()) * MaxForce;
        _CurrentCountdown = ((float)_Rand.NextDouble()) * MaxDuration;
    }
}
