using System;
using System.Collections.Generic;
using UnityEngine;

internal interface Interpolator
{
    void Setup(float historyLength, float maxExtrapolationTime);
    void AddDataPoint(float time, Vector3 point);
    Vector3 GetDataPoint(float time);
    void Update(float localTime);
}

public class LinearInterpolator : Interpolator
{
    public struct KeyPoint
    {
        public float Time;
        public Vector3 Point;
    }

    private float _HistoryLength;
    private float _MaxExtrapolationTime;
    private float _LastLocalTime;

    private List<KeyPoint> _DataQueue = new List<KeyPoint>();

    public void Setup(float historyLength, float maxExtrapolationTime)
    {
        _HistoryLength = historyLength;
        _MaxExtrapolationTime = maxExtrapolationTime;
    }
    public void AddDataPoint(float time, Vector3 point)
    {
        if (time < (_LastLocalTime - _HistoryLength) && _DataQueue.Count >= 2)
            return;

        for (int i = 0; i < _DataQueue.Count; i++)
        {
            if (time > _DataQueue[i].Time)
            {
                if (_DataQueue[i].Point != point)
                {
                    _DataQueue.Insert(i, new KeyPoint()
                    {
                        Time = time,
                        Point = point,
                    });
                }
                return;
            }
        }

        _DataQueue.Add(new KeyPoint()
        {
            Time = time,
            Point = point,
        });
    }

    public Vector3 GetDataPoint(float time)
    {
        int index = -1;
        for (int i = 0; i < _DataQueue.Count; i++)
        {
            if (time > _DataQueue[i].Time)
            {
                index = i;
                break;
            }
        }

        if (index >= 0)
        {
            var data = _DataQueue[index];
            if (index > 0)
            {
                var range = _DataQueue[index - 1].Time - data.Time;
                var dir = _DataQueue[index - 1].Point - data.Point;
                var diff = (time - data.Time) / range;

                return data.Point + (diff * dir);
            }
            else // extrapolate into the future
            {
                var diff = 0f;
                var dir = Vector3.zero;
                if (_DataQueue.Count > 1)
                {
                    int sampleOffset = Mathf.Min(5, _DataQueue.Count - 1);
                    var range = data.Time - _DataQueue[index + sampleOffset].Time;
                    if (range > 0)
                    {
                        dir = data.Point - _DataQueue[index + sampleOffset].Point;
                        diff = Mathf.Min(time - data.Time, _MaxExtrapolationTime) / range;
                    }
                }

                return data.Point + (diff * dir);
            }
        }

        if (_DataQueue.Count > 0) // extrapolate back in history
        {
            var lastData = _DataQueue[_DataQueue.Count - 1];
            var diff = 0f;
            var dir = Vector3.zero;

            if (_DataQueue.Count > 1)
            {
                int sampleOffset = Mathf.Min(5, _DataQueue.Count - 1);
                var range = _DataQueue[_DataQueue.Count - sampleOffset].Time - lastData.Time;
                if (range > 0)
                {
                    dir = _DataQueue[_DataQueue.Count - sampleOffset].Point - lastData.Point;
                    diff = (time - lastData.Time) / range;
                }
            }

            return lastData.Point - (diff * dir);
        }

        return Vector3.zero;
    }

    public void Update(float localTime)
    {
        _LastLocalTime = localTime;

        if (_DataQueue.Count > 2)
        {
            for (int i = _DataQueue.Count - 1; i >= 0; i--)
            {
                if (_DataQueue[i].Time < (_LastLocalTime - _HistoryLength))
                    _DataQueue.RemoveAt(i);
            }
        }
    }
}