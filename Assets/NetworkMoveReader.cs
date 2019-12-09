using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NetworkMoveReader : MonoBehaviour
{
    public enum InterpolationTypes
    {
        None,
        Linear,
    }

    public InterpolationTypes InterpolationType;
    public Dropdown InterpolationTypeDropDown;

    public float Delay;
    public float HistoryLength;
    public float MaxExtrapolation;

    public InputField DelayInput;
    public InputField HistoryLengthInput;
    public InputField MaxExtrapolationInput;

    public Transform ObjectToMove;

    public NetworkSimulator NetworkSimulator;

    private Interpolator _Interpolator;
    private InterpolationTypes _LastInterpolationType;

    void Start()
    {
        var options = System.Enum.GetValues(typeof(InterpolationTypes)).Cast<int>();

        InterpolationTypeDropDown.AddOptions(options.Select(e => ((InterpolationTypes)e).ToString()).ToList());
        InterpolationTypeDropDown.value = (int)InterpolationType;

        InterpolationTypeDropDown.onValueChanged.AddListener((value) => InterpolationType = (InterpolationTypes)value);

        DelayInput.text = Delay.ToString();
        HistoryLengthInput.text = HistoryLength.ToString();
        MaxExtrapolationInput.text = MaxExtrapolation.ToString();

        DelayInput.onValueChanged.AddListener((value) => Delay = float.Parse(value));
        HistoryLengthInput.onValueChanged.AddListener((value) => HistoryLength = float.Parse(value));
        MaxExtrapolationInput.onValueChanged.AddListener((value) => MaxExtrapolation = float.Parse(value));

        SetupInterpolator();
        NetworkSimulator.SetPackageListener(OnPackage);
    }

    void OnPackage(NetworkSimulator.NetPackage package)
    {
        if (_Interpolator != null)
            _Interpolator.AddDataPoint(package.LocalTime, (Vector3)package.Data);
        else
            ObjectToMove.position = (Vector3)package.Data;
    }

    void Update()
    {
        if (InterpolationType != _LastInterpolationType)
        {
            SetupInterpolator();
            _LastInterpolationType = InterpolationType;
        }

        if (_Interpolator != null)
        {
            _Interpolator.Update(Time.time);
            var point = _Interpolator.GetDataPoint(Time.time - Delay);
            ObjectToMove.position = point;
        }
    }

    private void SetupInterpolator()
    {
        switch (InterpolationType)
        {
            case InterpolationTypes.None:
                _Interpolator = null;
                break;
            case InterpolationTypes.Linear:
                _Interpolator = new LinearInterpolator();
                break;
        }

        if (_Interpolator != null)
            _Interpolator.Setup(HistoryLength, MaxExtrapolation);
    }
}
