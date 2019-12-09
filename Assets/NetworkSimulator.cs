using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class NetworkSimulator : MonoBehaviour
{
    public struct NetPackage
    {
        public int FrameCount;
        public float LocalTime;
        public float ReceiveTime;
        public object Data;
    }

    public InputField MinDelayInput;
    public InputField MaxDelayInput;
    public InputField MinDelayDurationInput;
    public InputField MaxDelayDurationInput;
    public InputField DelayVariationRangeInput;

    public InputField PacketLossInput;
    public InputField MinPacketLossDurationInput;
    public InputField MaxPacketLossDurationInput;
    public InputField MinPacketLossDelayInput;
    public InputField MaxPacketLossDelayInput;
    public InputField UniformPacketLossInput;

    public float MinDelay;
    public float MaxDelay;
    public float MinDelayDuration;
    public float MaxDelayDuration;
    public float DelayVariationRange;

    public float PacketLoss;
    public float MinPacketLossDuration;
    public float MaxPacketLossDuration;
    public float MinPacketLossDelay;
    public float MaxPacketLossDelay;
    public float UniformPacketLoss;

    private float _CurrentDelay;
    private float _NextDelayRestTime;
    private float _NextPacketLossRestTime;
    private float _NextPacketLossTime;

    private List<NetPackage> _PackageQue = new List<NetPackage>();
    private System.Action<NetPackage> _OnPackage;

    private void Start()
    {
        MinDelayInput.text = MinDelay.ToString();
        MaxDelayInput.text = MaxDelay.ToString();
        MinDelayDurationInput.text = MinDelayDuration.ToString();
        MaxDelayDurationInput.text = MaxDelayDuration.ToString();
        DelayVariationRangeInput.text = DelayVariationRange.ToString();

        PacketLossInput.text = PacketLoss.ToString();
        MinPacketLossDurationInput.text = MinPacketLossDuration.ToString();
        MaxPacketLossDurationInput.text = MaxPacketLossDuration.ToString();
        MinPacketLossDelayInput.text = MinPacketLossDelay.ToString();
        MaxPacketLossDelayInput.text = MaxPacketLossDelay.ToString();
        UniformPacketLossInput.text = UniformPacketLoss.ToString();

        MinDelayInput.onValueChanged.AddListener((value) => MinDelay = float.Parse(value));
        MaxDelayInput.onValueChanged.AddListener((value) => MaxDelay = float.Parse(value));
        MinDelayDurationInput.onValueChanged.AddListener((value) => MinDelayDuration = float.Parse(value));
        MaxDelayDurationInput.onValueChanged.AddListener((value) => MaxDelayDuration = float.Parse(value));
        DelayVariationRangeInput.onValueChanged.AddListener((value) => DelayVariationRange = float.Parse(value));

        PacketLossInput.onValueChanged.AddListener((value) => PacketLoss = float.Parse(value));
        MinPacketLossDurationInput.onValueChanged.AddListener((value) => MinPacketLossDuration = float.Parse(value));
        MaxPacketLossDurationInput.onValueChanged.AddListener((value) => MaxPacketLossDuration = float.Parse(value));
        MinPacketLossDelayInput.onValueChanged.AddListener((value) => MinPacketLossDelay = float.Parse(value));
        MaxPacketLossDelayInput.onValueChanged.AddListener((value) => MaxPacketLossDelay = float.Parse(value));
        UniformPacketLossInput.onValueChanged.AddListener((value) => UniformPacketLoss = float.Parse(value));

        PacketLossInput.onValueChanged.AddListener((value) => ResetPacketLoss());
        MinPacketLossDurationInput.onValueChanged.AddListener((value) => ResetPacketLoss());
        MaxPacketLossDurationInput.onValueChanged.AddListener((value) => ResetPacketLoss());
        MinPacketLossDelayInput.onValueChanged.AddListener((value) => ResetPacketLoss());
        MaxPacketLossDelayInput.onValueChanged.AddListener((value) => ResetPacketLoss());

        _NextPacketLossTime = Time.time + Random.Range(MinPacketLossDelay, MaxPacketLossDelay);
    }

    private void ResetPacketLoss()
    {
        _NextPacketLossRestTime = 0;
        _NextPacketLossTime = Time.time + Random.Range(MinPacketLossDelay, MaxPacketLossDelay);
    }

    public void EnqueuePackage(object data)
    {
        if (Random.Range(0f, 1f) < UniformPacketLoss)
            return;

        if (Random.Range(0f, 1f) < PacketLoss)
            return;

        var delayVariation = Random.Range(-DelayVariationRange, DelayVariationRange);
        var delay = Mathf.Max(MinDelay, _CurrentDelay + delayVariation);

        _PackageQue.Add(new NetPackage()
        {
            FrameCount = Time.frameCount,
            LocalTime = Time.time,
            ReceiveTime = Time.time + delay,
            Data = data,
        });
    }

    public void SetPackageListener(System.Action<NetPackage> onPackage)
    {
        _OnPackage = onPackage;
    }

    public void Update()
    {
        if (Time.time > _NextDelayRestTime)
        {
            _CurrentDelay = Random.Range(MinDelay, MaxDelay);
            _NextDelayRestTime = Time.time + Random.Range(MinDelayDuration, MaxDelayDuration);
        }

        if (_NextPacketLossRestTime > 0 && Time.time > _NextPacketLossRestTime)
        {
            _NextPacketLossRestTime = 0;
            _NextPacketLossTime = Time.time + Random.Range(MinPacketLossDelay, MaxPacketLossDelay);
        }
        else if (_NextPacketLossTime > 0 && Time.time > _NextPacketLossTime)
        {
            _NextPacketLossTime = 0;
            _NextPacketLossRestTime = Time.time + Random.Range(MinPacketLossDuration, MinPacketLossDuration);
        }

        for (int i = _PackageQue.Count - 1; i >= 0; i--)
        {
            if (Time.time > _PackageQue[i].ReceiveTime)
            {
                //Debug.Log("FrameCount: " + _PackageQue[i].FrameCount);

                if (_OnPackage != null)
                    _OnPackage(_PackageQue[i]);
                _PackageQue.RemoveAt(i);
            }
        }
    }
}
