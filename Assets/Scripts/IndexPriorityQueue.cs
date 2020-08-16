using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndexPriorityQueue
{
    private List<float> _keys;
    private List<int> _data;

    public IndexPriorityQueue(List<float> keys)
    {
        _keys = keys;
        _data = new List<int>();
    }

    public void Insert(int index)
    {
        _data.Add(index);
        ReorderUp();
    }

    public int Pop()
    {
        if (_data.Count > 0)
        {
            var first = _data[0];
            _data.RemoveAt(0);
            return first;
        }
        Debug.LogError("Empty dataset!");
        return 0;
    }

    public void ReorderUp()
    {
        _data.Sort((x, y) =>
        {
            if (_keys[_data[x]] > _keys[_data[y]]) return -1;
            return 0;
        });
        //for (int i = _data.Count - 1; i > 0; i--)
        //{
        //    if (_keys[_data[i]] < _keys[_data[i - 1]])
        //    {
        //        int tmp = _data[i];
        //        _data[i] = _data[i - 1];
        //        _data[i - 1] = tmp;
        //    }
        //}
    }

    public void ReorderDown()
    {
        _data.Sort((x, y) =>
        {
            if (_keys[_data[x]] > _keys[_data[y]]) return 1;
            return 0;
        });
    }

    public bool IsEmpty()
    {
        return _data.Count == 0;
    }
}
